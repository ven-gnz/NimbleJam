using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;

    [SerializeField] float jumpImpulse = 1.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float input = 0f;

        if (Keyboard.current.aKey.isPressed)
            input -= 1f;

        if (Keyboard.current.dKey.isPressed)
            input += 1f;

        rb.linearVelocity = new Vector2(
            input * moveSpeed,
            rb.linearVelocity.y
        );

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Jump!");
            rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {

    }
}
