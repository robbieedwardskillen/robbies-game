using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreEntrance2 : MonoBehaviour {
	GameObject storeRoom2;
	GameObject storeExit2;
	GameObject player;
	GameObject m_cam;
	Transition gameTransition;

	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		storeRoom2 = GameObject.Find ("store room 2");
		storeExit2 = GameObject.Find ("store room 2").transform.Find("Store Exit 2").gameObject;
	}
	void Start() {
		storeRoom2.SetActive (false);
	}
	private IEnumerator Countdown()
	{
		gameTransition.transitioning = true;
		yield return new WaitForSeconds(1);
		gameTransition.transitioning = false;
	}
	void OnTriggerEnter(Collider col) {
		if (col.name == "player_character"){
			StartCoroutine(Countdown());
			storeRoom2.SetActive (true);
			player.transform.position = new Vector3 (storeExit2.transform.position.x + 0.5f, 
			(storeExit2.transform.position.y), storeExit2.transform.position.z);
		}
	}
}


