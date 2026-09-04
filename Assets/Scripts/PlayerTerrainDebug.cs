using UnityEngine;

public class PlayerTerrainDebug : MonoBehaviour
{

    [SerializeField] private TerrainQueryHelper terrain;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3Int cell =
        //    terrain.WorldToCell(transform.position);

        //TerrainType type =
        //    terrain.GetTerrain(cell);

        //Debug.Log(type);


    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //foreach (var contact in collision.contacts)
        //{
        //    if (collision.collider.name == "Terrain_-3_-2")
        //    {
        //        Debug.Log(
        //            $"CONTACT {contact.point} " +
        //            $"normal={contact.normal}\n" +
        //            $"bounds min={collision.collider.bounds.min} " +
        //            $"max={collision.collider.bounds.max} " +
        //            $"center={collision.collider.bounds.center} " +
        //            $"size={collision.collider.bounds.size}"
        //        );
        //    }
        //}
    }
}
