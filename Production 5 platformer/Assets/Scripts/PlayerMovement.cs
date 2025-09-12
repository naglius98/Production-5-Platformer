using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float MovementSpeed = 5.0f;
    private float HorizontalMovement;

    void Update()
    {
        rb.linearVelocity = new Vector2(HorizontalMovement * MovementSpeed, rb.linearVelocity.y);
    }
    public void Move(InputAction.CallbackContext context)
    {
        HorizontalMovement = context.ReadValue<Vector2>().x;

    }
}
