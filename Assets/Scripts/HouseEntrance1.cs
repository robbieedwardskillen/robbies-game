using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseEntrance1 : MonoBehaviour
{
	GameObject houseRoom1;
	GameObject houseExit1;
	GameObject player;
	GameObject m_cam;
	Transition gameTransition;

	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		houseRoom1 = GameObject.Find ("house room 1");
		houseExit1 = GameObject.Find ("house room 1").transform.Find("House Exit 1").gameObject;
	}
    void Start() {
		houseRoom1.SetActive (false);
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
			houseRoom1.SetActive (true);
			player.transform.position = new Vector3 (houseExit1.transform.position.x - 0.5f, 
			(houseExit1.transform.position.y), houseExit1.transform.position.z);
		}
	}
}
