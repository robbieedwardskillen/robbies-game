using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
 
public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable {

	public string team;
	public ExitGames.Client.Photon.Hashtable hashTeam = new ExitGames.Client.Photon.Hashtable();
	public int playerId;
	public int playerCount;
	public int pvp = 0;
	public ExitGames.Client.Photon.Hashtable hashPvP = new ExitGames.Client.Photon.Hashtable();
	public float Height = 0.5f; // hard coding for now
	public float Health;
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
	private float lerpVal2 = 1f;
	private CinemachineFreeLook freeLookCam;
	private CinemachineVirtualCamera virtualLookCam;
	private CinemachineTargetGroup targetGroupCam;
	private GameObject currentTarget;
	private GameObject previousTarget;
	private CinemachineBrain cinemachineBrain;
	private Vector3 forward = Vector3.zero;
	private Vector3 right = Vector3.zero;
	private float transitionTime = 0f;
	private float transitionTime2 = 0f;
	private float changeWeaponTransitionTime = 0f;
	private float changeWeaponTransitionTime2 = 1f;
	private float reloadToShootTime = 0f;
	private float desiredDuration = 0.3f;
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
	private GameObject[] equippedWeps = new GameObject[2];
	public GameObject grenade;
	public GameObject arrow;
	public GameObject impactEffectGround;
	public GameObject impactEffectPlayer;
	public GameObject impactEffectWall;
	public GameObject impactEffectHole;
	private bool blocking = false;
	private bool firing = false;
	//private bool shootingArrow = false;
	private int grenades = 1;
	private int rifleAmmo = 4;
	private int maxRifleAmmo;
	public int handgunAmmo = 4;
	private int maxHandgunAmmo;
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
	private ParticleSystem.EmissionModule muzzleFlashParticleEmission;
	private ParticleSystem.EmissionModule muzzleFlashParticleEmission2;
	private Transform rightHandContainer;
	private Transform rightForearm;
	private Transform handgun;
	private Transform rifle;
	private Transform bow;
/* 	private Transform spear;
	private Transform mace;
	private Transform staff;
	private Transform knife; */
	private Transform bigSword;
	private Transform fireSword;
	private MeleeWeaponTrail mwt;
	private MeleeWeaponTrail mwt2;
	//photon making me hard code since they don't accept lists
	private	GameObject head; private GameObject upperLeftArm; private GameObject lowerLeftArm; private GameObject upperRightArm;
	private GameObject lowerRightArm; private GameObject hips; private GameObject spineGameObject; private GameObject upperLeftLeg;
	private GameObject lowerLeftLeg; private GameObject upperRightLeg; private GameObject lowerRightLeg;

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
	
	private bool connected = false;
	bool moveLegs = false;
	bool blockTurning = false;
	bool knockDown = false;
	bool changingWeps = false;

	[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
	public static GameObject LocalPlayerInstance;

	[Tooltip("The Player's UI GameObject Prefab")]
	[SerializeField]
	public GameObject PlayerUiPrefab;
    #region IPunObservable implementation


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

		if (stream.IsWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(pvp);
			stream.SendNext(team);
			stream.SendNext(Health);
			stream.SendNext(fire.gameObject.activeSelf);
			stream.SendNext(muzzleFlashParticleEmission.enabled);
			stream.SendNext(muzzleFlashParticleEmission2.enabled);
			stream.SendNext(mwt.enabled);
			stream.SendNext(mwt2.enabled);

			foreach (GameObject obj in equippedWeps){
				stream.SendNext(obj.activeSelf);
			}
			if (this.Health <= 0){
				stream.SendNext(playerAnimator.enabled);
				stream.SendNext(alive);
					
				stream.SendNext(gameObject.GetComponent<CapsuleCollider> ().isTrigger);
				stream.SendNext(gameObject.GetComponent<Rigidbody> ().isKinematic);
				
				//redundancy because of photon
				stream.SendNext(head.GetComponent<SphereCollider>().enabled);
				stream.SendNext(spineGameObject.GetComponent<BoxCollider>().enabled);
				stream.SendNext(hips.GetComponent<BoxCollider>().enabled);
				stream.SendNext(upperLeftArm.GetComponent<CapsuleCollider>().enabled);
				stream.SendNext(upperRightArm.GetComponent<CapsuleCollider>().enabled);
				stream.SendNext(upperLeftLeg.GetComponent<CapsuleCollider>().enabled);
				stream.SendNext(upperRightLeg.GetComponent<CapsuleCollider>().enabled);
				stream.SendNext(lowerLeftArm.GetComponent<CapsuleCollider>().enabled);
				stream.SendNext(lowerRightArm.GetComponent<CapsuleCollider>().enabled);
				stream.SendNext(lowerLeftLeg.GetComponent<CapsuleCollider>().enabled);
				stream.SendNext(lowerRightLeg.GetComponent<CapsuleCollider>().enabled);

				stream.SendNext(head.GetComponent<Rigidbody>().isKinematic);
				stream.SendNext(spineGameObject.GetComponent<Rigidbody>().isKinematic);
				stream.SendNext(hips.GetComponent<Rigidbody>().isKinematic);
				stream.SendNext(upperLeftArm.GetComponent<Rigidbody>().isKinematic);
				stream.SendNext(upperRightArm.GetComponent<Rigidbody>().isKinematic);
				stream.SendNext(upperLeftLeg.GetComponent<Rigidbody>().isKinematic);
				stream.SendNext(upperRightLeg.GetComponent<Rigidbody>().isKinematic);
				stream.SendNext(lowerLeftArm.GetComponent<Rigidbody>().isKinematic);
				stream.SendNext(lowerRightArm.GetComponent<Rigidbody>().isKinematic);
				stream.SendNext(lowerLeftLeg.GetComponent<Rigidbody>().isKinematic);
				stream.SendNext(lowerRightLeg.GetComponent<Rigidbody>().isKinematic);

				
			}
		}
		else
		{
			// Network player, receive data
			this.pvp = (int)stream.ReceiveNext();
			this.team = (string)stream.ReceiveNext();
			this.Health = (float)stream.ReceiveNext();
			this.fire.gameObject.SetActive((bool)stream.ReceiveNext());
			this.muzzleFlashParticleEmission.enabled = (bool)stream.ReceiveNext();
			this.muzzleFlashParticleEmission2.enabled = (bool)stream.ReceiveNext();
			this.mwt.enabled = (bool)stream.ReceiveNext();
			this.mwt2.enabled = (bool)stream.ReceiveNext();
			foreach (GameObject obj in equippedWeps){
				obj.SetActive((bool)stream.ReceiveNext());
			}
			if (this.Health <= 0){
				this.playerAnimator.enabled = (bool)stream.ReceiveNext();
				this.alive = (bool)stream.ReceiveNext();
				
				gameObject.GetComponent<CapsuleCollider> ().isTrigger = (bool)stream.ReceiveNext();
				gameObject.GetComponent<Rigidbody> ().isKinematic = (bool)stream.ReceiveNext();


				head.GetComponent<SphereCollider>().enabled = (bool)stream.ReceiveNext();
				spineGameObject.GetComponent<BoxCollider>().enabled = (bool)stream.ReceiveNext();
				hips.GetComponent<BoxCollider>().enabled = (bool)stream.ReceiveNext();
				upperLeftArm.GetComponent<CapsuleCollider>().enabled = (bool)stream.ReceiveNext();
				upperRightArm.GetComponent<CapsuleCollider>().enabled = (bool)stream.ReceiveNext();
				upperLeftLeg.GetComponent<CapsuleCollider>().enabled = (bool)stream.ReceiveNext();
				upperRightLeg.GetComponent<CapsuleCollider>().enabled = (bool)stream.ReceiveNext();
				lowerLeftArm.GetComponent<CapsuleCollider>().enabled = (bool)stream.ReceiveNext();
				lowerRightArm.GetComponent<CapsuleCollider>().enabled = (bool)stream.ReceiveNext();
				lowerLeftLeg.GetComponent<CapsuleCollider>().enabled = (bool)stream.ReceiveNext();
				lowerRightLeg.GetComponent<CapsuleCollider>().enabled = (bool)stream.ReceiveNext();

				head.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
				spineGameObject.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
				hips.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
				upperLeftArm.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
				upperRightArm.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
				upperLeftLeg.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
				upperRightLeg.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
				lowerLeftArm.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
				lowerRightArm.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
				lowerLeftLeg.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
				lowerRightLeg.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
			}
			

		}
    }
    #endregion

	#if UNITY_5_4_OR_NEWER
	void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
	{
		this.CalledOnLevelWasLoaded(scene.buildIndex);
	}
	#endif

	#if !UNITY_5_4_OR_NEWER
	void OnLevelWasLoaded(int level)
	{
		this.CalledOnLevelWasLoaded(level);
	}
	#endif

	void CalledOnLevelWasLoaded(int level)
	{
		GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
		_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
		//check position and change if in the wrong spot here
	}

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
	private void AllColliders(Transform parent, bool condition) {

		foreach (Transform t in parent) {
	 		if (t.gameObject.GetComponent<SphereCollider>() != null){
				if (t.gameObject.name == "head_28_cap_A" || t.gameObject.name == "Head" || t.gameObject.name =="rig:Head"){
					head = t.gameObject;
				}
				t.gameObject.GetComponent<SphereCollider>().enabled = condition;
				//Physics.IgnoreCollision(transform.GetComponent<CapsuleCollider>(), t.GetComponent<SphereCollider>(), !condition);
			}
			if (t.gameObject.GetComponent<CapsuleCollider> () != null) {
				if (t.gameObject.name == "Upper_Arm_L" || t.gameObject.name == "rig:LBicep"){
					upperLeftArm = t.gameObject;
				} else if (t.gameObject.name == "Upper_Arm_R" || t.gameObject.name == "rig:RBicep"){
					upperRightArm = t.gameObject;
				} else if (t.gameObject.name == "Upper_Leg_L" || t.gameObject.name == "rig:LThigh" ){
					upperLeftLeg = t.gameObject;
				} else if (t.gameObject.name == "Upper_Leg_R" || t.gameObject.name == "rig:RThigh"){
					upperRightLeg = t.gameObject;
				} else if (t.gameObject.name == "Lower_Arm_L" || t.gameObject.name == "rig:LForearm"){
					lowerLeftArm = t.gameObject;
				} else if (t.gameObject.name == "Lower_Arm_R" || t.gameObject.name == "rig:RForearm"){
					lowerRightArm = t.gameObject;
				} else if (t.gameObject.name == "Lower_Leg_L" || t.gameObject.name == "rig:LShin"){
					lowerLeftLeg = t.gameObject;
				} else if (t.gameObject.name == "Lower_Leg_R" || t.gameObject.name == "rig:RShin"){
					lowerRightLeg = t.gameObject;
				}
				t.gameObject.GetComponent<CapsuleCollider>().enabled = condition;
				//Physics.IgnoreCollision(transform.GetComponent<CapsuleCollider>(), t.GetComponent<CapsuleCollider>(), !condition);
			}
			if (t.gameObject.GetComponent<BoxCollider> () != null) {
				if (t.gameObject.name == "Spine" || t.gameObject.name == "rig:Lumbar2"){
					spineGameObject = t.gameObject;
				} else if (t.gameObject.name == "Hips" || t.gameObject.name == "rig:Lumbar1") {
					hips = t.gameObject;
				}
				t.gameObject.GetComponent<BoxCollider>().enabled = condition;
				//Physics.IgnoreCollision(transform.GetComponent<CapsuleCollider>(), t.GetComponent<BoxCollider>(), !condition);
			}
			if (t.gameObject.GetComponent<Rigidbody> () != null) {
				t.gameObject.GetComponent<Rigidbody> ().isKinematic = !condition;
			}
			AllColliders (t, condition);
		}
	}
	
	void recursiveFindingHead(Transform child){
		//I meant head objects
		foreach(Transform c in child){
			if (c.name == "Fire Area"){
				fireArea = c;
			}
			if (c.name == "Grenade Area"){
				grenadeArea = c;
			}
			if (c.childCount > 0){
				recursiveFindingHead(c);
			}
		}
	}
	void recursiveFindingHand(Transform child){
		foreach(Transform c in child){
			if (c.name == "Bip001 R Forearm" || c.name == "Lower_Arm_R" || c.name == "rig:RForearm"){
				rightForearm = c;
			}
			if (c.name == "R_hand_container" || c.name == "rig:right_weapon"){
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
			if (c.name == "Spine" || c.name == "rig:Lumbar1"){
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
		// #Important
		// used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
		if (photonView.IsMine){
			PlayerManager.LocalPlayerInstance = this.gameObject;
		}
		// #Critical
		// we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
		DontDestroyOnLoad(this.gameObject);
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

		alive = true;
		audio = gameObject.GetComponent<AudioSource> ();
		audio.volume = 0.2f;
		fire = transform.Find ("Fire");
		recursiveFindingSpine(transform);
		recursiveFindingHead(transform);
		recursiveFindingHand(transform);
		followTarget = gameObject.transform.Find("Follow Target");

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
/* 		spear = rightHandContainer.transform.Find("Spear");
		mace = rightHandContainer.transform.Find("Mace");
		staff = rightHandContainer.transform.Find("Staff");
		knife = rightHandContainer.transform.Find("Knife"); */
		bigSword = rightHandContainer.transform.Find("BigSword");
		fireSword = rightHandContainer.transform.Find("FireSword");
		shootZone = rightHandContainer.transform.Find("Shoot Zone");
			
		handgun.transform.gameObject.SetActive(false);
		rifle.transform.gameObject.SetActive(false);
		bow.transform.gameObject.SetActive(false);
/* 		spear.transform.gameObject.SetActive(false);
		mace.transform.gameObject.SetActive(false);
		staff.transform.gameObject.SetActive(false);
		knife.transform.gameObject.SetActive(false); */
		bigSword.transform.gameObject.SetActive(false);
		fireSword.transform.gameObject.SetActive(false);
		
		
		
		//selection screen should get chosen weps and put them into equippedWeps[0] & equippedWeps[1]

		equippedWeps[0] = bow.gameObject;
		equippedWeps[0].SetActive(true);
		equippedWeps[1] = rifle.gameObject;
		playerAnimator.SetBool(equippedWeps[0].name, true);
		muzzleFlash = handgun.Find("MuzzleFlashEffect");
		muzzleFlash2 = rifle.Find("MuzzleFlashEffect");
		muzzleFlashParticle = muzzleFlash.gameObject.GetComponent<ParticleSystem>();
		muzzleFlashParticleEmission = muzzleFlashParticle.emission;
		muzzleFlashParticle2 = muzzleFlash2.gameObject.GetComponent<ParticleSystem>();
		muzzleFlashParticleEmission2 = muzzleFlashParticle2.emission;
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
		cinemachineBrain = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>();
		camRotateWithZeroY = GameObject.Find("CamRotateWithZeroY");
		m_camRotation = GameObject.Find("Rotation");
		gameTransition = GameObject.Find("Main Camera").GetComponent<Transition>();


	
	}
	void setTeams() {
		if (playerCount == 1){
			team = "blue";
		}
		if (playerCount == 2){
			if (pvp == 1){
				if (playerId == 1){
					team = "blue";
				} else {
					team = "red";
				}
			}
			else {
				team = "blue";
			}
		}
		if (playerCount == 3){
			if (pvp == 1){
				if (playerId == 1){
					team = "blue";
				} else if (playerId == 2) {
					team = "red";
				} else {
					team = "green";
				}
			}
			else {
				team = "blue";
			}
		}
		if (playerCount == 4){
			if (pvp == 1){
				if (playerId == 1 || playerId == 2){
					team = "blue";
				} else {
					team = "red";
				}
			}
			else {
				team = "blue";
			}
		}
		if (playerCount == 6){
			if (pvp == 1){
				if (playerId == 1 || playerId == 2 || playerId == 3){
					team = "blue";
				} else {
					team = "red";
				}
			}
			else {
				team = "blue";
			}
		}
		if (playerCount == 10){
			if (pvp == 1){
				if (playerId == 1 || playerId == 2 || playerId == 3 || playerId == 4 || playerId == 5){
					team = "blue";
				} else {
					team = "red";
				}
			}
			else {
				team = "blue";
			}
		}
		hashTeam.Add("team", (string)team);
		foreach (Player player in PhotonNetwork.PlayerList) 
		{
			player.SetCustomProperties(hashPvP);
			player.SetCustomProperties(hashTeam);
		}
	}
	void Start() {
/* 		playerId = PhotonNetwork.LocalPlayer.ActorNumber;
		playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
		hashPvP.Add("pvp", (int)pvp); */
		//setting team
		//needs to check again after another player enters
		//setTeams();
		//end seting team

		Health = 5;
		maxHandgunAmmo = handgunAmmo;
		maxRifleAmmo = rifleAmmo;
		#if UNITY_5_4_OR_NEWER
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
		#endif
		if (PlayerUiPrefab != null)
		{
			GameObject _uiGo =  Instantiate(PlayerUiPrefab);
			_uiGo.SendMessage ("SetTarget", this, SendMessageOptions.RequireReceiver);
		}
		else
		{
			Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
		}

		AllColliders (gameObject.transform, false);

		crossHair = GameObject.Find("Canvas").transform.Find("reticle");
		GameObject.Find("Canvas").transform.Find("Control Panel").transform.gameObject.SetActive(false);

		
	}
	IEnumerator waitForSecondBigHit1(){
 		yield return new WaitForSeconds(0.25f);
		photonView.RPC("RPCTrigger2", RpcTarget.All, "bigSwing3");
		//playerAnimator.Play("bigSwing3", 0, .2f);
		yield return new WaitForSeconds(0.1f);
		if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("bigSwing3"))
		audio.PlayOneShot(soundEffectSwing1, 0.7f); 
	}
	IEnumerator waitForSecondBigHit2(){
		yield return new WaitForSeconds(0.25f);
		photonView.RPC("RPCTrigger2", RpcTarget.All, "bigSwing1");
		//playerAnimator.Play("bigSwing1", 0, .2f);
		yield return new WaitForSeconds(0.1f);
		if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("bigSwing1"))
		audio.PlayOneShot(soundEffectSwing1, 0.7f);
	}
	IEnumerator waitForSecondSmallHit1() {
		yield return new WaitForSeconds(0.05f);
		if (moveLegs) {
			playerAnimator.Play("swordLeft", 0, .4f);
			audio.PlayOneShot(soundEffectSwing2, 0.7f);
		} else {
			playerAnimator.Play("knifeUp", 0, .4f);
			audio.PlayOneShot(soundEffectSwing2, 0.7f);
		}
	}
	IEnumerator waitForSecondSmallHit2() {
		yield return new WaitForSeconds(0.05f);
		playerAnimator.Play("swordDown", 0, .4f);
		audio.PlayOneShot(soundEffectSwing3, 0.7f);
	}
	IEnumerator waitForSecondPunch() {
		yield return new WaitForSeconds(0.05f);
		if (moveLegs) {
			playerAnimator.Play("punch4", 0, .2f);
		} else {
			playerAnimator.Play("punch2", 0, .2f);
		}
		audio.PlayOneShot(soundEffectSwing2, 0.7f);
	}


 
	void FixedUpdate() {
		if (photonView.IsMine){
			velocity = (transform.position - previousPos) / Time.deltaTime;
			previousPos = transform.position;
		}
	}
 
	void Update () {

		if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
		{
			return;
		}


		if (playerAnimator.GetCurrentAnimatorStateInfo(10).normalizedTime < 1f){
			changingWeps = true;
		} else {
			if (playerAnimator.GetLayerWeight(10) > 0f){
				changeWeaponTransitionTime2 -= Time.deltaTime * 8f;
				playerAnimator.SetLayerWeight(10, Mathf.Lerp(changeWeaponTransitionTime2, 0, Time.deltaTime));
			} else {
				playerAnimator.SetLayerWeight(10, 0f);
			}

			changingWeps = false;
		}
		//check for PvP here
		//PhotonNetwork.LocalPlayer.CustomProperties["pvp"];
		if (controls.Gameplay.DPadUp.triggered){
			pvp = 1;
			hashPvP["pvp"] = pvp;
			PhotonNetwork.LocalPlayer.SetCustomProperties(hashPvP);
			
		}
		if (controls.Gameplay.DPadDown.triggered){
			pvp = 0;
			hashPvP["pvp"] = pvp;
			PhotonNetwork.LocalPlayer.SetCustomProperties(hashPvP);
		}



		if (GameManager.Instance != null){
			if (GameManager.Instance.connected && !connected){
				freeLookCam = GameObject.Find("CinemachineCam1").GetComponent<CinemachineFreeLook>();
				virtualLookCam = GameObject.Find("CinemachineCam2").GetComponent<CinemachineVirtualCamera>();
				freeLookCam.Priority = 1;
				virtualLookCam.Priority = 0;
				connected = true;

			}
		}


		if (photonView.IsMine && connected){

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
/* 						lerpVal += Time.deltaTime * 8f;
						playerAnimator.SetLayerWeight(6, Mathf.Lerp(lerpVal, 1, Time.deltaTime)); */
					//if (controls.Gameplay.Shoot.triggered){

						if(playerAnimator.GetCurrentAnimatorStateInfo(6).normalizedTime > 0.9f){ //non looping animation checking to see if animation is done
							if (shoot > 0.5f){
								if(playerAnimator.GetLayerWeight(4) <= 0.01 && playerAnimator.GetLayerWeight(6) >= 0.99f){
									playerAnimator.Play("Blend Tree Rifle Shoot", 6, 0f); 
								}
							}
						} 
						if (shoot > 0.5f){
							if(playerAnimator.GetLayerWeight(4) <= 0.3){//starts transitioning during reload
								lerpVal2 = 1f;
								lerpVal += Time.deltaTime * 5f;
								playerAnimator.SetLayerWeight(6, Mathf.Lerp(lerpVal, 1, Time.deltaTime));
							} else {
								lerpVal = 0f;
								lerpVal2 -= Time.deltaTime * 5f;
								playerAnimator.SetLayerWeight(6, Mathf.Lerp(lerpVal2, 0, Time.deltaTime));
							}
						} else {
							lerpVal = 0f;
							lerpVal2 -= Time.deltaTime * 5f;
							playerAnimator.SetLayerWeight(6, Mathf.Lerp(lerpVal2, 0, Time.deltaTime));
						}

					//}
				} else {
					lerpVal = 0f;
					lerpVal2 -= Time.deltaTime * 5f;
					playerAnimator.SetLayerWeight(6, Mathf.Lerp(lerpVal2, 0, Time.deltaTime));
				}
			}

			if ((handgun.gameObject.activeSelf == true)){


				if (aiming == true){ 

						if(playerAnimator.GetCurrentAnimatorStateInfo(7).normalizedTime > 0.9f){ //non looping animation checking to see if animation is done
							if (shoot > 0.5f){
								if(playerAnimator.GetLayerWeight(4) <= 0.01 && playerAnimator.GetLayerWeight(7) >= 0.99f){
									playerAnimator.Play("Shoot", 7, 0f); 
								}
							}
						} 
						if (shoot > 0.5f){
							
							if(playerAnimator.GetLayerWeight(4) <= 0.5){// starts transitioning during reload
								lerpVal2 = 1f;
								lerpVal += Time.deltaTime * 5f;
								playerAnimator.SetLayerWeight(7, Mathf.Lerp(lerpVal, 1, Time.deltaTime));
							} else {
								lerpVal = 0f;
								lerpVal2 -= Time.deltaTime * 5f;
								playerAnimator.SetLayerWeight(7, Mathf.Lerp(lerpVal2, 0, Time.deltaTime));
							}
						} else {
							lerpVal = 0f;
							lerpVal2 -= Time.deltaTime * 5f;
							playerAnimator.SetLayerWeight(7, Mathf.Lerp(lerpVal2, 0, Time.deltaTime));
						}

					//}
				} else {
					lerpVal = 0f;
					lerpVal2 -= Time.deltaTime * 5f;
					playerAnimator.SetLayerWeight(7, Mathf.Lerp(lerpVal2, 0, Time.deltaTime));
				}
				
			}
			if ((bow.gameObject.activeSelf == true)){

				if (aiming == true){ 

						if(playerAnimator.GetCurrentAnimatorStateInfo(8).normalizedTime > 0.9f){ //non looping animation checking to see if animation is done
							if (shoot > 0.5f){
								if(playerAnimator.GetLayerWeight(8) >= 0.99f){
									playerAnimator.Play("Shooting", 8, 0f); 
								}
							}
						} 
						if (shoot > 0.5f){
							lerpVal2 = 1f;
							lerpVal += Time.deltaTime * 5f;
							playerAnimator.SetLayerWeight(8, Mathf.Lerp(lerpVal, 1, Time.deltaTime));
						
						} else {
							lerpVal = 0f;
							lerpVal2 -= Time.deltaTime * 5f;
							playerAnimator.SetLayerWeight(8, Mathf.Lerp(lerpVal2, 0, Time.deltaTime));
						}

					//}
				} else {
					lerpVal = 0f;
					lerpVal2 -= Time.deltaTime * 5f;
					playerAnimator.SetLayerWeight(8, Mathf.Lerp(lerpVal2, 0, Time.deltaTime));
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
			(bigSword.gameObject.activeSelf == false)); // need to add rest of weapons


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
			if (transform.position.y < 0f){
				Health = 0;
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
				bigSword.transform.gameObject.GetComponent<Collider>().isTrigger = true;
				bigSword.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
				bigSwing = true;
			} else {
				if (!dynamicHitting){
					
					bigSword.transform.gameObject.GetComponent<Collider>().isTrigger = false;
					bigSword.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
					mwt2.enabled = false;
					bigSwing = false;
				}
			}

			//----begin player movement and rotation

			if (alive) {
				//squatting animation
				blocking = playerAnimator.GetBool("blocking");
				if (Health < 3) {
					playerAnimator.SetBool("injured", true);
				}
				if (controls.Gameplay.Action2.triggered && !busy && !changingWeps && shoot < 0.5f) {
					playerAnimator.SetTrigger("changeWep");
					changeWeapon();
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
/* 							if (controls.Gameplay.Action1.triggered && !bigSwing){
								StartCoroutine(waitForSound(soundEffectSwing1, "bigSwing4"));
								secondHit = false;
							} */
							if (controls.Gameplay.Action1.triggered && bigSwing && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && !secondHit){
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
							photonView.RPC("RPCTrigger2", RpcTarget.All, "bigSwing5");
							//playerAnimator.Play("bigSwing5", 0, .2f);
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
					
					playerAnimator.SetBool("firing", shoot > 0.5f);

					if (equippedHandgun){
						playerAnimator.SetLayerWeight(1, 0f);//arrowswhilewalking layer
						print(playerAnimator.GetCurrentAnimatorStateInfo(4).normalizedTime);
						if (playerAnimator.GetCurrentAnimatorStateInfo(4).normalizedTime < 1f){
							playerAnimator.SetBool("reloading", true);
						} else {
							playerAnimator.SetBool("reloading", false);

						}
						if (shoot >= 0.5f && !aiming) {
							if (playerAnimator.GetLayerWeight(4) < 0.01f){// not reloading
								if (handgunAmmo > 0 && !playerAnimator.GetBool("blocking") && !playerAnimator.GetBool("reloading")){
									playerAnimator.SetBool("ShootingHandgun", true);
								}
							}
							else {//reloading
								playerAnimator.SetBool("ShootingHandgun", false);
								
							}
						} 
						if (shoot < 0.5f) {
							playerAnimator.SetBool("ShootingHandgun", false);
							if (handgunAmmo < maxHandgunAmmo && !playerAnimator.GetBool("reloading") &&
							!playerAnimator.GetBool("blocking")){
								//manual reload here
							}
						}
						
					}
					if (equippedRifle){
						playerAnimator.SetLayerWeight(1, 0f);
						print(playerAnimator.GetCurrentAnimatorStateInfo(4).normalizedTime);
						if (playerAnimator.GetCurrentAnimatorStateInfo(4).normalizedTime < 1f){
							playerAnimator.SetBool("reloading", true);
						} else {
							playerAnimator.SetBool("reloading", false);
						}
						if (shoot >= 0.5f && !aiming) {
							if (playerAnimator.GetLayerWeight(4) < 0.01f){
								if (rifleAmmo > 0 && !playerAnimator.GetBool("blocking") && !playerAnimator.GetBool("reloading")) {
									playerAnimator.SetBool("ShootingRifle", true);
								}
							}
							else {
								playerAnimator.SetBool("ShootingRifle", false);
							}
						} 
						if (shoot < 0.5f) {
							playerAnimator.SetBool("ShootingRifle", false);
							if (rifleAmmo < maxRifleAmmo && !playerAnimator.GetBool("reloading") &&
							!playerAnimator.GetBool("blocking")){
								//manual reload here
							}
						}
					} 
					if (equippedBow && !blocking) {
						if (shoot >= 0.5f && !aiming) {
							//bow has infinite ammo
							playerAnimator.SetBool("ShootingBow", true);
							transitionTime2 = 0f;
							transitionTime += Time.deltaTime * 8; // so it doesn't teleport arms up
							playerAnimator.SetLayerWeight(1, Mathf.Lerp(0f, 1f, transitionTime));
							//float bowShootTime = playerAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime % 1;

						} else {
							playerAnimator.SetBool("ShootingBow", false);
							transitionTime = 0f;
							//playerAnimator.SetLayerWeight(1, 0f);
							transitionTime2 += Time.deltaTime * 8; // so it doesn't teleport arms down
							playerAnimator.SetLayerWeight(1, Mathf.Lerp(1f, 0f, transitionTime2));
						}
						
						//if holding fire button
					}
					else if (!equippedBow) {
						playerAnimator.SetBool("ShootingBow", false);
						playerAnimator.SetLayerWeight(1, 0f);
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
				if (Health <= 0) {
					playerAnimator.enabled = false;
					fire.gameObject.SetActive (false);
					gameObject.GetComponent<CapsuleCollider> ().isTrigger = true;
					gameObject.GetComponent<Rigidbody> ().isKinematic = true;
					AllColliders (gameObject.transform, true);
					alive = false;
				}
				//----end player death


			

				
				if(virtualLookCam.Priority > 0){
					m_camRotation.transform.position = virtualLookCam.transform.position;
					m_camRotation.transform.rotation = virtualLookCam.transform.rotation;
				}
				
				if (aiming){
					if(bow.gameObject.activeSelf){
						if(crossHair != null)
						crossHair.gameObject.SetActive(false);
					} else {
						if (crossHair != null){
							crossHair.gameObject.SetActive(true);
						} else {
							crossHair = GameObject.Find("Canvas").transform.Find("reticle");
						}
						
					}
					playerAnimator.SetFloat("rotateY", Mathf.Sin(-m_camRotation.transform.eulerAngles.x * (Mathf.PI / 180) + 0f));//radians to degrees offsetting 0-x% because animations are off
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
					if (crossHair != null){
						crossHair.gameObject.SetActive(false);
					} else {
						crossHair = GameObject.Find("Canvas").transform.Find("reticle");
					}
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
				playerAnimator.GetBool("ShootingBow") || dynamicHitting) || playerAnimator.GetBool("firing") && !playerAnimator.GetBool("blocking")){

					if (playerAnimator.GetLayerWeight(4) < 0.01f){//not reloading
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
					var relativeAngle = -Mathf.DeltaAngle(angleA, angleB);//delta angle calculates shortest distance of angle ex. 340 degrees is 20 degrees -delta because.. idk it works
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
				else if (!aiming) {//not shooting and not aiming
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
	}

	public void makeTransparent() {
		//change shader from Mobile/Bumped Diffuse to Unlit/Texture
	}
	public void makeNonTransparent() {
		//change shader from Unlit/Texture to Mobile/Bumped Diffuse
	}
	[PunRPC]
	public void RPCTrigger(string anim) {
		playerAnimator.SetTrigger(anim);
		//playerAnimator.Play(anim);
	}
	[PunRPC]
	void RPCTrigger2 (string anim) { // for big swing
		playerAnimator.Play(anim, 0, 0.2f);
	}
	[PunRPC]
	void RPCTrigger3 (string anim) {
		playerAnimator.Play(anim, 4, 0); //handgun animation
	}
	[PunRPC]
	void Flash (){
        muzzleFlashParticle.Play();
		muzzleFlashParticle2.Play();
    }

	[PunRPC]
	void ShootArrow() {
		if (photonView.IsMine){
			GameObject newArrow = null;
			Vector3 eulerRot = new Vector3(shootZone.eulerAngles.x, shootZone.eulerAngles.y, shootZone.eulerAngles.z);
			newArrow = PhotonNetwork.Instantiate (arrow.name, shootZone.transform.position, Quaternion.Euler(eulerRot)) as GameObject;
			//newArrow = Instantiate (arrow, shootZone.transform.position, Quaternion.Euler(eulerRot)) as GameObject;
			newArrow.transform.GetChild(0).gameObject.GetComponent<Rigidbody> ().AddForce (shootZone.forward * 75);
			newArrow.gameObject.tag = "arrow";
		}

	}

	[PunRPC]
	void ShootHandgun() {
		if(handgunAmmo > 0){ //does an extra shot because I have to start at the end of an animation
				handgunAmmo -= 1;
				RaycastHit hit;
				if (Physics.Raycast(handgun.position, shootZone.forward, out hit, range)){
					if (hit.rigidbody != null) {
						hit.rigidbody.AddForce(hit.normal * -15f);
					}
					if (hit.transform.tag == "Player" || hit.transform.name == "cop"){
						impactEffectPlayer.tag = "bullet";
						PhotonNetwork.Instantiate(impactEffectPlayer.name, hit.point, Quaternion.LookRotation(hit.normal));
					} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
						PhotonNetwork.Instantiate(impactEffectHole.name, hit.point, Quaternion.LookRotation(hit.normal));
						PhotonNetwork.Instantiate(impactEffectWall.name, hit.point, Quaternion.LookRotation(hit.normal));
					} else {
						PhotonNetwork.Instantiate(impactEffectGround.name, hit.point, Quaternion.LookRotation(hit.normal));
					}
				}
				//still runs the rest of the if statement even though it is false, learned something new
				if (handgunAmmo <= 0){
					if (playerAnimator.GetBool("reloading") == false){
						StartCoroutine(ReloadHandgunContinueShooting());
					}
				}
		} 
	}

	[PunRPC]
	void ShootRifle() {

		if (rifleAmmo > 0){
				rifleAmmo -= 1;
				//playerAnimator.SetTrigger("shoot");
				RaycastHit hit;
				if (IsGrounded()){//since gun is 90 degrees the wrong way
					if (Physics.Raycast(rifle.position, fireArea.forward, out hit, range)){
						if (hit.transform.tag == "Player" || hit.transform.name == "cop"){
							impactEffectPlayer.tag = "bullet";
							PhotonNetwork.Instantiate(impactEffectPlayer.name, hit.point, Quaternion.LookRotation(hit.normal));
						} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
							PhotonNetwork.Instantiate(impactEffectHole.name, hit.point, Quaternion.LookRotation(hit.normal));
							PhotonNetwork.Instantiate(impactEffectWall.name, hit.point, Quaternion.LookRotation(hit.normal));
						} else {
							PhotonNetwork.Instantiate(impactEffectGround.name, hit.point, Quaternion.LookRotation(hit.normal));
						}
					}
				} else {
					if (Physics.Raycast(rifle.position, -rifle.up, out hit, range)){
						if (hit.transform.tag == "Player" || hit.transform.name == "cop"){
							impactEffectPlayer.tag = "bullet";
							PhotonNetwork.Instantiate(impactEffectPlayer.name, hit.point, Quaternion.LookRotation(hit.normal));
						} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
							PhotonNetwork.Instantiate(impactEffectHole.name, hit.point, Quaternion.LookRotation(hit.normal));
							PhotonNetwork.Instantiate(impactEffectWall.name, hit.point, Quaternion.LookRotation(hit.normal));
						} else {
							PhotonNetwork.Instantiate(impactEffectGround.name, hit.point, Quaternion.LookRotation(hit.normal));
						}
					}
				}
			if (rifleAmmo <= 0){
				if (playerAnimator.GetBool("reloading") == false){
					StartCoroutine(ReloadRifleContinueShooting());
				}
			}

		}
	}

	//for handgun (bad naming convention but I don't care)
	void callFlash(AnimationEvent myEvent) {// called from the scene's animator
		if (photonView.IsMine){
			if (myEvent.intParameter == 3){ //only call from center shot to avoid multi calls when anim tree combines animations
				if(playerAnimator.GetLayerWeight(4) < 1f){
					if (handgunAmmo > 0){
						photonView.RPC("Flash", RpcTarget.All);
						photonView.RPC("ShootHandgun", RpcTarget.All);
					}
				}
			}
		}
	}
	//rifle
	void callFlash2(AnimationEvent myEvent) {
		if (photonView.IsMine){
			if (myEvent.intParameter == 3){ 
				if(playerAnimator.GetLayerWeight(4) < 1f){
					if (rifleAmmo > 0){
						photonView.RPC("Flash", RpcTarget.All);
						photonView.RPC("ShootRifle", RpcTarget.All);
					}
				}
			}
		}
	}
	void callArrow(AnimationEvent myEvent) {
		if (photonView.IsMine){
			if (myEvent.intParameter == 3){
				if(playerAnimator.GetLayerWeight(1) >= 0.9f || playerAnimator.GetLayerWeight(8) >= 0.9f){
					photonView.RPC("ShootArrow", RpcTarget.All);
				}
			}
		}

	}

	IEnumerator waitForSound (AudioClip ac, string anim){ // for big swing
		photonView.RPC("RPCTrigger2", RpcTarget.All, anim);
		//playerAnimator.Play(anim, 0, .2f);
		yield return new WaitForSeconds(0.35f);
		if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim)){
			audio.PlayOneShot(ac, 0.7f);
		}
	}
	IEnumerator ReloadHandgun() {//not used
		yield return new WaitForSeconds(0.1f);
		playerAnimator.SetLayerWeight(4, 1f);
		photonView.RPC("RPCTrigger3", RpcTarget.All, "Handgun Reload");
		//playerAnimator.SetBool("reloading", true);
		yield return new WaitForSeconds(1f);
		playerAnimator.SetLayerWeight(4, 0f);
		//playerAnimator.SetBool("reloading", false);
		handgunAmmo = 4;
	}

	IEnumerator ReloadHandgunContinueShooting() {
		
		yield return new WaitForSeconds(0.1f);//so hand doesn't go down idk why 0.1 seconds is enough time

		photonView.RPC("RPCTrigger3", RpcTarget.All, "Handgun Reload");

		while (reloadToShootTime < desiredDuration){
			reloadToShootTime += Time.deltaTime;
			float percentageComplete = reloadToShootTime / desiredDuration;
			playerAnimator.SetLayerWeight(4, Mathf.Lerp(0f, 1f, percentageComplete));
			yield return null;
		}

		reloadToShootTime = 0f;

		yield return new WaitForSeconds(0.5f);

		while (reloadToShootTime < desiredDuration){
			reloadToShootTime += Time.deltaTime;
			float percentageComplete = reloadToShootTime / desiredDuration;
			playerAnimator.SetLayerWeight(4, Mathf.Lerp(1f, 0f, percentageComplete));
			yield return null;
		}

		reloadToShootTime = 0f;

		handgunAmmo = 4;
	}

	IEnumerator ReloadRifleContinueShooting() {
		
		yield return new WaitForSeconds(0.1f);//to let other code run first maybe? Idk need this for some reason.

		photonView.RPC("RPCTrigger3", RpcTarget.All, "Rifle Reload");

		while (reloadToShootTime < desiredDuration){
			reloadToShootTime += Time.deltaTime;
			float percentageComplete = reloadToShootTime / desiredDuration;
			playerAnimator.SetLayerWeight(4, Mathf.Lerp(0f, 1f, percentageComplete)); //lerping to reload
			yield return null;
		}

		reloadToShootTime = 0f;
		
		yield return new WaitForSeconds(0.5f);


		while (reloadToShootTime < desiredDuration){
			reloadToShootTime += Time.deltaTime;
			float percentageComplete = reloadToShootTime / desiredDuration;
			playerAnimator.SetLayerWeight(4, Mathf.Lerp(1f, 0f, percentageComplete)); //lerping off reload
			yield return null;
		}
		//yield return new WaitForSeconds(0.5f);
		reloadToShootTime = 0f;

		rifleAmmo = 4;
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
		newGrenade = PhotonNetwork.Instantiate (grenade.name, grenadeArea.position, grenadeArea.rotation) as GameObject;
		newGrenade.GetComponent<Rigidbody> ().AddForce (grenadeArea.forward * 200);
		newGrenade.GetComponent<Rigidbody> ().AddForce (transform.up * 35);
		yield return new WaitForSeconds(2);
		newGrenade.transform.GetChild (0).gameObject.SetActive(true);
		newGrenade.gameObject.tag = "Explosion";
		newGrenade.transform.GetComponent<SphereCollider> ().isTrigger = true;
		newGrenade.transform.GetComponent<SphereCollider> ().radius = 12;

		newGrenade.gameObject.GetComponent<Renderer> ().enabled = false;
		yield return new WaitForSeconds (2);
		grenades = 1;
		if (photonView.IsMine)
		PhotonNetwork.Destroy (newGrenade);
	}
	IEnumerator shootFireball() {
		yield return new WaitForSeconds(0.2f);
		flames.enableEmission = true;//obsolete use emission.enabled instead of particlesystem.enableemission
		embers.enableEmission = true;
		yield return new WaitForSeconds(0.3f);
		flames.enableEmission = false;
		embers.enableEmission = false;
	}
	void changeWeapon() {
		changingWeps = true;
		while(changeWeaponTransitionTime < 1f){
			changeWeaponTransitionTime += Time.deltaTime * 2; 
			playerAnimator.SetLayerWeight(10, Mathf.Lerp(0f, 1f, changeWeaponTransitionTime));
		}
		
		playerAnimator.Play("changeWep", 10, 0);
		if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shoot") &&
		!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blend Tree Rifle")){

			if (equippedWeps[0].activeSelf && !equippedWeps[1].activeSelf){
				equippedWeps[0].SetActive(false);
				equippedWeps[1].SetActive(true);
				playerAnimator.SetBool(equippedWeps[1].name, true);
				playerAnimator.SetBool(equippedWeps[0].name, false);
				if (equippedWeps[1] == fireSword){
					fireSword.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
				}
				if (equippedWeps[1] == bigSword){
					bigSword.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
				}
			} 
			else if (equippedWeps[1].activeSelf && !equippedWeps[0].activeSelf){ 
				equippedWeps[1].SetActive(false);
				equippedWeps[0].SetActive(true);
				playerAnimator.SetBool(equippedWeps[1].name, false);
				playerAnimator.SetBool(equippedWeps[0].name, true);
				if (equippedWeps[0] == fireSword){
					fireSword.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
				}
				if (equippedWeps[0] == bigSword){
					bigSword.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
				}
			}
			
		}
		changeWeaponTransitionTime = 0f;
		changeWeaponTransitionTime2 = 1f;
		changingWeps = false;
	}
	void Land(){
		//needed for land animation
	}
	void OnTriggerEnter (Collider col) {
		if (!photonView.IsMine){return;}
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
	void OnTriggerStay(Collider other) {
		if (!photonView.IsMine){return;}
	}
	void OnParticleCollision(GameObject other){
		if (other.name == "Flames") {
			StartCoroutine(takeMediumWithFall());
		}
	}
 	void OnCollisionEnter (Collision col){
		if (!photonView.IsMine){return;}
			if (col.gameObject.tag == "Explosion") {
				StartCoroutine (takeBigDamage ());
			}
			if (col.gameObject.tag == "arrow"){
				if (!gameTransition.transitioning){
					Health -= 3f;
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
			Health -= 4;
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
			Health -= 3;
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
			Health -= 3f;
			yield return new WaitForSeconds (1);
			takingDamage = false;
		}
	}
	IEnumerator takeDamage() {
		
		if (!gameTransition.transitioning){
			takingDamage = true;
			Health -= 2f;
			yield return new WaitForSeconds (1);
			takingDamage = false;
		}
	}
	void takeDamageNoWait(string bulletType) {
		if (!gameTransition.transitioning){
			if (bulletType == "bullet")
			Health -= 1f;
			if (bulletType == "bigBullet")
			Health -= 3f;
			if (bulletType == "arrow"){
				
				Health -= 3f;
			}
		}
	}
	IEnumerator blinking() {
		
		for (int i = 0; i < 5; i++){
			head.SetActive(false);
			gameObject.transform.Find("body").gameObject.SetActive(false);
			yield return new WaitForSeconds (0.1f);
			head.SetActive(true);
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
						PhotonNetwork.Instantiate(impactEffectPlayer.name, hit.point, Quaternion.LookRotation(hit.normal));
					} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
						PhotonNetwork.Instantiate(impactEffectHole.name, hit.point, Quaternion.LookRotation(hit.normal));
						PhotonNetwork.Instantiate(impactEffectWall.name, hit.point, Quaternion.LookRotation(hit.normal));
					} else {
						PhotonNetwork.Instantiate(impactEffectGround.name, hit.point, Quaternion.LookRotation(hit.normal));
					}
				}
			} else {
				if (Physics.Raycast(handgun.position, handgun.forward, out hit, range)){
					if (hit.rigidbody != null) {
						hit.rigidbody.AddForce(hit.normal * -15f);
					}
					if (hit.transform.tag == "Player" || hit.transform.name == "cop"){
						impactEffectPlayer.tag = "bullet";
						PhotonNetwork.Instantiate(impactEffectPlayer.name, hit.point, Quaternion.LookRotation(hit.normal));
					} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
						PhotonNetwork.Instantiate(impactEffectHole.name, hit.point, Quaternion.LookRotation(hit.normal));
						PhotonNetwork.Instantiate(impactEffectWall.name, hit.point, Quaternion.LookRotation(hit.normal));
					} else {
						PhotonNetwork.Instantiate(impactEffectGround.name, hit.point, Quaternion.LookRotation(hit.normal));
					}
				}
			}
		} else {
			playerAnimator.Play("Handgun Reload", 4, 0);
			handgunAmmo = 4;
		}
	}
	void firingRifle() { //for zoomed in
		if(rifleAmmo > 0){
			rifleAmmo -= 1;
			photonView.RPC("Flash", RpcTarget.All);
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
						PhotonNetwork.Instantiate(impactEffectPlayer.name, hit.point, Quaternion.LookRotation(hit.normal));
					} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
						PhotonNetwork.Instantiate(impactEffectHole.name, hit.point, Quaternion.LookRotation(hit.normal));
						PhotonNetwork.Instantiate(impactEffectWall.name, hit.point, Quaternion.LookRotation(hit.normal));
					} else {
						PhotonNetwork.Instantiate(impactEffectGround.name, hit.point, Quaternion.LookRotation(hit.normal));
					}
				}
			} 
		} else {
			playerAnimator.Play("Rifle Reload", 4, 0);
			rifleAmmo = 4;
		}
	}


	IEnumerator shootingRifle() {
		yield return new WaitForSeconds (0.15f);
/* 		Vector3 eulerRot = new Vector3(rifle.eulerAngles.x, rifle.eulerAngles.y, rifle.eulerAngles.z);
		shootZone.eulerAngles = eulerRot; */

		if (rifleAmmo > 0){
			if (!muzzleFlashParticle2.isPlaying) {
				rifleAmmo -= 1;
				playerAnimator.SetTrigger("shoot");
				RaycastHit hit;
				if (IsGrounded()){//since gun is 90 degrees the wrong way
					if (Physics.Raycast(rifle.position, fireArea.forward, out hit, range)){


						if (hit.transform.tag == "Player" || hit.transform.name == "cop"){
							impactEffectPlayer.tag = "bullet";
							PhotonNetwork.Instantiate(impactEffectPlayer.name, hit.point, Quaternion.LookRotation(hit.normal));
						} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
							PhotonNetwork.Instantiate(impactEffectHole.name, hit.point, Quaternion.LookRotation(hit.normal));
							PhotonNetwork.Instantiate(impactEffectWall.name, hit.point, Quaternion.LookRotation(hit.normal));
						} else {
							PhotonNetwork.Instantiate(impactEffectGround.name, hit.point, Quaternion.LookRotation(hit.normal));
						}
					}
				} else {
					if (Physics.Raycast(rifle.position, -rifle.up, out hit, range)){
						if (hit.transform.tag == "Player" || hit.transform.name == "cop"){
							impactEffectPlayer.tag = "bullet";
							PhotonNetwork.Instantiate(impactEffectPlayer.name, hit.point, Quaternion.LookRotation(hit.normal));
						} else if (hit.transform.tag == "shelter" || hit.transform.tag == "lamp"){
							PhotonNetwork.Instantiate(impactEffectHole.name, hit.point, Quaternion.LookRotation(hit.normal));
							PhotonNetwork.Instantiate(impactEffectWall.name, hit.point, Quaternion.LookRotation(hit.normal));
						} else {
							PhotonNetwork.Instantiate(impactEffectGround.name, hit.point, Quaternion.LookRotation(hit.normal));
						}
					}
				}
				photonView.RPC("Flash", RpcTarget.All);
			}
		} else {
			playerAnimator.Play("Rifle Reload", 4, 0);
			rifleAmmo = 4;
		}	
	}

	bool IsGrounded() {
		//RaycastHit hit;
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
