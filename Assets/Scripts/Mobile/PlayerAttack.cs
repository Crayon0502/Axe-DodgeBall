using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject axePrefab;
    [SerializeField] GameObject handAxe;
    [SerializeField] Transform startPos;
    [SerializeField] Transform atkRange;
    [SerializeField] Transform atkRangeImage;
    [SerializeField] Image coolTimeImage;
    [SerializeField] float throwForce = 20f;
    [SerializeField] float attackSpeed = 1.5f;
    [SerializeField] AudioSource audioSource;

    [SerializeField] LayerMask lm;
    Animator anim;
    PlayerMove pm;
    bool doAttack = true;
    Vector3 atkDir = Vector3.zero;
    AttackJoystick aj;
    State state;


    void Start()
    {
        state = GetComponent<State>();
        aj = FindObjectOfType<AttackJoystick>();
        pm = GetComponent<PlayerMove>();
        anim = GetComponent<Animator>();
        atkRange.transform.SetParent(null);

        if (state.pv.IsMine)
        {
            aj.pa = this;
        }
        else
        {
            coolTimeImage.gameObject.SetActive(false);
        }

    }

    void Update()
    {
        atkRange.transform.position = transform.position;
    }

    // 공격
    public void Attack()
    {
        if (state.isDead ||!doAttack || !Gamemanager.instance.isGameStart) return;

        pm.isAttacking = true;

        transform.rotation = Quaternion.LookRotation(atkDir);

        handAxe.SetActive(false);

        state.pv.RPC("Atk", RpcTarget.AllBuffered);
        GameObject axe = PhotonNetwork.Instantiate("Axe", startPos.position, startPos.rotation);

        Axe axeController = axe.GetComponent<Axe>();
        axeController.pv.RPC("Throw", RpcTarget.AllBuffered, atkDir.normalized * throwForce);
        anim.SetTrigger("doAttack");

        StartCoroutine(AttackPossible());
        StartCoroutine(ShowCoolTime());
    }

    // 공격 범위 보여주기
    public void AtkRangeImage(Vector2 dir)
    {
        if (state.isDead) return;

        atkDir = new Vector3(dir.x, 0, dir.y);

        atkRange.forward = atkDir;

        RaycastHit hit;

        if (Physics.Raycast(atkRangeImage.position, atkRangeImage.forward, out hit, 100, lm))
        {
            float distance = Vector3.Distance(atkRangeImage.position, hit.point);
            atkRangeImage.localScale = new Vector3(atkRangeImage.localScale.x, atkRangeImage.localScale.y, distance);
        }
    }

    // 공격 중인지 여부 설정
    public void EndAttackAnimationEvent()
    {
        PlayerMove playerMove = GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            playerMove.SetAttacking(false);
        }
    }

    // 쿨타임
    IEnumerator AttackPossible()
    {
        doAttack = false;
        yield return new WaitForSeconds(attackSpeed);
        doAttack = true;

        handAxe.SetActive(true);
    }

    // 쿨타임 시각적 표시
    IEnumerator ShowCoolTime()
    {
        float timer = 0f;
        float coolTime = attackSpeed;

        while (timer < coolTime)
        {
            timer += Time.deltaTime;
            coolTimeImage.fillAmount = timer / coolTime;
            yield return null;
        }
    }

    public void AtkRange(bool isAttack)
    {
        if (state.isDead) return;

        atkRange.gameObject.SetActive(isAttack);
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    void Atk()
    {
        audioSource.Play();
    }
}
