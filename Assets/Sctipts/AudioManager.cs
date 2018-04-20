using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound {
	public string name;
	public AudioClip clip;

	[Range(0f,1f)]
	public float volume = 0.7f;
	[Range(0.5f,1.5f)]
	public float pitch = 1f;

	private AudioSource source;

	public void setSource (AudioSource _source) {
		source = _source;
		source.clip = clip;		
	}

	public void Play (){
		source.volume = volume;
		source.pitch = pitch;
		source.Play ();
	}
}

public class AudioManager : MonoBehaviour {
	public static AudioManager instance;

	[SerializeField]
	Sound[] sounds;
	// TODO: Dictionary

	void Awake () {
		if (instance != null) {
			Debug.LogError ("More than one AudioManager...");
		} else {
			instance = this;
		}
	}

	void Start (){
		for (int i = 0; i < sounds.Length; i++) {
			GameObject go = new GameObject ("Sound_"+i+"_"+sounds[i].name);
			go.transform.SetParent (this.transform);
			sounds [i].setSource (go.AddComponent<AudioSource> ());
		}
	}

	public void PlaySound (string _name){
		// TODO: Dictionary
		for (int i = 0; i < sounds.Length; i++) {
			if (sounds[i].name == _name){
				sounds [i].Play ();
				return;
			}
		}

		Debug.LogWarning ("AudioManager not found sounds "+ _name);
	}
}
