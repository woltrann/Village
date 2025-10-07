using UnityEngine;

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
    private Transform enemyTarget; // kilitlenilen düşman

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Dönme sorunlarını engelle
        healthBarFixedRotation = healthBarCanvas.rotation; // Can barını başlangıç rotasyonunu kaydet
        anim = GetComponent<Animator>();   // Animator referansı al
    }

    void FixedUpdate()
    {
        Move();
        CameraFollow();

    }

    void LateUpdate()
    {
        healthBarCanvas.rotation = healthBarFixedRotation;
    }

    void Move()
    {
        // Joystick değerlerini al
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        // Hareket yönü
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Rigidbody ile hareket
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

        // Animator için kontrol
        if (anim != null)
        {
            bool isMoving = moveDirection != Vector3.zero;
            anim.SetBool("isWalk", isMoving);
        }

        // Eğer düşman yoksa joystick yönüne dön
        if (enemyTarget == null && moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, toRotation, 0.2f);
        }
        // Eğer düşman varsa ona bak
        else if (enemyTarget != null)
        {
            Vector3 dirToEnemy = (enemyTarget.position - transform.position).normalized;
            dirToEnemy.y = 0; // sadece yatay eksende dönsün
            Quaternion lookRotation = Quaternion.LookRotation(dirToEnemy);
            rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, 0.2f);
        }
    }

    void CameraFollow()
    {
        if (cameraTransform != null)
        {
            // Kamera, karakterin konumuna offset eklenerek yerleştirilir
            cameraTransform.position = transform.position + cameraOffset;
            // Rotasyonu değiştirmiyoruz → kamera hep aynı bakış açısında kalıyor
        }
    }

    // Enemy alana girince tetiklenir
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!isAttacking)
            {
                isAttacking = true;
                anim.SetBool("Attack", true);

                enemyTarget = other.transform; // düşmanı kaydet
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Sürekli düşmana bakması için target güncel kalır
        if (other.CompareTag("Enemy"))
        {
            enemyTarget = other.transform;
        }
    }

    // Enemy alandan çıkarsa saldırı biter
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            isAttacking = false;
            anim.SetBool("Attack", false);

            enemyTarget = null; // düşman kaydı sıfırlanır
        }
    }
}
