using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {

	public GameObject bulletPrefab;
	public Transform bulletSpawnPoint;
	public GameObject cannonBall;
	public GameObject playerRig;
	private Animator playerAnimator;

	void Awake()
	{
		playerAnimator = playerRig.GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetButtonDown ("Fire1") && !cannonBall.activeSelf) 
		{
			//playerAnimator.SetTrigger ("NancyAttack");
			playerAnimator.Play ("NancyAttack");
			ShootBullet ();

		}	
	}

	void ShootBullet()
	{
		cannonBall.gameObject.SetActive (true);
		cannonBall.transform.position = bulletSpawnPoint.position;
	}
}
