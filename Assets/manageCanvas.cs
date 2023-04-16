using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Michsky.UI.Shift;

public class ManageCanvas : MonoBehaviour
{
    
    private PlayerControls controls;
    private float jump;
    private float action1;private float action2;private float action3;private float action4;private float action5;
	private float aim; private float sprint; private float shoot; private float blockDodge;
	private float dpadUp; private float dpadDown; private float dpadLeft; private float dpadRight;
	private float menu; 
    private Vector2 move;
	private Vector2 rotate;
    private float posX = 0;
	private float acceleration = 0f;

	//public UIManager UIManagerAsset;
    private GameObject mainCam;
    private GameObject canvasCam;
	private GameObject canvasGameObject;
	public GameObject canvasInGameGameObject;
	public GameObject loadingScreen;
    private Canvas canvas;
	private RadialMenu radialMenu;
	public radialMenuElement highlightedAbility;
	public int ability = 0;
    public bool inGame = false;
	public bool myPlayerInstantiated = false;
	private bool canChange = true;

    void Start()
    {	
		//print(uiManager.backgroundColorTint);
        mainCam = GameObject.Find("Main Camera");
		canvasGameObject = GameObject.Find("Canvas");
		canvasInGameGameObject = GameObject.Find("Canvas In Game");
		loadingScreen = GameObject.Find("Canvas In Game/Loading");
		radialMenu = GameObject.Find("Canvas In Game/RadialMenu").GetComponent<RadialMenu>();
		canvasInGameGameObject.SetActive(false);
        canvasCam = GameObject.Find("Canvas Camera");
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		controls = new PlayerControls();
		controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
		controls.Gameplay.Move.performed += ctx => move = Vector2.zero;
		controls.Gameplay.Rotate.performed += ctx => rotate = ctx.ReadValue<Vector2>();
		controls.Gameplay.Rotate.canceled += ctx => rotate = Vector2.zero;
		controls.Gameplay.Jump.performed += ctx => jump = ctx.ReadValue<float>();//a/x button
		controls.Gameplay.Jump.canceled += ctx => jump = 0;
		controls.Gameplay.Action1.performed += ctx => action1 = ctx.ReadValue<float>();//x/square button
		controls.Gameplay.Action1.canceled += ctx => action1 = 0;
		controls.Gameplay.Action2.performed += ctx => action2 = ctx.ReadValue<float>();//y/triangle button
		controls.Gameplay.Action2.canceled += ctx => action2 = 0;
		controls.Gameplay.Action3.performed += ctx => action3 = ctx.ReadValue<float>();//b/o button
		controls.Gameplay.Action3.canceled += ctx => action3 = 0;
		controls.Gameplay.Action4.performed += ctx => action4 = ctx.ReadValue<float>();//left shoulder
		controls.Gameplay.Action4.canceled += ctx => action4 = 0;
		controls.Gameplay.Action5.performed += ctx => action5 = ctx.ReadValue<float>();//right shoulder
		controls.Gameplay.Action5.canceled += ctx => action5 = 0;
		controls.Gameplay.DPadUp.performed += ctx => dpadUp = ctx.ReadValue<float>();
		controls.Gameplay.DPadUp.canceled += ctx => dpadUp = 0;
		controls.Gameplay.DPadDown.performed += ctx => dpadDown = ctx.ReadValue<float>();
		controls.Gameplay.DPadDown.canceled += ctx => dpadDown = 0;
		controls.Gameplay.DPadLeft.performed += ctx => dpadLeft = ctx.ReadValue<float>();
		controls.Gameplay.DPadLeft.canceled += ctx => dpadLeft = 0;
		controls.Gameplay.DPadRight.performed += ctx => dpadRight = ctx.ReadValue<float>();
		controls.Gameplay.DPadRight.canceled += ctx => dpadRight = 0;
		controls.Gameplay.Aim.performed += ctx => aim = ctx.ReadValue<float>();//right clicky
		controls.Gameplay.Aim.canceled += ctx => aim = 0;
		controls.Gameplay.Sprint.performed += ctx => sprint = ctx.ReadValue<float>();//right clicky
		controls.Gameplay.Sprint.canceled += ctx => sprint = 0;
		controls.Gameplay.Shoot.performed += ctx => shoot = ctx.ReadValue<float>();//right trigger
		controls.Gameplay.Shoot.canceled += ctx => shoot = 0;
		controls.Gameplay.BlockDodge.performed += ctx => blockDodge = ctx.ReadValue<float>();//left trigger
		controls.Gameplay.BlockDodge.canceled += ctx => blockDodge = 0;

		if (controls != null){
			controls.Gameplay.Enable();
		}


    }

	public bool MyPlayerInstantiated {
		get 
		{
			return myPlayerInstantiated; 
		}
		set
		{
			if (value != null){
				myPlayerInstantiated = value;
			}
		}
	}


    void Update()
    {//myPlayerInstantiated

		if (GameObject.Find("Launcher").GetComponent<Launcher>().connected){ //set to only work after joining game
			if (controls.Gameplay.Menu.triggered){
				SwitchCam(inGame);
				inGame = !inGame;
			}
		}
		if (canvasInGameGameObject != null){
			if(!myPlayerInstantiated){
				loadingScreen.SetActive(true);
			} else {
				loadingScreen.SetActive(false);
			}
		}

		if (canChange && myPlayerInstantiated)
			SetRadialMenuWithDPad();

    }
	public void SwitchCam(bool inTheGame) { 
		if (inTheGame == false){
			//audio.Stop(); //why would I do this?
			canvasGameObject.SetActive(true);
			canvasInGameGameObject.SetActive(false);
		}
		else {
			//audio.Play();
			canvasGameObject.SetActive(false);
			canvasInGameGameObject.SetActive(true);
		}
		
		mainCam.GetComponent<AudioListener>().enabled = inTheGame;
		mainCam.SetActive(inTheGame);
		canvasCam.GetComponent<AudioListener>().enabled = !inTheGame;
		canvasCam.SetActive(!inTheGame);
		
	}

	public void SetRadialMenuWithDPad(){
		if (dpadUp == 1 && noOtherDirections()){
			SetRadialMenuElementAndDeactivateTheRest(0);
		} 
		if (dpadUp == 1 && dpadRight == 1){
			SetRadialMenuElementAndDeactivateTheRest(1);
			StartCoroutine(AvoidAccidentalDirection());
		} 
		if (dpadRight == 1 && noOtherDirections()){
			SetRadialMenuElementAndDeactivateTheRest(2);
		}
		if (dpadRight == 1 && dpadDown == 1){
			SetRadialMenuElementAndDeactivateTheRest(3);
			StartCoroutine(AvoidAccidentalDirection());
		} 
		if (dpadDown == 1 && noOtherDirections()){
			SetRadialMenuElementAndDeactivateTheRest(4);
		} 
		if (dpadDown == 1 && dpadLeft == 1){
			SetRadialMenuElementAndDeactivateTheRest(5);
			StartCoroutine(AvoidAccidentalDirection());
		}
		if (dpadLeft == 1 && noOtherDirections()){
			SetRadialMenuElementAndDeactivateTheRest(6);
		} 
		if (dpadLeft == 1 && dpadUp == 1){
			SetRadialMenuElementAndDeactivateTheRest(7);
			StartCoroutine(AvoidAccidentalDirection());
		} 
	}

	public void SetRadialMenuElementAndDeactivateTheRest(int directionNumber) {
		for (int i = 0; i < radialMenu.elements.Count; i++){
			radialMenu.elements[i].GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0f);
		}
		switch (directionNumber)
		{
		case 0:
			highlightedAbility = radialMenu.elements[0];
			ability = 0;
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().enabled = true;
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().sprite = radialMenu.elements[0].GetImage2();
			radialMenu.elements[0].GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f ,0f);
			break;
		case 1:
			highlightedAbility = radialMenu.elements[1];
			ability = 1;
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().enabled = true;
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().sprite = radialMenu.elements[1].GetImage2();
			radialMenu.elements[1].GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f ,0f);
			break;
		case 2:
			highlightedAbility = radialMenu.elements[2];
			ability = 2;
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().enabled = true;
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().sprite = radialMenu.elements[2].GetImage2();
			radialMenu.elements[2].GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f ,0f);
			break;
		case 3:
			highlightedAbility = radialMenu.elements[3];
			ability = 3;
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().enabled = true;
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().sprite = radialMenu.elements[3].GetImage2();
			radialMenu.elements[3].GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f ,0f);
			break;
		case 4:
			ability = 4;
			highlightedAbility = radialMenu.elements[4];
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().enabled = true;
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().sprite = radialMenu.elements[4].GetImage2();
			radialMenu.elements[4].GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f ,0f);
			break;
		case 5:
			ability = 5;
			highlightedAbility = radialMenu.elements[5];
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().enabled = true;
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().sprite = radialMenu.elements[5].GetImage2();
			radialMenu.elements[5].GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f ,0f);
			break;
		case 6:
			ability = 6;
			highlightedAbility = radialMenu.elements[6];
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().enabled = true;
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().sprite = radialMenu.elements[6].GetImage2();
			radialMenu.elements[6].GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f ,0f);
			break;
		case 7:
			ability = 7;
			highlightedAbility = radialMenu.elements[7];
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().enabled = true;
			GameObject.Find("Canvas In Game/RadialMenu/mask/CardImage").GetComponent<Image>().sprite = radialMenu.elements[7].GetImage2();
			radialMenu.elements[7].GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f ,0f);
			break;
		}

		
	}

	IEnumerator AvoidAccidentalDirection() {
		canChange = false;
		yield return new WaitForSeconds(0.25f);
		canChange = true;
	}
	public bool noOtherDirections() {
		if (dpadUp + dpadRight + dpadDown + dpadLeft <= 1){
				return true;
		}
		else {
			return false;
		}
	}
}
