
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NPC : MonoBehaviour {
	public bool super = false;
	public bool alive = true;
	public bool busy = false;
	bool takingDamage = false;
	bool secondHit = false;
	public float moveSpeed = 1.5f;
	public int npcHealth = 5;
	private BoxCollider[] boxColliders;
	private CapsuleCollider[] capsuleColliders;
	private Transform fire;
	private Rigidbody[] rigidBodies;
	Animator npcAnimator;
	public GameObject thePlayer;
	public GameObject snowBall;	public GameObject superSnowBall;
	private int snowBalls = 15; public int maxSnowBalls = 15;
	private int waitTime;
	Transform snowBallArea;
	Transform swordBlade;
	MeleeWeaponTrail mwt;
	private AudioSource audio;
	public AudioClip soundEffectSwing1;
	public AudioClip soundEffectSwing2;
	public AudioClip soundEffectSwing3;
	public AudioClip soundEffectSwing4;
	Camera mainCamera;
	private Transform baseOfCharacter;
	Vector3 lastMoveDirection = Vector3.zero;
	NavMeshAgent navMeshAgent;
	float wanderRadius = 5f;
    float wanderTimer = 7.5f;
 
    private Transform target;
    private float timer;
	
	void OnEnable () {
        timer = wanderTimer;
    }
	public static Vector3 RandomNavSphere (Vector3 origin, float distance, int layermask) {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
		
            randomDirection += origin;
            NavMeshHit navHit;
           
            NavMesh.SamplePosition (randomDirection, out navHit, distance, layermask);
           
            return navHit.position;
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
	private void allColliders(Transform parent, bool condition) {
		foreach (Transform t in parent) {
			if (t.gameObject.GetComponent<CapsuleCollider> () != null) {
				t.gameObject.GetComponent<CapsuleCollider> ().enabled = condition;
			}

			if (t.gameObject.GetComponent<BoxCollider> () != null) {
				t.gameObject.GetComponent<BoxCollider> ().enabled = condition;
			}
			if (t.gameObject.GetComponent<SphereCollider> () != null) {
				t.gameObject.GetComponent<SphereCollider> ().enabled = condition;
			}
			if (t.gameObject.GetComponent<Rigidbody> () != null) {
				t.gameObject.GetComponent<Rigidbody> ().isKinematic = !condition;
			}

			allColliders (t, condition);
		}
	}
	private void SetDestinationToPlayer() {
		//navMeshAgent.SetDestination(thePlayer.transform.position);
	}
	private void WalkRandomly() {
		timer += Time.deltaTime;
 
        if (timer >= wanderTimer) {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            //navMeshAgent.SetDestination(newPos);
            timer = 0;
        } 
	}
	void Start () {
		thePlayer = GameObject.Find("player_character");
		navMeshAgent = this.GetComponent<NavMeshAgent> ();
		navMeshAgent.speed = 0.5f;
		audio = gameObject.GetComponent<AudioSource> ();
		audio.volume = 0.2f;
		fire = transform.Find ("Fire");

		waitTime = 25;
		foreach (Transform child in transform)
		{
			if (child.name == "Snow Ball Area") {
				snowBallArea = child;
			}
		}
		//----begin npc setup
		alive = true;
		getBase (gameObject.transform);
		allColliders (gameObject.transform, false);
		gameObject.GetComponent<Rigidbody> ().isKinematic = false;
		if (gameObject.name != "MaleCopNPC"){
			swordBlade = transform.Find ("Bip001")
			.transform.Find("Bip001 Pelvis")
			.transform.Find("Bip001 Spine")
			.transform.Find("Bip001 R Clavicle")
			.transform.Find("Bip001 R UpperArm")
			.transform.Find("Bip001 R Forearm")
			.transform.Find("Bip001 R Hand")
			.transform.Find("R_hand_container")
			.transform.Find("FireSword")
			.transform.Find("sword blade");
			mwt = swordBlade.GetComponent<MeleeWeaponTrail> ();
			swordBlade.transform.GetComponent<CapsuleCollider> ().enabled = true;
			swordBlade.transform.GetComponent<CapsuleCollider> ().isTrigger = true;
		}

		//---- end npc setup
		mainCamera = Camera.main; 
		npcAnimator = gameObject.GetComponent<Animator> ();
	}
	void Update() {
		//if (Input.GetButtonDown ("Jump") && IsGrounded()) {
			//audio.PlayOneShot (playerJump, 1.2f);
			gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 150f, 0));
		//}
		//SetDestinationToPlayer();
		WalkRandomly();
	}
	void FixedUpdate () {
		//bool isAerial = !IsGrounded ();
		bool knifeRight = npcAnimator.GetCurrentAnimatorStateInfo (0).IsName ("knifeRight");
		bool swordLeft = npcAnimator.GetCurrentAnimatorStateInfo (0).IsName ("swordLeft");
		bool swordRight = npcAnimator.GetCurrentAnimatorStateInfo (0).IsName ("swordRight");
		bool knifeUp = npcAnimator.GetCurrentAnimatorStateInfo (0).IsName ("knifeUp");
		bool knifeDown = npcAnimator.GetCurrentAnimatorStateInfo (0).IsName ("knifeDown");
		bool swordUp = npcAnimator.GetCurrentAnimatorStateInfo (0).IsName ("swordUp");
		bool swordDown = npcAnimator.GetCurrentAnimatorStateInfo (0).IsName ("swordDown");

		npcAnimator.SetBool("jumping", false);

		//----begin npc movement and rotation

		if (alive) {
 			if (knifeRight || knifeUp || knifeDown || swordLeft || swordRight || swordUp || swordDown) {
				busy = true;
			} else {
				busy = false;
			}
/* 			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");
			npcAnimator.SetFloat ("horizontalSpeed", h);
			npcAnimator.SetFloat ("verticalSpeed", v);
			bool isWalking = (v > 0.5f || h > 0.5f); */




/* 			if (!isAerial) {
				if (Input.GetButtonDown("Fire1") && !busy) {
					npcAnimator.Play("swordRight", 0, .4f);
					audio.PlayOneShot(soundEffectSwing1, 0.7f);
					secondHit = false;
				}
				if (Input.GetButtonDown("Fire1") && busy && secondHit == false) {
					if (isWalking) {
						npcAnimator.Play("knifeRight", 0, .4f);
						audio.PlayOneShot(soundEffectSwing1, 0.7f);
					} else {
						npcAnimator.Play("swordLeft", 0, .4f);
						audio.PlayOneShot(soundEffectSwing2, 0.7f);
					}
					secondHit = true;
				}
				if (Input.GetButtonDown("Fire2") && !busy) {
					if (isWalking) {
						npcAnimator.Play("knifeUp", 0, .4f);
						audio.PlayOneShot(soundEffectSwing4, 0.7f);
					} else {
						npcAnimator.Play("knifeDown", 0, .4f);
						audio.PlayOneShot(soundEffectSwing2, 0.7f);
					}
					secondHit = false;
				}
				if (Input.GetButtonDown("Fire2") && busy == secondHit == false) {
					npcAnimator.Play("swordUp", 0, .4f);
					audio.PlayOneShot(soundEffectSwing1, 0.7f);
					secondHit = true;
				}
			}
			else {
				if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")) && !busy){
					npcAnimator.Play("swordDown", 0, .4f);
					audio.PlayOneShot(soundEffectSwing3, 0.7f);
				}

			} */




			/*
			if (Input.GetButton ("Fire3") && !isAerial) {
				if (snowBalls > 0 && !busy) {
					if (super) {
						fire.gameObject.SetActive (true);
					}
					transform.gameObject.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, 10, 0));
					Vector3 backwards = (transform.position - snowBallArea.transform.position).normalized;
					transform.gameObject.GetComponent<Rigidbody> ().AddForce (backwards);

					if (!npcAnimator.GetBool ("blasting")) {
						npcAnimator.SetTrigger ("blast");
					}
					npcAnimator.SetBool ("blasting", true);
					blastAttack ();
				} else {
					npcAnimator.SetBool ("blasting", false);
					fire.gameObject.SetActive (false);
				}

			} else {
				fire.gameObject.SetActive (false);
				waitTime = 25;
				npcAnimator.SetBool ("blasting", false);
			}
			if (Input.GetButtonUp ("Fire3")) {
				waitTime = 25;
				snowBalls = maxSnowBalls;
				npcAnimator.SetBool ("blasting", false);
			}
 */





/* 			mainCamera = ;
			Vector3 forward = mainCamera.transform.TransformDirection(Vector3.forward);
			forward.y = 0f;
			Vector3 right = new Vector3 (forward.z, 0f, -forward.x);
			Quaternion rotation; */
/* 			if (isAerial) {
				npcAnimator.SetBool ("aerial", true);
			} else { */
				npcAnimator.SetBool ("aerial", false);
			//}
			npcAnimator.SetFloat ("horizontalSpeed", navMeshAgent.velocity.x);
			npcAnimator.SetFloat ("verticalSpeed", navMeshAgent.velocity.z);
			bool navIdle = navMeshAgent.velocity.x < 0.1f && navMeshAgent.velocity.y < 0.1f && navMeshAgent.velocity.z < 0.1f &&
			navMeshAgent.velocity.x > -0.1f && navMeshAgent.velocity.y > -0.1f && navMeshAgent.velocity.z > -0.1f;
			if (navMeshAgent.velocity == Vector3.zero || navIdle) {
				npcAnimator.SetBool("moving", false);
			} else {
				npcAnimator.SetBool("moving", true);
			}


			//----begin npc death
			if (npcHealth <= 0) {
				npcAnimator.enabled = false;
				fire.gameObject.SetActive (false);
				gameObject.GetComponent<CapsuleCollider> ().isTrigger = true;
				gameObject.GetComponent<Rigidbody> ().isKinematic = true;
				allColliders (gameObject.transform, true);
				alive = false;
			}
			//----end npc death

/* 			Vector3 moveDirection = (h * right + v * forward);

			if (moveDirection != Vector3.zero) {
				rotation = Quaternion.LookRotation(Vector3.up);
			} else {
				rotation = Quaternion.Euler (0, mainCamera.transform.eulerAngles.y, 0);
			}
			Vector3 movement = new Vector3(moveDirection.x * Time.deltaTime * moveSpeed, moveDirection.y,
				moveDirection.z * Time.deltaTime * moveSpeed);
			if (moveDirection != Vector3.zero) {
				rotation = Quaternion.LookRotation (moveDirection);
				lastMoveDirection = moveDirection;
			} 
			if (lastMoveDirection != Vector3.zero) {
				rotation = Quaternion.LookRotation (lastMoveDirection);
			} else {
				rotation = Quaternion.identity;
			}
			if (npcAnimator.GetBool ("blasting") == false && !busy) {
				transform.position += movement.normalized * moveSpeed * Time.deltaTime;
			} else {
				transform.position += movement.normalized * (moveSpeed / 4) * Time.deltaTime;
			}

			transform.rotation = rotation; */
		}
			
		//----end npc movement and rotation
	}
	IEnumerator rollThenBlowUp() {
		snowBalls -= 1;
		GameObject newSnowBall;
		if (!super) {
			newSnowBall = Instantiate (snowBall, snowBallArea.position, Quaternion.identity) as GameObject;
		} else {
			newSnowBall = Instantiate (superSnowBall, snowBallArea.position, Quaternion.identity) as GameObject;
		}
		newSnowBall.GetComponent<Rigidbody> ().AddForce (transform.forward * 500);
		newSnowBall.GetComponent<Rigidbody> ().AddForce (transform.up * 100);
		newSnowBall.GetComponent<Rigidbody> ().AddForce (transform.right * 100);
		newSnowBall.GetComponent<Rigidbody> ().AddForce (-transform.right * 100);
		yield return new WaitForSeconds(3);
		newSnowBall.transform.GetChild (0).gameObject.SetActive(true);
		newSnowBall.gameObject.tag = "Explosion";
		newSnowBall.transform.GetComponent<SphereCollider> ().isTrigger = true;
		if (super) {
			newSnowBall.transform.GetComponent<SphereCollider> ().radius = 8;
		} else {
			newSnowBall.transform.GetComponent<SphereCollider> ().radius = 4;
		}

		newSnowBall.gameObject.GetComponent<Renderer> ().enabled = false;
		if (super) {
			yield return new WaitForSeconds (3);
		} else {
			yield return new WaitForSeconds (1);
		}

		Destroy (newSnowBall);
	}
	void blastAttack() {
			waitTime -= 1;
			if (waitTime <= 1) {
				StartCoroutine (rollThenBlowUp ());
			}
	}

	void Land(){
		//needed for land animation
	}
	void OnTriggerEnter (Collider col) {
		if (alive && !takingDamage) {
			if (col.gameObject.tag == "Explosion") {
				StartCoroutine (takeBigDamage ());
			}
		}
	}
	void OnCollisionEnter (Collision col){

		if (alive && !takingDamage) {
			if (col.gameObject.tag == "damage") {
				StartCoroutine (takeDamage ());
			}
		}
	}
	IEnumerator takeBigDamage() {

		takingDamage = true;
		npcHealth -= 4;
		yield return new WaitForSeconds (2);
		takingDamage = false;
	}
	IEnumerator takeDamage() {
		takingDamage = true;
		npcHealth -= 1;
		yield return new WaitForSeconds (2);
		takingDamage = false;
	}

/* 	bool IsGrounded() {
		RaycastHit hit;
		if (baseOfCharacter) {
			if (Physics.Raycast(baseOfCharacter.position, -Vector3.up, out hit, 0.5f) || navMeshAgent.velocity == Vector3.zero)
			{
				Debug.DrawRay(baseOfCharacter.position, baseOfCharacter.TransformDirection(-Vector3.up) * hit.distance, Color.yellow);
				return true;
			}
			else
			{
				Debug.DrawRay(baseOfCharacter.position, baseOfCharacter.TransformDirection(-Vector3.up) * 1000, Color.white);
				return false;
			}
		}
		return false;
	} */
}
