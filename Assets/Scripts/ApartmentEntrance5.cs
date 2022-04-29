using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentEntrance5 : MonoBehaviour
{
	GameObject apartmentRoom5;
	GameObject apartmentExit5;
	GameObject player;
	GameObject m_cam;
	Transition gameTransition;

	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		apartmentRoom5 = GameObject.Find ("apartment room 5");
		apartmentExit5 = GameObject.Find ("apartment room 5").transform.Find("Apartment Exit 5").gameObject;
	}
    void Start() {
		apartmentRoom5.SetActive (false);
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
			apartmentRoom5.SetActive (true);
			player.transform.position = new Vector3 (apartmentExit5.transform.position.x - 0.5f, 
			(apartmentExit5.transform.position.y), apartmentExit5.transform.position.z);
		}
	}
}
