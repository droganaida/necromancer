using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour {

	public float timeToWait = 3f;
	private GameController gameController;

	private WaitForSeconds waitForTime;
	void Awake()
	{
		waitForTime = new WaitForSeconds (timeToWait);
	}

	void OnEnable()
	{
		StartCoroutine (WaitAndDeactivate ());
	}

	void OnDisable()
	{
		StopAllCoroutines ();
	}

	// Use this for initialization
	IEnumerator WaitAndDeactivate () 
	{
		yield return waitForTime;

		gameController.RemoveBulletFromList (this.gameObject);
		gameObject.SetActive (false);
		gameObject.GetComponent<PooledObject> ().pool.ReturnObject (this.gameObject);
	}

}
