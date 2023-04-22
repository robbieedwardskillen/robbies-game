using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Cinemachine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using com.zibra.liquid.Manipulators;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable {

	public string team;
	public ExitGames.Client.Photon.Hashtable hashTeam = new ExitGames.Client.Photon.Hashtable();
	public int playerCount;
	public int instantiationId;
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
	private float rollSpeed = 1f;
	private bool secondHit = false;
    private float range = 200f;
	public float moveSpeed = 1f;
	public float animationMoveSpeed = 0f;
	public bool underWater = false;
	public bool aiming = false;
	private float lerpVal = 0f;
	private float lerpVal2 = 0f;
	private Vector3 rayCollision = Vector3.zero;
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
	private float aim; private float sprint; private float shoot; private float blockDodge;
	private float dpadUp; private float dpadDown; private float dpadLeft; private float dpadRight;
	private float menu;
	private Vector3 movement;
	private Transform spine;
	private Transform rightFoot;
	private Collider[] rightFootHitColliders;
	private Collider[] leftFootHitColliders;
	private Transform leftFoot;
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
	public int handgunAmmo = 10;
	private int maxHandgunAmmo;
	private float attackSpeed = 1f;
	private Transform grenadeArea;
	private Transform fireArea;
	private Transform followTarget;
	private Transform crossHair;
	private Transform shootZone;
	private Transform swordBlade;
	private Transform ignitionFlame;
	private Transform muzzleFlash;
	private Transform muzzleFlash2;
	private ParticleSystem muzzleFlashParticle;
	private ParticleSystem muzzleFlashParticle2;
	private ParticleSystem.EmissionModule muzzleFlashParticleEmission;
	private ParticleSystem.EmissionModule muzzleFlashParticleEmission2;
	private ParticleSystem embers;
	private ParticleSystem.EmissionModule embersParticleEmission;
	private ParticleSystem flames;
	private ParticleSystem.EmissionModule flamesParticleEmission;
	private Transform rightHandContainer;
	private Transform rightForearm;
	private Transform rightHand;
	private Transform handgun;
	private Transform rifle;
	private Transform bow;
/* 	private Transform spear;
	private Transform mace;
	private Transform staff;
	private Transform knife; */
	private Transform bigSword;
	private Transform fireSword;


	private bool canCast = true;
	private ZibraLiquidForceField waterBallForceField;
	private ZibraLiquidEmitter waterBall;
	private ZibraLiquidEmitter attackWave;
	private ZibraLiquidEmitter healWave;

	public bool castingHealingWater = false;
	private bool eraserTimerOn = false;
	private float eraserTimer = 0f;
	private bool waterEmitting = false;
	private GameObject waterEraser;
	private bool foundTheLiquids = false;
	private Rigidbody rigidBody;
	private Vector3 networkPosition;
	private Quaternion networkRotation;

	private MeleeWeaponTrail mwt;
	private MeleeWeaponTrail mwt2;
	//photon making me hard code since they don't accept lists
	private	GameObject head; private GameObject upperLeftArm; private GameObject lowerLeftArm; private GameObject upperRightArm;
	private GameObject lowerRightArm; private GameObject hips; private GameObject spineGameObject; private GameObject upperLeftLeg;
	private GameObject lowerLeftLeg; private GameObject upperRightLeg; private GameObject lowerRightLeg; private GameObject crosshairLookAt;
	private GameObject tailA; private GameObject tailL; private GameObject tailR; private GameObject pointLight;


	private AudioSource audio;
	public AudioClip soundEffectSwing1;
	public AudioClip soundEffectSwing2;
	public AudioClip soundEffectSwing3;
	public AudioClip soundEffectSwing4;
	public AudioClip soundEffectFoot1;
	public AudioClip soundEffectFoot2;
	public AudioClip soundEffectFoot3;
	public AudioClip soundEffectFoot4;
	public AudioClip soundEffectHandgunShot1;
	public AudioClip soundEffectHandgunShot2;
	public AudioClip soundEffectHandgunShot3;
	public AudioClip soundEffectHandgunShot4;
	public AudioClip soundEffectHandgunReload;
	public AudioClip soundEffectArrow1;
	public AudioClip soundEffectArrow2;
	public AudioClip soundEffectJump;
	public AudioClip soundEffectRoll;
	public AudioClip healWaveSound;
	public AudioClip attackWaveSound;
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

	private ManageCanvas canvasManager;

	private bool slowMotion = false;

	private bool connected = false;
	bool moveLegs = false;
	bool blockTurning = false;
	bool knockDown = false;
	bool changingWeps = false;
	private int testing1 = 0;
		private int testing2 = 0;

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
			stream.SendNext(rigidBody.position);
			stream.SendNext(rigidBody.rotation);
			stream.SendNext(rigidBody.velocity);
			// We own this player: send the others our data
			stream.SendNext(pvp);
			stream.SendNext(team);
			stream.SendNext(Health);
/* 			if(this.pointLight != null)
			{
				if (this.pointLight.GetComponent<Light>() != null)
					stream.SendNext((bool)pointLight.GetComponent<Light>().enabled);
			} */
			
			//stream.SendNext(fire.gameObject.activeSelf);
			if (flames != null){
				stream.SendNext(flamesParticleEmission.enabled);
			}
			if (embers != null){
				stream.SendNext(embersParticleEmission.enabled);
			}
			if (muzzleFlashParticle != null)
				stream.SendNext(muzzleFlashParticleEmission.enabled);
			if (muzzleFlashParticle2 != null)
				stream.SendNext(muzzleFlashParticleEmission2.enabled);
			if (mwt != null)
				stream.SendNext(mwt.enabled);
			if (mwt2 != null)
				stream.SendNext(mwt2.enabled); 

 			if (equippedWeps.Length > 0){
				foreach (GameObject obj in equippedWeps){
					stream.SendNext(obj.activeSelf);
				}
			}
			stream.SendNext(bigSword.GetComponent<BoxCollider>().enabled);
			stream.SendNext(bigSword.GetComponent<BoxCollider>().isTrigger);

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


				stream.SendNext(tailA.GetComponent<Rigidbody>().isKinematic);
				stream.SendNext(tailL.GetComponent<Rigidbody>().isKinematic);
				stream.SendNext(tailR.GetComponent<Rigidbody>().isKinematic);
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
			networkPosition = (Vector3) stream.ReceiveNext();
			networkRotation = (Quaternion) stream.ReceiveNext();
			rigidBody.velocity = (Vector3) stream.ReceiveNext();

			float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp));
			networkPosition += (this.rigidBody.velocity * lag);


			// Network player, receive data
			this.pvp = (int)stream.ReceiveNext();
			this.team = (string)stream.ReceiveNext();
			this.Health = (float)stream.ReceiveNext();
/* 			if(this.pointLight != null)
			{
				if (this.pointLight.GetComponent<Light>() != null)
					this.pointLight.GetComponent<Light>().enabled = (bool) stream.ReceiveNext();
			} */

			//this.fire.gameObject.SetActive((bool)stream.ReceiveNext());
			if (this.flames != null){
				this.flamesParticleEmission.enabled = (bool)stream.ReceiveNext();
			}
			if (this.embers != null){
				this.embersParticleEmission.enabled = (bool)stream.ReceiveNext();
			}
			if (this.muzzleFlashParticle != null)
				this.muzzleFlashParticleEmission.enabled = (bool)stream.ReceiveNext();
			if (this.muzzleFlashParticle2 != null)
				this.muzzleFlashParticleEmission2.enabled = (bool)stream.ReceiveNext();
			if (this.mwt != null)
				this.mwt.enabled = (bool)stream.ReceiveNext();
			if (this.mwt2 != null)
				this.mwt2.enabled = (bool)stream.ReceiveNext();
			if (equippedWeps.Length > 0){
				foreach (GameObject obj in equippedWeps){
					obj.SetActive((bool)stream.ReceiveNext());
				}
			}
			bigSword.GetComponent<BoxCollider>().enabled = (bool)stream.ReceiveNext();
			bigSword.GetComponent<BoxCollider>().isTrigger = (bool)stream.ReceiveNext();
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

				tailA.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
				tailL.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
				tailR.GetComponent<Rigidbody>().isKinematic = (bool)stream.ReceiveNext();
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
		//this.CalledOnLevelWasLoaded(scene.buildIndex);
	}
	#endif

/* 	#if !UNITY_5_4_OR_NEWER
	void OnLevelWasLoaded(int level)
	{
		if (photonView.IsMine){
			this.CalledOnLevelWasLoaded(level);
		}
	}
	#endif

	void CalledOnLevelWasLoaded(int level)
	{
		GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
		_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
		//check position and change if in the wrong spot here
	} */

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

			if (t.gameObject.name == "rig:tail_A"){
				tailA = t.gameObject;
				t.gameObject.GetComponent<Rigidbody> ().isKinematic = !condition;
			}

			if (t.gameObject.name == "rig:tail_L"){
				tailL = t.gameObject;
				t.gameObject.GetComponent<Rigidbody> ().isKinematic = !condition;
			}

			if (t.gameObject.name == "rig:tail_R"){
				tailR = t.gameObject;
				t.gameObject.GetComponent<Rigidbody> ().isKinematic = !condition;
			}
			if (t.gameObject.name == "Point Light"){
				pointLight = t.gameObject;
				t.gameObject.GetComponent<Light>().enabled = false;
			}


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
			if (c.name == "rig:RHand") {
				rightHand = c;
			}
			if (c.name == "R_hand_container" || c.name == "rig:right_weapon"){
				rightHandContainer = c;
			}
			if (c.name == "crosshairLookAt") {
				crosshairLookAt = c.gameObject;
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
	void recursiveFindingRightFoot(Transform child) {
		foreach(Transform c in child){
			if (c.name == "rig:RFootBone1"){
				rightFoot = c;
				return;
			} 
			if (c.childCount > 0){
				recursiveFindingRightFoot(c);
			}
		}
	}
	void recursiveFindingLeftFoot(Transform child) {
		foreach(Transform c in child){
			if (c.name == "rig:LFootBone1"){
				leftFoot = c;
				return;
			} 
			if (c.childCount > 0){
				recursiveFindingLeftFoot(c);
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

		playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
		instantiationId = int.Parse(photonView.InstantiationId.ToString().Substring(0, 1));
/* 		healWave = GameObject.Find("HealWave" + instantiationId).GetComponent<ZibraLiquidEmitter>();
		attackWave = GameObject.Find("AttackWave" + instantiationId).GetComponent<ZibraLiquidEmitter>();
		waterBall = GameObject.Find("WaterBall" + instantiationId).GetComponent<ZibraLiquidEmitter>();
		waterBallForceField = GameObject.Find("WaterBallForceField" + instantiationId).GetComponent<ZibraLiquidForceField>();
		waterEraser = GameObject.Find("Void");
		StartCoroutine(eraseWater()); */
		
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
		playerAnimator = gameObject.GetComponent<Animator> ();
		playerAnimator.SetFloat("speed", 1.75f);

		previousPos = transform.position;

		alive = true;
		audio = gameObject.GetComponent<AudioSource> ();
		audio.volume = 1f;

		rigidBody = GetComponent<Rigidbody>();
		//fire = transform.Find ("Fire");
		recursiveFindingSpine(transform);
		recursiveFindingHead(transform);
		recursiveFindingHand(transform);
		recursiveFindingLeftFoot(transform);
		recursiveFindingRightFoot(transform);
		followTarget = gameObject.transform.Find("Follow Target");
		
		
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

		equippedWeps[0] = bigSword.gameObject;
		equippedWeps[0].SetActive(true);
		equippedWeps[1] = handgun.gameObject;
		playerAnimator.SetBool(equippedWeps[0].name, true);
		flames = transform.Find("Fire").GetComponent<ParticleSystem>();
		flamesParticleEmission = flames.emission;
		embers = transform.Find("Fire").transform.Find("FireEmbers").GetComponent<ParticleSystem>();
		embersParticleEmission = embers.emission;
		muzzleFlash = handgun.Find("MuzzleFlashEffect");
		muzzleFlash2 = rifle.Find("MuzzleFlashEffect");
		muzzleFlashParticle = muzzleFlash.gameObject.GetComponent<ParticleSystem>();
		muzzleFlashParticleEmission = muzzleFlashParticle.emission;
		muzzleFlashParticle2 = muzzleFlash2.gameObject.GetComponent<ParticleSystem>();
		muzzleFlashParticleEmission2 = muzzleFlashParticle2.emission;
		swordBlade = rightHandContainer.transform.Find("FireSword").transform.Find("sword blade");
		ignitionFlame = rightHandContainer.transform.Find("FireSword").transform.Find("IgnitionFlame");
		getBase (gameObject.transform);

		impactEffectPlayer.tag = "bullet";

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
		if (instantiationId == 1){
			team = "blue";
		}
		if (instantiationId == 2){
			if (pvp == 1){
				if (instantiationId == 1){
					team = "blue";
				} else {
					team = "red";
				}
			}
			else {
				team = "blue";
			}
		}
		if (instantiationId == 3){
			if (pvp == 1){
				if (instantiationId == 1){
					team = "blue";
				} else if (instantiationId == 2) {
					team = "red";
				} else {
					team = "green";
				}
			}
			else {
				team = "blue";
			}
		}
		if (instantiationId == 4){
			if (pvp == 1){
				if (instantiationId == 1 || instantiationId == 2){
					team = "blue";
				} else {
					team = "red";
				}
			}
			else {
				team = "blue";
			}
		}
		if (instantiationId == 6){
			if (pvp == 1){
				if (instantiationId == 1 || instantiationId == 2 || instantiationId == 3){
					team = "blue";
				} else {
					team = "red";
				}
			}
			else {
				team = "blue";
			}
		}
		if (instantiationId == 10){
			if (pvp == 1){
				if (instantiationId == 1 || instantiationId == 2 || instantiationId == 3 || instantiationId == 4 || instantiationId == 5){
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
	IEnumerator eraseWater() {
		waterEraser.transform.localScale = new Vector3(1000f,1000f,1000f);
		yield return new WaitForSeconds(0.1f);
		waterEraser.transform.localScale = new Vector3(0f,0f,0f);
	}
	void Start() {
		canvasManager = GameObject.Find("Canvas Manager").GetComponent<ManageCanvas>();
		canvasManager.MyPlayerInstantiated = true;
/* 		
		hashPvP.Add("pvp", (int)pvp); */
		//setting team
		//needs to check again after another player enters
		//setTeams();
		//end seting team

		Health = 10;
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

		

		GameObject.Find("Canvas In Game").transform.Find("Control Panel").transform.gameObject.SetActive(false);
		
		StartCoroutine(waitForGameToLoad());
/* 		healWave = GameObject.Find("HealWave" + instantiationId).GetComponent<ZibraLiquidEmitter>();
		attackWave = GameObject.Find("AttackWave" + instantiationId).GetComponent<ZibraLiquidEmitter>();
		waterBall = GameObject.Find("WaterBall" + instantiationId).GetComponent<ZibraLiquidEmitter>();
		waterBallForceField = GameObject.Find("WaterBallForceField" + instantiationId).GetComponent<ZibraLiquidForceField>();
		waterEraser = GameObject.Find("Void");
		StartCoroutine(eraseWater()); */



	}


	IEnumerator waitForGameToLoad(){
		GameObject[] reticles = GameObject.FindGameObjectsWithTag("reticle");
		foreach(var reticle in reticles){
			reticle.SetActive(false);
		}
		yield return new WaitForSeconds(1f);
		crossHair = GameObject.Find("Canvas In Game").transform.Find("reticle" + instantiationId);
		if (crossHair != null){
			crossHair.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f,0f);
			crossHair.gameObject.SetActive(false);
		}
		if(photonView.IsMine){

			bigSword.gameObject.layer = LayerMask.NameToLayer("MyWeapon");

		} else {
			bigSword.gameObject.layer = LayerMask.NameToLayer("EnemyWeapon");

		}
	


	}
	IEnumerator GCD(int layer, float time) {
		//not really global can vary
		canCast = false;
		playerAnimator.SetLayerWeight(layer, 1f);
		yield return new WaitForSeconds(time);
		canCast = true;
		playerAnimator.SetLayerWeight(layer, 0f);
	}


	IEnumerator CastSpell(float gcd, string layerAndAnimName, int layerNumber, float firstPause,  float audioPitch1, float audioPitch2, 
		AudioClip castSound, float volume, ZibraLiquidForceField ff, ZibraLiquidEmitter waterSpell, GameObject spell, float secondPause){
print("cast attack wave 1");
		if (spell != null || waterSpell != null){
						

			StartCoroutine(GCD(layerNumber, gcd));
			playerAnimator.Play(layerAndAnimName, layerNumber, 0f);
			yield return new WaitForSeconds(firstPause);

			if (audioPitch1 != 0)
				audio.pitch = audioPitch1;

			audio.PlayOneShot (castSound, volume);

			if (waterSpell != null){

				eraserTimer = 0f;
				eraserTimerOn = true;
				waterEmitting = true;
				waterSpell.enabled = true;

				if (ff != null)
					ff.enabled = true;

				yield return new WaitForSeconds(secondPause);

				if (audioPitch2 != 0)
					audio.pitch = audioPitch2;
				
				waterEmitting = false;
				waterSpell.enabled = false;

				if (ff != null)
					ff.enabled = false;

			}

		}

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
	IEnumerator slowMotionForABit() {
		slowMotion = true;
		Time.timeScale = 0.5f;
		yield return new WaitForSeconds(2.5f);
		Time.timeScale = 1f;
	}

 
	void FixedUpdate() {
/* 		if (controls.Gameplay.Jump.triggered){
			//if (instantiationId == photonView.CreatorActorNr)
			//if (!photonView.IsMine)
				//GetComponent<Rigidbody>().AddForce(new Vector3(-250f, 0f, 0));
		} */
		if (Health <= 0 && slowMotion == false){
			StartCoroutine(slowMotionForABit());
		}



		if (photonView.IsMine){
			velocity = (transform.position - previousPos) / Time.deltaTime;
			previousPos = transform.position;
		}
		if (!photonView.IsMine)
		{
			GetComponent<Rigidbody>().position = Vector3.MoveTowards(GetComponent<Rigidbody>().position, networkPosition, Time.fixedDeltaTime);
			GetComponent<Rigidbody>().rotation = Quaternion.RotateTowards(GetComponent<Rigidbody>().rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
		}
	}
 
	void Update() {
/*         if ((controls.Gameplay.Action5.triggered || Input.GetKeyDown("g")) && canvasManager.ability == 0 && waterEmitting == false && canCast){
			//global cooldown
			print("cast attack wave");
			//if used more than 10 times do an erase or after 3 seconds and not refreshed
			StartCoroutine(CastSpell(1f, "CastingWhileMoving2.Cast", 13, 0.4f, 0, 0, attackWaveSound, 0.5f, null, attackWave, null, 0.5f));
        }
 */

		if (!PhotonNetwork.IsConnected)
			return;

		//if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
		if (photonView.IsMine == false)
			return;

		if (aiming == true){
			if (crossHair != null)
				crossHair.gameObject.SetActive(true);
		} else {
			if (crossHair != null)
				crossHair.gameObject.SetActive(false);
		}




		if (healWave == null && attackWave == null && waterBall == null && waterBallForceField == null && waterEraser == null){
			if (GameObject.Find("HealWave" + instantiationId) != null && GameObject.Find("AttackWave" + instantiationId) != null && 
				GameObject.Find("WaterBall" + instantiationId) != null && GameObject.Find("WaterBallForceField" + instantiationId) != null &&
				GameObject.Find("Void") != null){
					healWave = GameObject.Find("HealWave" + instantiationId).GetComponent<ZibraLiquidEmitter>();
					attackWave = GameObject.Find("AttackWave" + instantiationId).GetComponent<ZibraLiquidEmitter>();
					waterBall = GameObject.Find("WaterBall" + instantiationId).GetComponent<ZibraLiquidEmitter>();
					waterBallForceField = GameObject.Find("WaterBallForceField" + instantiationId).GetComponent<ZibraLiquidForceField>();
					waterEraser = GameObject.Find("Void");
					StartCoroutine(eraseWater());

			}
		}


		

		if (healWave != null){
            if (waterEmitting == false)
            	healWave.enabled = false;
        } 
		if (attackWave != null){
            if (waterEmitting == false)
            	attackWave.enabled = false;
        }
		if (waterBall != null){
            if (waterEmitting == false){
				waterBall.enabled = false;
				waterBallForceField.enabled = false;
			} 
        }
		if (eraserTimerOn){
			eraserTimer += Time.deltaTime;
		}
		if (eraserTimer >= 5f){
			if (castingHealingWater){
				castingHealingWater = false;
			}
			StartCoroutine(eraseWater());
			eraserTimerOn = false;
			eraserTimer = 0f;
		}
		



        if ((controls.Gameplay.Action5.triggered || Input.GetKeyDown("g")) && canvasManager.ability == 0 && waterEmitting == false && canCast){
			//global cooldown

			//if used more than 10 times do an erase or after 3 seconds and not refreshed
			StartCoroutine(CastSpell(1f, "CastingWhileMoving2.Cast", 13, 0.4f, 0, 0, attackWaveSound, 0.5f, null, attackWave, null, 0.5f));
            

        }
        if ((controls.Gameplay.Action5.triggered || Input.GetKeyDown("h")) && canvasManager.ability == 3 && waterEmitting == false && canCast){
			//10 second cooldown?
			print("cast heal wave");
			castingHealingWater = true;
            StartCoroutine(CastSpell(1f, "CastingWhileMoving1.Cast", 12, 0.3f, 0, 0, healWaveSound, 0.5f, null, healWave, null, 0.25f));

        }
		if ((controls.Gameplay.Action5.triggered || Input.GetKeyDown("t")) && canvasManager.ability == 4 && waterEmitting == false && canCast){
			print("cast water ball");
			StartCoroutine(CastSpell(2f, "CastingWhileMoving3.Cast", 14, 0.4f, 0.2f, 1f, healWaveSound, 0.5f, waterBallForceField, waterBall, null, 0.75f));

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


		if (photonView.IsMine && connected){// && connected?
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
			} else {
				//for crosshairLookAt
				Debug.DrawRay(virtualLookCam.transform.position, virtualLookCam.transform.forward, Color.green);
				var ray = new Ray(virtualLookCam.transform.position, virtualLookCam.transform.forward);
				RaycastHit camRaycastHit;
				if (Physics.Raycast(ray, out camRaycastHit)){
					rayCollision = camRaycastHit.point;
				}

				//testing
				if (shoot > 0.5){
					crosshairLookAt.transform.LookAt(rayCollision);
					print(crosshairLookAt.transform.rotation);
				} else {
					print(crosshairLookAt.transform.rotation);
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

						
					if(playerAnimator.GetLayerWeight(4) <= 0.5){// starts transitioning during reload


						

						lerpVal2 = 1f;
						lerpVal += Time.deltaTime * 5f;
						playerAnimator.SetLayerWeight(7, Mathf.Lerp(lerpVal, 1, Time.deltaTime));
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
			//underWater = (transform.position.y <= 40.3f) ? true : false;
			underWater = (transform.position.y <= -4.5f) ? true : false;
/* 			if (transform.position.y < 0f){
				Health = 0;
			} */
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
				//if (jump == 1 && transform.position.y <= 40f) {
				if (jump == 1 && transform.position.y <= -4.5f) {
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
					//transform.GetComponent<Rigidbody>().AddForce(transform.forward * 10f); //add force to roll anim during actual roll time
				}
				if (!rolling && !changingWeps){
					//start attacks
					if (controls.Gameplay.Action4.triggered && !isAerial) {
						audio.PlayOneShot (soundEffectRoll, 0.5f);
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
/* 								StartCoroutine(shootFireball());
								playerAnimator.Play("fireball", 0, .5f);
								secondHit = false; */
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

						//commenting out grenades for now

						/* if (grenades > 0) {
							playerAnimator.SetTrigger ("throw");
							StartCoroutine (rollThenBlowUp ());
						}  */
					} 

					if (controls.Gameplay.Jump.triggered && !isAerial && !playerAnimator.GetBool("blocking") && !underWater) {
						//moveLegs = false;
						audio.PlayOneShot (soundEffectJump, 0.5f);
						rigidBody.AddForce(new Vector3(0, 200f, 0));
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
						rightFootTouchingGround();
						leftFootTouchingGround();
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
					photonView.RPC("NoFlames", RpcTarget.All);
					//fire.gameObject.SetActive (false);
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

					playerAnimator.SetFloat("rotateY", Mathf.Sin(-m_camRotation.transform.eulerAngles.x * (Mathf.PI / 180) + 0f));//radians to degrees offsetting 0-x% because animations are off
					transform.rotation = Quaternion.Euler(0f, m_camRotation.transform.eulerAngles.y, 0f);
					RaycastHit hit;
					if(Physics.Raycast(m_camRotation.transform.position, m_camRotation.transform.forward, out hit, 200.0f)){

						followTarget.LookAt(hit.point);//fix later
					} else {
						followTarget.transform.rotation = Quaternion.Euler(m_camRotation.transform.eulerAngles.x,
						m_camRotation.transform.eulerAngles.y, m_camRotation.transform.eulerAngles.z);
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
				if (rolling){
					rollSpeed = 1f; //fix later (can roll without moving currently)
				} else {
					rollSpeed = 1f;
				}
				if (!gameTransition.transitioning && alive){

						if (!playerAnimator.GetBool("blocking") && !bigSwing && !knockDown && !gettingUp && !fireball) {
							transform.position += movement * (moveSpeed * rollSpeed) * Time.deltaTime;
						}
						if (bigSwing && isAerial) {
							transform.position += movement * (moveSpeed * rollSpeed) * Time.deltaTime;
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
	void LoseMediumHealth() {
		Health -= 3;
	}
	[PunRPC]
	void LoseSmallHealth() {
		Health -= 2;
	}
	[PunRPC]
	void Flames() {
		flames.Play();
		embers.Play();
	}
	[PunRPC]
	void NoFlames() {
		flames.Stop();
		embers.Stop();
	}
	[PunRPC]
	void ShootArrow() {
		if (photonView.IsMine){
			GameObject newArrow = null;
			Vector3 eulerRot = new Vector3(shootZone.eulerAngles.x, shootZone.eulerAngles.y, shootZone.eulerAngles.z);
			newArrow = PhotonNetwork.Instantiate (arrow.name, shootZone.transform.position, Quaternion.Euler(eulerRot)) as GameObject;
			//newArrow = Instantiate (arrow, shootZone.transform.position, Quaternion.Euler(eulerRot)) as GameObject;
			newArrow.transform.GetChild(0).gameObject.GetComponent<Rigidbody> ().AddForce (shootZone.forward * 75);//75
			newArrow.gameObject.tag = "arrow";
			shootArrowSound();
		}

	}

	[PunRPC]
	void ShootHandgun() {
		if (!photonView.IsMine)
			return;
		if(handgunAmmo > 0){ //does an extra shot because I have to start at the end of an animation
				handgunAmmo -= 1;

				Transform shootZoneOrCrosshair;
				if(aiming){
					shootZoneOrCrosshair = crosshairLookAt.transform;
				} else {
					shootZoneOrCrosshair = shootZone;
				}

				RaycastHit hit;
				shootHandgunSound();
				if (Physics.Raycast(handgun.position, shootZoneOrCrosshair.forward, out hit, range)){
					if (hit.rigidbody != null) {
						hit.rigidbody.AddForce(hit.normal * -15f);
					}
					if (hit.transform.tag == "Player" || hit.transform.name == "cop" || hit.transform.name == "BulletImpactFleshSmallEffect(Clone)"){
						print("test2");
						//impactEffectPlayer.tag = "bullet";
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

				//add rifle sound here
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
	[PunRPC]
	void FootSoundGrass() {
		int randomFootSound = Random.Range(0, 2);
		if (randomFootSound == 0){
			if (!audio.isPlaying)
			audio.PlayOneShot(soundEffectFoot1, 1f);
		} else if (randomFootSound == 1){
			if (!audio.isPlaying)
			audio.PlayOneShot(soundEffectFoot2, 1f);
		} 
	}
	[PunRPC]
	void FootSoundHardSurface() {
		int randomFootSound = Random.Range(0, 2);
		if (randomFootSound == 0){
			if (!audio.isPlaying)
			audio.PlayOneShot(soundEffectFoot3, 1f);
		} else if (randomFootSound == 1){
			if (!audio.isPlaying)
			audio.PlayOneShot(soundEffectFoot4, 1f);
		} 
	}

	void shootArrowSound() {
		int randomArrowSound = Random.Range(0, 2);
		if (randomArrowSound == 0){
			audio.PlayOneShot(soundEffectArrow1, 0.5f);
		} else if (randomArrowSound == 1){
			audio.PlayOneShot(soundEffectArrow2, 0.5f);
		} 
	}

	void reloadHandgunSound() {
			audio.PlayOneShot(soundEffectHandgunReload, 0.35f);
	}
	void shootHandgunSound() {
		int randomHandgunSound = Random.Range(0, 4);
		if (randomHandgunSound == 0){
			audio.PlayOneShot(soundEffectHandgunShot1, 0.25f);
		} else if (randomHandgunSound == 1){
			audio.PlayOneShot(soundEffectHandgunShot2, 0.25f);
		} else if (randomHandgunSound == 2){
			audio.PlayOneShot(soundEffectHandgunShot3, 0.25f);
		} else if (randomHandgunSound == 3){
			audio.PlayOneShot(soundEffectHandgunShot4, 0.25f);
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
/* 					if (playerAnimator.GetLayerWeight(8) >= 0.9f && playerAnimator.GetLayerWeight(1) < 0.1f){
						photonView.RPC("ShootArrow", RpcTarget.All);
					} */
					if (playerAnimator.GetLayerWeight(8) < 0.1f && playerAnimator.GetLayerWeight(1) > 0.9f){
						photonView.RPC("ShootArrow", RpcTarget.All);
					}
					//photonView.RPC("ShootArrow", RpcTarget.All);
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
		reloadHandgunSound();
		playerAnimator.SetLayerWeight(4, 1f);
		photonView.RPC("RPCTrigger3", RpcTarget.All, "Handgun Reload");
		//playerAnimator.SetBool("reloading", true);
		yield return new WaitForSeconds(1f);
		playerAnimator.SetLayerWeight(4, 0f);
		//playerAnimator.SetBool("reloading", false);
		handgunAmmo = maxHandgunAmmo;
	}

	IEnumerator ReloadHandgunContinueShooting() {
		
		yield return new WaitForSeconds(0.1f);//so hand doesn't go down idk why 0.1 seconds is enough time
		reloadHandgunSound();
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

		handgunAmmo = maxHandgunAmmo;
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
		//if (!photonView.IsMine){return;}
		if (!takingDamage && photonView.IsMine) {
			if (col.gameObject.layer == LayerMask.NameToLayer("EnemyWeapon")){
				StartCoroutine(takeMediumDamage());
			}
			if (col.gameObject.tag == "bullet" ||  col.gameObject.name.Contains("bullet")){
				StartCoroutine(takeSmallDamage());
			}

/* 			if (col.gameObject.tag == "Explosion") {
				StartCoroutine (takeBigDamage ());
			}
			if (col.gameObject.tag == "bullet" || col.gameObject.tag == "bigBullet" || col.gameObject.name.Contains("bullet")){
				takeDamageNoWait (col.gameObject.tag);
			}
			if ((col.gameObject.tag == "arrow" || col.gameObject.tag == "damage") && !takingDamage){
				if (mwt.enabled && transform.root != col.gameObject.transform.root)
				StartCoroutine (takeDamage());
			} 
			if (col.gameObject.tag == "Medium Damage" && !takingDamage){
				if(mwt2.enabled && transform.root != col.gameObject.transform.root)
				StartCoroutine (takeMediumDamage());
			} */
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
		//if (!photonView.IsMine){return;}
		print(col.collider);
		if (photonView.IsMine){
			if (!takingDamage ){
				//collision's collider?
				//use col.collider for child collisions
				if (col.collider.gameObject.layer == LayerMask.NameToLayer("EnemyWeapon")){
					StartCoroutine(takeSmallDamage());
				}
				if (col.gameObject.tag == "Explosion") {
					StartCoroutine (takeBigDamage ());
				}
				if (col.gameObject.tag == "arrow"){
					//if (!gameTransition.transitioning){
					StartCoroutine(takeMediumDamage());
						//Destroy(col.gameObject);
					//}
				}		

	/* 			if (col.gameObject.tag == "bullet" ||  col.gameObject.name.Contains("bullet")){
					StartCoroutine(takeSmallDamage());
				} */
			}

			if (col.gameObject.name == "ground"){
				//playerAnimator.SetBool("falling", false); don't have a falling variable maybe add but probably not
				isAerial = false;
			}
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
			photonView.RPC("LoseMediumHealth", RpcTarget.All);
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
		if (photonView.IsMine){
			takingDamage = true;
			photonView.RPC("LoseMediumHealth", RpcTarget.All);
			yield return new WaitForSeconds (0.5f);
			takingDamage = false;
		}

		
	}
	IEnumerator takeSmallDamage() {
		if (photonView.IsMine){
			takingDamage = true;
			photonView.RPC("LoseSmallHealth", RpcTarget.All);
			yield return new WaitForSeconds (0.5f);
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
		//if (!gameTransition.transitioning){
			if (bulletType == "bullet")
			Health -= 1f;
			if (bulletType == "bigBullet")
			Health -= 3f;
			if (bulletType == "arrow"){
				
				Health -= 3f;
			}
		//}
	}
	IEnumerator blinking() {
		photonView.RPC("Flames", RpcTarget.All);
		yield return new WaitForSeconds (0.5f);
		photonView.RPC("NoFlames", RpcTarget.All);
	
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
			handgunAmmo = maxHandgunAmmo;
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
			return Physics.CheckSphere(baseOfCharacter.position, 0.1f, 1);//default layer is 1
			
		}
		return false;    
	}

	void rightFootTouchingGround() {
		if (rightFoot) {
			if (Physics.CheckSphere(rightFoot.position, 0.05f, 1)) {//default layer is 1
				rightFootHitColliders = Physics.OverlapSphere(rightFoot.position, 0.05f);
				if (rightFootHitColliders.Length != 0) {
					if (rightFootHitColliders[0].name == "ground"){
						photonView.RPC("FootSoundGrass", RpcTarget.All);
					} else {
						photonView.RPC("FootSoundHardSurface", RpcTarget.All);
					}
					
				}

				//else
				//photonView.RPC("FootSoundHardSurface", RpcTarget.All);
			}
		}
	}
	void leftFootTouchingGround() {
		if (leftFoot) {
			if (Physics.CheckSphere(leftFoot.position, 0.05f, 1)) {//default layer is 1
				leftFootHitColliders = Physics.OverlapSphere(leftFoot.position, 0.05f);
				if (leftFootHitColliders.Length != 0) {
					if (leftFootHitColliders[0].name == "ground"){
						photonView.RPC("FootSoundGrass", RpcTarget.All);
					} else {
						photonView.RPC("FootSoundHardSurface", RpcTarget.All);
					}
					
				}

				//else
				//photonView.RPC("FootSoundHardSurface", RpcTarget.All);
			}
		}
	}


	public static void DumpToConsole(object obj){
		var output = JsonUtility.ToJson(obj, true);
		Debug.Log(output);
	}
}
