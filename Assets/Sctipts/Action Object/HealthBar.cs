using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	public Image ribbon;

	public void SetHB (float scaleHB){
		if (scaleHB > 0.7f) {
			//ribbon.color = Color.green;
			ribbon.color = new Color32( 0x47, 0xD7, 0x73, 0xFF );
			if (scaleHB > 1f) {
				scaleHB = 1f;
			}
		} else if (scaleHB > 0.4f) {
			ribbon.color = Color.yellow;
		} else {
			ribbon.color = Color.red;
			if (scaleHB < 0f){
				scaleHB = 0f;
			}
		}
		ribbon.transform.localScale = new Vector3 (scaleHB, 1, 1); // 1 - ribbon.transform.localScale.y(z)
	}
}
