using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    [Header("References")]
    public FloatingJoystick joystick;
    public Animator animator;
    public Rigidbody rb;

    [Header("Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float attackDamage = 10f;
    public float attackInterval = 1f;

    [Header("Health Bar Settings")]
    public Transform healthBarCanvas;
    private Quaternion healthBarFixedRotation;

    [Header("Camera Follow")]
    public Transform cameraTransform; // Kamerayı buraya atayacağız
    public Vector3 cameraOffset = new Vector3(0, 10f, -8f);
    private Quaternion cameraFixedRotation;

    private Vector3 moveDirection;
    private float attackTimer = 0f;

    // Saldırı ve düşmanlar
    public bool IsAttacking = false;
    private List<EnemyManager> enemiesInRange = new List<EnemyManager>();
    private EnemyManager nearestEnemy;

    private void Awake()
    {
        Instance = this;

        if (healthBarCanvas != null)
            healthBarFixedRotation = healthBarCanvas.rotation;

        // Kameranın ilk rotasyonunu kaydet
        if (cameraTransform != null)
            cameraFixedRotation = cameraTransform.rotation;
    }

    void Update()
    {
        if (PlayerStats.Instance.currentHealth <= 0) return;

        // ----------------- Hareket -----------------
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        float speed = moveDirection.magnitude;
        animator.SetFloat("Speed", speed);

        // ----------------- En yakın düşmana yönelme -----------------
        if (enemiesInRange.Count > 0)
        {
            nearestEnemy = GetNearestEnemy();

            if (nearestEnemy != null)
            {
                // Yönünü düşmana çevir
                Vector3 direction = (nearestEnemy.transform.position - transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                }

                // Saldırı animasyonu aktif
                animator.SetBool("isAttacking", true);
                IsAttacking = true;
            }
        }
        else
        {
            // Düşman yok → joystick yönüne bak
            IsAttacking = false;
            nearestEnemy = null;
            attackTimer = 0f;
            animator.SetBool("isAttacking", false);

            if (moveDirection.magnitude >= 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // ----------------- Kamera Takibi -----------------
        if (cameraTransform != null)
        {
            // Kamerayı direkt karakter pozisyonuna taşı
            cameraTransform.position = transform.position + cameraOffset;

            // Kameranın rotasyonu sabit kalsın
            cameraTransform.rotation = cameraFixedRotation;
        }
    }

    void FixedUpdate()
    {
        if (PlayerStats.Instance.isDead) return; //  Ölüyse hareket etme

        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    void LateUpdate()
    {
        if (healthBarCanvas != null)
            healthBarCanvas.rotation = healthBarFixedRotation;
    }

    // ----------------- Hasar verme -----------------
    public void DealDamage()
    {
        if (PlayerStats.Instance.currentHealth <= 0) return;
        if (nearestEnemy != null)
        {
            nearestEnemy.TakeDamage(attackDamage);
        }
    }

    // ----------------- En yakın düşmanı bul -----------------
    private EnemyManager GetNearestEnemy()
    {
        enemiesInRange.RemoveAll(e => e == null);

        EnemyManager nearest = null;
        float minDist = Mathf.Infinity;

        foreach (EnemyManager e in enemiesInRange)
        {
            if (e == null) continue;
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = e;
            }
        }
        return nearest;
    }

    public void RemoveEnemyFromList(EnemyManager enemy)
    {
        if (enemiesInRange.Contains(enemy))
            enemiesInRange.Remove(enemy);
    }

    // ----------------- Trigger ile temas -----------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyManager enemy = other.GetComponent<EnemyManager>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
                enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyManager enemy = other.GetComponent<EnemyManager>();
            if (enemy != null && enemiesInRange.Contains(enemy))
                enemiesInRange.Remove(enemy);
        }
    }
}
