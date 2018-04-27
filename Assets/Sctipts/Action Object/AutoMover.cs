using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMover : MonoBehaviour {

	private Rigidbody2D rb2d;
	private GameController gameController;
	private bool moving;

	public float moveSpeed = .01f;
	public string directionName;
	private Vector2 direction;
//	Vector2 min;
//	Vector2 max;

	// Use this for initialization
	void Awake () {
		rb2d = GetComponent<Rigidbody2D> ();

		if (directionName == "left") {
			direction = Vector2.left;
		} else if (directionName == "right") {
			direction = Vector2.right;
		}
	}

//	void Start (){
//		min = Camera.main.ViewportToWorldPoint (new Vector2 (0, 0)); // bottom-left (corner) of the screen
//		max = Camera.main.ViewportToWorldPoint (new Vector2 (1, 1)); // top-right (corner) of the screen
//		print (max);
//
//	}

	void OnEnable() {
		moving = true;
	}

	void OnDisable() {
		moving = false;
	}

	public void Stop() {
		moving = false;
	}

	void FixedUpdate() {
		if (moving) {
			rb2d.MovePosition ((Vector2) transform.position + direction * moveSpeed);
		}

	}
//
//	void Update (){
//		if (moving) {
//			if (gameObject.CompareTag ("Bullet")) {
//				print (gameObject.transform.position.x + "   " + max.x);
//				if (gameObject.transform.position.x > max.x) {
//					gameController.RemoveBulletFromList (this.gameObject);
//					this.gameObject.SetActive (false);
//					gameObject.GetComponent<PooledObject> ().pool.ReturnObject (this.gameObject);
//				}
//			}
//		}
//	}
}
