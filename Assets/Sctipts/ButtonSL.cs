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

	void Awake () {
		listImage = new List<Image> ();
		listImage.Add (img1);
		listImage.Add (img2);
		listImage.Add (img3);
	}

	void Start (){
		falseAllImages ();
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
	

}
