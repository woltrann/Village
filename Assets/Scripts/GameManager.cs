using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject GamePanel;
    public GameObject MenuPanel;
    public GameObject StorePanel;
    public GameObject CharacterPanel;
    public GameObject BuldingPanel;
    public GameObject SettingsPanel;

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
    public void StartAnimation()
    {

    }
    public void StorePanelOC()=> StorePanel.SetActive(!StorePanel.activeSelf);
    public void CharacterPanelOC()=> CharacterPanel.SetActive(!CharacterPanel.activeSelf);
    public void BuldingPanelOC()=> BuldingPanel.SetActive(!BuldingPanel.activeSelf);
    public void SettingsPanelOC()=> SettingsPanel.SetActive(!SettingsPanel.activeSelf);

}
