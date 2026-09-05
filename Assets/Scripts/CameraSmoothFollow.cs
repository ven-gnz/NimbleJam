using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    public Transform target; 
    public float smoothing = 5f;

    private Vector3 offset; 

    void Start()
    {
        
        offset = transform.position - target.position + Vector3.up * 2.5f;
    }

    void FixedUpdate()
    {
        
        Vector3 targetCamPos = target.position + offset;

        
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}
