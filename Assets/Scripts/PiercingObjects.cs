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

        if (c.gameObject.tag != "Player" || c.gameObject.GetComponent<Rigidbody>() == null){
            ArrowHitSound();
            rb.isKinematic=true;
            //issues with pun and changing parent
            //transform.parent.transform.SetParent(c.transform);//arrow 1 is child of arrow(Clone) b/c rotation issues
            StartCoroutine(waitThenDelete());
        }
    }
    IEnumerator waitThenDelete() {
        yield return new WaitForSeconds (5);
        if (photonView.IsMine){
            if (this.transform.parent.name == "arrow(Clone)"){
                PhotonNetwork.Destroy (this.transform.parent.gameObject);
            }
            
        }
    }


}
