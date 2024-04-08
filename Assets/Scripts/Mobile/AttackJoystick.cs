using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackJoystick : JoystickScript
{
    public PlayerAttack pa;

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        pa.Attack();
    }

    void Update()
    {
        if (pa == null) return;

        if (isInput)
        {
            pa.AtkRangeImage(inputDir);
        }

        pa.AtkRange(isInput);
    }
}
