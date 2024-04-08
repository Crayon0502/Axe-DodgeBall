using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BlinkEffect : MonoBehaviourPunCallbacks
{
    PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        StartCoroutine(DestroyParticle());
    }

    IEnumerator DestroyParticle()
    {
        yield return new WaitForSeconds(0.7f);

        if (pv != null && pv.IsMine)
            pv.RPC("DestroyParticleRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DestroyParticleRPC() => Destroy(gameObject);
}
