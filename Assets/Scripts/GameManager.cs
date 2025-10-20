using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject MenuPanel;
    public GameObject GamePanel;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        MenuPanel.SetActive(false);
        GamePanel.SetActive(true);
    }
}
