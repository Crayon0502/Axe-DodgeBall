using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] 
    RectTransform circle;

    RectTransform rect;

    [SerializeField, Range(10, 150)]
    float circleRange;

    protected Vector2 inputDir;
    protected bool isInput;

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // �巡�� ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        ControllJoystick(eventData);
        isInput = true;
    }

    // �巡�� ��
    public void OnDrag(PointerEventData eventData)
    {
        ControllJoystick(eventData);
    }

    // �巡�� ��
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        circle.anchoredPosition = Vector2.zero;
        isInput = false;
    }

    void ControllJoystick(PointerEventData eventData)
    {
        Vector2 pointerPosition = eventData.position; // �巡�� ���� ������ ��ġ
        Vector2 rectPosition = rect.position; // ���̽�ƽ�� ��ġ

        // �����Ϳ� ���̽�ƽ�� �Ÿ��� ����Ͽ� ���� ���� ���� ��ġ�ϵ��� ����
        Vector2 direction = pointerPosition - rectPosition;
        if (direction.magnitude > circleRange)
        {
            direction = direction.normalized * circleRange;
        }

        // ���̽�ƽ�� ��ġ ����
        circle.position = rectPosition + direction;

        // �Է� ���� ����
        inputDir = direction / circleRange;
    }
}
