using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    [Header("Data")]
    public bool gameOver = false;

    void Awake() {
        if (gameManager == null) {
            gameManager = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }   
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndGame() {
        Debug.Log("bazinger");
    }
}
