using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuSelect : MonoBehaviour {
	public GameObject startArrow;
	public GameObject continueArrow;
	// Use this for initialization
	bool start = true;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {	
		if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Jump")) {
			//start or continue
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			startArrow.SetActive(false);
			continueArrow.SetActive(true);
			start = false;
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			startArrow.SetActive(true);
			continueArrow.SetActive(false);
			start = true;
		}
		if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2") || Input.GetButtonUp("Fire3") || Input.GetButtonDown ("Jump")) {
			SceneManager.LoadScene(1);
		}
	}
}
