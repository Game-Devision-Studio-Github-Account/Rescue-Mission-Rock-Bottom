using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    public GameObject vcam;

    public UnityEvent onEnter;
    public UnityEvent onExit;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger) {
            vcam.SetActive(true);
            onEnter.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
       if(other.CompareTag("Player") && !other.isTrigger)  {
            vcam.SetActive(false);
            onExit.Invoke();
       }
    }
}
