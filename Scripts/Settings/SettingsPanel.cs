using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] GameObject settignsMenu;
    [SerializeField] GameObject inputManagerPanel;
    PlayerController playerController;

    bool cursorCurrentLockStat = true;

    void Start()
    {
        settignsMenu.SetActive(false);
        inputManagerPanel.SetActive(false);
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update() 
    {
        
        if(Input.GetKeyDown(InputManager.instance.escape))
        {
           ToggleSettings();
        }
    }

    void ToggleSettings()
    {
        settignsMenu.SetActive(!settignsMenu.activeSelf);
        if(!settignsMenu.activeSelf)
        {
            inputManagerPanel.SetActive(false);
            GameManager.ToggleCursorStats(cursorCurrentLockStat);
        }
        else
        {
            cursorCurrentLockStat = !playerController.isCamCanMove;
            GameManager.ToggleCursorStats(true);
        }

        PlayerUI.canOpenPanel = !settignsMenu.activeSelf;
    }

    public void ToggleInputManagerPanel()
    {
        inputManagerPanel.SetActive(!inputManagerPanel.activeSelf);
    }
}
