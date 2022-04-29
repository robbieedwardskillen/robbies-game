using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShackExit2 : MonoBehaviour
{
	GameObject shackRoom1;
	GameObject player;
	GameObject shackEntrance2;
	GameObject m_cam;
	Transition gameTransition;
	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		shackRoom1 = GameObject.Find ("shack room 1");
		shackEntrance2 = GameObject.Find("Entrances").transform.Find("Shack Entrance 2").gameObject;
	}
	private IEnumerator Countdown()
	{ 
		gameTransition.transitioning = true;
		player.transform.position = new Vector3 (shackEntrance2.transform.position.x + 0.5f, 
		(shackEntrance2.transform.position.y), shackEntrance2.transform.position.z);
		shackEntrance2.SetActive (true);
		yield return new WaitForSeconds(1);
		shackRoom1.SetActive (false);
		gameTransition.transitioning = false;
	}

	void OnTriggerEnter(Collider col) {
		if (col.name == "player_character"){
			StartCoroutine(Countdown());
		}
	}
}
