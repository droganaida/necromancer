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
			// проверка на нижнюю границу перемещения
			if (newPos.y < bottom){
				newPos.y = bottom;
			}
			// проверка на вехнюю границу перемещения
			if (newPos.y > up){
				newPos.y = up;
			}
			// если новая позиция игрока не изменилась, то и нечего включать анимацию и изменять позицию
			if (newPos != transform.position) {
				transform.position = newPos;
				animator.Play ("NancyMove");
			}

		}
	}

}
