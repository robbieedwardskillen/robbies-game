using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentEntrance4 : MonoBehaviour
{
	GameObject apartmentRoom4;
	GameObject apartmentExit4;
	GameObject player;
	GameObject m_cam;
	Transition gameTransition;

	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		apartmentRoom4 = GameObject.Find ("apartment room 4");
		apartmentExit4 = GameObject.Find ("apartment room 4").transform.Find("Apartment Exit 4").gameObject;
	}
    void Start() {
		apartmentRoom4.SetActive (false);
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
			apartmentRoom4.SetActive (true);
			player.transform.position = new Vector4 (apartmentExit4.transform.position.x - 0.5f, 
			(apartmentExit4.transform.position.y), apartmentExit4.transform.position.z);
		}
	}
}
