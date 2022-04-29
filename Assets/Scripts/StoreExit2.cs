using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreExit2 : MonoBehaviour {
	GameObject storeRoom2;
	GameObject player;
	GameObject storeEntrance2;
	GameObject m_cam;
	Transition gameTransition;
	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		storeRoom2 = GameObject.Find ("store room 2");
		storeEntrance2 = GameObject.Find("Entrances").transform.Find("Store Entrance 2").gameObject;
	}
	private IEnumerator Countdown()
	{ 
		gameTransition.transitioning = true;
		player.transform.position = new Vector3 (storeEntrance2.transform.position.x + -0.5f, 
		(storeEntrance2.transform.position.y), storeEntrance2.transform.position.z);
		storeEntrance2.SetActive (true);
		yield return new WaitForSeconds(1);
		storeRoom2.SetActive (false);
		gameTransition.transitioning = false;
	}

	void OnTriggerEnter(Collider col) {
		if (col.name == "player_character"){
			StartCoroutine(Countdown());
		}
	}
}
