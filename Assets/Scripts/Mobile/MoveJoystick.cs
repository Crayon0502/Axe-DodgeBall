using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveJoystick : JoystickScript
{
    public Button blinkButton;
    public Image coolTimeImage;
    public Text coolTimeText;
    [SerializeField] float blinkCollTime = 10f;

    public PlayerMove pm;

    void Update()
    {
        if (pm == null) return;

        if (isInput)
        {
            pm.Move(inputDir);
            pm.MoveAnim(inputDir);
        }
        else
        {
            pm.MoveAnim(Vector2.zero);
        }
    }

    // Á¡¸ê
    public void Blink()
    {
        if (!Gamemanager.instance.isGameStart) return;

        if (isInput)
            pm.Blink(inputDir);
        else
            pm.Blink(Vector2.zero);

        StartCoroutine(BlinkPossible());
        StartCoroutine(ShowCoolTime());
    }

    // ÄðÅ¸ÀÓ
    IEnumerator BlinkPossible()
    {
        blinkButton.interactable = false;
        coolTimeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(blinkCollTime);
        blinkButton.interactable = true;
        coolTimeText.gameObject.SetActive(false);
    }

    // ÄðÅ¸ÀÓ ½Ã°¢Àû Ç¥½Ã
    IEnumerator ShowCoolTime()
    {
        float timer = 0f;
        float coolTime = blinkCollTime;

        while (timer < coolTime)
        {
            timer += Time.deltaTime;
            coolTimeImage.fillAmount = timer / coolTime;
            coolTimeText.text = (coolTime - timer).ToString("N1");
            yield return null;
        }
    }
}
