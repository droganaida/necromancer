using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGameOver : MonoBehaviour {

	public GameController gameController;


	void OnTriggerEnter2D(Collider2D something)	{
		if (something.gameObject.CompareTag ("Enemy")) {
			something.gameObject.GetComponent<DieOnHit> ().Portal ();
		}
		//gameController.EndGame ();
	}

}
