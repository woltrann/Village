using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;           // Karakterin hızı
    public FloatingJoystick joystick;      // Joystick referansı (Unity'de atanacak)

    [Header("Camera Settings")]
    public Transform cameraTransform;      // Takip edecek kamera
    public Vector3 cameraOffset = new Vector3(0, 10f, -8f); // Kameranın konumu (yükseklik, uzaklık)

    [Header("Health Bar Settings")]
    private Quaternion healthBarFixedRotation; // Can barının sabit rotasyonu
    public Transform healthBarCanvas;      // Karakterin içindeki can barı (Canvas)

    private Rigidbody rb;
    private Vector3 moveDirection;
    private Animator anim;

    private bool isAttacking = false;

    // En yakındaki düşmanı takip etmek için liste
    private List<Transform> enemiesInRange = new List<Transform>();
    private Transform enemyTarget; // O anki hedef düşman

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        anim = GetComponent<Animator>();

        if (healthBarCanvas != null)
            healthBarFixedRotation = healthBarCanvas.rotation;
    }

    void FixedUpdate()
    {
        UpdateEnemyTarget(); // En yakındaki düşmanı sürekli kontrol et
        Move();
        CameraFollow();
    }

    void LateUpdate()
    {
        // Can barı hep sabit kalsın
        if (healthBarCanvas != null)
            healthBarCanvas.rotation = healthBarFixedRotation;
    }

    void Move()
    {
        // Joystick değerlerini al
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Rigidbody ile hareket
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

        // Animator kontrolü
        bool isMoving = moveDirection != Vector3.zero;
        anim.SetBool("isWalk", isMoving);

        // Eğer düşman varsa ona bak
        if (enemyTarget != null)
        {
            Vector3 dirToEnemy = (enemyTarget.position - transform.position).normalized;
            dirToEnemy.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(dirToEnemy);
            rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, 0.2f);
        }
        // Düşman yoksa joystick yönüne bak
        else if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, toRotation, 0.2f);
        }
    }

    void CameraFollow()
    {
        if (cameraTransform != null)
            cameraTransform.position = transform.position + cameraOffset;
    }

    // En yakındaki düşmanı bulur
    void UpdateEnemyTarget()
    {
        if (enemiesInRange.Count == 0)
        {
            enemyTarget = null;
            return;
        }

        float closestDist = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Transform enemy in enemiesInRange)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestEnemy = enemy;
            }
        }

        enemyTarget = closestEnemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!enemiesInRange.Contains(other.transform))
                enemiesInRange.Add(other.transform);

            anim.SetBool("Attack", true);
            isAttacking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.transform);

            // Eğer alanda hiç düşman kalmadıysa saldırıyı kapat
            if (enemiesInRange.Count == 0)
            {
                isAttacking = false;
                anim.SetBool("Attack", false);
            }
        }
    }
}
