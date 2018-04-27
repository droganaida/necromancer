using UnityEngine;

public class LevelManager : MonoBehaviour {
	public GameObject peasantPrefab; // Transform?

	private GameController engine;

	public enum EnemiesUnit {
		Peasant = 0,
		Melnik = 1,
		Kuznets = 2
	}
	
	void Awake () {
		engine = GetComponent<GameController> ();

	}

	public void loadLevel (int lev){
		
	}
}


