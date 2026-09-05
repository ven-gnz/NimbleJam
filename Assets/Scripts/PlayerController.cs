using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public enum PlacementDirections
{
    Left,
    Right,
    Down,
    DownLeft,
    Downright
}

public enum DigDirections
{
    Topleft,
    Top,
    TopRight,
    Left,
    Right,
    DownLeft,
    Down,
    DownRight
}


public class PlayerController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    [SerializeField] Collider2D playerCollider;

    [SerializeField] float coyoteTime = 0.4f;

    [SerializeField] float jumpImpulse = 10f;
    [SerializeField] private LayerMask groundLayer;
    private int groundContacts;
    private bool IsGrounded => groundContacts > 0;
    [SerializeField] public PlayerUI playerUI;

    [SerializeField] float jumpBufferTime = 1.0f;

    private float jumpBufferTimer;

    private bool touchingLeftWall;
    private bool touchingRightWall;

    [SerializeField] private TerrainQueryHelper terrain;
    private int carriedDirt = 0;
    public int CarriedDirt => carriedDirt;

    public bool isDigging;
    private float coyoteTimer;
    private bool isGrounded;
    private bool canJump;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        terrain = FindAnyObjectByType<TerrainQueryHelper>();
        playerCollider = GetComponent<Collider2D>();
        playerUI = GetComponent<PlayerUI>();
    }


    void Start()
    {
        isDigging = false;
    }

    // Update is called once per frame
    void Update()
    {

       
        // o for digging
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            if (isDigging) return;


            var keyboard = Keyboard.current;

            // Diagonals
            if (keyboard.sKey.isPressed && keyboard.dKey.isPressed)
            {
                TryDig(DigDirections.DownRight);
                return;
            }

            else if (keyboard.sKey.isPressed && keyboard.aKey.isPressed)
            {
                TryDig(DigDirections.DownLeft);
                return;
            }

            else if (keyboard.wKey.isPressed && keyboard.aKey.isPressed)
            {
                TryDig(DigDirections.Topleft);
                return;
            }

            else if (keyboard.wKey.isPressed && keyboard.dKey.isPressed)
            {
                TryDig(DigDirections.TopRight);
                return;
            }


            else if (Keyboard.current.wKey.isPressed)
            {
                TryDig(DigDirections.Top);
                return;
            }

            else if (Keyboard.current.aKey.isPressed)
            {
                TryDig(DigDirections.Left);
                return;
            }
            else if (Keyboard.current.dKey.isPressed)
            {
                TryDig(DigDirections.Right);
                return;
            }
            else if (Keyboard.current.sKey.isPressed)
            {
                TryDig(DigDirections.Down);
                return;
            }

            return;
        }

        // p for placing dirt
        if(Keyboard.current.pKey.wasPressedThisFrame)
        {

            if(Keyboard.current.aKey.isPressed && Keyboard.current.sKey.isPressed)
            {
                TryPlaceBlock(PlacementDirections.DownLeft);
                return;
            }

            else if (Keyboard.current.dKey.isPressed && Keyboard.current.sKey.isPressed)
            {
                TryPlaceBlock(PlacementDirections.Downright);
                return;
            }

            else if (Keyboard.current.aKey.isPressed)
            {
                TryPlaceBlock(PlacementDirections.Left);
                return;
            }
            else if (Keyboard.current.dKey.isPressed)
            {
                TryPlaceBlock(PlacementDirections.Right);
                return;
            }
            else if (Keyboard.current.sKey.isPressed)
            {
                TryPlaceBlock(PlacementDirections.Down);
                return;
            }
            return;
        }

        float input = 0f;

        if (Keyboard.current.aKey.isPressed)
            input -= 1f;

        if (Keyboard.current.dKey.isPressed)
            input += 1f;

        if (touchingLeftWall && input < 0f)
            input = 0f;

        if (touchingRightWall && input > 0f)
            input = 0f;

        rb.linearVelocity = new Vector2(
            input * moveSpeed,
            rb.linearVelocity.y
        );

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;


        if (jumpBufferTimer > 0f &&
            coyoteTimer > 0f)
        {
            Jump();

            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        //Debug.Log($"Collision with: {collision.gameObject.name}");

        foreach (ContactPoint2D contact in collision.contacts)
        {
            //Debug.Log($"Normal: {contact.normal}");
        }

        //if ((groundLayer.value & (1 << collision.gameObject.layer)) == 0)
        //    return;

        //foreach (ContactPoint2D contact in collision.contacts)
        //{
        //    if (contact.normal.y > 0.5f)
        //    {
        //        groundContacts++;
        //        return;
        //    }
        //}
    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
               
            }
            if (contact.normal.x > 0.5f)
                touchingLeftWall = true;

            if (contact.normal.x < -0.5f)
                touchingRightWall = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //if ((groundLayer.value & (1 << collision.gameObject.layer)) == 0)
        //    return;

        //foreach (ContactPoint2D contact in collision.contacts)
        //{
        //    if (contact.normal.y > 0.5f)
        //    {
        //        groundContacts = Mathf.Max(0, groundContacts - 1);
        //        return;
        //    }
        //}

        isGrounded = false;
        touchingLeftWall = false;
        touchingRightWall = false;
    }

    private void TryDig(DigDirections direction)
    {
        //Debug.Log("starting to dig! " + isDigging);
        isDigging = true;
        
        Vector3Int targetCell = GetDigCell(direction);
        TerrainType terrainType =
            terrain.GetTerrain(targetCell);

        if (terrainType == TerrainType.Empty || terrainType == TerrainType.Rock)
        {
            //Debug.Log("Dig failed.");
            isDigging = false;
            return;
        }

        bool success = terrain.RemoveTerrain(targetCell);
        if(terrainType == TerrainType.Dirt)
        {
            carriedDirt++;
            playerUI.UpdateDirtCount(carriedDirt);
        }
        
        //Debug.Log(
        //    success
        //        ? "Dig successful!"
        //        : "Dig failed.");

        StartCoroutine(HandleDigDelay());
        
        //Debug.Log("End dig routine " + isDigging);
    }


    // 0.5f is a sort of magic offset, since the player collider is not exactly the size of one tile.
    // also there are some bugs in here when going directly down, or at least it assumes too strict of a landing on exactly
    // on the center
    private Vector3Int GetDigCell(DigDirections direction)
    {

        Vector3 position;
        Vector3Int playerCell;

        switch (direction)
        {
   

            case DigDirections.Topleft:
                position = transform.position + Vector3.up * 0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.up + Vector3Int.left;

            case DigDirections.TopRight:
                position = transform.position + Vector3.up * 0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.up + Vector3Int.right;

            case DigDirections.DownLeft:
                position = transform.position + Vector3.down * 0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.left;

            case DigDirections.DownRight:
                position = transform.position + Vector3.down * 0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.right;


            case DigDirections.Top:
                position = transform.position + Vector3.up * 0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.up;


            case DigDirections.Left:
                position = transform.position + Vector3.up * 0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.left;

            case DigDirections.Right:
                position = transform.position + Vector3.up * 0.5f;
                playerCell =
                    terrain.WorldToCell(position);
                return playerCell + Vector3Int.right;

 

            case DigDirections.Down:
                position = transform.position + Vector3.down * 0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell;
        }

        return new Vector3Int(0,0,0);
    }

    private void TryPlaceBlock(PlacementDirections direction)
    {
        // check whether the tile neighbors existing terrain
        
        Vector3Int targetCell = GetTargetCell(direction);
        bool success = false;
        if(carriedDirt > 0)
        {
            success = terrain.RequestTerrain(targetCell);
        }

        if (success)
        {
            --carriedDirt;
            playerUI.UpdateDirtCount(carriedDirt);
        }
        


    }
    
    private IEnumerator HandleDigDelay()
    {
        yield return new WaitForSeconds(1f);
        isDigging = false;
    }

    private Vector3Int GetTargetCell(PlacementDirections direction)
    {
        Vector3 position;
        Vector3Int playerCell;

        switch (direction)
        {
            case PlacementDirections.DownLeft:
               
                position = transform.position + Vector3.down * 0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.left;

            case PlacementDirections.Downright:
                position = transform.position + Vector3.down * 0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.right;

            case PlacementDirections.Left:
                position = transform.position + Vector3.down * -0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.left;

            case PlacementDirections.Right:
                position = transform.position + Vector3.down * -0.5f;
                playerCell =
            terrain.WorldToCell(position);
                return playerCell + Vector3Int.right;
            case PlacementDirections.Down:
                position = transform.position + Vector3.down * 0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.down;
        }

        position = transform.position + Vector3.down * 0.5f;
        playerCell = terrain.WorldToCell(position);
        return playerCell;
    }
}
