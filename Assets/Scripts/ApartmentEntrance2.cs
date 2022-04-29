using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentEntrance2 : MonoBehaviour
{
	GameObject apartmentRoom2;
	GameObject apartmentExit2;
	GameObject player;
	GameObject m_cam;
	Transition gameTransition;

	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		apartmentRoom2 = GameObject.Find ("apartment room 2");
		apartmentExit2 = GameObject.Find ("apartment room 2").transform.Find("Apartment Exit 2").gameObject;
	}
    void Start() {
		apartmentRoom2.SetActive (false);
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
			apartmentRoom2.SetActive (true);
			player.transform.position = new Vector3 (apartmentExit2.transform.position.x - 0.5f, 
			(apartmentExit2.transform.position.y), apartmentExit2.transform.position.z);
		}
	}
}
