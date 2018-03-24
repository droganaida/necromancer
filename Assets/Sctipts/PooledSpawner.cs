using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledSpawner : MonoBehaviour {

	public float spawnRate = 3f;
	public SimpleObjectPool[] objectPools;
	public Transform[] spawnPoints;
	public GameController gameController;

	private Coroutine spawnRoutine = null;

	public void StartSpawning()
	{
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
			DieOnHit dieOnHit = clone.GetComponent<DieOnHit> ();
			dieOnHit.SetGameController (gameController);
			yield return new WaitForSeconds (Random.Range(1f,spawnRate));

		}
	}
}
