using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LorePageUI : MonoBehaviour
{
    public KeyCode dismissInput = KeyCode.Space;
    public Image noteImage;

    void Update()
    {
        if (Input.GetKeyDown(dismissInput)) {
            gameObject.SetActive(false);
        }   
    }
}
