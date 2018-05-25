using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public List<GameObject> allBullets = new List<GameObject>();
	public List<GameObject> allEnemies = new List<GameObject>();
	public List<GameObject> allEffects = new List<GameObject>();
	public List<GameObject> allPool = new List<GameObject> ();

	private int score = 0;
	private int maxHP = 20; //
	private int HP;
	private LevelManager LM;
	private MenuManager MM;
	public int currentLevel = 1;
	public int unlockLevel = 1;
	private AudioManager audioManager;
	public SimpleObjectPool[] objectPool;
	private PooledObject pooledObject;
	private Transform enemies;
	private int gameState;
	private bool isWave = false;
	private bool isGame = false;


	void Awake () {
		LM = GetComponent<LevelManager> ();
		MM = GetComponent<MenuManager> ();

		if (PlayerPrefs.HasKey ("currentLevel")) {
			currentLevel = PlayerPrefs.GetInt ("currentLevel");
		}
		if (PlayerPrefs.HasKey ("unlockLevel")) {
			unlockLevel = PlayerPrefs.GetInt ("unlockLevel");
		}

	}

	void Start() {
		// caching
		audioManager = AudioManager.instance;
		if (audioManager == null) {
			Debug.LogError ("Warning. Not found AudioManager on scene");
		}


/*
у нас есть 2 состояния: запуск игры и перезапуск уровня. но при этом сцена всего одна.
поэтому нам нужен признак который будет регулировать загрузку сплешскрина или уровня.
*/
		// TODO: возможно переделать состояние SplashScreen отдельной сценой. SplashScreen -> Main
		if (PlayerPrefs.HasKey ("SplashScreen")) {
			if (PlayerPrefs.GetInt ("SplashScreen") == 1) {
				PlayerPrefs.SetInt ("SplashScreen", 0);
				MM.ChangeMenu (MenuManager.GameState.SplashScreen);
			} else {
				// признак NextGame or ReatartGame 
				if (PlayerPrefs.HasKey ("RestartGame")) {
					if (PlayerPrefs.GetInt ("RestartGame") == 1) {
						PlayerPrefs.SetInt ("RestartGame", 0);
						MM.ChangeMenu (MenuManager.GameState.Game);
						StartGame ();
					} else {
						MM.ChangeMenu (MenuManager.GameState.ChooseLevel);
					}
				}
			}
		} else { // PlayerPrefs.HasKey ("SplashScreen")
			PlayerPrefs.SetInt ("SplashScreen", 0);
			MM.ChangeMenu (MenuManager.GameState.SplashScreen);
		}
		PlayerPrefs.Save ();

		//LM.LoadLevel (1);

		//StartGame ();
	}

	public bool getStatusGame (){
		return isGame;
	}

	public void setIsGame (bool b) {
		isGame = b;
	}

	public int getScore (){
		return score;
	}

/*
https://docs.unity3d.com/ru/current/Manual/ExecutionOrder.html
OnApplicationQuit: Эта функция вызывается для всех игровых объектов перед тем, как приложение закрывается. 
В редакторе вызывается тогда, когда игрок останавливает игровой режим. 
В веб-плеере вызывается по закрытия веб окна.
*/
	void OnApplicationQuit () {
		// делаем признак что при след загрузке сплеш скрин отобразится,а не сразу выбор уровней
		PlayerPrefs.SetInt ("SplashScreen", 1);
		PlayerPrefs.SetInt ("RestartGame", 0);
		PlayerPrefs.Save ();
	}

	void Update () {
		#if UNITY_ANDROID && !UNITY_EDITOR
		//if running on Android, check for Menu/Home and exit
		if (Application.platform == RuntimePlatform.Android) {
			if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Menu)) {
				MM.toGameQuit ();
			}
		}
		#endif

	}

	public void StartGame () {
		HP = maxHP;

		// TODO: все это переместить в ui-controller
		MM.scoreValueDisplay.text = "Score: " + score.ToString ();
		MM.castleHP.text = HP.ToString ();
		MM.curLevel.text = "Lev: " + currentLevel.ToString ();
		MM.unlockLev.text = "ULev: " + unlockLevel.ToString ();

		LM.LoadLevel (currentLevel);
	}

	// TODO: пересмотреть рестарт гейм без лоад сцене
	public void RestartGame() {
		PlayerPrefs.SetInt ("RestartGame", 1);
		PlayerPrefs.Save ();

		SceneManager.LoadScene ("Main");
	}

	private void EndGame() {
		Debug.Log ("================GAME OVER ");
		Time.timeScale = 0.1f;
		LM.StopSpawning ();

		MM.toGameOver ();
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
		} else {
			return;
		}
		clone.GetComponent<EndAnimation> ().SetGameController (this);
		AddEffectToList (clone);
	}

	// очистка открытых уровней
	public void clearAllLevel (){
		PlayerPrefs.SetInt ("unlockLevel", 1);
		PlayerPrefs.Save ();
		// save произойдет ниже
		setLevel (1);
	}
	// установка текущего уровня
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
			//unlockLevel++;
			PlayerPrefs.SetInt ("unlockLevel",unlockLevel);
		}
		PlayerPrefs.SetInt ("currentLevel",currentLevel);
		PlayerPrefs.Save ();
	}

	public void decLevel () {
		if (currentLevel > 1) {
			currentLevel--;
		}
		PlayerPrefs.SetInt ("currentLevel",currentLevel);
		PlayerPrefs.Save ();
	}

	// списки с обьектами, для дальнейшей с ним работой...
	// массовое удаление, заморозка, ...
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

	// убрать все с игрового поля
	// TODO: осторожно! стремно работает :)
	//
	public void ClearAllList (){
		// TODO: а не проще удалить контейнер?

		// удаляем врагов
		while (allEnemies.Count > 0) {
			if (allEnemies[0].GetComponent<DieOnHit> ()) {
				allEnemies[0].GetComponent<DieOnHit> ().ResetUnit ();
				break;
			}
		}

		// удаляем пули
		while (allBullets.Count > 0) {
			Destroy (allBullets[0].gameObject);
		}
		allBullets.Clear ();

		// удаляем эффекты
		while (allEffects.Count > 0) {
			Destroy (allEffects[0].gameObject);
		}
		allEffects.Clear ();
	}

	// очисктка пулов
	// TODO: а может и не стоит полностью их чистить, а только частично по потребностям уровня?
	private void ClearAllPool () {
		foreach (var item in allPool) {
			while (item.gameObject.transform.childCount > 0) {
				Destroy(item.gameObject.transform.GetChild (0).gameObject);
			}
		}
	}

	// wait some time, deactivate object and return object in pool
	IEnumerator DelayDeactivate(GameObject deactivatedObject, float delayTime) {
		yield return new WaitForSeconds (delayTime);//.25f

		RemoveEffectFromList(deactivatedObject);
		pooledObject = deactivatedObject.GetComponent<PooledObject> ();
		pooledObject.pool.ReturnObject (deactivatedObject);
		deactivatedObject.SetActive (false);
	}

	// for end animation event
	public void Deactivate(GameObject deactivatedObject) {
		RemoveEffectFromList(deactivatedObject);
		pooledObject = deactivatedObject.GetComponent<PooledObject> ();
		pooledObject.pool.ReturnObject (deactivatedObject);
		deactivatedObject.SetActive (false);
	}

	// ведем счет нашим очкам
	public void AddScore(int points) {
		score += points;
		MM.scoreValueDisplay.text = "Score: " + score.ToString ();
		//audioManager.PlaySound ("soundWow");

		StartCoroutine (CheckGame());
	}

	IEnumerator CheckGame (){
		yield return new WaitForSeconds (0.3f);

		// проверка на конец уровня
		if (allEnemies.Count == 0  && LM.CheckEndAllWave()) { 
			WinGame ();
		}

		// проверка на следующую волну
		if (allEnemies.Count == 0 && LM.CheckEndWave ()){
			LM.StartTimer();
		}
	}

	public void WinGame (){
		// переключаем маркер на следующий уровень
		incLevel ();
		// начисление звезд
		int scull = 0;
		if (HP == maxHP) {
			scull = 3;
		} else if ((float)HP >= (float)maxHP * 0.7f) {
			scull = 2;
		} else {
			scull = 1;
		}

		// сохраняем значение в пользовательские данные
		string nameLevel = "Level" + (currentLevel-1).ToString ();
		if (PlayerPrefs.HasKey (nameLevel)) {
			if (PlayerPrefs.GetInt(nameLevel) < scull) {
				PlayerPrefs.SetInt (nameLevel, scull);
			}
		} else {
			PlayerPrefs.SetInt (nameLevel, scull);
		}
		PlayerPrefs.Save ();

		MM.toWin ();
	}

	// урон по базе
	public void TakeDamage (int damage) {
		HP -= damage;
		if (HP <= 0) {
			EndGame ();
		}
		MM.castleHP.text = HP.ToString ();

		StartCoroutine (CheckGame());
	}

	// ремонт базы
	public void RepairCastle (int repair){
		HP += repair;
		if (HP > maxHP) { // а может и не проверять? типа бонусные там, сколько насобирал то и пусть будет?
			HP = maxHP;
		}
	}

	// проверка что под курсором: игровой обьект или гуи
	public bool CursorOverUI() {
		#if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR)
		int cursorID = Input.GetTouch(0).fingerId;
		return EventSystem.current.IsPointerOverGameObject(cursorID);
		#else
		return EventSystem.current.IsPointerOverGameObject();
		#endif
	}


}
