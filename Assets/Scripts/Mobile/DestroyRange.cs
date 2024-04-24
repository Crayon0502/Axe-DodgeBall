using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyRange : MonoBehaviourPunCallbacks
{
    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
