using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentEntrance1 : MonoBehaviour {
	GameObject player;
	GameObject apartmentRoom1;
	GameObject apartmentExit1;
	GameObject m_cam;
	Transition gameTransition;
	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		apartmentRoom1 = GameObject.Find ("apartment room 1");
		apartmentExit1 = GameObject.Find ("apartment room 1").transform.Find("Apartment Exit 1").gameObject;
	}
	void Start() {
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
			apartmentRoom1.SetActive (true);
			player.transform.position = new Vector3 (apartmentExit1.transform.position.x - 0.5f, 
			(apartmentExit1.transform.position.y), apartmentExit1.transform.position.z);

		}
	}
}
