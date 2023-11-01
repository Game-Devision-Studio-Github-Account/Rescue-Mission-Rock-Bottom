using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    [Header("References - UI")]
    public LorePageUI lorePageUI;
    public GameObject gameOverUI;
    [Header("Input")]
    public KeyCode restartInput = KeyCode.Space;
    [Header("Game Data")]
    public static bool gameOver = false;
    [Header("Game Files")]
    public Sprite[] loreNoteImages;

    void Awake() {
        if (gameManager == null) {
            gameManager = this;
            //DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }   
    }
    
    void OnEnable()
    {
        if (lorePageUI == null) lorePageUI = FindObjectOfType<LorePageUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        gameOverUI.SetActive(gameOver);

        if (gameOver) {
            if (Input.GetKeyDown(restartInput)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public void EndGame() {
        gameOver = true;
    }

    public void ShowLoreNote() {
        int a = (int)Random.Range(0, loreNoteImages.Length);
        lorePageUI.noteImage.sprite = loreNoteImages[a];
        lorePageUI.gameObject.SetActive(true);
    }
}
