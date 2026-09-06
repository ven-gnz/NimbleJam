using UnityEngine;

public class ParallaxScroll : MonoBehaviour
{
    public Transform player;
    public float parallaxStrength = 0.3f;

    private float startX;
    private float playerStartX;

    void Start()
    {
        startX = transform.position.x;
        playerStartX = player.position.x;
    }

    void LateUpdate()
    {
        float playerDelta = player.position.x - playerStartX;

        transform.position = new Vector3(
            startX + playerDelta * parallaxStrength,
            transform.position.y,
            transform.position.z
        );
    }
}
