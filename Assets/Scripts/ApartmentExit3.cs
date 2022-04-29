using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentExit3 : MonoBehaviour
{
	GameObject apartmentRoom3;
	GameObject player;
	GameObject apartmentEntrance3;
	GameObject m_cam;
	Transition gameTransition;
	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		apartmentRoom3 = GameObject.Find ("apartment room 3");
		apartmentEntrance3 = GameObject.Find("Entrances").transform.Find("Apartment Entrance 3").gameObject;
	}
	private IEnumerator Countdown()
	{ 
		gameTransition.transitioning = true;
		player.transform.position = new Vector3 (apartmentEntrance3.transform.position.x + -0.5f, 
		(apartmentEntrance3.transform.position.y), apartmentEntrance3.transform.position.z);
		apartmentEntrance3.SetActive (true);
		yield return new WaitForSeconds(1);
		apartmentRoom3.SetActive (false);
		gameTransition.transitioning = false;
	}
	void OnTriggerEnter(Collider col) {
		if (col.name == "player_character"){
			StartCoroutine(Countdown());
		}
	}
}
