using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FightButtonAnimation : MonoBehaviour
{
    [Header("References")]
    public RectTransform swordLeft;
    public RectTransform swordRight;
    public Button fightButton;

    [Header("Settings")]
    public Vector2 leftTargetPos = new Vector2(80, 0);  // Hedef pozisyon
    public Vector2 rightTargetPos = new Vector2(-80, 0);
    public float rotationAngle = 45f;                   // Kılıçların açısı
    public float animDuration = 0.5f;
    //public AudioSource clashSound;                      // Opsiyonel ses

    private Vector2 leftStartPos;
    private Vector2 rightStartPos;
    private Quaternion leftStartRot;
    private Quaternion rightStartRot;

    private void Start()
    {
        // Başlangıç pozisyonlarını kaydet
        leftStartPos = swordLeft.anchoredPosition;
        rightStartPos = swordRight.anchoredPosition;
        leftStartRot = swordLeft.rotation;
        rightStartRot = swordRight.rotation;

        // Butona tıklama eventi bağla
        fightButton.onClick.AddListener(OnFightPressed);
    }

    private void OnFightPressed()
    {
        // Önce varsa önceki animasyonları sıfırla
        DOTween.Kill(swordLeft);
        DOTween.Kill(swordRight);

        // Sol kılıç animasyonu
        swordLeft.DOAnchorPos(leftTargetPos, animDuration).SetEase(Ease.OutBack);
        swordLeft.DORotate(new Vector3(0, 0, rotationAngle), animDuration).SetEase(Ease.OutBack);

        // Sağ kılıç animasyonu
        swordRight.DOAnchorPos(rightTargetPos, animDuration).SetEase(Ease.OutBack);
        swordRight.DORotate(new Vector3(0, 0, -rotationAngle), animDuration).SetEase(Ease.OutBack);

        // ?? Çarpışma efekti (gecikmeli)
        DOVirtual.DelayedCall(animDuration - 0.1f, () =>
        {
            //if (clashSound != null) clashSound.Play(); // Çarpışma sesi
            swordLeft.DOShakePosition(0.2f, 10f, 10, 90);
            swordRight.DOShakePosition(0.2f, 10f, 10, 90);
        });

        // ?? Geri eski haline dönsün
        DOVirtual.DelayedCall(animDuration + 0.4f, ResetSwords);
    }

    private void ResetSwords()
    {
        swordLeft.DOAnchorPos(leftStartPos, 0.4f).SetEase(Ease.InBack);
        swordRight.DOAnchorPos(rightStartPos, 0.4f).SetEase(Ease.InBack);
        swordLeft.DORotateQuaternion(leftStartRot, 0.4f).SetEase(Ease.InBack);
        swordRight.DORotateQuaternion(rightStartRot, 0.4f).SetEase(Ease.InBack)
         .OnComplete(() =>
          {
              Debug.Log("Animasyon tamamlandı!");
              // 🔥 Animasyon tamamlandıktan sonra oyun başlasın
              GameManager.Instance.StartGame();
          });
    }
}
