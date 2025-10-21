using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PhysicRotator : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed;

    void Start()
    {
        InputHolder.inst.actions.Player.Look.performed += Rotate;
    }
    void Rotate(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        Vector3 rot = new Vector3(input.y, input.x, 0);
        rb.AddRelativeTorque(rot);
    }
}