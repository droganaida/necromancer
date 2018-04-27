using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSL : MonoBehaviour {

	public Image img1;
	public Image img2;
	public Image img3;
	public Text btnText;
	public Sprite imgUnlockLevel;
	public Sprite imgLockLevel;
	private List<Image> listImage;
	private GameController GC;
	private MenuManager MM;

	void Awake () {
		listImage = new List<Image> ();
		listImage.Add (img1);
		listImage.Add (img2);
		listImage.Add (img3);

		falseAllImages ();
	}
		
	public void RefreshScull () {
		string nameLevel = "Level" + GetComponentInChildren<Text> ().text;
		if (PlayerPrefs.HasKey (nameLevel)) {
			setSculls (PlayerPrefs.GetInt (nameLevel));
		}
	}

	public void RefreshSprite(){
		int lev = System.Convert.ToInt32(GetComponentInChildren<Text> ().text);
		if (lev <= GC.unlockLevel) {
			setSprite ("unlockL");
			GetComponent<Button> ().onClick.RemoveAllListeners ();
			GetComponent<Button> ().onClick.AddListener (btnClick);
			string nameLevel = "Level" + lev.ToString ();
			if (PlayerPrefs.HasKey (nameLevel)) {
				setSculls (PlayerPrefs.GetInt (nameLevel));
			}
		} else {
			setSprite ("lockL");
		}
	}

	// set our sculls
	public void setSculls (int scull){
		if ((scull > 0) && (scull <= listImage.Count)) {
			for (int i = 0; i < scull; i++) {
				listImage [i].gameObject.SetActive (true);
			}
		}  
	}

	public void falseAllImages () {
		// hide our sculls
		foreach (Image item in listImage) {
			item.gameObject.SetActive (false);
		}
	}

	public void setSprite (string str) {
		switch (str) {
		case "lockL":
			GetComponent<Image> ().sprite = imgLockLevel;
			break;
		case "unlockL":
			GetComponent<Image> ().sprite = imgUnlockLevel;
			break;
		default:
			break;
		}
	}

	public void SetGameController(GameController gc) {
		GC = gc;
	}
	public void SetMenuManager(MenuManager mm) {
		MM = mm;
	}

	public void btnClick (){
		print ("click buttonSL "+ GetComponentInChildren<Text>().text);
		MM.toSelectedLevel (System.Convert.ToInt32( GetComponentInChildren<Text> ().text ));
//		GC.setLevel ();
//		MM.ChangeMenu (MenuManager.GameState.Game);
//		GC.StartGame ();
	}

}
