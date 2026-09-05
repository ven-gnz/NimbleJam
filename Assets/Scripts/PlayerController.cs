using UnityEngine;
using UnityEngine.InputSystem;


public enum InteractionDirection
{
    Left,
    Right,
    Down,
    DownLeft,
    Downright
}

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    [SerializeField] Collider2D playerCollider;

    [SerializeField] float jumpImpulse = 5f;

    [SerializeField] private TerrainQueryHelper terrain;
    [SerializeField] private int maxDirt = 64;
    private int carriedDirt = 0;
    public int CarriedDirt => carriedDirt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        terrain = FindAnyObjectByType<TerrainQueryHelper>();
        playerCollider = GetComponent<Collider2D>();
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

       
        // o for digging
        if (Keyboard.current.oKey.isPressed)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                TryDig(InteractionDirection.Left);
            }
            else if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                TryDig(InteractionDirection.Right);
            }
            else if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                TryDig(InteractionDirection.Down);
            }
            return;
        }
        // p for placing dirt
        if(Keyboard.current.pKey.isPressed)
        {

            if(Keyboard.current.aKey.wasPressedThisFrame && Keyboard.current.sKey.wasPressedThisFrame)
            {
                TryPlaceBlock(InteractionDirection.Downright);
            }

            else if (Keyboard.current.dKey.wasPressedThisFrame && Keyboard.current.sKey.wasPressedThisFrame)
            {
                TryPlaceBlock(InteractionDirection.DownLeft);
            }

            else if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                TryPlaceBlock(InteractionDirection.Left);
            }
            else if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                TryPlaceBlock(InteractionDirection.Right);
            }
            else if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                TryPlaceBlock(InteractionDirection.Down);
            }
            return;
        }

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
            rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        }
    }

    private void TryDig(InteractionDirection direction)
    {
        
        
        Vector3Int targetCell = GetDigCell(direction);
        TerrainType terrainType =
            terrain.GetTerrain(targetCell);

        if (terrainType != TerrainType.Soft)
        {
            //Debug.Log("Dig failed.");
            return;
        }

        bool success = terrain.RemoveTerrain(targetCell);
        carriedDirt++;
        //Debug.Log(
        //    success
        //        ? "Dig successful!"
        //        : "Dig failed.");

    }



    private Vector3Int GetDigCell(InteractionDirection direction)
    {

        Vector3 feetPosition;
        Vector3Int playerCell;

        switch (direction)
        {
            case InteractionDirection.Left:
                feetPosition = transform.position + Vector3.down * -0.5f;
                playerCell = terrain.WorldToCell(feetPosition);
                return playerCell + Vector3Int.left;

            case InteractionDirection.Right:
                feetPosition = transform.position + Vector3.down * -0.5f;
                playerCell =
            terrain.WorldToCell(feetPosition);
                return playerCell + Vector3Int.right;

            case InteractionDirection.Down:
                feetPosition = transform.position + Vector3.down * 0.5f;
                playerCell = terrain.WorldToCell(feetPosition);
                return playerCell;
        }

        return new Vector3Int(0,0,0);
    }

    private void TryPlaceBlock(InteractionDirection direction)
    {
        // check whether the tile neighbors existing terrain
        
        Vector3Int targetCell = GetTargetCell(direction);
        if(carriedDirt > 0)
        {
            bool success = terrain.RequestTerrain(targetCell);
        }
       
       
    }

    private Vector3Int GetTargetCell(InteractionDirection direction)
    {
        Vector3 position;
        Vector3Int playerCell;

        switch (direction)
        {
            case InteractionDirection.DownLeft:
                //try, if the 
                position = transform.position + Vector3.down * 0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.left;
            case InteractionDirection.Downright:
                position = transform.position + Vector3.down * 0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.right;
            case InteractionDirection.Left:
                position = transform.position + Vector3.down * -0.5f;
                playerCell = terrain.WorldToCell(position);
                return playerCell + Vector3Int.left;

            case InteractionDirection.Right:
                position = transform.position + Vector3.down * -0.5f;
                playerCell =
            terrain.WorldToCell(position);
                return playerCell + Vector3Int.right;

        }

        position = transform.position + Vector3.down * 0.5f;
        playerCell = terrain.WorldToCell(position);
        return playerCell;
    }
}
