using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class State : MonoBehaviourPunCallbacks
{
    public PhotonView pv;

    public bool isDead;
    [SerializeField] Text NicknameText;
    [SerializeField] Image Hp_bar;
    [SerializeField] float hp;
    [SerializeField] float maxHp;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        hp = maxHp;

        NicknameText.text = pv.IsMine ? PhotonNetwork.LocalPlayer.NickName : pv.Owner.NickName;
        NicknameText.color = pv.IsMine ? Color.green : Color.red;
    }

    public void Hit(int damage)
    {
        if (isDead) return;

        hp -= damage;
        pv.RPC("OnHit", RpcTarget.AllBuffered, hp);
    }

    //public override void OnLeftRoom()
    //{
    //    Destroy(gameObject);
    //}

    [PunRPC]
    void OnHit(float hp)
    {
        Hp_bar.fillAmount = hp / maxHp;

        if (hp <= 0)
        {
            isDead = true;
            anim.SetTrigger("isDie");
            Gamemanager.instance.IsWin = !pv.IsMine;
        }
    }
}
