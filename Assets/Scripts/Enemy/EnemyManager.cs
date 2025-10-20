using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public event System.Action<EnemyManager> OnEnemyDied;

    [Header("References")]
    public Transform player;
    public Transform mainTarget; // Ana kule
    public Animator animator;
    public Rigidbody rb;

    [Header("Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float attackDamage = 10f;
    public float detectionRadius = 8f; // 🔥 Oyuncuyu algılama yarıçapı
    public float attackRange = 2f;     // 🔥 Saldırı mesafesi

    public float maxHealth = 50f;
    private float currentHealth;

    public int takeGold;
    public int takeExp;

    [Header("Health Bar")]
    public Slider healthBar;
    public Transform healthBarCanvas;
    private Quaternion healthBarFixedRotation;

    private bool isWalking = true;
    private bool isAttacking = false;
    private bool isDead = false;

    private Transform currentTarget; // 🔥 Anlık hedef (player veya kule)

    private void Awake()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (healthBarCanvas != null)
            healthBarFixedRotation = healthBarCanvas.rotation;
    }

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (mainTarget == null)
            mainTarget = GameObject.FindGameObjectWithTag("Main")?.transform;

        currentTarget = mainTarget; // 🔥 Başlangıçta kuleye yönel
    }

    void Update()
    {
        if (isDead || currentTarget == null) return;

        // 🔥 Önce oyuncunun menzilde olup olmadığını kontrol et
        float playerDist = Vector3.Distance(transform.position, player.position);
        if (playerDist <= detectionRadius)
            currentTarget = player; // Oyuncuya yönel
        else
            currentTarget = mainTarget; // Kuleye dön

        // 🔥 Hedefe doğru hareket et veya saldır
        float distToTarget = Vector3.Distance(transform.position, currentTarget.position);
        if (distToTarget > attackRange)
        {
            isWalking = true;
            isAttacking = false;
        }
        else
        {
            isWalking = false;
            isAttacking = true;
        }

        // 🔥 Animasyon kontrolü
        animator.SetBool("isWalk", isWalking);
        animator.SetBool("isAttacking", isAttacking);
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (isWalking && currentTarget != null)
        {
            Vector3 dir = (currentTarget.position - transform.position).normalized;
            dir.y = 0;
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
            FaceTarget();
        }
        else if (isAttacking && currentTarget != null)
        {
            FaceTarget();
        }
    }

    void LateUpdate()
    {
        if (healthBarCanvas != null)
            healthBarCanvas.rotation = healthBarFixedRotation;
    }

    private void FaceTarget()
    {
        Vector3 dir = (currentTarget.position - transform.position).normalized;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    public void DealDamageToPlayer()
    {
        if (isDead) return;

        // 🔥 Eğer hedef kuleyse ona zarar verme, oyuncuya zarar ver
        if (currentTarget.CompareTag("Player"))
        {
            if (PlayerStats.Instance.currentHealth > 0)
                PlayerStats.Instance.TakeDamage(attackDamage);
        }
        else if (currentTarget.CompareTag("Main"))
        {
            if (BaseManager.Instance.currentHealth > 0)
                BaseManager.Instance.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("isDead");
        rb.isKinematic = true;

        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
            col.enabled = false;

        if (PlayerMovement.Instance != null)
            PlayerMovement.Instance.RemoveEnemyFromList(this);

        OnEnemyDied?.Invoke(this);

        PlayerStats.Instance.AddGold(takeGold);
        PlayerStats.Instance.GainExp(takeExp);

        Destroy(gameObject, 2f);
    }
}
