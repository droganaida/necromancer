using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using UnityEngine.EventSystems;
 

public class PartWave  {
	public LevelManager.EnemiesUnit UnitName { get; set; }
	public int UnitCount { get; set; }
}

public class Wave  {
	public List<PartWave> dataWaves = new List<PartWave> ();
	public List<LevelManager.EnemiesUnit> goodWave = new List<LevelManager.EnemiesUnit> ();
}

public class LevelBase {
	public List<Wave> dataLevel;
	public int[,] data;
	public int sizeRow = 3;
	public string backName;

	public LevelBase () {
		// TODO: maybe something seting?
		//init ();
	}

	public void init (){
		int countCells = data.Length;
		Debug.Log (countCells);

		dataLevel = new	List<Wave> ();
		Wave newWave = null;
		int count = countCells / sizeRow;
		int prevWavePart = 0;
		for (int i = 0; i < count; i++) {
			// если номер волны больше предыдущей волны, то создаем новую волну
			if (data [i,0] > prevWavePart) {
				prevWavePart = data[i,0];
				// TODO: может просто пихать при создании?
				// первая волна
				if (newWave != null) {
					Debug.Log ("newWave.Count " + newWave.dataWaves.Count);
					Debug.Log ("goodWave.Count " + newWave.goodWave.Count);
					dataLevel.Add (newWave);
				}
				newWave = new Wave ();
			}

			newWave.dataWaves.Add (new PartWave() {UnitName=(LevelManager.EnemiesUnit)data[i,1], UnitCount=data[i,2]});
			for (int j = 0; j < data[i,2]; j++) {
				newWave.goodWave.Add ((LevelManager.EnemiesUnit)data[i,1]);
			}

			// последняя волна
			if (i == count - 1) {
				Debug.Log ("newWave.Count " + newWave.dataWaves.Count);
				Debug.Log ("goodWave.Count " + newWave.goodWave.Count);
				dataLevel.Add (newWave);
			}
		}
		Debug.Log ("dataLevel.Count " + dataLevel.Count);

//		int count2 = 0;
//		foreach (Wave item in dataLevel) {
//			Debug.Log ("Wave #" + ++count2 );
//			foreach (LevelManager.EnemiesUnit uName in item.goodWave) {
//				Debug.Log (uName);
//			}
//		}
	}
	// список волн
	public List <Wave> GetDataLevel () {
		return dataLevel;
	}
	//список юнитов в волне с номером инт
	public List <LevelManager.EnemiesUnit> GetDataWaveOfIndex (int num) {
		if (num > dataLevel.Count-1 || num < 0) {
			Debug.Log ("num (" +num+") NOT IN dataLevel.Count ("+ dataLevel.Count+")");
			return null;
		}
		return dataLevel [num].goodWave;
	}
	// количество волн в уровне
	public int GetCountWaves (){
		return dataLevel.Count;
	}
}




public class LevelManager : MonoBehaviour {
	public float spawnRate = 3f;
	public SimpleObjectPool[] objectPools;
	public Transform[] spawnPoints;
	private Transform enemies;

	private Coroutine spawnRoutine = null;

	private GameController gC;
	private MenuManager MM;
	private int maxLevel = 20;
	private LevelBase newLevel = null;
	public List <LevelManager.EnemiesUnit> listNowWave; // список юнитов текущей волны
	private int indexWave = 0;	// номер текущей волны
	private int timeBetweenWaves = 10;
	private int timerCount = 0;
	private Color originalScoreTextColor;	// оригинальный цвет ScoreText
	private bool stopTimer = false;	// признак таймера

	public enum EnemiesUnit {
		Peasant = 1,
		Melnik = 2,
		Kuznets = 3
	}
	
	void Awake () {
		gC = GetComponent<GameController> ();
		MM = GetComponent<MenuManager> ();

		// TODO: collect all pool
		print ("spawnFromPool: "+this.gameObject.transform.childCount);
		int childCount = this.gameObject.transform.childCount;
		for (int i = 0; i < childCount; i++) {
			gC.allPool.Add (this.gameObject.transform.GetChild(i).gameObject);
		}

		InitLevel ();
	}

	private void InitLevel() {
		if (enemies != null) {
			Destroy (GameObject.Find("Enemies"));
		}
		enemies = new GameObject ("Enemies").transform;

		originalScoreTextColor = MM.scoreValueDisplay.GetComponent<Text> ().color;
	}

	// создаем обьект уровня
	private LevelBase GetLevel (int lev) { //LevelBase
		if (lev < 0 && lev > maxLevel) {
			return null; // null
		}

		switch (lev) {
		case 1:
			return new Level01 ();
			// TODO: заменить на Action из строки? "Level"+lev
		case 2:
			return new Level02 ();
		case 3:
			return new Level03 ();
		case 4:
			return new Level04 ();
		case 5:
			return new Level05 ();
		case 6:
			return new Level06 ();
		case 7:
			return new Level07 ();
		case 8:
			return new Level08 ();
		case 9:
			return new Level09 ();
		case 10:
			return new Level10 ();
		default:
			print ("GetLevel return null");
			return null;
		}
	}

	// загружает данные волны
	private bool nextWave (int indexW) {
		listNowWave = newLevel.GetDataWaveOfIndex (indexW);
		if (listNowWave != null) {
			print ("listNowWave.Count " + listNowWave.Count);
			return true;
		}
		return false;
	}

	// запускает уровень
	public void LoadLevel (int lev){
		//
		// some background sprite setting?
		//
		newLevel = GetLevel (lev);
		if (newLevel == null) {
			Debug.Log ("newLevel == null");
			return;
		}

		StartTimer ();
		//StartSpawning ();
	}

	// таймер отсчета времени до начала волны
	public void StartTimer () {
		print ("StartTimer");
		// если фолс то таймер не запускаем
		if (!nextWave (indexWave))
			return;

		gC.setIsGame (false);
		timerCount = timeBetweenWaves;
		stopTimer = false;
		MM.changeScoreButtonActive (true);
		MM.setTextWaveTxt ("Wave " + (indexWave+1).ToString() + "/" + newLevel.GetCountWaves().ToString());
		MM.scoreValueDisplay.GetComponent<Text>().color = Color.red;
		StartCoroutine (TimerLoop());
	}
	// остановка таймера начала волны
	public void StopTimer () {
		print ("click StopTimer");
		StopCoroutine (TimerLoop ());
		stopTimer = true;

		MM.changeScoreButtonActive (false);
		MM.scoreValueDisplay.text = "Score: " + gC.getScore().ToString ();
		StartCoroutine (SpawnLoop ());
	}
	// таймер до начала войны
	IEnumerator TimerLoop ()  {
		while (timerCount > 0) {
			if (stopTimer) {
				yield break;
			}
			MM.scoreValueDisplay.text = "Timer: " + timerCount.ToString ();
			yield return new WaitForSeconds (1);

			timerCount--;
		} 

		print ("end TimerLoop");
		MM.changeScoreButtonActive (false);
		MM.scoreValueDisplay.text = "Score: " + gC.getScore().ToString ();
		StartCoroutine (SpawnLoop ());
	}

	// проверка на конец волны
	public bool CheckEndWave () {
		if (listNowWave.Count > 0) {
			return false;
		}
		return true;
	}
	// проверка на окончание всех волн
	public bool CheckEndAllWave () {
		if (indexWave < newLevel.GetCountWaves ()) {
			return false;
		}
		return true;
	}

	public void StopSpawning() {
		StopCoroutine (SpawnLoop ());
	}

	// запуск волны
	IEnumerator SpawnLoop ()  {
		// вернем нашей текстовой метке нормальный цвет
		MM.scoreValueDisplay.GetComponent<Text>().color = originalScoreTextColor;
		gC.setIsGame (true);

		while (true) {

			int randomIndex = -1;
			if (listNowWave.Count > 0) {
				randomIndex = UnityEngine.Random.Range (0, listNowWave.Count - 1);
			} else {
				print ("listNowWave.Count = 0  indexWave++");

				indexWave++;
				if (indexWave < newLevel.GetCountWaves ()) {
					StopSpawning ();
				} else {
					print ("END ALL WAVES ON LEVEL");
				}

				yield break;
			}

			int indexPool = -1;
			switch (listNowWave [randomIndex]) {
			case LevelManager.EnemiesUnit.Peasant :
				// TODO: можно заменить напрямую на (int)Enum без свитча
				indexPool = 0;
				break;
			case LevelManager.EnemiesUnit.Melnik :
				indexPool = 1;
				break;
			default:
				break;
			}
			if (indexPool == -1) {
				print ("indexPool == -1");
				yield break;
			}
			listNowWave.RemoveAt (randomIndex);
			GameObject clone = objectPools[indexPool].GetObject();
			clone.SetActive (true);
			int randomSpawnPointIndex = UnityEngine.Random.Range (0, spawnPoints.Length);
			clone.transform.position = spawnPoints [randomSpawnPointIndex].position;
			clone.transform.SetParent (enemies);
			DieOnHit dieOnHit = clone.GetComponent<DieOnHit> ();
			dieOnHit.SetGameController (gC);
			gC.AddEnemyToList (clone);

			yield return new WaitForSeconds (UnityEngine.Random.Range(1f,spawnRate));
		}
	}
}


