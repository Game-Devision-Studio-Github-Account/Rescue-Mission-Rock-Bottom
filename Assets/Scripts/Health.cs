using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int health;
    [HideInInspector]
    public int maxHealth;
    [HideInInspector]
    public bool invincible = false;

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
        if (invincible) return health;

        health -= d;

        onDamage.Invoke();

        if (health <= 0) {
            onDeath.Invoke();
        }

        return health;
    }
}
