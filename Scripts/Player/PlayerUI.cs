using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] GameObject inventoryPanel;
    public GameObject campFirePanel;
    public GameObject constructionPanel;
    [SerializeField] GameObject skillsPanel;
    [SerializeField] GameObject playerCateristiqueStatsPanel;
    public GameObject miniMapPanel;
    public GameObject miniMapCameraObject;
    
    public static bool canOpenPanel = true;

    public GameObject usableObjectText;

    [SerializeField]
    PlayerController playerController;
    [SerializeField]
    PlayerConstructor playerConstructor;
    public Player player;
    [SerializeField]
    PlayerInventory playerInventory;
    [HideInInspector]
    public CampFire campFire;
    [SerializeField]
    PlayerCaracteristiqueStats pcs;

    [SerializeField] Text cointext;
    [SerializeField] Image healthFill;
    [SerializeField] Text healthText;
    [SerializeField] Image hungerFill;
    [SerializeField] Text hungerText;
    [SerializeField] Image manaFill;
    [SerializeField] Text manaText;
    public Image coockingTimeFill;

    public Button destroyCampFireButton;

    public Transform activeSkillSpriteContentTr;
    public GameObject activeSkillSpriteGo;

    void Start() 
    {
        coockingTimeFill.fillAmount = 0;
        inventoryPanel.SetActive(false);
        campFirePanel.SetActive(false);
        constructionPanel.SetActive(false);
        skillsPanel.SetActive(false);
        playerCateristiqueStatsPanel.SetActive(false);
        miniMapPanel.SetActive(false);
        ToggleUsableText(false, "");
        SetCoinText();
    }

    void  Update() 
    {
        if(Input.GetKeyDown(InputManager.instance.inventory))
            ToggleInventory();
        
        if(Input.GetKeyDown(InputManager.instance.skillPanel))
            ToggleSkillsPanel();
        
        if(Input.GetKeyDown(InputManager.instance.caracteristiquePanel))
            TogglePlayerCaracteristiqueStats();
        
        if(Input.GetKeyDown(InputManager.instance.map))
            ToggleMiniMapPanel();

        if(Input.GetKeyDown(InputManager.instance.constructionPanel))
            if(!constructionPanel.activeSelf)
                ToggleConstructorPanel();
        
        if(Input.GetKeyUp(InputManager.instance.constructionPanel))
        {
            if(constructionPanel.activeSelf && playerConstructor.objectToConstructAtClose != null)
            {
                playerConstructor.PreConstruct(playerConstructor.objectToConstructAtClose);
                ToggleConstructorPanel();
            }
            else if(constructionPanel.activeSelf)
                ToggleConstructorPanel();
        }

        SetPlayerFill(player.GetHealthPct(), player.GetHungerPct(), player.GetManaPct());
    }

    public void SetCoinText()
    {
        cointext.text = player.coin + " Coins";
    }

    void ToggleInventory()
    {
        if(!canOpenPanel && !inventoryPanel.activeSelf || campFirePanel.activeSelf || PlayerInventoryCell.itemIsDrag)
        {return;}

        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        GameManager.ToggleCursorStats(inventoryPanel.activeSelf);
        canOpenPanel = !inventoryPanel.activeSelf;
        constructionPanel.SetActive(false);
        playerInventory.objectDescriptionPanel.SetActive(false);
    }

    public void ToggleFireCamp(CampFire _campFire)
    {
        if(!canOpenPanel && !campFirePanel.activeSelf)
        {return;}

        inventoryPanel.SetActive(!campFirePanel.activeSelf);
        canOpenPanel = campFirePanel.activeSelf;
        playerController.canMove = campFirePanel.activeSelf;
        campFirePanel.SetActive(!campFirePanel.activeSelf);
        GameManager.ToggleCursorStats(campFirePanel.activeSelf);
        constructionPanel.SetActive(false);
        if(campFirePanel.activeSelf)
        {
            campFire = _campFire;
        }
        else
        {
            campFire = null;
        }
    }

    public void ToggleConstructorPanel()
    {
        if(!canOpenPanel && !constructionPanel.activeSelf )
        {return;}

        constructionPanel.SetActive(!constructionPanel.activeSelf);
        canOpenPanel = !constructionPanel.activeSelf;
        GameManager.ToggleCursorStats(constructionPanel.activeSelf);
        if(!constructionPanel.activeSelf && playerConstructor.objectToConstructAtClose != null)
        {
            playerConstructor.PreConstruct(playerConstructor.objectToConstructAtClose);
        }
    }

    void ToggleSkillsPanel()
    {
        if(!canOpenPanel && !skillsPanel.activeSelf)
        {return;}

        skillsPanel.SetActive(!skillsPanel.activeSelf);
        canOpenPanel = !skillsPanel.activeSelf;
        GameManager.ToggleCursorStats(skillsPanel.activeSelf);
    }

    void TogglePlayerCaracteristiqueStats()
    {
        if(!canOpenPanel && !playerCateristiqueStatsPanel.activeSelf)
        {return;}

        playerCateristiqueStatsPanel.SetActive(!playerCateristiqueStatsPanel.activeSelf);
        canOpenPanel = !playerCateristiqueStatsPanel.activeSelf;
        GameManager.ToggleCursorStats(playerCateristiqueStatsPanel.activeSelf);
    }

    void ToggleMiniMapPanel()
    {
        if(!canOpenPanel && !miniMapPanel.activeSelf)
        {return;}

        miniMapPanel.SetActive(!miniMapPanel.activeSelf);
        canOpenPanel = !miniMapPanel.activeSelf;
        miniMapCameraObject.SetActive(miniMapPanel.activeSelf);
        GameManager.ToggleCursorStats(miniMapPanel.activeSelf);
    }

    void SetPlayerFill(float _healthAmount, float _hungerAmount, float _manaAmount)
    {
        healthFill.fillAmount = _healthAmount;
        healthText.text = player.currentPlayerLife + "/" + pcs.GetMaxPlayerLife(false);
        hungerFill.fillAmount = _hungerAmount;
        hungerText.text = Mathf.CeilToInt(player.currentPlayerHunger) + "/" + pcs.GetMaxPlayerHunger();
        manaFill.fillAmount = _manaAmount;
        manaText.text = (int)player.currentMana + "/" + pcs.GetMaxPlayerMana();
    }

    public void ToggleUsableText(bool statue, string _textContent)//pour afficher un text sous la forme ex: "F pour utiliser le feu de camp"
    {
        Text _text = usableObjectText.GetComponent<Text>();
        _text.text = InputManager.instance.useKey + " pour " + _textContent;
        usableObjectText.SetActive(statue);
    }
}
