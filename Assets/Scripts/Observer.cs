using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Observer : MonoBehaviour
{
    [SerializeField] int speed = 10;
    [SerializeField] int cameraSensitivity = 10;

    private CharacterController m_characterController;

    private float m_xRotation = 0f;
    private float m_yRotation = 0f;

    private Vector3 m_previousPosition;

    public Action<Vector3> onPositionChange;

     private void Start()
    {
        m_characterController = GetComponent<CharacterController>();
    }

    public void RotateCamera(Vector2 rotatingVector)
    {
        m_xRotation += rotatingVector.x * cameraSensitivity * Time.deltaTime;
        m_yRotation -= rotatingVector.y * cameraSensitivity * Time.deltaTime;

        m_yRotation = Mathf.Clamp(m_yRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(m_yRotation, m_xRotation, 0f);
    }

    public void Move(Vector3 direction)
    {
        Vector3 forwardMovement = transform.forward * direction.z;
        forwardMovement.y = 0;
        Vector3 horizontalMovement = transform.right * direction.x;
        horizontalMovement.y = 0;
        Vector3 verticalMovement = Vector3.up * direction.y;

        Vector3 movement = Vector3.ClampMagnitude(forwardMovement + horizontalMovement + verticalMovement, 1);
        m_characterController.Move(movement * Time.deltaTime * speed);

        if(transform.position - m_previousPosition != Vector3.zero)
        {
            if (onPositionChange == null) return;
            onPositionChange.Invoke(transform.position);
            m_previousPosition = transform.position;
        }
    }

    
}
