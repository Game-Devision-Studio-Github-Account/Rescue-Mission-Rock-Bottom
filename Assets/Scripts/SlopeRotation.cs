using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeRotation : MonoBehaviour
{
    public Rigidbody2D rb;
    [Range(0.0f, 1.0f)]
    public float rotationLerp = 0.2f;

    void OnEnable()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotateTowards(Quaternion targetRotation) {
        rb.SetRotation(Quaternion.Lerp(transform.rotation, targetRotation, rotationLerp));
    }

    public void RotateTowards(float angle) {
        rb.SetRotation(Mathf.Lerp(transform.rotation.eulerAngles.z, angle, rotationLerp));
    }

    public void RotateFloor(Vector3 groundNormal) {
        RotateTowards(AltFromToRotation(Vector2.up, groundNormal, Quaternion.Euler(new Vector3(0, 0, 180))));
    }

    static public Quaternion AltFromToRotation(Vector3 dir1, Vector3 dir2,
        Quaternion whenOppositeVectors = default(Quaternion)) {
 
        float r = 1f + Vector3.Dot(dir1, dir2);
        
        if(r < 1E-6f) {
            if(whenOppositeVectors == default(Quaternion)) {
            // simply get the default behavior
            return Quaternion.FromToRotation(dir1, dir2);
            }
            return whenOppositeVectors;
        }
        
        Vector3 w = Vector3.Cross(dir1, dir2);
        return new Quaternion(w.x, w.y, w.z, r).normalized;
    }
}
