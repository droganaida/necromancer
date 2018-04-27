using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOnHit : MonoBehaviour {

	public GameObject playerRig;
	public int pointsValue = 1;
	public int healthPoints = 1;
	private int currentHealthPoints;

	private GameController gameController;
	private PooledObject pooledObject;
	private AutoMover mover;
	private Animator animator;

	public int typeUnit;
	private string nameTrigger1 = "Go";
	private string nameTrigger2 = "GoDamage";
	public GameObject healthBar;

	void Awake() {
		
		mover = GetComponent<AutoMover> ();
		animator = playerRig.GetComponent<Animator> ();
		animator.SetTrigger (nameTrigger1);
		//animator.SetTrigger ("PeasantGoDamage2");


	}

	IEnumerator animateMe()
	{
		Debug.Log ("================animateMe");
		yield return new WaitForSeconds (2f);
		switch (typeUnit) {
		case 1:
			animator.Play ("PeasantGo");
			break;
		case 2:
			animator.Play ("MillerGo-1");
			break;
		default:
			animator.Play ("PeasantGo");
			break;
		}
	}

	void OnEnable() {
		playerRig.SetActive (true);
		currentHealthPoints = healthPoints;
		animator.SetTrigger (nameTrigger1);
		//StartCoroutine (animateMe ());

		healthBar.GetComponent<HealthBar>().SetHB(1f);
	}

	public void SetGameController(GameController controllerFromSpawner)
	{
		gameController = controllerFromSpawner;
	}

	// TODO: это как бы нужно убрать)
//	void OnMouseDown()
//	{
//		ResetUnit ();
//	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag ("Bullet")) 
		{
			// TODO: логику чо делать с обьектом, должен делать сам обьект...
			gameController.RemoveBulletFromList (collision.gameObject);
			collision.gameObject.SetActive (false);
			collision.gameObject.GetComponent<PooledObject> ().pool.ReturnObject (collision.gameObject);

			// TODO: change on weapon damage
			StartCoroutine (charDamage (1));
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

		float scaleHB = (float)currentHealthPoints / (float)healthPoints;
		healthBar.GetComponent<HealthBar>().SetHB(scaleHB);
		//print ("scaleHB : "+ scaleHB);

		if (currentHealthPoints <= 0) {
			UpdateScore ();

			ResetUnit ();
		} else {
			// TODO: повреждения всеже привести в порядок. эдак 0-50% 
			if ((float)currentHealthPoints / (float)healthPoints <= 0.5f) {
				animName = nameTrigger2;// + currentHealthPoints;
				animator.SetTrigger (animName);
			}
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

	public void Portal () {
		gameController.TakeDamage (currentHealthPoints);

		ResetUnit ();
	}

	public void ResetUnit () {
		animator.SetTrigger (nameTrigger1);
		playerRig.SetActive (false);

		mover.Stop ();
		currentHealthPoints = healthPoints;
		//healthBar.GetComponent<HealthBar>().SetHB(1f);
		gameController.Boom (this.gameObject.transform.position);
		gameController.RemoveEnemyFromList (this.gameObject);

		pooledObject = GetComponent<PooledObject> ();
		pooledObject.pool.ReturnObject (this.gameObject);
	}
}
