using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageCanvas : MonoBehaviour
{
    
    private PlayerControls controls;
    private float jump;
    private float action1;private float action2;private float action3;private float action4;private float action5;
	private float aim; private float shoot; private float blockDodge;
	private float dpadUp; private float dpadDown; private float dpadLeft; private float dpadRight;
	private float menu; 
    private Vector2 move;
	private Vector2 rotate;
    private float posX = 0;
	private float acceleration = 0f;

    private GameObject mainCam;
    private GameObject canvasCam;
	private GameObject canvasGameObject;
	private GameObject canvasInGameGameObject;
    private Canvas canvas;

    public bool inGame = false;
	

    void Start()
    {	
        mainCam = GameObject.Find("Main Camera");
		canvasGameObject = GameObject.Find("Canvas");
		canvasInGameGameObject = GameObject.Find("Canvas In Game");
        canvasCam = GameObject.Find("Canvas Camera");
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		controls = new PlayerControls();
        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
		controls.Gameplay.Move.performed += ctx => move = Vector2.zero;
		controls.Gameplay.Rotate.performed += ctx => rotate = ctx.ReadValue<Vector2>();
		controls.Gameplay.Rotate.canceled += ctx => rotate = Vector2.zero;
		controls.Gameplay.Jump.performed += ctx => jump = ctx.ReadValue<float>();
		controls.Gameplay.Jump.canceled += ctx => jump = 0;
		controls.Gameplay.Action1.performed += ctx => action1 = ctx.ReadValue<float>();
		controls.Gameplay.Action1.canceled += ctx => action1 = 0;
		controls.Gameplay.Action2.performed += ctx => action2 = ctx.ReadValue<float>();
		controls.Gameplay.Action2.canceled += ctx => action2 = 0;
		controls.Gameplay.Action3.performed += ctx => action3 = ctx.ReadValue<float>();
		controls.Gameplay.Action3.canceled += ctx => action3 = 0;
		controls.Gameplay.Action4.performed += ctx => action4 = ctx.ReadValue<float>();
		controls.Gameplay.Action4.canceled += ctx => action4 = 0;
		controls.Gameplay.Action5.performed += ctx => action5 = ctx.ReadValue<float>();
		controls.Gameplay.Action5.canceled += ctx => action5 = 0;
		controls.Gameplay.DPadUp.performed += ctx => dpadUp = ctx.ReadValue<float>();
		controls.Gameplay.DPadUp.canceled += ctx => dpadUp = 0;
		controls.Gameplay.DPadDown.performed += ctx => dpadDown = ctx.ReadValue<float>();
		controls.Gameplay.DPadDown.canceled += ctx => dpadDown = 0;
		controls.Gameplay.DPadLeft.performed += ctx => dpadLeft = ctx.ReadValue<float>();
		controls.Gameplay.DPadLeft.canceled += ctx => dpadLeft = 0;
		controls.Gameplay.DPadRight.performed += ctx => dpadRight = ctx.ReadValue<float>();
		controls.Gameplay.DPadRight.canceled += ctx => dpadRight = 0;
		controls.Gameplay.Aim.performed += ctx => aim = ctx.ReadValue<float>();
		controls.Gameplay.Aim.canceled += ctx => aim = 0;
		controls.Gameplay.Shoot.performed += ctx => shoot = ctx.ReadValue<float>();
		controls.Gameplay.Shoot.canceled += ctx => shoot = 0;
		controls.Gameplay.BlockDodge.performed += ctx => blockDodge = ctx.ReadValue<float>();
		controls.Gameplay.BlockDodge.canceled += ctx => blockDodge = 0;

		if (controls != null){
			controls.Gameplay.Enable();
		}

    }


    void Update()
    {
		if (GameObject.Find("Launcher").GetComponent<Launcher>().connected){ //set to only work after joining game
			if (controls.Gameplay.Menu.triggered){
				print(inGame);
				SwitchCam(inGame);
				inGame = !inGame;
			}
		}
    }
	public void SwitchCam(bool inTheGame) { //true for no menu false for menu (except at start cause this doesn't get called then)
		if (inTheGame == false){
			//canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvasGameObject.SetActive(true);
			canvasInGameGameObject.SetActive(false);
		}
		else {
			//canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvasGameObject.SetActive(false);
			canvasInGameGameObject.SetActive(true);
		}
		
		mainCam.GetComponent<AudioListener>().enabled = inTheGame;
		mainCam.SetActive(inTheGame);
		canvasCam.GetComponent<AudioListener>().enabled = !inTheGame;
		canvasCam.SetActive(!inTheGame);
		
	}
}
