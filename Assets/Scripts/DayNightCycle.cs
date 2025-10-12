using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance;

    [Header("Aydınlatma Ayarları")]
    public Light directionalLight;
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.1f, 0.1f, 0.25f);
    public float transitionSpeed = 2f;

    [Header("Durum")]
    public bool isNight = false;
    private float targetIntensity;
    private Color targetColor;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (directionalLight == null)
            directionalLight = RenderSettings.sun;

        SetDay(); // başlangıçta gündüz
    }

    void Update()
    {
        if (directionalLight == null) return;

        // Işık geçişini yumuşat
        directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, targetIntensity, Time.deltaTime * transitionSpeed);
        directionalLight.color = Color.Lerp(directionalLight.color, targetColor, Time.deltaTime * transitionSpeed);
    }

    public void ToggleDayNight()
    {
        if (isNight)
            SetDay();
        else
            SetNight();
    }

    public void SetDay()
    {
        isNight = false;
        targetIntensity = 1f;
        targetColor = dayColor;
        Debug.Log("🌞 Gündüz başladı");
    }

    public void SetNight()
    {
        isNight = true;
        targetIntensity = 0.2f;
        targetColor = nightColor;
        Debug.Log("🌙 Gece başladı");
    }
}
