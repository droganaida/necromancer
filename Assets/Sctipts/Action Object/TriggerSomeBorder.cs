﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSomeBorder : MonoBehaviour {

	public GameController gameController;
	public string nameTrigger;

	void OnTriggerEnter2D(Collider2D something)	{
		// левая сторна екрана, наш замок
		if (nameTrigger == "left") {
			if (something.gameObject.CompareTag ("Enemy")) {
				something.gameObject.GetComponent<DieOnHit> ().Portal ();
			}
		}

		// правая сторона екрана, пули уничтожаются
		if (nameTrigger == "right") {
			if (something.gameObject.CompareTag ("Bullet")) {
				gameController.RemoveBulletFromList (something.gameObject);
				something.gameObject.SetActive (false);
				something.gameObject.GetComponent<PooledObject> ().pool.ReturnObject (something.gameObject);
			}
		}
	}

}
