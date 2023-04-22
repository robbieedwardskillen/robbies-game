using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PiercingObjects : MonoBehaviourPunCallbacks
{
    Rigidbody rb;
    private AudioSource audio;
	public AudioClip arrowHitSound1;
    public AudioClip arrowHitSound2;
/*     void Start()
    {//start doesn't get called for some reason
        
    } */
    void OnEnable() {
        audio = gameObject.GetComponent<AudioSource> ();
        rb = gameObject.GetComponent<Rigidbody>();
    }
    void OnDisable() {

    }

	void ArrowHitSound() {
		int randomArrowHitSound = Random.Range(0, 2);
		if (randomArrowHitSound == 0){
			audio.PlayOneShot(arrowHitSound1, 0.05f);
		} else if (randomArrowHitSound == 1){
			audio.PlayOneShot(arrowHitSound2, 0.05f);
		} 
	}
    void OnCollisionEnter(Collision c) {
        rb.isKinematic=true;
        if (c.gameObject.tag != "Player" || c.gameObject.GetComponent<Rigidbody>() == null){
            ArrowHitSound();
        }
        if (c.gameObject.tag == "Player"){
            GetComponent<MeleeWeaponTrail>().enabled = false;
            
            if (this.gameObject.tag != "dead arrow"){
                GetComponent<BoxCollider>().enabled = false;
                transform.parent.transform.SetParent(c.transform);//arrow 1 is child of arrow(Clone) b/c rotation issues
            }
            StartCoroutine(waitSmallThenDelete());
        }
        else {
            StartCoroutine(waitThenDelete());
        }
        
    }
    IEnumerator waitThenDelete() {

        yield return new WaitForSeconds(0.2f);
        this.gameObject.tag = "dead arrow";
        yield return new WaitForSeconds (10);
        if (photonView.IsMine){
            if (this.transform.parent.name == "arrow(Clone)"){
                PhotonNetwork.Destroy (this.transform.parent.gameObject);
            }
            
        }
    }
    IEnumerator waitSmallThenDelete() {
        yield return new WaitForSeconds(0.2f);
        this.gameObject.tag = "dead arrow";
        yield return new WaitForSeconds (1f);
        if (photonView.IsMine){
            if (this.transform.parent.name == "arrow(Clone)"){
                PhotonNetwork.Destroy (this.transform.parent.gameObject);
            }
            
        }
    }

}
