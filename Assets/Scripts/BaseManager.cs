using UnityEngine;
using UnityEngine.UI;

public class BaseManager : MonoBehaviour
{
    public static BaseManager Instance;

    [Header("Base Stats")]
    public int level = 1;
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar;
    public bool isDead = false;


    private void Awake()
    {
        Instance = this;
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }



    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    void Die()
    {
        isDead = true;
        
    }
}
