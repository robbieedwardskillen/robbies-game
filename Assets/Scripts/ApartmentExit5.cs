using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentExit5 : MonoBehaviour
{
	GameObject apartmentRoom5;
	GameObject player;
	GameObject apartmentEntrance5;
	GameObject m_cam;
	Transition gameTransition;
	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		apartmentRoom5 = GameObject.Find ("apartment room 5");
		apartmentEntrance5 = GameObject.Find("Entrances").transform.Find("Apartment Entrance 5").gameObject;
	}
	private IEnumerator Countdown()
	{ 
		gameTransition.transitioning = true;
		player.transform.position = new Vector3 (apartmentEntrance5.transform.position.x, 
		(apartmentEntrance5.transform.position.y), apartmentEntrance5.transform.position.z + -0.5f);
		apartmentEntrance5.SetActive (true);
		yield return new WaitForSeconds(1);
		apartmentRoom5.SetActive (false);
		gameTransition.transitioning = false;
	}
	void OnTriggerEnter(Collider col) {
		if (col.name == "player_character"){
			StartCoroutine(Countdown());
			player.transform.position = new Vector3 (apartmentEntrance5.transform.position.x + 0.5f, 
			(apartmentEntrance5.transform.position.y), apartmentEntrance5.transform.position.z);
		}
	}
}
