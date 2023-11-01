using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTools : MonoBehaviour
{
    public enum Alignment {
        Right,
        Left
    }

    public Alignment initialAlignment = Alignment.Right;
    public bool flipped;
    // Start is called before the first frame update
    void Start()
    {

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
}
