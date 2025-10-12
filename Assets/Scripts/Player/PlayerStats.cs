using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    public Animator animator;
    public bool isDead=false;

    [Header("Base Stats")]
    public int level = 1;
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar;
    public float attackDamage = 10f;
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
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth ;
    }

    // CAN SÝSTEMÝ-----------------------
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth ; 
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
        animator.SetBool("isAttacking", false);
        animator.SetTrigger("isDead"); 
    }

    
    // DENEYÝM & LEVEL UP-----------------------
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
        expToNextLevel += 50; // her level için daha fazla exp gereksinimi

        // Statlarý biraz arttýralým
        maxHealth += 10;
        attackDamage += 2;
        moveSpeed += 0.1f;

        currentHealth = maxHealth; // Level atlayýnca full can
        Debug.Log("Level Up! Yeni seviye: " + level);
    }


    // ALTIN SÝSTEMÝ----------------------------
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
