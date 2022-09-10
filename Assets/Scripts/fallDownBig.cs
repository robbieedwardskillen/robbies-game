using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class fallDownBig : MonoBehaviourPunCallbacks, IPunObservable
{
    private Rigidbody rb;
    public float health = 5;
    private float radius = 1.25F;
    private float power = 20.0F;
    private float speed;

    #region IPunObservable implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
		if (stream.IsWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(health);
		}
		else
		{
			// Network player, receive data
			this.health = (float)stream.ReceiveNext();
		}
    }
    #endregion
    void Awake() 
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		if (gameObject.GetComponent<Rigidbody> () != null) {
			rb = gameObject.GetComponent<Rigidbody> ();
        }

    }
    void Update() {
        if (PhotonNetwork.IsConnected == true)
        {
            return;
        }

    }

    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "damage" || col.gameObject.tag == "Medium Damage" ||
        col.gameObject.tag == "groundBullet" || col.gameObject.tag == "Explosion"){
            health -= 1;
        }
        if (health <= 0){
            if (gameObject.tag != "deadTree"){
                if (speed >= 3.5){
                    gameObject.tag = "Explosion";
                }
                gameObject.GetComponent<Rigidbody> ().isKinematic = false;
                if (col.transform.position.x > gameObject.transform.position.x){
                    gameObject.GetComponent<Rigidbody> ().AddForce(new Vector3(1000f, 0, 0));
                } else if (col.transform.position.x < gameObject.transform.position.x) {
                    gameObject.GetComponent<Rigidbody> ().AddForce(new Vector3(-1000f, 0, 0));
                }
                if (col.transform.position.z > gameObject.transform.position.z){
                    gameObject.GetComponent<Rigidbody> ().AddForce(new Vector3(0, 0, 1000f));
                } else if (col.transform.position.z < gameObject.transform.position.z){
                    gameObject.GetComponent<Rigidbody> ().AddForce(new Vector3(0, 0, -1000f));
                }
                StartCoroutine(RollThenStop());
            }
        }
    }
    IEnumerator RollThenStop() {
		yield return new WaitForSeconds(6);
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.tag = "deadTree";

	}
    void OnCollisionEnter(Collision col){
/*         if (col.gameObject.tag == "Player" && gameObject.tag == "Explosion"){
            if (col.transform.position.x > gameObject.transform.position.x){
                col.gameObject.GetComponent<Rigidbody> ().AddForce(new Vector3(50f, 65f, 0));
            } else if (col.transform.position.x < gameObject.transform.position.x) {
                col.gameObject.GetComponent<Rigidbody> ().AddForce(new Vector3(-50f, 65f, 0));
            }
            if (col.transform.position.z > gameObject.transform.position.z){
                col.gameObject.GetComponent<Rigidbody> ().AddForce(new Vector3(0, 65f, 50f));
            } else if (col.transform.position.z < gameObject.transform.position.z){
                col.gameObject.GetComponent<Rigidbody> ().AddForce(new Vector3(0, 65f, -50f));
            }
        }
 */

        if (col.gameObject.tag == "damage" || col.gameObject.tag == "Medium Damage" ||
        col.gameObject.tag == "groundBullet" || col.gameObject.tag == "Explosion"){
            health -= 1;
        }
        if (health <= 0){
/*             if (gameObject.tag != "deadTree"){
                if (speed >= 3.5){
                    gameObject.tag = "Explosion";
                } */
                gameObject.GetComponent<Rigidbody> ().isKinematic = false;
                if (col.transform.position.x > gameObject.transform.position.x){
                    gameObject.GetComponent<Rigidbody> ().AddForce(new Vector3(1000f, 0, 0));
                } else if (col.transform.position.x < gameObject.transform.position.x) {
                    gameObject.GetComponent<Rigidbody> ().AddForce(new Vector3(-1000f, 0, 0));
                }
                if (col.transform.position.z > gameObject.transform.position.z){
                    gameObject.GetComponent<Rigidbody> ().AddForce(new Vector3(0, 0, 1000f));
                } else if (col.transform.position.z < gameObject.transform.position.z){
                    gameObject.GetComponent<Rigidbody> ().AddForce(new Vector3(0, 0, -1000f));
                }
                StartCoroutine(RollThenStop());
            //}
        }
    }
}
