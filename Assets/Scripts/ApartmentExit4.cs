using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentExit4 : MonoBehaviour
{
	GameObject apartmentRoom4;
	GameObject player;
	GameObject apartmentEntrance4;
	GameObject m_cam;
	Transition gameTransition;
	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		apartmentRoom4 = GameObject.Find ("apartment room 4");
		apartmentEntrance4 = GameObject.Find("Entrances").transform.Find("Apartment Entrance 4").gameObject;
	}
	private IEnumerator Countdown()
	{ 
		gameTransition.transitioning = true;
		player.transform.position = new Vector3 (apartmentEntrance4.transform.position.x, 
		(apartmentEntrance4.transform.position.y), apartmentEntrance4.transform.position.z + -0.5f);
		apartmentEntrance4.SetActive (true);
		yield return new WaitForSeconds(1);
		apartmentRoom4.SetActive (false);
		gameTransition.transitioning = false;
	}
	void OnTriggerEnter(Collider col) {
		if (col.name == "player_character"){
			StartCoroutine(Countdown());
		}
	}
}
