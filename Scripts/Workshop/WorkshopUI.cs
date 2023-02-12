using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WorkshopUI : MonoBehaviour
{
    [SerializeField] WorkshopManager workshopManager;
    [SerializeField] PlayerSkillsManager playerSkillsManager;
    [SerializeField] DialogueSystem dialogueSystem;
    bool canClosePanel = false;

    public GameObject workshopPanel;
    public GameObject shopPanel;
    [Header("Skills")]
    [SerializeField] GameObject craftableUISkillPrefab;
    [SerializeField] Transform craftableSkillItemContent;
    [Header("Items")]
    [SerializeField] GameObject uiItemPrefab;
    [SerializeField] Transform craftableItemItemUIContent;
    [SerializeField] Transform shopItemUIContent;

    [SerializeField] GameObject[] workshopTabs;

    List<WorkshopItemUI> allCraftableSkill = new List<WorkshopItemUI>();

    void Start()
    {
        workshopPanel.SetActive(false);
        shopPanel.SetActive(false);
        ChangeTab(0);
        AddAllItemItemInUI();
        AddAllSkillItemUI();
    }

    void Update() 
    {
        if(workshopPanel.activeSelf)
            if(Input.GetKeyDown(InputManager.instance.escape) || Input.GetKeyDown(InputManager.instance.useKey))
                if(canClosePanel)
                    ToggleWorkshopPanel();
        
        if(shopPanel.activeSelf)
            if(Input.GetKeyDown(InputManager.instance.escape) || Input.GetKeyDown(InputManager.instance.useKey))
                if(canClosePanel)
                    ToggleShopPanel();
    }

    public void CloseAllPanel()
    {
        if(workshopPanel.activeSelf)
            ToggleWorkshopPanel();
        if(shopPanel.activeSelf)
            ToggleShopPanel();
    }

    public void UpdateAllUI()
    {
        SetCraftableSkillUI();
        SetCraftableItemUI();
    }

    public void ChangeTab(int tabId)
    {
        for(int i=0; i<workshopTabs.Length; i++)
            if(i == tabId)
                workshopTabs[i].SetActive(true);
            else
                workshopTabs[i].SetActive(false);
    }

    void SetCraftableSkillUI()
    {
        foreach(CraftableSkill data in workshopManager.workshopDataBase.craftableSkills)
        {
            if(!data.workshopItemUI)
                continue;

            if(playerSkillsManager.GetCategorie(data.craftedSkill.skillCategorieType).categorieLvl >= data.categorieLvlRequired)//check le lvl de la class 
            {
                data.workshopItemUI.gameObject.SetActive(true);
                SetCraftableSkillItem(data.workshopItemUI, data);
            }
            else
                data.workshopItemUI.gameObject.SetActive(false);
        }
        foreach(Skill skill in playerSkillsManager.GetAllUsedSkill())
        {
            if(allCraftableSkill.Count > 0)
                foreach(WorkshopItemUI workshopItemUI in allCraftableSkill)
                {
                    if(skill == workshopItemUI.skill)
                    {
                        allCraftableSkill.Remove(workshopItemUI);
                        Destroy(workshopItemUI.gameObject);
                        break;
                    }
                }
        }
    }

    void SetCraftableSkillItem(WorkshopItemUI workshopItemUI, CraftableSkill data)
    {
        workshopItemUI.titleText.text = data.craftedSkill.skillName;
        string craftConditionString = "";
        craftConditionString += GetRequiredItemsString(data.requireItemNumbers, data.requiredItems, false, 0);
        workshopItemUI.craftConditionText.text = craftConditionString;

        workshopItemUI.descriptiontext.text = playerSkillsManager.GetCategorie(data.craftedSkill.skillCategorieType).categorieName + "\n";
        workshopItemUI.descriptiontext.text += GetComponent<SkillsPanelManager>().SetSkillUIDescription(data.craftedSkill);
    }

    void AddAllSkillItemUI()
    {
        foreach(CraftableSkill data in workshopManager.workshopDataBase.craftableSkills)
        {
            GameObject craftableClone = Instantiate(craftableUISkillPrefab, craftableSkillItemContent);
            WorkshopItemUI workshopItemUI = craftableClone.GetComponent<WorkshopItemUI>();
            data.workshopItemUI = workshopItemUI;
            SetCraftableSkillItem(workshopItemUI, data);
            workshopItemUI.craftButton.onClick.AddListener(() => CraftSkillButton(data));
            workshopItemUI.skill = data.craftedSkill;
            allCraftableSkill.Add(workshopItemUI);
        }
    }

    public void CraftSkillButton(CraftableSkill craftableSkill)
    {
        workshopManager.CanCraftSkill(craftableSkill);
    }

    void SetCraftableItemUI()
    {
        foreach(CraftableItem data in  workshopManager.workshopDataBase.craftableItems)
        {
            if(playerSkillsManager.GetCategorie(data.categoriesType).categorieLvl >= data.categorieLvlRequired)
            {
                if(data.isCraftable)
                {
                    data.workshopItemUI.gameObject.SetActive(true);
                    SetCraftableItemItemUI(data.workshopItemUI, data);
                }
                if(data.isBuyable)
                {
                    data.shopWorkshopItemUI.gameObject.SetActive(true);
                    SetCraftableItemItemUI(data.shopWorkshopItemUI, data);
                }
            }
            else
            {
                if(data.isCraftable)
                    data.workshopItemUI.gameObject.SetActive(false);
                if(data.isBuyable)
                    data.shopWorkshopItemUI.gameObject.SetActive(false);
            }
        }
    }

    void AddAllItemItemInUI()
    {
        foreach(CraftableItem data in  workshopManager.workshopDataBase.craftableItems)
        {
            if(data.isBuyable)
            {
                GameObject shopClone = Instantiate(uiItemPrefab, shopItemUIContent);
                WorkshopItemUI shopUIClone = shopClone.GetComponent<WorkshopItemUI>();
                SetInstantiateItemUI(data, shopUIClone);
                data.shopWorkshopItemUI = shopUIClone;
            }
            if(data.isCraftable)
            {
                GameObject craftableUIClone = Instantiate(uiItemPrefab, craftableItemItemUIContent);
                WorkshopItemUI workshopItemUI = craftableUIClone.GetComponent<WorkshopItemUI>();
                SetInstantiateItemUI(data, workshopItemUI);
                data.workshopItemUI = workshopItemUI;
            }
        }
    }

    void SetInstantiateItemUI(CraftableItem data, WorkshopItemUI newWorkshopItemUI)
    {
        SetCraftableItemItemUI(newWorkshopItemUI, data);
        newWorkshopItemUI.craftButton.onClick.AddListener(() => CraftItemButton(data));
        newWorkshopItemUI.item = data.craftedItem;
    }   

    public void CraftItemButton(CraftableItem data)
    {
        workshopManager.CanCraftItem(data);
    }

    void SetCraftableItemItemUI(WorkshopItemUI workshopItemUI, CraftableItem data)
    {
        workshopItemUI.titleText.text = data.craftedItem.itemName;
        string craftConditionString = GetRequiredItemsString(data.requireItemNumbers, data.requiredItems, data.isBuyable, data.requiredCoinsNumber);
        workshopItemUI.craftConditionText.text = craftConditionString;

        workshopItemUI.icon.sprite = data.craftedItem.itemIcone;
    }

    string GetRequiredItemsString(int[] requireItemNumbers, ItemData[] requiredItems, bool isBuyable, int requiredCoinsNumber)
    {
        string craftConditionString = "";
        if(!isBuyable)
        {
            PlayerInventory playerInventory = GetComponent<PlayerInventory>();
            for(int i=0; i<requiredItems.Length; i++)
            {
                if(i>0)
                    craftConditionString += "\n";

                int currentItemNumber = 0;
                foreach(PlayerInventoryCell itemCell in playerInventory.cells)
                    if(itemCell.inventoryItem != null && itemCell.inventoryItem.itemData == requiredItems[i])
                        currentItemNumber += itemCell.inventoryItem.stackNumber;
                
                craftConditionString += currentItemNumber + "/";
                craftConditionString += requireItemNumbers[i] + " " + requiredItems[i].itemName + " nécessaire";
            }
        }
        else
            craftConditionString += requiredCoinsNumber + " d'or requis pour l'achat";
        
        return craftConditionString;
    }

    public void ToggleWorkshopPanel()
    {
        if(!workshopPanel.activeSelf)
            StartCoroutine(WaitClosePanel(workshopPanel));
        else
        {
            canClosePanel = false;
            dialogueSystem.CloseDialogue(dialogueSystem.defaultDialoguePanel);
        }
        TogglePanel(workshopPanel);
    }

    public void ToggleShopPanel()
    {
        if(!shopPanel.activeSelf)
            StartCoroutine(WaitClosePanel(shopPanel));
        else
        {
            canClosePanel = false;
            dialogueSystem.CloseDialogue(dialogueSystem.defaultDialoguePanel);
        }
        TogglePanel(shopPanel);
    }

    IEnumerator WaitClosePanel(GameObject panel)//Pour ne pas l'ouvrir juste après l'avoir fermé dans PlayerPickAndDropItem
    {
        yield return new WaitForSeconds(0.1f);
        canClosePanel = true;
    }

    void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
        SetToggleParameters(panel.activeSelf);
        GetComponent<PlayerUI>().constructionPanel.SetActive(false);
        GetComponent<PlayerInventory>().objectDescriptionPanel.SetActive(false);
    }

    void SetToggleParameters(bool isActive)
    {
        if(isActive)
            UpdateAllUI();
        GameManager.ToggleCursorStats(isActive);
        playerSkillsManager.GetComponent<PlayerController>().canMove = !isActive;
        PlayerUI.canOpenPanel = !isActive;
    }
}