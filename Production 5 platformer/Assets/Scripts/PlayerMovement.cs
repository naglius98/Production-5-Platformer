using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Movement")]
    public float MovementSpeed = 5.0f;
    private float HorizontalMovement;

    [Header("Jumping")] 
    public float JumpPower = 10.0f;

    void Update()
    {
        rb.linearVelocity = new Vector2(HorizontalMovement * MovementSpeed, rb.linearVelocity.y);
    }


    public void Move(InputAction.CallbackContext context)
    {
        HorizontalMovement = context.ReadValue<Vector2>().x;

    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpPower);
        }
        else if (context.canceled)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }
}
