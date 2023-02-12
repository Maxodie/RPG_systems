using UnityEngine;
using UnityEngine.UI;

public class WorkshopItemUI : MonoBehaviour
{
    PlayerInventory playerInventory;

    [HideInInspector] public Skill skill;//le skill à craft (si != null)
    [HideInInspector] public ItemData item;//l'item à craft (si != null)

    public Image icon;

    public Text titleText;
    public Text descriptiontext;
    public Text craftConditionText;
    public Button craftButton;

    void Start()
    {
        playerInventory = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerInventory>();
    }

    public void MouseEnter()
    {
        if(!item)
            return;
        
        playerInventory.SetDescriptionPanelPosition(icon.transform.position, icon.GetComponent<RectTransform>());
        InventoryItem inventoryItem = new InventoryItem();
        inventoryItem.itemData = item;
        inventoryItem.useNumber = item.maxUseNumber;
        inventoryItem.stackNumber = 1;
        playerInventory.SetDescriptionText(inventoryItem);
    }

    public void MouseExit() 
    {
        if(item)
            playerInventory.objectDescriptionPanel.SetActive(false);
    }
}
