using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Collectible : MonoBehaviour
{
    public float collectionRange;
    public bool destroyOnCollect = true;
    public UnityEvent onCollect;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Collect() {
        onCollect.Invoke();

        if (destroyOnCollect) Destroy(gameObject);
    }
}
