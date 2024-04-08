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

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        ControllJoystick(eventData);
        isInput = true;
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        ControllJoystick(eventData);
    }

    // 드래그 끝
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        circle.anchoredPosition = Vector2.zero;
        isInput = false;
    }

    void ControllJoystick(PointerEventData eventData)
    {
        Vector2 pointerPosition = eventData.position; // 드래그 중인 포인터 위치
        Vector2 rectPosition = rect.position; // 조이스틱의 위치

        // 포인터와 조이스틱의 거리를 계산하여 원의 범위 내에 위치하도록 보정
        Vector2 direction = pointerPosition - rectPosition;
        if (direction.magnitude > circleRange)
        {
            direction = direction.normalized * circleRange;
        }

        // 조이스틱의 위치 설정
        circle.position = rectPosition + direction;

        // 입력 방향 설정
        inputDir = direction / circleRange;
    }
}
