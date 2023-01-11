using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PiercingObjects : MonoBehaviourPunCallbacks
{
    Rigidbody rb;

/*     void Start()
    {//start doesn't get called for some reason
        
    } */
    void OnEnable() {
        rb = gameObject.GetComponent<Rigidbody>();
    }
    void OnDisable() {

    }

    void OnCollisionEnter(Collision c) {

        if (c.gameObject.tag != "Player"){
            rb.isKinematic=true;
            transform.parent.transform.SetParent(c.transform);//arrow 1 is child of arrow(Clone) b/c rotation issues
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
