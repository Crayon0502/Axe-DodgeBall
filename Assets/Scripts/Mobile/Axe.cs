using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public PhotonView pv;
    [SerializeField] float rotSpeed = 20f;
    [SerializeField] int damage;

    [PunRPC]
    public void Throw(Vector3 direction)
    {
        // 도끼에 힘을 가해 던지기
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = direction;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward, rotSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!pv.IsMine && other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            other.GetComponent<State>().Hit(damage);
            pv.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
}

