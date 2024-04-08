using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Photon.Pun;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    [SerializeField] float speed;
    CharacterController cc;
    Animator anim;
    CameraMove camMove;
    State state;
    MoveJoystick mj;

    public bool isAttacking = false;

    void Start()
    {
        mj = FindObjectOfType<MoveJoystick>();
        state = GetComponent<State>();
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        camMove = Camera.main.GetComponent<CameraMove>();

        if (state.pv.IsMine)
        {
            mj.pm = this;
            camMove.player = transform;
        }
    }

    // �÷��̾� �ȱ� �ִϸ��̼�
    public void MoveAnim(Vector2 inputDir)
    {
        if (state.isDead) return;

        Vector3 moveDir = new Vector3(inputDir.x, 0, inputDir.y);
        anim.SetFloat("Speed", Mathf.Abs((moveDir * speed).magnitude));
    }

    // �÷��̾� ������
    public void Move(Vector2 inputDir)
    {
        if (state.isDead) return;

        Vector3 moveDir = new Vector3(inputDir.x, 0, inputDir.y);
        cc.Move(moveDir * speed * Time.deltaTime);

        if (!isAttacking)
            transform.forward = moveDir;
    }

    // ����
    public void Blink(Vector2 inputDir)
    {
        if (state.isDead) return;

        state.pv.RPC("BlinkEffect", RpcTarget.AllBuffered, transform.position);

        Vector3 moveDir = new Vector3(inputDir.x, 0, inputDir.y);
        Vector3 blinkPosition = transform.position + moveDir * 3f;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, moveDir, out hit, 3f))
        {
            blinkPosition = hit.point - moveDir.normalized * 0.2f;
        }

        cc.enabled = false;
        transform.position = blinkPosition;
        cc.enabled = true;

        state.pv.RPC("BlinkEffect", RpcTarget.AllBuffered, transform.position);
    }

    [PunRPC]
    void BlinkEffect(Vector3 pos)
    {
        if (state.isDead) return;
        PhotonNetwork.Instantiate("BlinkEffect", pos, Quaternion.identity);
    }

    // ���� ������ ���� ����
    public void SetAttacking(bool attacking)
    {
        isAttacking = attacking;
    }
}
