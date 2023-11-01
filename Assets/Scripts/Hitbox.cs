using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public LayerMask excludedLayers;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!(excludedLayers == (excludedLayers | 1 << col.collider.gameObject.layer))) {
            Health h = col.otherCollider.GetComponent<Health>();
            if (h != null) {
                h.Damage(damage);
            }
        }
    }
}
