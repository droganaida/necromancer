using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	// menu elements
	public Text scoreValueDisplay;
	public Text castleHP;
	public GameObject panelMenu;
	public GameObject panelGame;
	public Button btnMenu1;
	public Button btnMenu2;
	public Button btnMenu3;
	public Button btnMenu4;
	public Button btnMenu5;
	public Button btnPause;
	public Button btnRestart;

	//public GameObject reloadButton;
	public PooledSpawner spawnFromPool;

	public List<GameObject> allBullets = new List<GameObject>();
	public List<GameObject> allEnemies = new List<GameObject>();

	private int score = 0;
	private int HP = 20;
	private LevelManager LM;
	private int currentLevel;
	private AudioManager audioManager;

	public enum GameState {
		SplashScreen = 0,
		MainMenu = 1,
		ChooseLevel = 2,
		Game = 4,
		Pause = 5,
		Win = 6,
		GameOver = 7,
		Settings = 8,
		Update = 9,
		Achivment = 10,
		MoreGame = 11,
		Credits = 12
	}

	void Awake () {
		panelMenu.SetActive (false);
		btnMenu1.gameObject.SetActive (false);
		btnMenu2.gameObject.SetActive (false);
		btnMenu3.gameObject.SetActive (false);
		btnMenu4.gameObject.SetActive (false);
		btnMenu5.gameObject.SetActive (false);

		LM = GetComponent<LevelManager> ();
		if (PlayerPrefs.HasKey("curentLevel")) {
			currentLevel = PlayerPrefs.GetInt ("curentLevel");
		}

	}

	void Start() {
		// caching
		audioManager = AudioManager.instance;
		if (audioManager == null) {
			Debug.LogError ("Warning. Not found AudioManager on scene");
		}

		scoreValueDisplay.text = "Score: " + score.ToString ();
		castleHP.text = "HP: " + HP.ToString ();

		//yield return new WaitForSeconds (1f);
		//mainText.text = " ";
		spawnFromPool.StartSpawning ();
	}

	void createMap (int lev){
		LM.loadLevel (lev);
		//LevelBase newLev = "Level0" as LevelBase;
	}

	public void setLevel(int lev){
		if (lev < 0)
			return;
		currentLevel = lev;
	}
	public void incLevel(int lev) {
		if (lev < 0)
			return;
		currentLevel++;
	}

	public void AddBulletToList (GameObject bullet){
		allBullets.Add (bullet);
	}
	public void RemoveBulletFromList (GameObject bullet){
		allBullets.Remove (bullet);
	}
	public void AddEnemyToList (GameObject enemy){
		allEnemies.Add (enemy);
	}
	public void RemoveEnemyToList (GameObject enemy){
		allEnemies.Remove (enemy);
	}

	public void AddScore(int points) {
		score += points;
		scoreValueDisplay.text = "Score: " + score.ToString ();
		//audioManager.PlaySound ("soundWow");
	}
	public void TakeDamage (int damage) {
		HP -= damage;
		if (HP <= 0) {
			EndGame ();
		}
		castleHP.text = "HP: " + HP.ToString ();
	}

	public void EndGame() {
		Debug.Log ("================GAME OVER ");
		Time.timeScale = 0.1f;
		spawnFromPool.StopSpawning ();

		foreach (GameObject enemy in allEnemies) {
			// может удалить?
			enemy.GetComponent<DieOnHit> ().Portal ();
		}
	}

	public void RestartGame() {
		Time.timeScale = 1f;
		SceneManager.LoadScene ("Main");
	}

	public void PauseGame () {
		Time.timeScale = 0.0f;

		panelGame.SetActive (false);
		panelMenu.SetActive (true);

		btnMenu1.gameObject.SetActive (true);
		btnMenu1.onClick.AddListener (ResumeGame);
		btnMenu5.gameObject.SetActive (true);
		btnMenu5.onClick.AddListener (GameQuit);
	}

	public void ResumeGame (){
		btnMenu1.onClick.RemoveListener (ResumeGame);
		btnMenu1.gameObject.SetActive (false);
		btnMenu5.onClick.RemoveListener (GameQuit);
		btnMenu5.gameObject.SetActive (false);
		panelMenu.SetActive (false);
		panelGame.SetActive (true);

		Time.timeScale = 1f;
	}

	public void GameQuit (){
		Application.Quit ();
	}

	void Update () {
		//#if UNITY_ANDROID && !UNITY_EDITOR
		//if running on Android, check for Menu/Home and exit
		if (Application.platform == RuntimePlatform.Android) {
			if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Menu)) {
				//Menu (9);
			}
		}
		//#endif
	}
}
