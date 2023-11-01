using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 direction;
    public float force;

    public GameObject breakParticles;

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.right = rb.velocity;
    }

    void OnCollisionEnter2D(Collision2D col) {

        if (breakParticles != null) {
            GameObject breakGO = Instantiate(breakParticles);
            breakGO.transform.position = transform.position;
        }
        


        Destroy(gameObject);
    }
}
