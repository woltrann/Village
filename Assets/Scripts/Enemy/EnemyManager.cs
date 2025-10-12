using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("References")]
    public Transform player;
    public Animator animator;
    public Rigidbody rb;

    [Header("Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float attackDamage = 10f; // Player'a vereceği hasar

    public float maxHealth = 50f;     // Düşman canı
    public float currentHealth;

    [Header("Health Bar")]
    public Slider healthBar;
    public Transform healthBarCanvas;
    private Quaternion healthBarFixedRotation;

    private bool isWalking = true;
    private bool isAttacking = false;
    private bool isDead = false;


    private void Awake()
    {
        Instance = this;
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
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (PlayerStats.Instance.currentHealth > 0)
        {
            if (isDead || player == null) return;

            // Düşman trigger içindeyse saldır, değilse yürü
            if (isWalking)
            {
                Vector3 moveDirection = (player.position - transform.position).normalized;
                moveDirection.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

                animator.SetBool("isWalk", true);
                animator.SetBool("isAttacking", false);
            }
            else if (isAttacking)
            {
                Vector3 moveDirection = (player.position - transform.position).normalized;
                moveDirection.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

                animator.SetBool("isWalk", false);
                animator.SetBool("isAttacking", true);

            }
        }
        else
        {
            animator.SetBool("isWalk", false);
            animator.SetBool("isAttacking", false);
            animator.SetTrigger("Exit");

        }
        
    }

    void FixedUpdate()
    {
        if (isWalking)
        {
            Vector3 moveDirection = (player.position - transform.position).normalized;
            moveDirection.y = 0;
            Vector3 newPos = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            newPos.y = rb.position.y;
            rb.MovePosition(newPos);
        }
    }

    void LateUpdate()
    {
        if (healthBarCanvas != null)
            healthBarCanvas.rotation = healthBarFixedRotation;
    }

    // Düşman trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isWalking = false;
            isAttacking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isWalking = true;
            isAttacking = false;
        }
    }
    //  Animasyon eventinden çağrılacak
    public void DealDamageToPlayer()
    {
        if (isDead) return;
        if (PlayerStats.Instance.currentHealth <= 0) return;


        PlayerStats.Instance.TakeDamage(attackDamage);
        
    }

    // Player saldırısı animasyon eventinden çağrılacak
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (healthBar != null) healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        PlayerMovement player = Object.FindAnyObjectByType<PlayerMovement>();
        if (player != null)
            player.RemoveEnemyFromList(this);

        animator.SetTrigger("isDead");
        rb.isKinematic = true;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        Destroy(gameObject, 2f);
    }
}
