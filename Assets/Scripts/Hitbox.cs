using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public LayerMask excludedLayers;
    public int damage;

    public bool active = true;

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
            Health h = col.collider.GetComponent<Health>();
            if (h != null) {
                Debug.Log(col.collider.gameObject.name);
                if (active) h.Damage(damage);
            }
        }
    }
}
