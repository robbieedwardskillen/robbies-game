using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcSword : MonoBehaviour {
	ParticleSystem shockWave;
	private AudioSource audio;
	public AudioClip swordClash;
	GameObject theNPC;
	NPC npcScript;
	void Start() {
		theNPC = gameObject.transform.root.gameObject;
		npcScript = theNPC.GetComponent<NPC>();
		audio = gameObject.GetComponent<AudioSource> ();
		audio.volume = 0.2f;
		shockWave = gameObject.transform.GetChild (0).gameObject.GetComponent<ParticleSystem>();
	}
	void OnTriggerEnter(Collider hit) {
		if (hit.gameObject.name != "FemaleNPC" && hit.gameObject.name != "MaleNPC" && npcScript.busy){
			shockWave.transform.gameObject.SetActive (true);
			shockWave.Clear ();
			shockWave.Play ();
			audio.PlayOneShot(swordClash, 0.7f);
		}

	}
	void OnTriggerExit(Collider hit) {
		shockWave.transform.gameObject.SetActive (false);
	}
}