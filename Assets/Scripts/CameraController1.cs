using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;
public class CameraController1 : MonoBehaviour {
	private GameObject player;
	private Player playerScript;
	private PlayerControls controls;
	private Vector2 rotate;
	private bool aiming = false;
	private GameObject loadingScreen;
	private Vector3 offset;
	private bool setAimingOffset = false;
	private bool setRegularOffset = true;
	private GameObject aptRoom1;
	private GameObject aptRoom2;
	private GameObject aptRoom3;
	private GameObject aptRoom4;
	private GameObject aptRoom5;
	private GameObject storeRoom1;
	private GameObject storeRoom2;
	private GameObject shackRoom1;
	private Vector3 offPos;
	private Vector3 smoothPos;
	private Vector3 finalPos;
	private Vector3 pos;
	private Vector3 velocity;
	Camera m_cam;
	Transition gameTransition;
	Ray raycast;
	RaycastHit hit;
	bool isHit;
	float hitDistance;
	public PostProcessVolume volume;
	private DepthOfField depthOfField;
	// Use this for initialization
	void Awake()
	{
		player = GameObject.Find("Player");
		controls = new PlayerControls();
		controls.Gameplay.Rotate.performed += ctx => rotate = ctx.ReadValue<Vector2>();
		controls.Gameplay.Rotate.canceled += ctx => rotate = Vector2.zero;
		playerScript = player.GetComponent<Player>();
		aiming = playerScript.aiming;
		loadingScreen = GameObject.Find("Loading Screen");
		m_cam = Camera.main;
		offset = new Vector3(0f, 1.1f, 2.3f);
		gameTransition = m_cam.GetComponent<Transition>();
		velocity = Vector3.zero;
		aptRoom1 = GameObject.Find ("apartment room 1");
		aptRoom2 = GameObject.Find ("apartment room 2");
		aptRoom3 = GameObject.Find ("apartment room 3");
		aptRoom4 = GameObject.Find ("apartment room 4");
		aptRoom5 = GameObject.Find ("apartment room 5");
		storeRoom1 = GameObject.Find ("store room 1");
		storeRoom2 = GameObject.Find ("store room 2");
		shackRoom1 = GameObject.Find ("shack room 1");
	}
	void Start() {
		volume.profile.TryGetSettings(out depthOfField);
		if (controls != null){
			controls.Gameplay.Enable();
		}
	}
	void OnEnable() {
		if (controls != null){
			controls.Gameplay.Enable();
		}
	}
	void OnDisable() {
		if (controls != null){
			controls.Gameplay.Disable();
		}
		
	}
	public void Update() {
		if (/* !aiming */ false){
			//blur effects
			depthOfField.active = true;
			raycast = new Ray(transform.position, transform.forward * 100);
			isHit = false;
			if (Physics.Raycast(raycast, out hit, 100f)){
				isHit = true;
				hitDistance = Vector3.Distance(transform.position, hit.point);
			}
			else {
				if (hitDistance < 100f) {
					hitDistance++;
				}
			}
			SetFocus();
			
		} else {
			depthOfField.active = false;
		}
		
	}
	void SetFocus() {
		depthOfField.focusDistance.value = Mathf.Lerp(depthOfField.focusDistance.value, hitDistance, Time.deltaTime * 10f);
	}
	void FixedUpdate()
	{

		bool outside = shackRoom1.activeSelf == false && 
			storeRoom1.activeSelf == false &&
			storeRoom2.activeSelf == false && 
			aptRoom1.activeSelf == false &&
			aptRoom2.activeSelf == false &&
			aptRoom3.activeSelf == false &&
			aptRoom4.activeSelf == false &&
			aptRoom5.activeSelf == false;
		if (gameTransition.transitioning){
			loadingScreen.active = true;
		} else {
			loadingScreen.active = false;
		}

		if (outside) {
			
 			if (!playerScript.underWater){ 
				m_cam.fieldOfView = 60f;
 			} else {
				m_cam.fieldOfView = 35f;
			} 

			m_cam.clearFlags = CameraClearFlags.Skybox;
			m_cam.orthographic = false;
			
		} else {
			m_cam.clearFlags = CameraClearFlags.Depth;
		}




			
	}
}
