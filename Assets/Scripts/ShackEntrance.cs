using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShackEntrance : MonoBehaviour {
	GameObject player;
	GameObject storeRoom1;
	GameObject storeRoom2;
	GameObject shackRoom1;
	GameObject apartmentRoom1;
	GameObject storeExit1;
	GameObject storeExit2;
	GameObject shackExit1;
	GameObject apartmentExit1;

	GameObject m_cam;
	Transition gameTransition;
	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		storeRoom1 = GameObject.Find ("store room 1");
		storeRoom2 = GameObject.Find ("store room 2");
		shackRoom1 = GameObject.Find ("shack room 1");
		apartmentRoom1 = GameObject.Find ("apartment room 1");
		storeExit1 = GameObject.Find ("store room 1").transform.Find("Store Exit 1").gameObject;
		storeExit2 = GameObject.Find ("store room 2").transform.Find("Store Exit 2").gameObject;
		shackExit1 = GameObject.Find ("shack room 1").transform.Find("Shack Exit 1").gameObject;
		apartmentExit1 = GameObject.Find ("apartment room 1").transform.Find("Apartment Exit 1").gameObject;
	}
	void Start() {
		storeRoom1.SetActive (false);
		storeRoom2.SetActive (false);
		shackRoom1.SetActive (false);
		apartmentRoom1.SetActive (false);
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
			shackExit1.SetActive (true);
			shackRoom1.SetActive (true);
			player.transform.position = new Vector3 (shackExit1.transform.position.x - 0.5f, 
			(shackExit1.transform.position.y), shackExit1.transform.position.z);

		}

	}
}
