using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerInputHandler : MonoBehaviour
{
    public UnityEvent<Vector3> PointerClick;
    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);

        else
            Instance = this;

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
        
            var mPos = Input.mousePosition;
            PointerClick?.Invoke(mPos);
        }
        
        if (Input.touchCount <= 0) return;
        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;
        var mousePos = Input.mousePosition;
        PointerClick?.Invoke(mousePos);
    }
}