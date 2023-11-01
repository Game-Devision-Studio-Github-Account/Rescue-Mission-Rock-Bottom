using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite fullHeartImage;
    public Sprite halfHeartImage;
    public Sprite emptyHeartImage;

    void Update()
    {
        for (int i = 0; i < transform.childCount; i++) {
            //Destroy(transform.GetChild(i).gameObject);
            transform.GetChild(i).rotation = Quaternion.identity;
        }
    }

    public void UpdateHearts(int health, int maxHealth)
    {
        int mh = (int)Mathf.Ceil(((float)maxHealth)/2);
        float h = ((float)health) / 2;
        h = h - 1;
        
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }

        Image[] sprites = new Image[mh];

        for (int i = 0; i < mh; i ++) {
            GameObject heart = new GameObject("Heart", typeof(Image));
            //heart.transform.parent = transform;
            heart.transform.SetParent(transform, false);

            Sprite s = fullHeartImage;
            if (h < i) s = emptyHeartImage;
            if ((h - i) == -0.5f) s = halfHeartImage;

            heart.GetComponent<Image>().sprite = s;
        }
    }
}
