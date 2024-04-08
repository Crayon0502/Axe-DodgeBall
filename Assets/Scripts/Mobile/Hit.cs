using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace temp
{
    public class Hit : MonoBehaviour
    {
        [SerializeField] Image hp_bar;
        [SerializeField] float hp;
        [SerializeField] float maxHp;

        void Start()
        {
            hp = maxHp;
        }

        void Update()
        {
            // 테스트용
            if (Input.GetKeyDown(KeyCode.C))
            {
                OnHit(1f);
            }
        }

        public void OnHit(float damage)
        {
            hp -= damage;

            hp_bar.fillAmount = hp / maxHp;
            //Gamemanager.instance.HpBarUpdate(hp, maxHp);

            if (hp <= 0)
                Destroy(gameObject);
        }
    }
}


