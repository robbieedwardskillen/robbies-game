using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentExit2 : MonoBehaviour
{
	GameObject apartmentRoom2;
	GameObject player;
	GameObject apartmentEntrance2;
	GameObject m_cam;
	Transition gameTransition;
	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		apartmentRoom2 = GameObject.Find ("apartment room 2");
		apartmentEntrance2 = GameObject.Find("Entrances").transform.Find("Apartment Entrance 2").gameObject;
	}
	private IEnumerator Countdown()
	{ 
		gameTransition.transitioning = true;
		player.transform.position = new Vector3 (apartmentEntrance2.transform.position.x + -0.5f, 
		(apartmentEntrance2.transform.position.y), apartmentEntrance2.transform.position.z);
		apartmentEntrance2.SetActive (true);
		yield return new WaitForSeconds(1);
		apartmentRoom2.SetActive (false);
		gameTransition.transitioning = false;
	}
	void OnTriggerEnter(Collider col) {
		if (col.name == "player_character"){
			StartCoroutine(Countdown());
		}
	}
}
