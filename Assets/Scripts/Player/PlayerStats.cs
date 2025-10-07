using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Base Stats")]
    public int level = 1;
    public int maxHealth = 100;
    public int currentHealth;
    public int attackDamage = 10;
    public float moveSpeed = 5f;
    public float attackSpeed = 1f;

    [Header("Experience System")]
    public int currentExp = 0;
    public int expToNextLevel = 100;

    [Header("Gold / Currency")]
    public int gold = 0;

    private void Awake()
    {
        Instance = this;
        currentHealth = maxHealth;        
    }

    // CAN S�STEM�-----------------------
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    void Die()
    {
        Debug.Log("Player �ld�!"); 
    }

    
    // DENEY�M & LEVEL UP-----------------------
    public void GainExp(int amount)
    {
        currentExp += amount;

        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentExp = 0;
        expToNextLevel += 50; // her level i�in daha fazla exp gereksinimi

        // Statlar� biraz artt�ral�m
        maxHealth += 10;
        attackDamage += 2;
        moveSpeed += 0.1f;

        currentHealth = maxHealth; // Level atlay�nca full can
        Debug.Log("Level Up! Yeni seviye: " + level);
    }


    // ALTIN S�STEM�----------------------------
    public void AddGold(int amount)
    {
        gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }
}
