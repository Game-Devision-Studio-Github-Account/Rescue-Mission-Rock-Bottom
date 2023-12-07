using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTools : MonoBehaviour
{
    public enum Alignment {
        Right,
        Left
    }
    
    public SpriteRenderer sprite;

    public Alignment initialAlignment = Alignment.Right;
    public bool flipped;
    bool waiting;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        int a = flipped ? -1 : 1;
        int b = (initialAlignment == Alignment.Right) ? 1 : -1;

        transform.localScale = new Vector2(
            a * b,
            transform.localScale.y
        );
    }

    public void Flash(Color color, float duration) {
        
        if (waiting) return;
        
        Color originalColor = sprite.color;
        sprite.color = color;

        StartCoroutine(FlashRoutine(originalColor, duration));
        
    }

    IEnumerator FlashRoutine(Color color, float duration)
    {
        waiting = true;
        Debug.Log("bazing");
        yield return new WaitForSecondsRealtime(duration);
        sprite.color = color;
        Debug.Log("a");
        waiting = false;
    }
}