using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreExit : MonoBehaviour {
	GameObject storeRoom1;
	GameObject player;
	GameObject storeEntrance1;
	GameObject m_cam;
	Transition gameTransition;
	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		storeRoom1 = GameObject.Find ("store room 1");
		storeEntrance1 = GameObject.Find("Entrances").transform.Find("Store Entrance 1").gameObject;
	}
	private IEnumerator Countdown()
	{ 
		gameTransition.transitioning = true;
		player.transform.position = new Vector3 (storeEntrance1.transform.position.x + -0.5f, 
		(storeEntrance1.transform.position.y), storeEntrance1.transform.position.z);
		storeEntrance1.SetActive (true);
		yield return new WaitForSeconds(1);
		storeRoom1.SetActive (false);
		gameTransition.transitioning = false;
	}

	void OnTriggerEnter(Collider col) {
		if (col.name == "player_character"){
			StartCoroutine(Countdown());
		}
	}
}
