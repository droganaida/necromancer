using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class Menu {
	string name;
	//List<Button> listButton = new List<Button> ();
	//List<Action> listAction;

}
public delegate void MyDelegate(int input);

public class MenuManager : MonoBehaviour {

	private GameController gC;

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
	public Transform selectContainer;
	public GameObject btnSLPrefab;

	// UI menu elements
	public Text scoreValueDisplay;
	public Text castleHP;
	public Text curLevel;
	public Text unlockLev;
	public Text waveLevel;

	public Button ScoreButtonNextWave;

	private int unlockLevel;


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
	List <GameObject> listBtnSL = new List<GameObject> ();



	void Awake () {
		gC = GetComponent<GameController> (); 

		dicUA.Add ("Main Menu", toMainMenu);
		dicUA.Add ("Select Menu", toSelectMenu);
		dicUA.Add ("Settings", toSettings);
		dicUA.Add ("Credits", toCredits);
		dicUA.Add ("More Games", toMoreGames);
		dicUA.Add ("Quit Game", toGameQuit);
		dicUA.Add ("Pause", toPauseGame);
		dicUA.Add ("Resume", toResumeGame);
		dicUA.Add ("Restart", gC.RestartGame);

		dicUA.Add ("Next", toResumeGame);
		dicUA.Add ("New Game", toResumeGame);
		dicUA.Add ("Continue", toResumeGame);
		dicUA.Add ("Start Game", toResumeGame);

		// init all menu

	}

	// Use this for initialization
	void Start () {
		// init select level button
		SettingBtnSL ();

	}

	void SettingBtnSL () {
		// clear select button list
		listBtnSL.Clear();

		for (int i = 1; i < 11; i++) {
			// TODO: может это все сапрятать в ButtonSL ?
			GameObject btn = Instantiate (btnSLPrefab) as GameObject;
			btn.gameObject.transform.SetParent (selectContainer);
			btn.GetComponentInChildren<Text> ().text = i.ToString ();
			btn.transform.localScale = new Vector3 (1f, 1f, 1f);
			btn.GetComponent<ButtonSL> ().SetGameController (gC);
			btn.GetComponent<ButtonSL> ().SetMenuManager(this);
			listBtnSL.Add (btn);
			if (i <= gC.unlockLevel) {
				btn.GetComponent<ButtonSL> ().setSprite ("unlockL");
				btn.GetComponent<Button> ().onClick.AddListener (btn.GetComponent<ButtonSL> ().btnClick);
				string nameLevel = "Level" + i.ToString ();
				if (PlayerPrefs.HasKey (nameLevel)) {
					btn.GetComponent<ButtonSL> ().setSculls (PlayerPrefs.GetInt (nameLevel));
				}
			} else {
				btn.GetComponent<ButtonSL> ().setSprite ("lockL");
			}
		}
	}

	void setButtonElement (Button btn, string txt, UnityAction ua){
		btn.gameObject.SetActive (true);
		if (btn.GetComponentInChildren<Text> ()) {
			btn.GetComponentInChildren<Text> ().text = txt;
		}
		btn.onClick.AddListener (ua);
		listButton.Add (btn, ua);
	}

	void removeButtonListener (Button btn, UnityAction ua){
		btn.onClick.RemoveListener (ua);
	}

	public void ChangeMenu (GameState menu) {
		print ("GameState: " + menu); 

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

		switch (menu) {
		// int something = (int) Question.Role;
		case GameState.SplashScreen:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);

			setButtonElement (btnMenu5, "TAP", toMainMenu);
			break;

		case GameState.MainMenu:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);

			// TODO: maybe Continue/New Game ? 
			setButtonElement (btnMenu1, "Play Game", toSelectMenu);
			setButtonElement (btnMenu2, "Settings", toSettings);
			setButtonElement (btnMenu3, "Credits", toCredits);
			setButtonElement (btnMenu4, "More Game", toMoreGames);
			/*
			эппл запрещает делать кнопку выхода из приложения
			*/
			#if (!UNITY_IOS)
			setButtonElement (btnMenu5, "Exit", toGameQuit);
			#endif
			break;

		case GameState.ChooseLevel:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);
			panelSelectMenu.SetActive (true);

			RefreshBtnSL ();

			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		case GameState.Game:
			Time.timeScale = 1f;

			panelGame.SetActive (true);
			break;

		case GameState.Pause:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);

			setButtonElement (btnMenu1, "Resume", toResumeGame);
			setButtonElement (btnMenu2, "Restart", toRestartGame);
			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		case GameState.Win:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);

			setButtonElement (btnMenu1, "Next", toNext);
			setButtonElement (btnMenu2, "Restart", toRestartGame);
			setButtonElement (btnMenu3, "Select Level", toSelectMenu);
			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		case GameState.GameOver:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);

			setButtonElement (btnMenu1, "Restart", toRestartGame);
			setButtonElement (btnMenu2, "Select Level", toSelectMenu);
			setButtonElement (btnMenu5, "Main Menu", toMainMenu);
			break;

		case GameState.Settings:
			panelMenu.SetActive (true);
			panelButtonMenu.SetActive (true);
			setButtonElement (btnMenu1, "Clear Level", toClearLevel);
			setButtonElement (btnMenu2, "Clear UnlockLevel", toClearAllLevel);
			setButtonElement (btnMenu3, "Clear Scull data", toClearScullData);
			setButtonElement (btnMenu4, "Clear ALL User Data", toClearAllUserData);
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
		// обновим buttonSL
		RefreshBtnSL();

		ChangeMenu (GameState.ChooseLevel);
	}

	public void toNext () {
		gC.RestartGame ();
	}

	public void toStartGame (){
		ChangeMenu (GameState.Game);
		gC.StartGame ();
	}

	public void toRestartGame () { // Win/GameOver
		gC.decLevel ();
		gC.RestartGame ();
	}

	public void toGameOver(){
		ChangeMenu (GameState.GameOver);
	}

	public void toWin (){
		//GC.incLevel ();
		ChangeMenu (GameState.Win);
	}

	public void toClearLevel(){
		gC.setLevel (1);
		RefreshBtnSL ();
		ChangeMenu (GameState.MainMenu);
	}

	public void toClearAllLevel (){
		gC.clearAllLevel ();
		RefreshBtnSL ();
		ChangeMenu (GameState.MainMenu);
	}

	public void toSelectedLevel (int lev){
		gC.setLevel (lev);
		ChangeMenu (GameState.Game);
		gC.RestartGame ();
		
	}

	public void toClearAllUserData (){
		PlayerPrefs.DeleteAll ();
		PlayerPrefs.Save ();

		gC.clearAllLevel ();

		ChangeMenu (GameState.MainMenu);
	}

	public void toClearScullData () {
		for (int i = 1; i <= gC.unlockLevel; i++) {
			string nameLevel = "Level" + i.ToString ();
			if (PlayerPrefs.HasKey (nameLevel)) {
				PlayerPrefs.DeleteKey (nameLevel);
			}
		}
		PlayerPrefs.Save ();
		ChangeMenu (GameState.MainMenu);
	}

	void RefreshBtnSL(){
		foreach (var item in listBtnSL) {
			item.GetComponent<ButtonSL> ().RefreshScull ();
			item.GetComponent<ButtonSL> ().RefreshSprite ();
		}
	}

	public void ShowHideText (UnityEngine.UI.Text text){
		text.gameObject.SetActive (!text.gameObject.activeSelf);
	}
	public void ShowHideObj (GameObject obj){
		obj.gameObject.SetActive (!obj.gameObject.activeSelf);
	}

	public void changeScoreButtonActive (bool b) {
		ScoreButtonNextWave.gameObject.SetActive (b);
	}

	public void setTextWaveTxt (string str) {
		waveLevel.text = str;
	}
}
