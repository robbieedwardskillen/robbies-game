using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sword : MonoBehaviour {
	ParticleSystem shockWave;
	private AudioSource audio;
	public AudioClip swordClash;
	GameObject thePlayer;
	Player playerScript;
	void Start() {
		thePlayer = gameObject.transform.root.gameObject;
		playerScript = thePlayer.GetComponent<Player>();
		audio = gameObject.GetComponent<AudioSource> ();
		audio.volume = 0.2f;
		if (gameObject.transform.GetChild (0).gameObject.GetComponent<ParticleSystem>() != null){
			shockWave = gameObject.transform.GetChild (0).gameObject.GetComponent<ParticleSystem>();
		}
		
	}
	void OnTriggerEnter(Collider hit) {
		if (hit.gameObject.name != "player_character" && playerScript.busy){
			if (shockWave != null){
				shockWave.transform.gameObject.SetActive (true);
				shockWave.Clear ();
				shockWave.Play ();
			}
			audio.PlayOneShot(swordClash, 0.7f);
		}

	}
	void OnTriggerExit(Collider hit) {
		if (shockWave != null)
		shockWave.transform.gameObject.SetActive (false);
	}
}
