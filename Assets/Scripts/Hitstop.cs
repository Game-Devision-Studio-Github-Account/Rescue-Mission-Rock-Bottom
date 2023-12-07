using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitstop : MonoBehaviour
{
    public static Hitstop hitstop;
    
    bool waiting;

    void Awake() {
        if (hitstop == null) {
            hitstop = this;
            //DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }   
    }

    public void Stop(float duration) {
        if(waiting) return;
        Time.timeScale = 0.0f;
        StartCoroutine(Wait(duration));
    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;
    }
}
