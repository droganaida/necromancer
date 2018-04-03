using UnityEngine;

public class PlayerMover : MonoBehaviour {

	public GameObject playerRig;
	private Animator animator;

	public float speed = 2f;
	public Transform upMovementDot;
	public Transform bottomMovementDot;

	private float bottom;
	private float up;

	void Awake() {
		animator = playerRig.GetComponent<Animator> ();

		bottom = bottomMovementDot.transform.position.y;
		up = upMovementDot.transform.position.y;
	}


	void Update ()  {
		float vAxis = Input.GetAxisRaw ("Vertical"); 
		if (vAxis != 0f) {
			Vector3 newPos = transform.position + new Vector3 (0, vAxis * speed * Time.deltaTime, 0);
			if (newPos.y < bottom){
				newPos.y = bottom;
			}
			if (newPos.y > up){
				newPos.y = up;
			}
			if (newPos != transform.position) {
				transform.position = newPos;
				animator.Play ("NancyMove");
			}

		}
	}

}
