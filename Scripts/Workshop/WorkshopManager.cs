using UnityEngine;

public class WorkshopManager : MonoBehaviour
{
    public DataBase workshopDataBase;

    public WorkshopUI workshopUI;
    [TextArea] public string openShopDialogue;

    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] Player player;

    public void OpenWorkshop()
    {
        if(PlayerUI.canOpenPanel)
            workshopUI.ToggleWorkshopPanel();
    }

    public void OpenShop()
    {
        if(PlayerUI.canOpenPanel)
            workshopUI.ToggleShopPanel();
        else
            return;
        DialogueSystem dialogueSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DialogueSystem>();
        dialogueSystem.MakeDialogue(openShopDialogue, dialogueSystem.dialogueText, dialogueSystem.defaultDialoguePanel);
    }

//Craft
    public void CanCraftSkill(CraftableSkill data)
    {
        if(IsEnoughItemNumber(data.requiredItems, data.requireItemNumbers))
            CraftSkill(data.craftedSkill, data, playerInventory);
    }

    void CraftSkill(Skill skill, CraftableSkill data, PlayerInventory playerInventory)
    {
        DeleteEnoughItem(data.requiredItems, data.requireItemNumbers);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkillsManager>().AddSkill(skill.skillType);
        workshopUI.UpdateAllUI();
    }

    public void CanCraftItem(CraftableItem data)
    {
        if(data.isBuyable)
        {
            if(player.coin >= data.requiredCoinsNumber)
                CraftItem(data);
        }
        else if(IsEnoughItemNumber(data.requiredItems, data.requireItemNumbers))
            CraftItem(data);
    }

    void CraftItem(CraftableItem data)
    {
        if(data.isBuyable)
        {
            player.ChangeCoinCount(data.requiredCoinsNumber, true);
        }
        else
            DeleteEnoughItem(data.requiredItems, data.requireItemNumbers);

        Transform playerTr = player.transform;
        GameObject _clone = Instantiate(data.craftedItem.itemObject, playerTr.position, playerTr.rotation);
        Item item = _clone.GetComponent<Item>();
        item.SetStart();
        playerTr.GetComponent<PlayerPickAndDropItem>().PickItem(_clone, item.inventoryItem);
        workshopUI.UpdateAllUI();
    }

//Check item
    bool IsEnoughItemNumber(ItemData[] requiredItems, int[] requiredNumbers)
    {
        int[] currentItemNumbers = new int[requiredItems.Length];
        bool[] requiredItemCondition = new bool[requiredItems.Length];
        for(int i=0; i<requiredItems.Length; i++)
            foreach(PlayerInventoryCell itemCell in playerInventory.cells)
                if(itemCell.inventoryItem != null && itemCell.inventoryItem.itemData == requiredItems[i])
                    currentItemNumbers[i] += itemCell.inventoryItem.stackNumber;
                
        for(int i=0; i<currentItemNumbers.Length; i++)
            if(currentItemNumbers[i] >= requiredNumbers[i])
                requiredItemCondition[i] = true;
        
        bool isDone = true;
        foreach(bool condition in requiredItemCondition)
            if(!condition)
                isDone = false;
        
        return isDone;
    }

    void DeleteEnoughItem(ItemData[] requiredItems, int[] requiredNumbers)
    {
        int[] currentItemNumbers = new int[requiredItems.Length];
        for(int i=0; i<requiredItems.Length; i++)
            foreach(PlayerInventoryCell itemCell in playerInventory.cells)
                if(itemCell.inventoryItem != null && itemCell.inventoryItem.itemData == requiredItems[i])
                {
                    if(currentItemNumbers[i] < requiredNumbers[i])
                    {
                        currentItemNumbers[i] += itemCell.inventoryItem.stackNumber;
                        int stackNumber = itemCell.inventoryItem.stackNumber;
                        itemCell.inventoryItem.stackNumber -= requiredNumbers[i];
                        itemCell.SetCellItemUI();
                        if(itemCell.inventoryItem.stackNumber <= 0)
                        {
                            int negativeNumber = stackNumber+itemCell.inventoryItem.stackNumber;
                            currentItemNumbers[i] -= negativeNumber;
                            itemCell.DeleteItem(true);
                        }
                    }
                }
    }
}
