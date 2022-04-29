using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseExit1 : MonoBehaviour
{
	GameObject houseRoom1;
	GameObject player;
	GameObject houseEntrance1;
	GameObject m_cam;
	Transition gameTransition;
	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		houseRoom1 = GameObject.Find ("house room 1");
		houseEntrance1 = GameObject.Find("Entrances").transform.Find("House Entrance 1").gameObject;
	}
	private IEnumerator Countdown()
	{ 
		gameTransition.transitioning = true;
		player.transform.position = new Vector3 (houseEntrance1.transform.position.x , 
		(houseEntrance1.transform.position.y), houseEntrance1.transform.position.z + -0.5f);
		houseEntrance1.SetActive (true);
		yield return new WaitForSeconds(1);
		houseRoom1.SetActive (false);
		gameTransition.transitioning = false;
	}
	void OnTriggerEnter(Collider col) {
		if (col.name == "player_character"){
			StartCoroutine(Countdown());
		}
	}
}
