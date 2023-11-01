using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Collectible : MonoBehaviour
{
    public float collectionRange;
    public float approachTime = 0.5f;
    float time = 0;
    float startDist;
    bool approaching = false;
    public bool destroyOnCollect = true;
    public UnityEvent onCollect;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = PlayerMovement.PlayerGO.transform.position;
        if (!approaching) {
            float dist = (playerPos - transform.position).magnitude;
            if (dist <= collectionRange) {
                approaching = true;
                startDist = dist;
            } 
        } else {
            Vector3 offset = transform.position - playerPos;
            offset.Normalize();
            offset *= startDist;
            offset *= 1 - (time / approachTime);
            transform.position = playerPos + offset;

            time += Time.fixedDeltaTime;
        }
    }

    void FixedUpdate()
    {
        
        
    }

    public void Collect() {
        onCollect.Invoke();

        if (destroyOnCollect) Destroy(gameObject);
    }
}
