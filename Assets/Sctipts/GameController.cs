using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	// menu elements
	public Text scoreValueDisplay;
	public Text castleHP;
	public Text curLevel;
	public Text unlockLev;

	public PooledSpawner spawnFromPool;

	public List<GameObject> allBullets = new List<GameObject>();
	public List<GameObject> allEnemies = new List<GameObject>();
	public List<GameObject> allEffects = new List<GameObject>();

	private int score = 0;
	private int HP = 20;
	private LevelManager LM;
	private int currentLevel;
	public int unlockLevel;
	private AudioManager audioManager;
	public SimpleObjectPool[] objectPool;
	private PooledObject pooledObject;
	private Transform enemies;
	private MenuManager MM;
	private int gameState;


	void Awake () {
		LM = GetComponent<LevelManager> ();
		currentLevel = 1;

		if (PlayerPrefs.HasKey("currentLevel")) {
			currentLevel = PlayerPrefs.GetInt ("currentLevel");
		} //PlayerPrefs.Save();
		if (PlayerPrefs.HasKey("unlockLevel")) {
			unlockLevel = PlayerPrefs.GetInt ("unlockLevel");
		}

		MM = GetComponent<MenuManager> ();
	}

	void Start() {
		// caching
		audioManager = AudioManager.instance;
		if (audioManager == null) {
			Debug.LogError ("Warning. Not found AudioManager on scene");
		}

		scoreValueDisplay.text = "Score: " + score.ToString ();
		castleHP.text = "HP: " + HP.ToString ();
		curLevel.text = "Level: " + currentLevel.ToString ();
		unlockLev.text = "UnlockLev: " + unlockLevel.ToString ();
///////////

		StartGame ();
	}
		
	// animation of death. deactivate after animation event
	public void Boom (Vector3 pos) {
		GameObject clone = objectPool[0].GetObject();
		clone.SetActive (true);
		clone.transform.position = pos;
		if (enemies == null) {
			enemies = GameObject.Find("Enemies").transform;
		}
		if (enemies != null) {
			clone.transform.SetParent (enemies);
		}
		clone.GetComponent<EndAnimation> ().SetGameController (this);
		AddEffectToList (clone);
	}

	void createMap (int lev){
		LM.loadLevel (lev);
		//LevelBase newLev = "Level0" as LevelBase;
	}

	public void setLevel(int lev){
		if (lev < 0)
			return;
		currentLevel = lev;
		//PlayerPrefs.SetInt ("unlockLevel",lev);
		PlayerPrefs.SetInt ("currentLevel",currentLevel);
		PlayerPrefs.Save ();
	}
	public void incLevel() {
		currentLevel++;
		if (currentLevel >= unlockLevel) {
			unlockLevel = currentLevel;
			unlockLevel++;
			PlayerPrefs.SetInt ("unlockLevel",unlockLevel);
		}
		PlayerPrefs.SetInt ("currentLevel",currentLevel);
		PlayerPrefs.Save ();
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
	public void RemoveEnemyFromList (GameObject enemy){
		allEnemies.Remove (enemy);
	}
	public void AddEffectToList (GameObject effect){
		allEffects.Add (effect);
	}
	public void RemoveEffectFromList (GameObject effect){
		allEffects.Remove (effect);
	}

	// wait some time, deactivate object and return object in pool
	IEnumerator DelayDeactivate(GameObject deactivatedObject) {
		yield return new WaitForSeconds (.25f);

		RemoveEffectFromList(deactivatedObject);
		pooledObject = deactivatedObject.GetComponent<PooledObject> ();
		pooledObject.pool.ReturnObject (deactivatedObject);
		deactivatedObject.SetActive (false);
	}

	// TODO: something for end animation event
	public void Deactivate(GameObject deactivatedObject) {
		RemoveEffectFromList(deactivatedObject);
		pooledObject = deactivatedObject.GetComponent<PooledObject> ();
		pooledObject.pool.ReturnObject (deactivatedObject);
		deactivatedObject.SetActive (false);
	}


	public void AddScore(int points) {
		score += points;
		scoreValueDisplay.text = "Score: " + score.ToString ();
		//audioManager.PlaySound ("soundWow");

		if (score >= currentLevel * 10) {
			spawnFromPool.StopSpawning ();

			MM.toWin ();
		}
	}
	public void TakeDamage (int damage) {
		HP -= damage;
		if (HP <= 0) {
			EndGame ();
		}
		castleHP.text = "HP: " + HP.ToString ();
	}

	public void StartGame (){
		MM.ChangeMenu (MenuManager.GameState.Game);
		spawnFromPool.StartSpawning ();

	}

	private void EndGame() {
		Debug.Log ("================GAME OVER ");
		Time.timeScale = 0.1f;
		spawnFromPool.StopSpawning ();

		MM.toGameOver ();
	}

	// TODO: пересмотреть рестарт гейм без лоад сцене
	public void RestartGame() {
		MM.ChangeMenu (MenuManager.GameState.Game);
		SceneManager.LoadScene ("Main");
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
