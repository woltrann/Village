using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;

    [Header("References")]
    public Animator anim;
    public Transform target; // Oyuncu (Player)
    public UnityEngine.AI.NavMeshAgent agent;

    private bool isDead = false;
    private bool isAttacking = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (anim == null)
            anim = GetComponent<Animator>();

        if (agent == null)
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        agent.speed = moveSpeed;
    }

    void Update()
    {
        if (isDead || target == null)
            return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            if (!isAttacking)
                StartCoroutine(AttackRoutine());
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        if (isAttacking) return;

        anim.SetBool("isWalk", true);
        anim.SetBool("isAttack", false);
        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        agent.isStopped = true;
        anim.SetBool("isWalk", false);
        anim.SetBool("isAttack", true);

        // hasar zamaný (animasyona göre ayarlayabilirsin)
        yield return new WaitForSeconds(0.4f);

        if (target != null && !isDead)
        {
            // Oyuncuya hasar ver
            PlayerStats playerStats = target.GetComponent<PlayerStats>();
            if (playerStats != null)
                playerStats.TakeDamage(damage);
        }

        // Saldýrý gecikmesi
        yield return new WaitForSeconds(attackSpeed);
        isAttacking = false;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // Hasar animasyonu (isteðe baðlý)
        anim.SetTrigger("Hit");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        anim.SetBool("isWalk", false);
        anim.SetBool("isAttack", false);
        anim.SetBool("isDie", true);
        agent.isStopped = true;

        // Öldükten kýsa süre sonra yok et (ya da pool’a geri gönder)
        Destroy(gameObject, 1f);
    }

    // Dilersen debug/test amaçlý hasar denemesi:
    [ContextMenu("TestDamage")]
    void TestDamage()
    {
        TakeDamage(25f);
    }
}
