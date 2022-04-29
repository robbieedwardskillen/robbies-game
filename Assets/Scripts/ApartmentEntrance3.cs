using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentEntrance3 : MonoBehaviour
{
	GameObject apartmentRoom3;
	GameObject apartmentExit3;
	GameObject player;
	GameObject m_cam;
	Transition gameTransition;

	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		apartmentRoom3 = GameObject.Find ("apartment room 3");
        apartmentExit3 = GameObject.Find ("apartment room 3").transform.Find("Apartment Exit 3").gameObject;
	}
    void Start() {
		apartmentRoom3.SetActive (false);
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
			apartmentRoom3.SetActive (true);
			player.transform.position = new Vector3 (apartmentExit3.transform.position.x - 0.5f, 
			(apartmentExit3.transform.position.y), apartmentExit3.transform.position.z);
		}
	}
}
