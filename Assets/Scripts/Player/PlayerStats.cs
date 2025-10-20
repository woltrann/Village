using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    public Animator animator;
    public bool isDead = false;

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
    public int totalExpGained = 0; // 🔥 Toplam exp biriktirme değişkeni
    public TextMeshProUGUI expText;
    public TextMeshProUGUI currentLevelText;
    //public Slider expBar;

    [Header("Gold / Currency")]
    public int gold = 0;
    public TextMeshProUGUI goldText;

    [Header("End Panel (Toplam Exp Göstergesi)")]
    public GameObject endPanel;            // 🔥 Oyun sonunda açılacak panel
    public TextMeshProUGUI totalExpText;   // 🔥 Toplam exp texti
    public Slider totalExpSlider;          // 🔥 Toplam exp slider'ı

    private void Awake()
    {
        Instance = this;
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        goldText.text = "Altın: " + gold.ToString();
    }

    // ------------------ CAN SİSTEMİ ------------------
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

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("isAttacking", false);
        animator.SetTrigger("isDead");

        if (endPanel != null)        // 🔥 Oyunun sonunda toplam exp panelini göster
        {
            endPanel.SetActive(true);
            totalExpText.text = "Toplam EXP: " + totalExpGained.ToString();

             totalExpSlider.maxValue = expToNextLevel;
             totalExpSlider.value = 0;
            
            StartCoroutine(FillTotalExpSmooth());            // 🔥 Exp dolum animasyonunu başlat
            EnemyWaveManager.Instance.waveIndex = 0;
        }
    }

    // ------------------ EXP SİSTEMİ ------------------
    public void GainExp(int amount)
    {
        totalExpGained += amount; // 🔥 Toplam exp biriktir
        currentExp += amount;
        UpdateExpUI();

    }

    void LevelUp()
    {
        level++;
        
        expToNextLevel += 50; // Her seviye için daha fazla exp
        expText.text = $"{currentExp}/{expToNextLevel}";
        //maxHealth += 10;
        attackDamage += 2;
        moveSpeed += 0.1f;
        //currentHealth = maxHealth;
        Debug.Log("Level Up! Yeni Seviye: " + level);
        if (currentLevelText != null)
            currentLevelText.text = "Lvl " + level;
    }

    void UpdateExpUI()
    {
        if (expText != null)
            expText.text = $"{currentExp}/{expToNextLevel}";
    }
    private IEnumerator FillTotalExpSmooth()
    {
        int remainingExp = totalExpGained;
        int currentLevelExp = 0;
        float fillDurationPerLevel = 1.2f; // her level barı ne kadar sürede dolsun
        float fillProgress = 0f;

        while (remainingExp > 0)
        {
            int expNeeded = expToNextLevel - currentLevelExp;
            int expToAdd = Mathf.Min(expNeeded, remainingExp);

            float startValue = totalExpSlider.value;
            float endValue = expToAdd;
            fillProgress = 0f;

            // 🔥 Bu level'ın exp'ini pürüzsüz doldur
            while (fillProgress < 1f)
            {
                fillProgress += Time.deltaTime / fillDurationPerLevel;
                totalExpSlider.value = Mathf.Lerp(startValue, expToAdd, fillProgress);
                yield return null;
            }

            // 🔹 Level doldu mu?
            currentLevelExp += expToAdd;
            remainingExp -= expToAdd;

            if (currentLevelExp >= expToNextLevel)
            {
                currentExp-=expToNextLevel;
                LevelUp();
                currentLevelExp = 0;
                totalExpSlider.value = 0;

            }
        }

        Debug.Log("✅ Tüm exp dolumu tamamlandı!");
    }

    // ------------------ ALTIN SİSTEMİ ------------------
    public void AddGold(int amount)
    {
        gold += amount;
        goldText.text = "Altın: " + gold.ToString();
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            goldText.text = "Altın: " + gold.ToString();
            return true;
        }
        return false;
    }
}
