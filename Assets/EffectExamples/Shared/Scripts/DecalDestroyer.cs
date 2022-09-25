using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DecalDestroyer : MonoBehaviourPunCallbacks {

	private float lifeTime;

	private IEnumerator Start()
	{
		lifeTime = gameObject.GetComponent<ParticleSystem>().duration - 0.1f;
		yield return new WaitForSeconds(0.05f);
		if (photonView.IsMine){
			yield return new WaitForSeconds(lifeTime);
			PhotonNetwork.Destroy(gameObject);
		}
	}
}
