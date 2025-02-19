using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 movement;

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    void Update()
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y) * speed * Time.deltaTime;
        transform.Translate(move);
    }
}   
