using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentExit1 : MonoBehaviour {
	GameObject apartmentRoom1;
	GameObject player;
	GameObject apartmentEntrance1;
	GameObject m_cam;
	Transition gameTransition;
	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		apartmentRoom1 = GameObject.Find ("apartment room 1");
		apartmentEntrance1 = GameObject.Find("Entrances").transform.Find("Apartment Entrance 1").gameObject;
	}
	private IEnumerator Countdown()
	{ 
		gameTransition.transitioning = true;
		player.transform.position = new Vector3 (apartmentEntrance1.transform.position.x + -0.5f, 
		(apartmentEntrance1.transform.position.y), apartmentEntrance1.transform.position.z);
		apartmentEntrance1.SetActive (true);
		yield return new WaitForSeconds(1);
		apartmentRoom1.SetActive (false);
		gameTransition.transitioning = false;
	}

	void OnTriggerEnter(Collider col) {
		if (col.name == "player_character"){
			StartCoroutine(Countdown());
		}
	}
}
