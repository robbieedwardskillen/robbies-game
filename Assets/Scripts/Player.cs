using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Cinemachine;
public class Player : MonoBehaviour {

	public bool super = false;
	public bool alive = true;
	public bool busy = false;
	public bool punching = false;
	public bool bigSwing = false;
	private bool dynamicHitting = false;
	private bool takingDamage = false;
	private bool isAerial = false;
	private bool rolling = false;
	private bool secondHit = false;
    private float range = 200f;
	public float moveSpeed = 1f;
	public float animationMoveSpeed = 0f;
	public bool underWater = false;
	public bool aiming = false;
	private float lerpVal = 0f;
	private CinemachineFreeLook freeLookCam;
	private CinemachineVirtualCamera virtualLookCam;
	private CinemachineTargetGroup targetGroupCam;
	private GameObject currentTarget;
	private GameObject previousTarget;
	private CinemachineBrain cinemachineBrain;
	private Vector3 forward = Vector3.zero;
	private Vector3 right = Vector3.zero;
	private float transitionTime = 0f;
	private float changeWeaponTransitionTime = 0f;
	private float playerHealth;
	private BoxCollider[] boxColliders;
	private CapsuleCollider[] capsuleColliders;
	private Transform fire;
	private Rigidbody[] rigidBodies;
	private Animator playerAnimator;
	private float v = 0f;
	private float h = 0f;
	private PlayerControls controls;
	private Vector2 move;
	private Vector2 rotate;
	private Vector2 attackRotate;
	private Vector2 smoothInputVelocity;
	private float jump;
	private float action1;private float action2;private float action3;private float action4;private float action5;
	private float aim; private float shoot; private float blockDodge;
	private float dpadUp; private float dpadDown; private float dpadLeft; private float dpadRight;
	private float menu;
	private Vector3 movement;
	private Transform spine;
	private Transform head;
	private Transform[] equippedWeps = new Transform[2];
	private Transform chosenWep1;
	private Transform chosenWep2;
	public GameObject grenade;
	public GameObject arrow;
	public GameObject impactEffectGround;
	public GameObject impactEffectPlayer;
	public GameObject impactEffectWall;
	private bool blocking = false;
	private bool firing = false;
	private bool shootingArrow = false;
	private int grenades = 1;
	private int rifleAmmo = 20;
	private int handgunAmmo = 10;
	private float attackSpeed = 1f;
	private Transform grenadeArea;
	private Transform fireArea;
	private Transform followTarget;
	private Transform crossHair;
	private Transform shootZone;
	private ParticleSystem flames;
	private ParticleSystem embers;
	private Transform swordBlade;
	private Transform ignitionFlame;
	private Transform muzzleFlash;
	private Transform muzzleFlash2;
	private ParticleSystem muzzleFlashParticle;
	private ParticleSystem muzzleFlashParticle2;
	private Transform rightHandContainer;
	private Transform rightForearm;
	private Transform handgun;
	private Transform rifle;
	private Transform bow;
	private Transform spear;
	private Transform mace;
	private Transform staff;
	private Transform knife;
	private Transform bigSword;
	private Transform fireSword;
	private MeleeWeaponTrail mwt;
	private MeleeWeaponTrail mwt2;
	private AudioSource audio;
	public AudioClip soundEffectSwing1;
	public AudioClip soundEffectSwing2;
	public AudioClip soundEffectSwing3;
	public AudioClip soundEffectSwing4;
	private Camera m_cam;
	private GameObject camRotateWithZeroY;
	private GameObject m_camRotation;
	private Transition gameTransition;
	private Transform baseOfCharacter;
	private Vector3 lastMoveDirection = Vector3.zero;
	private Vector3 currentRotation;
	private Vector3 lastRotation;
	private Vector3 previousPos;
	private Vector3 velocity;
	bool moveLegs = false;
	bool blockTurning = false;
	bool knockDown = false;
	bool changingWeps = false;
	//----feet
	private void getBase(Transform parent) {
		foreach (Transform child in parent) { 
			if (child.name == "base") {
				baseOfCharacter = child;
			}
			getBase(child);
		}
	}
	//----end feet
	private void allColliders(Transform parent, bool condition) {
		foreach (Transform t in parent) {
			if (t.gameObject.GetComponent<SphereCollider>() != null){
				t.gameObject.GetComponent<SphereCollider>().enabled = condition;
				//Physics.IgnoreCollision(transform.GetComponent<CapsuleCollider>(), t.GetComponent<SphereCollider>(), !condition);
			}
			if (t.gameObject.GetComponent<CapsuleCollider> () != null) {
				t.gameObject.GetComponent<CapsuleCollider>().enabled = condition;
				//Physics.IgnoreCollision(transform.GetComponent<CapsuleCollider>(), t.GetComponent<CapsuleCollider>(), !condition);
			}

			if (t.gameObject.GetComponent<BoxCollider> () != null) {
				t.gameObject.GetComponent<BoxCollider>().enabled = condition;
				//Physics.IgnoreCollision(transform.GetComponent<CapsuleCollider>(), t.GetComponent<BoxCollider>(), !condition);

			}
			if (t.gameObject.GetComponent<Rigidbody> () != null) {
				t.gameObject.GetComponent<Rigidbody> ().isKinematic = !condition;
			}

			allColliders (t, condition);
		}
	}
	void recursiveFindingHead(Transform child){
		foreach(Transform c in child){
			if (c.name == "Fire Area"){
				fireArea = c;
			}
			if (c.name == "Grenade Area"){
				grenadeArea = c;
			}
			if (c.name == "head_28_cap_A" || c.name == "Head"){
				head = c;
				return;
			}
			if (c.childCount > 0){
				recursiveFindingHead(c);
			}
		}
	}
	void recursiveFindingHand(Transform child){
		foreach(Transform c in child){
			if (c.name == "Bip001 R Forearm" || c.name == "Lower_Arm_R"){
				rightForearm = c;
			}
			if (c.name == "R_hand_container"){
				rightHandContainer = c;
				return;
			}
			if (c.childCount > 0){
				recursiveFindingHand(c);
			}
			
		}
	}
	void recursiveFindingSpine(Transform child){
		foreach(Transform c in child){
			if (c.name == "Spine" || c.name == "Bip001 Spine"){
				spine = c;
				return;
			} 
			if (c.childCount > 0){
				recursiveFindingSpine(c);
			}
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
	void Awake () {

		freeLookCam = GameObject.Find("CinemachineCam1").GetComponent<CinemachineFreeLook>();
		virtualLookCam = GameObject.Find("CinemachineCam2").GetComponent<CinemachineVirtualCamera>();

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
		playerAnimator = gameObject.GetComponent<Animator> ();
		playerAnimator.SetFloat("speed", 1.75f);

		previousPos = transform.position;
		playerHealth = 300;
		alive = true;
		audio = gameObject.GetComponent<AudioSource> ();
		audio.volume = 0.2f;
		fire = transform.Find ("Fire");
		recursiveFindingSpine(transform);
		recursiveFindingHead(transform);
		recursiveFindingHand(transform);
		followTarget = gameObject.transform.Find("Follow Target");
		crossHair = GameObject.Find("UI").transform.Find("reticle");
		foreach (Transform child in spine)
		{
			if (child.name == "Flames"){
				flames = child.GetComponent<ParticleSystem>();
				flames.enableEmission = false;
				foreach(Transform c in child){
					if (c.name == "FireEmbers") {
						embers = c.GetComponent<ParticleSystem>();
						embers.enableEmission = false;
					}
				}
			}
		}
		
		
		//----begin player setup
		handgun = rightHandContainer.transform.Find("Handgun");
		rifle = rightHandContainer.transform.Find("Rifle");
		bow = rightHandContainer.transform.Find("Bow");
		spear = rightHandContainer.transform.Find("Spear");
		mace = rightHandContainer.transform.Find("Mace");
		staff = rightHandContainer.transform.Find("Staff");
		knife = rightHandContainer.transform.Find("Knife");
		bigSword = rightHandContainer.transform.Find("BigSword");
		fireSword = rightHandContainer.transform.Find("FireSword");
		shootZone = rightHandContainer.transform.Find("Shoot Zone");
			
		handgun.transform.gameObject.SetActive(false);
		rifle.transform.gameObject.SetActive(false);
		bow.transform.gameObject.SetActive(false);
		spear.transform.gameObject.SetActive(false);
		mace.transform.gameObject.SetActive(false);
		staff.transform.gameObject.SetActive(false);
		knife.transform.gameObject.SetActive(false);
		bigSword.transform.gameObject.SetActive(false);
		fireSword.transform.gameObject.SetActive(false);
		
		
		
		//selection screen get chosen weps and put them into chosenWep1 & chosenWep2
		chosenWep1 = bow;
		chosenWep1.gameObject.SetActive(true);
		chosenWep2 = bigSword;
		equippedWeps[0] = chosenWep1;
		equippedWeps[1] = chosenWep2;
		playerAnimator.SetBool(chosenWep1.name, true);
		muzzleFlash = handgun.Find("MuzzleFlashEffect");
		muzzleFlash2 = rifle.Find("MuzzleFlashEffect");
		muzzleFlashParticle = muzzleFlash.gameObject.GetComponent<ParticleSystem>();
		muzzleFlashParticle2 = muzzleFlash2.gameObject.GetComponent<ParticleSystem>();
		swordBlade = rightHandContainer.transform.Find("FireSword").transform.Find("sword blade");
		ignitionFlame = rightHandContainer.transform.Find("FireSword").transform.Find("IgnitionFlame");
		getBase (gameObject.transform);

		gameObject.GetComponent<Rigidbody> ().isKinematic = false;
		mwt = swordBlade.GetComponent<MeleeWeaponTrail> ();
		mwt2 = bigSword.GetComponent<MeleeWeaponTrail> ();
		swordBlade.transform.GetComponent<CapsuleCollider> ().isTrigger = true;

		//---- end player setup
		m_cam = GameObject.Find("Main Camera").GetComponent<Camera>(); 
		forward = m_cam.transform.forward;
		cinemachineBrain = m_cam.GetComponent<CinemachineBrain>();
		camRotateWithZeroY = GameObject.Find("CamRotateWithZeroY");
		m_camRotation = GameObject.Find("Rotation");
		gameTransition = m_cam.GetComponent<Transition>();
		
		freeLookCam.Priority = 1;
		virtualLookCam.Priority = 0;
		
	}
	void Start() {
		allColliders (gameObject.transform, false);
	}
	IEnumerator waitForSecondBigHit1(){
		yield return new WaitForSeconds(0.5f);
		playerAnimator.Play("bigSwing3", 0, .2f);
		yield return new WaitForSeconds(0.1f);
		if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("bigSwing3"))
		audio.PlayOneShot(soundEffectSwing1, 0.7f);
	}
	IEnumerator waitForSecondBigHit2(){
		yield return new WaitForSeconds(0.5f);
		playerAnimator.Play("bigSwing1", 0, .2f);
		yield return new WaitForSeconds(0.1f);
		if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("bigSwing1"))
		audio.PlayOneShot(soundEffectSwing1, 0.7f);
	}
	IEnumerator waitForSecondSmallHit1() {
		yield return new WaitForSeconds(0.15f);
		if (moveLegs) {
			playerAnimator.Play("swordLeft", 0, .4f);
			audio.PlayOneShot(soundEffectSwing2, 0.7f);
		} else {
			playerAnimator.Play("knifeUp", 0, .4f);
			audio.PlayOneShot(soundEffectSwing2, 0.7f);
		}
	}
	IEnumerator waitForSecondSmallHit2() {
		yield return new WaitForSeconds(0.15f);
		playerAnimator.Play("swordDown", 0, .4f);
		audio.PlayOneShot(soundEffectSwing3, 0.7f);
	}
	IEnumerator waitForSecondPunch() {
		yield return new WaitForSeconds(0.12f);
		if (moveLegs) {
			playerAnimator.Play("punch4", 0, .2f);
		} else {
			playerAnimator.Play("punch2", 0, .2f);
		}
		audio.PlayOneShot(soundEffectSwing2, 0.7f);
	}

 
	void FixedUpdate()
	{
		velocity = (transform.position - previousPos) / Time.deltaTime;
		previousPos = transform.position;

	}
 
	void Update () {
		playerAnimator.SetBool("jumping", isAerial);
		rolling = playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("roll");

		if (alive) {
			h = Input.GetAxis("Horizontal");
			v = Input.GetAxis("Vertical");
		}

	
			//TESTING
/* 		if()
		{
			Debug.Break();
		} */
			//END TESTING
		if (controls.Gameplay.Menu.triggered){
			print("open game menu");
		}
		if (controls.Gameplay.Aim.triggered){
			if(aiming == false){
				aiming = true;
			} else {
				aiming = false;
			}
		} 
		if (!aiming){
			if (controls.Gameplay.Action4.triggered){ //left shoulder button
				//add target
				//if (previousTarget != null){
					//currentTarget = previousTarget
				//} else {
					//currentTarget = 
				//}
				
				//previousTarget = 
			}
			if (controls.Gameplay.Action5.triggered){ //right shoulder button
				//not sure yet
			}
		}
		
		if (rifle.gameObject.activeSelf == true){
			if (aiming == true){ //smooth the layer weight
					lerpVal += Time.deltaTime * 8f;
					playerAnimator.SetLayerWeight(6, Mathf.Lerp(lerpVal, 1, Time.deltaTime));
				if (controls.Gameplay.Shoot.triggered){
					if(playerAnimator.GetCurrentAnimatorStateInfo(6).normalizedTime > 0.9f){
						firingRifle();
					}

				}
			} else {
				lerpVal = 0f;
				playerAnimator.SetLayerWeight(6, 0);
			}
		}

		if ((handgun.gameObject.activeSelf == true)){
			if (aiming == true){
					lerpVal += Time.deltaTime * 8f;
					playerAnimator.SetLayerWeight(7, Mathf.Lerp(lerpVal, 1, Time.deltaTime));
				if (controls.Gameplay.Shoot.triggered){
					if(playerAnimator.GetCurrentAnimatorStateInfo(7).normalizedTime > 0.9f){
						muzzleFlashParticle.Play();
						playerAnimator.Play("Firing Handgun", -1, 0f);
						firingHandgun();
					}
					
				}
			} else {
				lerpVal = 0f;
				playerAnimator.SetLayerWeight(7, 0);
			}
		}

		bool equippedFireSword = (fireSword.gameObject.activeSelf == true);
		bool equippedHandgun = (handgun.gameObject.activeSelf == true);
		bool equippedRifle = (rifle.gameObject.activeSelf == true);
		bool equippedBow = (bow.gameObject.activeSelf == true);
		bool equippedBigSword = (bigSword.gameObject.activeSelf == true);
		bool equippedNothing = ((fireSword.gameObject.activeSelf == false) &&
		(bow.gameObject.activeSelf == false) &&
		(handgun.gameObject.activeSelf == false) && 
		(rifle.gameObject.activeSelf == false) && 
		(bigSword.gameObject.activeSelf == false)); 


		if (equippedBigSword)
		attackRotate = Vector2.SmoothDamp(attackRotate, rotate, ref smoothInputVelocity, 1 / (attackSpeed * 15f));
		else if (equippedBow || equippedHandgun || equippedRifle)
		attackRotate = Vector2.SmoothDamp(attackRotate, rotate, ref smoothInputVelocity, 1 / (attackSpeed * 25f));
		//add speeds for the rest
		else
		attackRotate = new Vector2(0f,0f);
		playerAnimator.SetFloat("attackRotateX", attackRotate.x);
		playerAnimator.SetFloat("attackRotateY", attackRotate.y);


		if(takingDamage && alive){
			StartCoroutine(blinking());
		}
		underWater = (transform.position.y <= 40.3f) ? true : false;
		if (transform.position.y < 15f){
			playerHealth = 0;
		}
		isAerial = !IsGrounded ();
		if (isAerial){
			playerAnimator.SetLayerWeight(5, 1);
		} else {
			playerAnimator.SetLayerWeight(5, 0);
		}
		bool gettingUp = playerAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Getting Up");
		bool punch1 = playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("punch1");
		bool punch2 = playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("punch2");
		bool punch3 = playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("punch3");
		bool punch4 = playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("punch4");
		bool kick = playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("kick");
		bool fireball = playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("fireball");
		bool knifeRight = playerAnimator.GetCurrentAnimatorStateInfo (0).IsName ("knifeRight");
		bool swordLeft = playerAnimator.GetCurrentAnimatorStateInfo (0).IsName ("swordLeft");
		bool swordRight = playerAnimator.GetCurrentAnimatorStateInfo (0).IsName ("swordRight");
		bool knifeUp = playerAnimator.GetCurrentAnimatorStateInfo (0).IsName ("knifeUp");
		bool knifeDown = playerAnimator.GetCurrentAnimatorStateInfo (0).IsName ("knifeDown");
		bool swordUp = playerAnimator.GetCurrentAnimatorStateInfo (0).IsName ("swordUp");
		bool swordDown = playerAnimator.GetCurrentAnimatorStateInfo (0).IsName ("swordDown");
		var rb = transform.GetComponent<Rigidbody>();
		if (underWater){
			if (jump == 1 && transform.position.y <= 40f) {
					moveLegs = false;
					gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 4f, 0));
				}		
			if (isAerial){
				rb.drag = 7.5f;
			} else {
				rb.drag = 2.5f;
			}
			rb.mass = 1f;
		} else {
			rb.mass = 1f;
			rb.drag = 2.5f;
		}

		if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("bigSwing1") ||
		playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("bigSwing2") ||
		playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("bigSwing3") ||
		playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("bigSwing4") ||
		playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("bigSwing5") ||
		playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("bigSwing6")){
			mwt2.enabled = true;
			bigSword.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
			bigSwing = true;
		} else {
			if (!dynamicHitting){
				bigSword.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
				mwt2.enabled = false;
				bigSwing = false;
			}
		}

		//----begin player movement and rotation

		if (alive) {
			//squatting animation
			blocking = playerAnimator.GetBool("blocking");
			if (playerHealth < 3) {
				playerAnimator.SetBool("injured", true);
			}
			if (controls.Gameplay.Action2.triggered && !busy && !changingWeps) {
				playerAnimator.SetTrigger("changeWep");
				StartCoroutine(changeWeaponTime());
			}
			if (punch1 || punch2 || punch3 || punch4 || kick || fireball) {
				punching = true;
			} else {
				punching = false;
			}
			if (knifeRight || knifeUp || knifeDown || swordLeft || swordRight || swordUp || swordDown) {
				mwt.enabled = true;
				swordBlade.transform.GetComponent<CapsuleCollider> ().enabled = true;
				ignitionFlame.transform.gameObject.SetActive(true);
				swordBlade.transform.gameObject.SetActive(true);
				busy = true;

			} else {
				if (blocking){
					ignitionFlame.transform.gameObject.SetActive(true);
					swordBlade.transform.gameObject.SetActive(true);
				} else {
					swordBlade.transform.GetComponent<CapsuleCollider> ().enabled = false;
					mwt.enabled = false;
					ignitionFlame.transform.gameObject.SetActive(false);
					swordBlade.transform.gameObject.SetActive(false);
				}

				busy = false;
			}

			if (Mathf.Abs(v) > Mathf.Abs(h)){
				playerAnimator.SetFloat("averageSpeed", Mathf.Abs(v));
			} else {
				playerAnimator.SetFloat("averageSpeed", Mathf.Abs(h));
			}
			
			playerAnimator.SetFloat ("horizontalSpeed", h);
			playerAnimator.SetFloat ("verticalSpeed", v);
			
			if (rolling && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f
			&& playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.6f){
				transform.GetComponent<Rigidbody>().AddForce(transform.forward * 23f); //add force to roll anim during actual roll time
			}
			if (!rolling && !changingWeps){
				//start attacks
				if (controls.Gameplay.Action4.triggered && !isAerial) {
					playerAnimator.SetTrigger("roll");
				} 
				
				if (!isAerial) {
					if (equippedBigSword && !blocking){
						if (controls.Gameplay.Action1.triggered && !bigSwing){
							StartCoroutine(waitForSound(soundEffectSwing1, "bigSwing4"));
							secondHit = false;
						}
						if (controls.Gameplay.Action1.triggered && bigSwing && !secondHit){
							StartCoroutine(waitForSecondBigHit1());
							secondHit = true;
						}
					}
					if (equippedBigSword && blocking) {
						playerAnimator.speed = 1;
						if (controls.Gameplay.Action1.triggered && !bigSwing) {
							StartCoroutine(waitForSound(soundEffectSwing1, "bigSwing6"));
							secondHit = false;
						}
						if (controls.Gameplay.Action1.triggered && bigSwing && !secondHit) {
							StartCoroutine(waitForSecondBigHit2());
							secondHit = true;
						}
					}
					if (equippedFireSword && !blocking){
						if (controls.Gameplay.Action1.triggered && !busy) {
							if (moveLegs) {
								playerAnimator.Play("swordRight", 0, .4f);
								audio.PlayOneShot(soundEffectSwing1, 0.7f);
							} else {
								playerAnimator.Play("knifeRight", 0, .4f);
								audio.PlayOneShot(soundEffectSwing1, 0.7f);
							}
							secondHit = false;
						}
						if (controls.Gameplay.Action1.triggered && busy && secondHit == false) {
							StartCoroutine(waitForSecondSmallHit1());
							secondHit = true;
						}
					}
					if (equippedNothing && !blocking) {
						if (controls.Gameplay.Action1.triggered && !punching) {
							audio.PlayOneShot(soundEffectSwing1, 0.7f);
							if (moveLegs) {
								playerAnimator.Play("punch3", 0, .2f);
							} else {
								playerAnimator.Play("punch1", 0, .2f);
							}
							secondHit = false;
						}
						if (controls.Gameplay.Action1.triggered && punching && secondHit == false) {
							StartCoroutine(waitForSecondPunch());
							secondHit = true;
						}
					}
					if (equippedNothing && blocking) {
						playerAnimator.speed = 1;
						if (controls.Gameplay.Action1.triggered && !punching) {
							StartCoroutine(shootFireball());
							playerAnimator.Play("fireball", 0, .5f);
							secondHit = false;
						}
					}
				}
				else {

				if (equippedFireSword && !blocking){
					if (controls.Gameplay.Action1.triggered && !busy){
							playerAnimator.Play("swordUp", 0, .4f);
							audio.PlayOneShot(soundEffectSwing4, 0.7f);
							secondHit = false;
						}
						if (controls.Gameplay.Action1.triggered && busy && secondHit == false){
							StartCoroutine(waitForSecondSmallHit2());
							secondHit = true;
						}
					}
					if (equippedNothing && !blocking) {
						if (controls.Gameplay.Action1.triggered && !punching) {
							audio.PlayOneShot(soundEffectSwing1, 0.7f);
							playerAnimator.Play("kick", 0, .2f);
							secondHit = false;
						}

					}
				}
				if (equippedBigSword){
					if (controls.Gameplay.Action1.triggered && !bigSwing){
						playerAnimator.Play("bigSwing5", 0, .2f);
						audio.PlayOneShot(soundEffectSwing1, 0.7f);
					}
					if (shoot > 0.5f){
						dynamicHitting = true;
						mwt2.enabled = true;
						bigSword.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
						transitionTime += Time.deltaTime * 8; // so it doesn't teleport arms up
						playerAnimator.SetLayerWeight(11, Mathf.Lerp(0f, 1f, transitionTime));
					} else {
						dynamicHitting = false;
						if(!bigSwing){
							bigSword.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
							mwt2.enabled = false;
							bigSwing = false;
						}
						transitionTime = 0f;
						playerAnimator.SetLayerWeight(11, 0f);
					}
				}
				if (equippedHandgun){
					playerAnimator.SetLayerWeight(1, 0f);
					if (handgunAmmo <= 0 || playerAnimator.GetCurrentAnimatorStateInfo(4).normalizedTime < 1){
						playerAnimator.SetLayerWeight(4, 1f);
					} else {
						playerAnimator.SetLayerWeight(4, 0f);
					}
					if (shoot > 0.5f && !aiming) {
						if (playerAnimator.GetCurrentAnimatorStateInfo(4).normalizedTime >= 1 && !playerAnimator.IsInTransition(4)){
							if (handgunAmmo > 0){
								playerAnimator.SetBool("ShootingHandgun", true);
							} else {
								playerAnimator.SetBool("ShootingHandgun", false);
							}
							StartCoroutine(shootingHandgun());
						}
					}  else {
						playerAnimator.SetBool("ShootingHandgun", false);
					}
					
				}
				if (equippedRifle){
					playerAnimator.SetLayerWeight(1, 0f);
					if (rifleAmmo <= 0 || playerAnimator.GetCurrentAnimatorStateInfo(4).normalizedTime < 1){
						playerAnimator.SetLayerWeight(4, 1f);
					} else {
						playerAnimator.SetLayerWeight(4, 0f);
					}
					if (shoot > 0.5f && !aiming) {
						if (playerAnimator.GetCurrentAnimatorStateInfo(4).normalizedTime >= 1 && !playerAnimator.IsInTransition(4)){
							if (rifleAmmo > 0) {
								playerAnimator.SetBool("ShootingRifle", true);
							} else {
								playerAnimator.SetBool("ShootingRifle", false);
							}
							StartCoroutine(shootingRifle());
						}
					}  else {
						playerAnimator.SetBool("ShootingRifle", false);
					}
				} 
				if (equippedBow && !blocking) {
					if (shoot > 0.5f && !aiming) {
						//bow has infinite ammo
						playerAnimator.SetBool("ShootingBow", true);
						transitionTime += Time.deltaTime * 8; // so it doesn't teleport arms up
						playerAnimator.SetLayerWeight(1, Mathf.Lerp(0f, 1f, transitionTime));
						float bowShootTime = playerAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime % 1;
						if (bowShootTime >= 0.2f && bowShootTime <= 0.4f){
							if (shootingArrow == false) {
								StartCoroutine (shootArrow ());
							}
						} 
					} else {
						playerAnimator.SetBool("ShootingBow", false);
						transitionTime = 0f;
						playerAnimator.SetLayerWeight(1, 0f);
					}
					
					//if holding fire button
				}
				else if (!equippedBow) {
					playerAnimator.SetBool("ShootingBow", false);
					playerAnimator.SetLayerWeight(1,0f);
				}

				//end attacks
				if (controls.Gameplay.Action5.triggered) {
					if (grenades > 0) {
						playerAnimator.SetTrigger ("throw");
						StartCoroutine (rollThenBlowUp ());
					} 
				} 

				if (controls.Gameplay.Jump.triggered && !isAerial && !playerAnimator.GetBool("blocking") && !underWater) {
					//moveLegs = false;
					//audio.PlayOneShot (playerJump, 1.2f);
					gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 200f, 0));
				}		


				if (action3 == 1) {
					playerAnimator.SetBool("blocking", true);
					lastRotation = transform.rotation.eulerAngles;
					StartCoroutine("checkForMovement", lastRotation);
					if (lastRotation.y != currentRotation.y){
						if (!blockTurning){
							StartCoroutine(strafeABit());
						}
					} 
				} else {
					playerAnimator.SetBool("blocking", false);
				}	
	
			}

			if (h < 0.01 && h > -0.01 && v < 0.01 && v > -0.01) {
				moveLegs = false;
				playerAnimator.SetBool("moving", false);
			} else {
				if (!gameTransition.transitioning && !playerAnimator.GetBool("blocking") && !bigSwing && !knockDown &&
				!playerAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Getting Up")){
					moveLegs = true;
					playerAnimator.SetBool("moving", true);
				} else {
					if ((bigSwing && rolling) || (bigSwing && isAerial)){
						moveLegs = true;
						playerAnimator.SetBool("moving", true);
					} else {
						moveLegs = false;
						playerAnimator.SetBool("moving", false);
					}
					
				}
			}
			//----begin player death
			if (playerHealth <= 0) {
				playerAnimator.enabled = false;
				allColliders (gameObject.transform, false);//don't ignore collisions
				fire.gameObject.SetActive (false);
				gameObject.GetComponent<CapsuleCollider> ().isTrigger = true;
				gameObject.GetComponent<Rigidbody> ().isKinematic = true;
				allColliders (gameObject.transform, true);
				alive = false;
			}
			//----end player death


		


			if(virtualLookCam.Priority > 0){
				m_camRotation.transform.position = virtualLookCam.transform.position;
				m_camRotation.transform.rotation = virtualLookCam.transform.rotation;
			}
			
			if (aiming){
				if(bow.gameObject.activeSelf){
					crossHair.gameObject.SetActive(false);
				} else {
					crossHair.gameObject.SetActive(true);
				}
				playerAnimator.SetFloat("rotateY", Mathf.Sin(-m_camRotation.transform.eulerAngles.x * (Mathf.PI / 180)));//radians to degrees
				transform.rotation = Quaternion.Euler(0f, m_camRotation.transform.eulerAngles.y, 0f);
				RaycastHit hit;
				if(Physics.Raycast(m_camRotation.transform.position, m_camRotation.transform.forward, out hit, 200.0f)){
					Debug.DrawRay(m_camRotation.transform.position, m_camRotation.transform.forward * 100.0f, Color.blue);
					followTarget.LookAt(hit.point);//fix later
				} else {
					followTarget.transform.rotation = Quaternion.Euler(m_camRotation.transform.eulerAngles.x,
					m_camRotation.transform.eulerAngles.y, m_camRotation.transform.eulerAngles.z);
				}
			} else {
				crossHair.gameObject.SetActive(false);
			}

			camRotateWithZeroY.transform.position = new Vector3(m_cam.transform.position.x, transform.position.y, m_cam.transform.position.z);
			camRotateWithZeroY.transform.LookAt(transform.position);

			forward = camRotateWithZeroY.transform.forward;
			right = camRotateWithZeroY.transform.right;

	
			forward.y = 0f;
			Quaternion rotation;
			
			Vector3 moveDirection = (h * right + v * forward);
			//moveDirection is not a direction it is a position just copied the name from a dum

			if (knockDown == true){
				playerAnimator.SetBool("falling", true);
			} else {
				playerAnimator.SetBool("falling", false);
			}

			if (h > 0.80f || v > 0.80f || h < -0.80f || v < -0.80f){
				moveSpeed = 2f;
			} else {
				moveSpeed = 1f;
			}
			if (moveDirection != Vector3.zero) {
				rotation = Quaternion.LookRotation(Vector3.up);
			} else {
				rotation = Quaternion.Euler (forward);
			}
			movement = Vector3.ClampMagnitude(new Vector3(moveDirection.x * Time.deltaTime * moveSpeed, moveDirection.y,
				moveDirection.z * Time.deltaTime * moveSpeed) * 100, 1f);
 			if (moveDirection != Vector3.zero && !knockDown && !gettingUp && !fireball && !aiming) { //!aiming
				rotation = Quaternion.LookRotation (moveDirection);
				lastMoveDirection = moveDirection; 
			}  
			
			if (!aiming && (playerAnimator.GetBool("ShootingHandgun") || playerAnimator.GetBool("ShootingRifle") ||
			playerAnimator.GetBool("ShootingBow") || dynamicHitting) || playerAnimator.GetLayerWeight(4) >= 1 && !playerAnimator.GetBool("blocking")){
				if (playerAnimator.GetLayerWeight(4) < 1f){//reloading
					firing = true;
				} else {
					firing = false;
				}
				freeLookCam.m_YAxis.m_MaxSpeed = 0; //can't rotate vertically
				freeLookCam.m_XAxis.m_MaxSpeed = 0; //can't rotate horizontally
				playerAnimator.SetFloat("rotateY", rotate.y); //freelook aim up/down
				playerAnimator.SetFloat("rotateX", rotate.x); //freelook aim left/right (bow)

				//strafing and turning free look

				var forwardA = m_cam.transform.rotation * Vector3.forward;
				var forwardB = transform.rotation * Vector3.forward;

				// get a numeric angle for each vector, on the X-Z plane (relative to world forward)
				var angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
				var angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;

				// get the signed difference in these angles
				var relativeAngle = -Mathf.DeltaAngle(angleA, angleB);
				Vector3 relativeRotationVec = new Vector3(0f, relativeAngle, 0f);
				Quaternion relativeRotation = Quaternion.Euler(relativeRotationVec);
				Vector3 relativeRotationForward = relativeRotation * Vector3.forward;
				Vector3 relativeRotationRight = relativeRotation * Vector3.right;
				Vector3 relativeMoveDirection = (relativeRotationRight * h + relativeRotationForward * v);

				float howVertical = relativeMoveDirection.z;
				float howHorizontal = relativeMoveDirection.x;
				//*******need to work on this stuff********
				//reverse horizontal
				playerAnimator.SetFloat("horizontalStrafing", howHorizontal);
				playerAnimator.SetFloat("verticalStrafing", howVertical);
				if (howVertical < 0f && Mathf.Abs(howHorizontal) < 0.1f){ //moving backwards without strafing
					playerAnimator.SetLayerWeight(9, 0f);
					playerAnimator.SetLayerWeight(3, 1f);
					playerAnimator.SetFloat("turn", 1f);
				} else if (howVertical >= 0f){ //moving forwards
					playerAnimator.SetLayerWeight(9, howVertical - Mathf.Abs(howHorizontal) * 2 );//how vertical joystick is - a set horizontal value
					playerAnimator.SetLayerWeight(2, Mathf.Abs(howHorizontal) * 2);
				} else if (howVertical < 0f && Mathf.Abs(howHorizontal) > 0.1f){ //moving backwards with strafing
					playerAnimator.SetLayerWeight(9, 0f);
					playerAnimator.SetLayerWeight(2, Mathf.Abs(howHorizontal) * 2); 
				}
				
				if((!moveLegs && Mathf.Abs(howHorizontal) == 0f && Mathf.Abs(rotate.x) > 0.01f)){ //not moving and rotating
					playerAnimator.SetFloat("turn", 1f);
				} else {
					if (howVertical < 0f){
						playerAnimator.SetLayerWeight(3, Mathf.Abs(howVertical) - Mathf.Abs(howHorizontal));
						playerAnimator.SetFloat("turn", Mathf.Abs(howVertical) - Mathf.Abs(howHorizontal));
					} else {
						if (howVertical >= 0f && !blockTurning){
							playerAnimator.SetLayerWeight(3, 0f);
							playerAnimator.SetFloat("turn", 0f);
						}
					}
				}
			}
			else if (!aiming) {
				firing = false;
				freeLookCam.m_YAxis.m_MaxSpeed = 2; //can rotate vertically again
				freeLookCam.m_XAxis.m_MaxSpeed = 200;
				playerAnimator.SetLayerWeight(9, 0f);
				playerAnimator.SetLayerWeight(2, 0f);
				if (!blockTurning)
				playerAnimator.SetLayerWeight(3, 0f);
			}
			if (aiming){
				freeLookCam.Priority = 0;
				virtualLookCam.Priority = 1;
				StartCoroutine(transitionToCam2());
				//strafing and turning virtual cam
				float howVertical = Input.GetAxis("Vertical");
				float howHorizontal = Input.GetAxis("Horizontal");
				playerAnimator.SetFloat("verticalStrafing", howVertical);
				playerAnimator.SetFloat("horizontalStrafing", howHorizontal);
				playerAnimator.SetLayerWeight(2, Mathf.Abs(howHorizontal));
				playerAnimator.SetFloat("turn", rotate.x);
				if (howVertical < 0f){ //not moving forward (is at idle animation when not moving)
					playerAnimator.SetFloat("turn", 1f);
				}
				
				if((!moveLegs && Mathf.Abs(howHorizontal) == 0f && Mathf.Abs(rotate.x) > 0.01f)){ //not moving and rotating
					playerAnimator.SetFloat("turn", 1f);
				} else {
					if (howVertical < 0f){
						playerAnimator.SetLayerWeight(3, Mathf.Abs(howVertical) - Mathf.Abs(howHorizontal));
						playerAnimator.SetFloat("turn", Mathf.Abs(howVertical) - Mathf.Abs(howHorizontal));
					} else {
						if (howVertical >= 0f && !blockTurning){
							playerAnimator.SetLayerWeight(3, 0f);
							playerAnimator.SetFloat("turn", 0f);
						}
	
					}
				}
			} else {
				StartCoroutine(transitionToCam1());
				virtualLookCam.Priority = 0;
				freeLookCam.Priority = 1;
			}
			if (lastMoveDirection != Vector3.zero) {
				rotation = Quaternion.LookRotation (lastMoveDirection);
			} else {
				rotation = Quaternion.identity;
			}
			if (!gameTransition.transitioning && alive){
				if (rolling) {
					if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f) {
						transform.position += movement * (moveSpeed * 1f) * Time.deltaTime;
					} else {
						transform.position += movement * (moveSpeed * 0.4f) * Time.deltaTime;
					}
				} else {
					if (!playerAnimator.GetBool("blocking") && !bigSwing && !knockDown && !gettingUp && !fireball) {
						transform.position += movement * moveSpeed * Time.deltaTime;
					}
					if (bigSwing && isAerial) {
						transform.position += movement * moveSpeed * Time.deltaTime;
					}

				}
			}

			if (!aiming && !firing){
				transform.rotation = rotation;
			} 
			
		}
	}

	IEnumerator waitForSound (AudioClip ac, string anim){
		playerAnimator.Play(anim, 0, .2f);
		yield return new WaitForSeconds(0.35f);
		if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim)){
			audio.PlayOneShot(ac, 0.7f);
		}
	}
	IEnumerator checkForMovement(Vector3 lastRotation){
		yield return new WaitForSeconds(0.1f);
		currentRotation = transform.rotation.eulerAngles;
	}
	IEnumerator strafeABit(){
		blockTurning = true;
		playerAnimator.SetLayerWeight(3, 0.6f);
		playerAnimator.SetFloat("turn", 1f);
		yield return new WaitForSeconds(0.4f);
		blockTurning = false;
		playerAnimator.SetLayerWeight(3, 0f);
		playerAnimator.SetFloat("turn", 0f);

	}
	IEnumerator transitionToCam1() {
		yield return new WaitForSeconds(0.09f);
		virtualLookCam.Priority = 0;
		freeLookCam.Priority = 1;
	}
	IEnumerator transitionToCam2() {
		yield return new WaitForSeconds(0.09f);
		virtualLookCam.Priority = 1;
		freeLookCam.Priority = 0;
	}
	IEnumerator rollThenBlowUp() {
		grenades -= 1;
		yield return new WaitForSeconds(0.25f);
		GameObject newGrenade;
		newGrenade = Instantiate (grenade, grenadeArea.position, grenadeArea.rotation) as GameObject;
		newGrenade.GetComponent<Rigidbody> ().AddForce (grenadeArea.forward * 200);
		newGrenade.GetComponent<Rigidbody> ().AddForce (transform.up * 25);
		yield return new WaitForSeconds(2);
		newGrenade.transform.GetChild (0).gameObject.SetActive(true);
		newGrenade.gameObject.tag = "Explosion";
		newGrenade.transform.GetComponent<SphereCollider> ().isTrigger = true;
		newGrenade.transform.GetComponent<SphereCollider> ().radius = 12;

		newGrenade.gameObject.GetComponent<Renderer> ().enabled = false;
		yield return new WaitForSeconds (2);
		grenades = 1;
		Destroy (newGrenade);
	}
	IEnumerator shootFireball() {
		yield return new WaitForSeconds(0.2f);
		flames.enableEmission = true;
		embers.enableEmission = true;
		yield return new WaitForSeconds(0.3f);
		flames.enableEmission = false;
		embers.enableEmission = false;
	}
	IEnumerator changeWeaponTime() {
		changingWeps = true;
		while(changeWeaponTransitionTime < 1f){
			changeWeaponTransitionTime += Time.deltaTime * 8; 
			playerAnimator.SetLayerWeight(10, Mathf.Lerp(0f, 1f, changeWeaponTransitionTime));
		}
		
		playerAnimator.Play("changeWep", 10, 0);
		yield return new WaitForSeconds(0.4f);
		if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shoot") &&
		!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fireing") && 
		!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle Rifle") &&
		!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blend Tree Rifle")){
			bool alreadyChanged = false;
			if (equippedWeps[0].gameObject.activeSelf){
				alreadyChanged = true;
				equippedWeps[0].gameObject.SetActive(false);
				equippedWeps[1].gameObject.SetActive(true);
				playerAnimator.SetBool(chosenWep2.name, true);
				playerAnimator.SetBool(chosenWep1.name, false);
				if (equippedWeps[1] == fireSword){
					fireSword.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
				}
				if (equippedWeps[1] == bigSword){
					bigSword.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
				}
			} 
			if (equippedWeps[1].gameObject.activeSelf && !alreadyChanged){
				equippedWeps[1].gameObject.SetActive(false);
				equippedWeps[0].gameObject.SetActive(true);
				playerAnimator.SetBool(chosenWep2.name, false);
				playerAnimator.SetBool(chosenWep1.name, true);
				if (equippedWeps[0] == fireSword){
					fireSword.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
				}
				if (equippedWeps[0] == bigSword){
					bigSword.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
				}
			}
			
		}
		playerAnimator.SetLayerWeight(10,0f);
		changeWeaponTransitionTime = 0f;
		changingWeps = false;
	}
	void Land(){
		//needed for land animation
	}
	void OnTriggerEnter (Collider col) {
		if (alive) {
			if (col.gameObject.tag == "Explosion") {
				StartCoroutine (takeBigDamage ());
			}
			if (col.gameObject.tag == "bullet" || col.gameObject.tag == "bigBullet"){
				takeDamageNoWait (col.gameObject.tag);
			}
			if (col.gameObject.tag == "damage" && !takingDamage){
				if (mwt.enabled && transform.root != col.gameObject.transform.root)
				StartCoroutine (takeDamage());
			} 
			if (col.gameObject.tag == "Medium Damage" && !takingDamage){
				if(mwt2.enabled && transform.root != col.gameObject.transform.root)
				StartCoroutine (takeMediumDamage());
			}
		}
	}
	void OnParticleCollision(GameObject other){
		if (other.name == "Flames") {
			StartCoroutine(takeMediumWithFall());
		}
	}
 	void OnCollisionEnter (Collision col){
			if (col.gameObject.tag == "Explosion") {
				StartCoroutine (takeBigDamage ());
			}
			if (col.gameObject.tag == "arrow"){
				if (!gameTransition.transitioning){
					playerHealth -= 3f;
					Destroy(col.gameObject);
				}
			}
			if (col.gameObject.name == "ground"){
				playerAnimator.SetBool("falling", false); 
				isAerial = false;
			}
	} 
	IEnumerator takeBigDamage() {
		if (!gameTransition.transitioning){
			knockDown = true;
			takingDamage = true;
			playerHealth -= 4;
			yield return new WaitForSeconds (1);
			takingDamage = false;
			if (IsGrounded()){
				playerAnimator.SetBool("falling", false); 
				playerAnimator.SetTrigger("getUp");
				knockDown = false;
			} else {
				yield return new WaitForSeconds(0.25f);
			}
		}
	}
	IEnumerator takeMediumWithFall() {
		if (!gameTransition.transitioning){
			knockDown = true;
			takingDamage = true;
			playerHealth -= 3;
			yield return new WaitForSeconds (1);
			takingDamage = false;
			if (IsGrounded()){
				playerAnimator.SetBool("falling", false); 
				playerAnimator.SetTrigger("getUp");
				knockDown = false;
			} else {
				yield return new WaitForSeconds(0.25f);
			}
		}
	}
	IEnumerator takeMediumDamage() {
		if (!gameTransition.transitioning){
			takingDamage = true;
			playerHealth -= 3f;
			yield return new WaitForSeconds (1);
			takingDamage = false;
		}
	}
	IEnumerator takeDamage() {
		
		if (!gameTransition.transitioning){
			takingDamage = true;
			playerHealth -= 2f;
			yield return new WaitForSeconds (1);
			takingDamage = false;
		}
	}
	void takeDamageNoWait(string bulletType) {
		if (!gameTransition.transitioning){
			if (bulletType == "bullet")
			playerHealth -= 1f;
			if (bulletType == "bigBullet")
			playerHealth -= 3f;
			if (bulletType == "arrow"){
				
				playerHealth -= 3f;
			}
		}
	}
	IEnumerator blinking() {
		for (int i = 0; i < 5; i++){
			head.gameObject.SetActive(false);
			gameObject.transform.Find("body").gameObject.SetActive(false);
			yield return new WaitForSeconds (0.1f);
			head.gameObject.SetActive(true);
			gameObject.transform.Find("body").gameObject.SetActive(true);
			yield return new WaitForSeconds (0.1f);
		}
	}
	void firingHandgun() {
		if(handgunAmmo > 0){
			handgunAmmo -= 1;
			RaycastHit hit;
			if (IsGrounded()){
				if (Physics.Raycast(fireArea.transform.position, m_cam.transform.forward, out hit, range)){
					if (hit.rigidbody != null) {
						hit.rigidbody.AddForce(hit.normal * -15f);
					}
					if (hit.transform.tag == "Player" || hit.transform.name == "cop"){
						impactEffectPlayer.tag = "bullet";
						Instantiate(impactEffectPlayer, hit.point, Quaternion.LookRotation(hit.normal));
					} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
						Instantiate(impactEffectWall, hit.point, Quaternion.LookRotation(hit.normal));
					} else {
						Instantiate(impactEffectGround, hit.point, Quaternion.LookRotation(hit.normal));
					}
				}
			} else {
				if (Physics.Raycast(handgun.position, handgun.forward, out hit, range)){
					if (hit.rigidbody != null) {
						hit.rigidbody.AddForce(hit.normal * -15f);
					}
					if (hit.transform.tag == "Player" || hit.transform.name == "cop"){
						impactEffectPlayer.tag = "bullet";
						Instantiate(impactEffectPlayer, hit.point, Quaternion.LookRotation(hit.normal));
					} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
						Instantiate(impactEffectWall, hit.point, Quaternion.LookRotation(hit.normal));
					} else {
						Instantiate(impactEffectGround, hit.point, Quaternion.LookRotation(hit.normal));
					}
				}
			}
		} else {
			playerAnimator.Play("Handgun Reload", 4, 0);
			handgunAmmo = 1000;//testing
		}
	}
	void firingRifle() { //for zoomed in
		if(rifleAmmo > 0){
			rifleAmmo -= 1;
			muzzleFlashParticle2.Play();
			//playerAnimator.Play("Firing Rifle", -1, 0f);
			playerAnimator.SetTrigger("shoot");
			RaycastHit hit;
			if (IsGrounded()){
				if (Physics.Raycast(fireArea.transform.position, m_cam.transform.forward, out hit, range)){
					if (hit.rigidbody != null) {
						hit.rigidbody.AddForce(hit.normal * -15f);
					}
					if (hit.transform.tag == "Player" || hit.transform.name == "cop"){
						impactEffectPlayer.tag = "bullet";
						Instantiate(impactEffectPlayer, hit.point, Quaternion.LookRotation(hit.normal));
					} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
						Instantiate(impactEffectWall, hit.point, Quaternion.LookRotation(hit.normal));
					} else {
						Instantiate(impactEffectGround, hit.point, Quaternion.LookRotation(hit.normal));
					}
				}
			} 
		} else {
			playerAnimator.Play("Rifle Reload", 4, 0);
			rifleAmmo = 30;
		}
	}
	IEnumerator shootingHandgun() {
		yield return new WaitForSeconds (0.15f);
		if(handgunAmmo > 0){
			
			if (!muzzleFlashParticle.isPlaying) {
				handgunAmmo -= 1;
				muzzleFlashParticle.Play();
				playerAnimator.SetTrigger("shoot");
				RaycastHit hit;
				if (Physics.Raycast(handgun.position, handgun.forward, out hit, range)){
					if (hit.rigidbody != null) {
						hit.rigidbody.AddForce(hit.normal * -15f);
					}
					if (hit.transform.tag == "Player" || hit.transform.name == "cop"){
						impactEffectPlayer.tag = "bullet";
						Instantiate(impactEffectPlayer, hit.point, Quaternion.LookRotation(hit.normal));
					} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
						Instantiate(impactEffectWall, hit.point, Quaternion.LookRotation(hit.normal));
					} else {
						Instantiate(impactEffectGround, hit.point, Quaternion.LookRotation(hit.normal));
					}
				}
				

			}
		} else {
			playerAnimator.Play("Handgun Reload", 4, 0);
			handgunAmmo = 1000;
		}
		
	}

	IEnumerator shootingRifle() {
		yield return new WaitForSeconds (0.15f);
		Vector3 eulerRot = new Vector3(rifle.eulerAngles.x, rifle.eulerAngles.y, rifle.eulerAngles.z);
		shootZone.eulerAngles = eulerRot;

		if (rifleAmmo > 0){
			if (!muzzleFlashParticle2.isPlaying) {
				rifleAmmo -= 1;
				muzzleFlashParticle2.Play();
				playerAnimator.SetTrigger("shoot");
				RaycastHit hit;
				if (IsGrounded()){//since gun is 90 degrees the wrong way
					if (Physics.Raycast(shootZone.position, -shootZone.up, out hit, range)){
						if (hit.transform.tag == "Player" || hit.transform.name == "cop"){
							impactEffectPlayer.tag = "bullet";
							Instantiate(impactEffectPlayer, hit.point, Quaternion.LookRotation(hit.normal));
						} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
							Instantiate(impactEffectWall, hit.point, Quaternion.LookRotation(hit.normal));
						} else {
							Instantiate(impactEffectGround, hit.point, Quaternion.LookRotation(hit.normal));
						}
					}
				} else {
					if (Physics.Raycast(rifle.position, -rifle.up, out hit, range)){
						if (hit.transform.tag == "Player" || hit.transform.name == "cop"){
							impactEffectPlayer.tag = "bullet";
							Instantiate(impactEffectPlayer, hit.point, Quaternion.LookRotation(hit.normal));
						} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
							Instantiate(impactEffectWall, hit.point, Quaternion.LookRotation(hit.normal));
						} else {
							Instantiate(impactEffectGround, hit.point, Quaternion.LookRotation(hit.normal));
						}
					}
				}
				
			}
		} else {
			playerAnimator.Play("Rifle Reload", 4, 0);
			rifleAmmo = 30;
		}	
	}
	IEnumerator shootArrow() {
		shootingArrow = true;
		GameObject newArrow = null;
		Vector3 eulerRot = new Vector3(shootZone.eulerAngles.x, shootZone.eulerAngles.y, shootZone.eulerAngles.z);
		newArrow = Instantiate (arrow, shootZone.transform.position, Quaternion.Euler(eulerRot)) as GameObject;
		newArrow.transform.GetChild(0).gameObject.GetComponent<Rigidbody> ().AddForce (shootZone.forward * 50);
		newArrow.gameObject.tag = "arrow";
		yield return new WaitForSeconds(0.25f);
		shootingArrow = false;
		if (newArrow != null){
			yield return new WaitForSeconds (5);
			Destroy (newArrow);
		}

	}
	bool IsGrounded() {
		RaycastHit hit;
		if (baseOfCharacter) {
			return Physics.CheckSphere(baseOfCharacter.position, 0.05f, 1);//default layer is 1
		}
		return false;
	}
	public static void DumpToConsole(object obj){
		var output = JsonUtility.ToJson(obj, true);
		Debug.Log(output);
	}
}
