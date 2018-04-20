using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class Menu {
	string name;
	List<Button> listButton = new List<Button> ();
	//List<Action> listAction;

}

public class MenuManager : MonoBehaviour {

	private GameController GC;

	// menu elements
	public GameObject panelGame;
	public GameObject panelMenu;
	public GameObject panelButtonMenu;
	public GameObject panelSelectMenu;
	public Button btnMenu1;
	public Button btnMenu2;
	public Button btnMenu3;
	public Button btnMenu4;
	public Button btnMenu5;
	public Button btnPause;
	public Button btnRestart;

	private GameState nowMenu = 0;
	private GameState prevMenu = 0;

	public enum GameState {
		SplashScreen = 0,
		MainMenu = 1,
		ChooseLevel = 2,
		Game = 4,
		Pause = 5,
		Win = 6,
		GameOver = 7,
		Settings = 8,
		Update = 9,
		Achivment = 10,
		MoreGames = 11,
		Credits = 12
	}

	Dictionary<string, UnityAction> dicUA = new Dictionary<string, UnityAction>();
	Dictionary<Button, UnityAction> listButton = new Dictionary<Button, UnityAction>();


	void Awake () {
		GC = GetComponent<GameController> (); 

		dicUA.Add ("Main Menu", toMainMenu);
		dicUA.Add ("Select Menu", toSelectMenu);
		dicUA.Add ("Settings", toSettings);
		dicUA.Add ("Credits", toCredits);
		dicUA.Add ("More Games", toMoreGames);
		dicUA.Add ("Quit Game", toGameQuit);
		dicUA.Add ("Pause", toPauseGame);
		dicUA.Add ("Resume", toResumeGame);
		dicUA.Add ("Restart", GC.RestartGame);

		dicUA.Add ("Next", toResumeGame);
		dicUA.Add ("New Game", toResumeGame);
		dicUA.Add ("Continue", toResumeGame);
		dicUA.Add ("Start Game", toResumeGame);

		// init all menu
	}

	// Use this for initialization
	void Start () {
		
	}

	void setButtonElement (Button btn, string txt, UnityAction ua){
		btn.gameObject.SetActive (true);
		btn.GetComponentInChildren<Text> ().text = txt;
		btn.onClick.AddListener (ua);
		listButton.Add (btn, ua);
	}

	void removeButtonListener (Button btn, UnityAction ua){
		btn.onClick.RemoveListener (ua);
	}

	public void ChangeMenu (GameState menu) {
		print ("GameState: " + menu);
		// TODO: maybe stack<menu> ?
		prevMenu = nowMenu;
		nowMenu = menu; 

		// disable all
		panelGame.SetActive (false);
		panelMenu.SetActive (false);
		panelSelectMenu.SetActive (false);
		panelButtonMenu.SetActive (false);
		btnMenu1.gameObject.SetActive (false);
		btnMenu2.gameObject.SetActive (false);
		btnMenu3.gameObject.SetActive (false);
		btnMenu4.gameObject.SetActive (false);
		btnMenu5.gameObject.SetActive (false);

		// отписались от всех слушателей
		foreach( KeyValuePair<Button, UnityAction> kvp in listButton ) {
			kvp.Key.onClick.RemoveListener (kvp.Value);
		}
		listButton.Clear();

		Time.timeScale = 0.0f;

		switch (nowMenu) {
		// int something = (int) Question.Role;
		case GameState.SplashScreen:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);

			setButtonElement (btnMenu5, "PLAY", toMainMenu);
			break;

		case GameState.MainMenu:
//			if (prevMenu == GameState.SplashScreen) {
//				btnMenu5.onClick.RemoveListener (toMainMenu);
//			}
//			if (prevMenu == GameState.Win) {
//				btnMenu1.onClick.RemoveListener (toResumeGame);
//				btnMenu2.onClick.RemoveListener (GC.RestartGame);
//				btnMenu5.onClick.RemoveListener (toMainMenu);
//			}
//			if (prevMenu == GameState.GameOver) {
//				btnMenu1.onClick.RemoveListener (GC.RestartGame);
//				//btnMenu2.onClick.RemoveListener (GC.RestartGame);
//				btnMenu5.onClick.RemoveListener (toMainMenu);
//			}
//			if (prevMenu == GameState.Pause) {
//				btnMenu1.onClick.RemoveListener (toResumeGame);
//				btnMenu2.onClick.RemoveListener (GC.RestartGame);
//				btnMenu5.onClick.RemoveListener (toMainMenu);
//			}

			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);

			setButtonElement (btnMenu1, "New Game", toResumeGame);
			setButtonElement (btnMenu2, "Settings", toSettings);
			setButtonElement (btnMenu3, "Credits", toCredits);
			setButtonElement (btnMenu4, "More Game", toMoreGames);
			setButtonElement (btnMenu5, "Exit", toGameQuit);
			break;

		case GameState.ChooseLevel:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);
			panelSelectMenu.SetActive (true);

			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		case GameState.Game:
			Time.timeScale = 1f;

			panelGame.SetActive (true);
			break;

		case GameState.Pause:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);

			// TODO: добавить в список для дальнейшего удаления
			setButtonElement (btnMenu1, "Resume", toResumeGame);
			setButtonElement (btnMenu2, "Restart", GC.RestartGame);
			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		case GameState.Win:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);

			setButtonElement (btnMenu1, "Next", toNext);
			setButtonElement (btnMenu2, "Restart", GC.RestartGame);
			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		case GameState.GameOver:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);

			setButtonElement (btnMenu1, "Restart", GC.RestartGame);
			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		case GameState.Settings:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);
			setButtonElement (btnMenu1, "Clear Level", toClearLevel);
			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		case GameState.Update:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);
			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		case GameState.Achivment:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);
			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		case GameState.MoreGames:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);
			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		case GameState.Credits:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);
			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		default:
			break;
		}
	}

	public void toPauseGame () {
		ChangeMenu (GameState.Pause);
	}
	public void toResumeGame (){
		ChangeMenu (GameState.Game);
	}
	public void toGameQuit (){
		print ("Application.Quit");
		Application.Quit ();
	}
	public void toMainMenu () {
		ChangeMenu (GameState.MainMenu);
	}
	public void toCredits () {
		ChangeMenu (GameState.Credits);
	}
	public void toSettings () {
		ChangeMenu (GameState.Settings);
	}
	public void toMoreGames () {
		ChangeMenu (GameState.MoreGames);
	}
	public void toSelectMenu () {
		ChangeMenu (GameState.ChooseLevel);
	}
	public void toNext () {
		GC.incLevel ();
		GC.RestartGame ();
	}
	public void toGameOver(){
		ChangeMenu (GameState.GameOver);
	}
	public void toWin (){
		ChangeMenu (GameState.Win);
	}
	public void toClearLevel(){
		GC.setLevel (1);
		ChangeMenu (GameState.MainMenu);
	}
}
