using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementTest : MonoBehaviour
{
    [SerializeField]
    public ElementTestSet[] elementTestSets;

    PlayerMovement pm;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(ElementTestSet e in elementTestSets) {
            if(Input.GetKeyDown(e.input)) pm.SetElement(e.element);
        }
    }
}

[System.Serializable]
public class ElementTestSet {
    public SlimeElement element;
    public KeyCode input;
}
