using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledSpawner : MonoBehaviour {

	public float spawnRate = 3f;
	public SimpleObjectPool[] objectPools;
	public Transform[] spawnPoints;
	private Transform enemies;
	public GameController gameController;

	private Coroutine spawnRoutine = null;


	public void StartSpawning() {
		if (enemies != null) {
			Destroy (GameObject.Find("Enemies"));
		}
		enemies = new GameObject ("Enemies").transform;

		spawnRoutine = StartCoroutine (SpawnLoop());
	}

	public void StopSpawning()
	{
		StopCoroutine (spawnRoutine);
	}

	IEnumerator SpawnLoop () 
	{
		while (true) 
		{
			int randomPoolIndex = Random.Range (0, objectPools.Length);
			GameObject clone = objectPools[randomPoolIndex].GetObject();
			clone.SetActive (true);
			int randomSpawnPointIndex = Random.Range (0, spawnPoints.Length);
			clone.transform.position = spawnPoints [randomSpawnPointIndex].position;
			clone.transform.SetParent (enemies);
			DieOnHit dieOnHit = clone.GetComponent<DieOnHit> ();
			dieOnHit.SetGameController (gameController);
			gameController.AddEnemyToList (clone);
			yield return new WaitForSeconds (Random.Range(1f,spawnRate));

		}
	}
}
