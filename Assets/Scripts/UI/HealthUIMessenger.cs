using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUIMessenger : MonoBehaviour
{
    // Start is called before the first frame update
    public Health health;
    public HealthUI healthUI;

    void Awake()
    {
        if (health == null) health = GetComponent<Health>();
        if (healthUI == null) healthUI = FindObjectOfType<HealthUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealthUI() {
        healthUI.UpdateHearts(health.health, health.maxHealth);
    }
}
