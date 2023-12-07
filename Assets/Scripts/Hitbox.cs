using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public LayerMask excludedLayers;
    public int damage;

    public bool active = true;

    public float hitstop = 0.05f;

    public Hitstop hs;
    public GameObject hitEffect;

    // Start is called before the first frame update
    void Start()
    {
        hs = Hitstop.hitstop;
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
                
                if (active && !h.invincible) {
                    hs.Stop(hitstop);

                    if (hitEffect != null) {
                        GameObject hitGO = Instantiate(hitEffect);
                        hitGO.transform.position = transform.position;
                        hitGO.transform.position = col.contacts[0].point;
                    }

                    h.Damage(damage);
                }

                
                
            }
        }
    }
}
