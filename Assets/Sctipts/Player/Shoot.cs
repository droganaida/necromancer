using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {

	public GameObject bulletPrefab;
	public Transform bulletSpawnPoint;
	public GameObject cannonBall;
	public GameObject playerRig;
	private Animator playerAnimator;

	public float weaponCooldown = 0.3f;
	private float timerFire;	// 
	public SimpleObjectPool[] objectPool;
	public GameController gameController;
	private Transform bullets;

	void Awake() {
		playerAnimator = playerRig.GetComponent<Animator> ();
		timerFire = weaponCooldown;

		// создаем контейнер для пуль и кастов
		if (bullets != null) {
			Destroy (GameObject.Find("Bullets"));
		}
		bullets = new GameObject ("Bullets").transform;

	}

	// Update is called once per frame
	void Update ()  {
		timerFire -= Time.deltaTime;

		// timerFire < 0.0f - время калдауна
		// TODO: if GUI dont shoot
		if (Input.GetButtonDown ("Fire1") && timerFire < 0.0f) {
			playerAnimator.Play ("NancyAttack"); //NancyAttack NancyCast
			timerFire = weaponCooldown;

			ShootBullet ();
		}	
	}

	void ShootBullet() {
		GameObject clone = objectPool[0].GetObject();
		clone.SetActive (true);
		clone.transform.position = bulletSpawnPoint.position;
		clone.transform.SetParent (bullets);
		gameController.AddBulletToList (clone);
	}
}
