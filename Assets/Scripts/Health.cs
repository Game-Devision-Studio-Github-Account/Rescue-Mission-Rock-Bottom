using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int health;
    public int maxHealth;

    [Header("Events")]
    public UnityEvent onEnable;
    public UnityEvent onDamage;
    public UnityEvent onDeath;

    void Awake()
    {
        maxHealth = health;
    }

    void OnEnable()
    {
        onEnable.Invoke();
    }

    public int Damage(int d) {
        health -= d;

        onDamage.Invoke();

        if (health <= 0) {
            onDeath.Invoke();
        }

        return health;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) {
            Damage(1);
        }
    }
}
