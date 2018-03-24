using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    public GameManager gameController;

	void Awake () {
        if (!GameManager.instance) {
			Instantiate (gameController);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
