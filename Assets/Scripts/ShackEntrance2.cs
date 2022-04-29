using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShackEntrance2 : MonoBehaviour
{
	GameObject shackRoom1;
	GameObject shackExit2;
	GameObject player;
	GameObject m_cam;
	Transition gameTransition;

	void Awake() {
		m_cam = GameObject.Find("Main Camera");
		gameTransition = m_cam.GetComponent<Transition>();
		player = GameObject.Find ("player_character");
		shackRoom1 = GameObject.Find ("shack room 1");
		shackExit2 = GameObject.Find ("shack room 1").transform.Find("Shack Exit 2").gameObject;
	}
    void Start() {
		shackRoom1.SetActive (false);
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
			shackRoom1.SetActive (true);
			player.transform.position = new Vector3 (shackExit2.transform.position.x , 
			(shackExit2.transform.position.y), shackExit2.transform.position.z - 1.25f);
		}
	}
}
