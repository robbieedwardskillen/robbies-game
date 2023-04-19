using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallDownSmall : MonoBehaviour
{
    private Rigidbody rb;
    float health = 1;
/*     private float radius = 1.25F;
    private float power = 10.0F; */
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		if (gameObject.GetComponent<Rigidbody> () != null) {
			rb = gameObject.GetComponent<Rigidbody> ();
        }

    }
    void Update() {
        if (health <= 0)
        StartCoroutine(CalculateSpeed());
    }
    IEnumerator CalculateSpeed(){
        Vector3 lastPosition = transform.position;
        yield return new WaitForFixedUpdate();
        speed = (lastPosition - transform.position).magnitude / Time.deltaTime;
        print(speed);
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
        if (col.gameObject.tag == "Player" && gameObject.tag == "Explosion"){
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

    }
}
