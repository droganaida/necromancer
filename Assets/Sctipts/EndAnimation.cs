using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAnimation : MonoBehaviour {
	private GameController gc;

	public void deactivate () {
		gc.Deactivate (this.gameObject);
	}

	public void SetGameController(GameController originalGC) {
		gc = originalGC;
	}

}
