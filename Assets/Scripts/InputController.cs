using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField] Observer observer;
    [SerializeField] InputActionAsset actionAsset;

    private InputActionMap m_controlsMap;
    private InputActionMap m_uiMap;

    private InputAction m_moveAction;
    private InputAction m_rotateAction;

    private void Awake()
    {
        m_uiMap = actionAsset.FindActionMap("UiControls");
        m_controlsMap = actionAsset.FindActionMap("CharacterControls");

        m_moveAction = m_controlsMap.FindAction("Move");
        m_rotateAction = m_controlsMap.FindAction("CameraMove");
    }

    private void OnDisable()
    {
        m_controlsMap.Disable();
        m_uiMap.Disable();
    }

    private void Start()
    {
        m_controlsMap.Enable();
        m_uiMap.Enable();
    }

    private void Update()
    {
        MoveObserver();
        RotateObserver();
    }

    private void MoveObserver()
    {
        Vector3 movementVector = m_moveAction.ReadValue<Vector3>();
        observer.Move(movementVector);
    }

    private void RotateObserver()
    {
        Vector3 rotationVector = m_rotateAction.ReadValue<Vector2>();
        observer.RotateCamera(rotationVector);
    }
}
