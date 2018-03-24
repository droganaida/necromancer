using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour {

	public float moveDuration = .05f;

	public WayPoint currentWayPoint;
	private WayPoint nextWayPoint;
	private bool moving;
	private float nextMoveTime;
	public GameObject playerRig;
	private Animator animator;

	void Awake()
	{
		animator = playerRig.GetComponent<Animator> ();
	}

	void Update () 
	{
		if (Time.time > nextMoveTime) 
		{
			nextMoveTime = Time.time + moveDuration;

			if (Input.GetAxisRaw("Vertical") == 1f)
			{
				if (currentWayPoint.wayPointAbove != null)
					nextWayPoint = currentWayPoint.wayPointAbove;
			}

			if (Input.GetAxisRaw("Vertical") == -1f) 
			{
				if (currentWayPoint.wayPointBelow != null)
					nextWayPoint = currentWayPoint.wayPointBelow;
			}

			if (!moving && nextWayPoint != null) 
			{
				StartCoroutine (TransitionWaypoints ());
			}

		}

	}

	IEnumerator TransitionWaypoints()
	{

		float elapsedTime = 0;
		moving = true;
		animator.Play("NancyMove");

		while (elapsedTime <= moveDuration) 
		{
			transform.position = Vector2.Lerp (currentWayPoint.transform.position, nextWayPoint.transform.position, (elapsedTime / moveDuration));
			elapsedTime += Time.deltaTime;
			yield return null;
		} 

		currentWayPoint = nextWayPoint;
		nextWayPoint = null;
		moving = false;
	}

}
