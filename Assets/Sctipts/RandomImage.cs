using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomImage : MonoBehaviour {

	public Sprite[] spriteImages;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Awake () {
		
		int randomImageIndex = Random.Range (0, spriteImages.Length);
		Image img = GetComponent<Image>();
		img.sprite = spriteImages [randomImageIndex];
	}
}
