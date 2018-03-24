using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOnHit : MonoBehaviour {

	public GameObject explosion;
	public GameObject playerRig;
	public int pointsValue = 1;
	public int healthPoints = 1;
	private int currentHealthPoints;

	private GameController gameController;
	private PooledObject pooledObject;
	private AutoMover mover;
	private Animator animator;

	void Awake()
	{
		mover = GetComponent<AutoMover> ();
		animator = playerRig.GetComponent<Animator> ();
		animator.SetTrigger ("PeasantGo");
		//animator.SetTrigger ("PeasantGoDamage2");
	}

	IEnumerator animateMe()
	{
		Debug.Log ("================animateMe");
		yield return new WaitForSeconds (2f);
		animator.Play ("PeasantGo");
	}

	void OnEnable()
	{
		explosion.SetActive (false);
		playerRig.SetActive (true);
		currentHealthPoints = healthPoints;
		//animator.SetTrigger ("PeasantGoDamage2");
		//StartCoroutine (animateMe ());
	}

	public void SetGameController(GameController controllerFromSpawner)
	{
		gameController = controllerFromSpawner;
	}


	void OnMouseDown()
	{
		playerRig.SetActive (false);
		explosion.SetActive (true);
		StartCoroutine(DelayDeactivate(explosion));
		UpdateScore ();
		mover.Stop ();
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag ("Bullet")) 
		{
			collision.gameObject.SetActive (false);
			StartCoroutine (charDamage (1));
			/* knightSprites.SetActive (false);
			explosion.SetActive (true);
			StartCoroutine(DelayDeactivate(explosion));
			UpdateScore ();
			mover.Stop (); */
		}
	}

	IEnumerator DelayDeactivate(GameObject deactivatedObject)
	{
		yield return new WaitForSeconds (.25f);
		deactivatedObject.SetActive (false);
		pooledObject = GetComponent<PooledObject> ();
		pooledObject.pool.ReturnObject (this.gameObject);
	}

	string animName;

	IEnumerator charDamage(int damage)
	{
		currentHealthPoints = currentHealthPoints - damage;
		//animator.SetTrigger("Damage");

		if (currentHealthPoints <= 0) {
			//animator.SetTrigger ("PeasantGo");
			playerRig.SetActive (false);
			//animator.Play ("PeasantGo");
			explosion.SetActive (true);
			StartCoroutine (DelayDeactivate (explosion));
			UpdateScore ();
			mover.Stop ();
			currentHealthPoints = healthPoints;
		} else {
			animName = "PeasantGoDamage" + currentHealthPoints;
			animator.SetTrigger(animName);
		}

		yield return null;

	}

	public void UpdateScore()
	{
		if (gameController) 
		{
			gameController.AddScore (pointsValue);
		}
	}
}
