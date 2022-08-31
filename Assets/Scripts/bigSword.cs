using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bigSword : MonoBehaviour
{
    ParticleSystem bloodSprayEffect;
	private AudioSource audio;
	//public AudioClip swordClash;
	GameObject thePlayer;
	PlayerScript playerScript;
	void Start() {
		thePlayer = gameObject.transform.root.gameObject;
		playerScript = thePlayer.GetComponent<PlayerScript>();
		audio = gameObject.GetComponent<AudioSource> ();
		audio.volume = 0.2f;
		bloodSprayEffect = gameObject.transform.Find("BloodSprayEffect").gameObject.GetComponent<ParticleSystem>();
	}
	void OnTriggerEnter(Collider hit) {
		if ((hit.gameObject.name == "player_character" || hit.gameObject.name == "cop")
         && playerScript.bigSwing && transform.root != hit.gameObject.transform.root){
			if (bloodSprayEffect != null){
				bloodSprayEffect.transform.gameObject.SetActive (true);
				bloodSprayEffect.Clear ();
				bloodSprayEffect.Play ();
                StartCoroutine(Blood());
			}
			//audio.PlayOneShot(swordClash, 0.7f);
		}
	}
    IEnumerator Blood() {
	    yield return new WaitForSeconds (0.25f);
		if (bloodSprayEffect != null)
		bloodSprayEffect.transform.gameObject.SetActive (false);
	}

}
