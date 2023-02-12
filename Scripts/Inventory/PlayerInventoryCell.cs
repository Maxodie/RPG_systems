using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInventoryCell : MonoBehaviour, IPointerClickHandler 
{
    PlayerInventory playerInventory;
    Player player;
    PlayerUI playerUI;
    public int id;

    public bool isUsedCell = false;
    public Image itemImage;
    public InventoryItem inventoryItem;
    public Image useItemFill;
    public GameObject useItemGameObject;
    public Text stackNumberText;

    static int cellDragID = -1;
    static Sprite spriteDragImage = null;
    public static bool itemIsDrag = false;
    static int dragStackNumber = 0;

    GameObject itemDragClone = null;

    public void SetPlayerInventory(PlayerInventory _playerInventory)
    {
        playerInventory = _playerInventory;
        player = playerInventory.player;
        playerUI = playerInventory.GetComponent<PlayerUI>();
    }

    public void SetCellItemUI() 
    {
        if(inventoryItem != null)
        {
            if(inventoryItem.stackNumber <= 1)
                stackNumberText.gameObject.SetActive(false);
            else
            {
                stackNumberText.gameObject.SetActive(true);
                stackNumberText.text = "X" + inventoryItem.stackNumber.ToString();
            }

            if(inventoryItem.useNumber == inventoryItem.itemData.maxUseNumber)
            {
                useItemGameObject.SetActive(false);
            }
            else
            {
                useItemGameObject.SetActive(true);
                useItemFill.fillAmount = (float)inventoryItem.useNumber / (float)inventoryItem.itemData.maxUseNumber;
            }
        }
        else
        {
            useItemGameObject.SetActive(false);
            stackNumberText.gameObject.SetActive(false);
        }
    }
    
    public void ItemTakeDamage(int _amount)
    {
        if(inventoryItem != null)
        {
            inventoryItem.useNumber -= _amount;
            if(inventoryItem.useNumber <= 0)
            {
                if(playerInventory.primaryHandItem.id == id)
                    inventoryItem.itemData.DestroyItemUsed(playerInventory.playerUse.itemInHand, playerInventory.playerPickAndDropItem, playerInventory.playerUse, inventoryItem);

                if(inventoryItem.stackNumber <= 0)
                {
                    DeleteItem(true);
                    playerInventory.player.SetArmor();
                }
                else
                    inventoryItem.useNumber = inventoryItem.itemData.maxUseNumber;
            }
            SetCellItemUI();
        }
    }

    public void Click() 
    {
        if(inventoryItem != null && !itemIsDrag)
        {
            playerInventory.EquipeItemFromInventory(id, inventoryItem);
            GameObject descriptionPanelTr = playerInventory.objectDescriptionPanel;
            if(inventoryItem == null)
                descriptionPanelTr.SetActive(false);
            else
            {
                playerInventory.SetDescriptionText(inventoryItem);
                playerInventory.SetDescriptionPanelPosition(transform.position, GetComponent<RectTransform>());
            }
        }
    }

    public void OnPointerClick (PointerEventData eventData)//quand on click droit sur la case pour drop l'item
    {
        if(inventoryItem == null || itemIsDrag)
            return;
        if (eventData.button == PointerEventData.InputButton.Right) 
        {
            PlayerPickAndDropItem _playerPickAndDropItem = player.GetComponent<PlayerPickAndDropItem>();
            PlayerUse playerUse = player.GetComponent<PlayerUse>();

            GameObject _itemInHand = null;
            InventoryItem _itemInHandInventoryItem = null;
            
            if(id != playerInventory.primaryHandItem.id)//si l'item n'est pas dans la main le fait spawn 
            {
                GameObject _clone = Instantiate(inventoryItem.itemData.itemObject, playerUse.transform);
                Item item =  _clone.GetComponent<Item>();
                item.inventoryItem = inventoryItem;
                item.isStart = false;
                
                _itemInHand = playerUse.itemInHand;
                _itemInHandInventoryItem = playerInventory.primaryHandItem.inventoryItem;
                playerUse.SetItemInHand(_clone);//change l'item dans la main
            }

            _playerPickAndDropItem.DropItem();

            if(_itemInHand != null)//si il y avais un objet dans la main le reset 
            {
                playerInventory.PutItemInInventory(_itemInHandInventoryItem, playerInventory.primaryHandItem.id);
                playerUse.SetItemInHand(_itemInHand);
            }
            DeleteItem(false);
        }
    }

    public void DeleteItem(bool isDestroyPh)
    {
        itemImage.sprite = null;
        inventoryItem = null;
        isUsedCell  = false;
        SetCellItemUI();
        Image _image = GetComponent<Image>();
        if(_image != null)
            itemImage.sprite = _image.sprite;
        
        if(id == playerInventory.primaryHandItem.id && isDestroyPh == true)
            Destroy(playerInventory.playerUse.itemInHand);
    }
    //Drag and drop item in another slot of inventory
    public void DragItem() //1er
    {
        if(isUsedCell && !itemIsDrag)
        {
            spriteDragImage = itemImage.sprite;

            itemDragClone = Instantiate(playerInventory.dragItemPrefab, gameObject.transform.position, gameObject.transform.rotation);
            itemDragClone.transform.SetParent(playerInventory.dragItemParentTransform);
            itemDragClone.GetComponent<Image>().sprite = itemImage.sprite;

            dragStackNumber = inventoryItem.stackNumber;

            if(Input.GetKey(InputManager.instance.leftShift) && inventoryItem.stackNumber > 1)
            {
                dragStackNumber = Mathf.CeilToInt(inventoryItem.stackNumber/2);
                inventoryItem.stackNumber -= dragStackNumber;
                playerInventory.PutItemInInventory(inventoryItem, id);
            }
            else
            {
                Image _image = GetComponent<Image>();
                if(_image != null)
                    itemImage.sprite = GetComponent<Image>().sprite;
                isUsedCell = false;
                useItemGameObject.SetActive(false);
                stackNumberText.gameObject.SetActive(false);
            }
            GameObject descriptionPanelTr = playerInventory.objectDescriptionPanel;
            descriptionPanelTr.SetActive(false);
            itemIsDrag = true;
            cellDragID = id;
        }
        if(itemDragClone != null)
        {
            itemDragClone.transform.position = Input.mousePosition;
        }
    }
    
    public void DropItem() //2eme
    {
        if(cellDragID < 0)
            return;

        InventoryItem _inventoryItem = new InventoryItem();
        InventoryItem.MakeEqualTo(_inventoryItem, playerInventory.cells[cellDragID].inventoryItem);
        _inventoryItem.stackNumber = dragStackNumber;

        if(!isUsedCell && itemIsDrag || inventoryItem != null && inventoryItem.itemData == playerInventory.cells[cellDragID].inventoryItem.itemData 
        && playerInventory.cells[cellDragID].inventoryItem.itemData.isStackable && itemIsDrag)
        {
            if(!_inventoryItem.itemData.isArmor && playerInventory.armorItem.id == id || 
            !_inventoryItem.itemData.isCookable && id == playerInventory.campFireCell.id ||
            !_inventoryItem.itemData.isCombustible && id == playerInventory.campFireCombustibleCell.id ||
            inventoryItem != null && inventoryItem.useNumber != _inventoryItem.useNumber)
                return;

            isUsedCell = true;
            itemImage.sprite = spriteDragImage;
            if(inventoryItem != null)
            {
                playerInventory.AdditionalItemInInventory(_inventoryItem, id);
                //itemIsDrag = false;
            }
            else
            {
                inventoryItem = new InventoryItem();
                if(playerInventory.cells[cellDragID].inventoryItem.stackNumber <= 0 || !playerInventory.cells[cellDragID].isUsedCell)
                    inventoryItem.useNumber = _inventoryItem.useNumber;
                else
                    inventoryItem.useNumber = _inventoryItem.itemData.maxUseNumber;
                inventoryItem.itemData = _inventoryItem.itemData;
                inventoryItem.stackNumber = dragStackNumber;
            }

            if(!playerInventory.cells[cellDragID].isUsedCell)
            {
                playerInventory.cells[cellDragID].DeleteItem(false);
            }
            itemIsDrag = false;

            if(playerInventory.armorItem.id == id)
            {
                player.SetArmor();
            }
            else if(id == playerInventory.primaryHandItem.id)
            {
                playerInventory.PickByMainHand(_inventoryItem.itemData.itemObject, _inventoryItem);
            }
            else if(_inventoryItem.itemData.isCookable && id == playerInventory.campFireCell.id ||
            _inventoryItem.itemData.isCombustible && id == playerInventory.campFireCombustibleCell.id)
            {
                playerUI.campFire.AddItemForCoock(playerUI.coockingTimeFill);
            }

            GameObject descriptionPanelTr = playerInventory.objectDescriptionPanel;
            descriptionPanelTr.SetActive(true);
            OnEnter();
        }
        else if(inventoryItem != null && !playerInventory.cells[cellDragID].isUsedCell)
        {
            //inverse les item de place
            InventoryItem.MakeEqualTo(playerInventory.cells[cellDragID].inventoryItem, inventoryItem);
            playerInventory.cells[cellDragID].itemImage.sprite = itemImage.sprite;
            InventoryItem.MakeEqualTo(inventoryItem, _inventoryItem);
            itemImage.sprite = spriteDragImage;

            playerInventory.cells[cellDragID].SetCellItemUI();

            if(playerInventory.armorItem.id == id)
            {
                player.SetArmor();
            }
            else if(id == playerInventory.primaryHandItem.id || cellDragID == playerInventory.primaryHandItem.id)
            {
                if(id == playerInventory.primaryHandItem.id)
                    playerInventory.PickByMainHand(_inventoryItem.itemData.itemObject, _inventoryItem);
                else
                {
                    playerInventory.PickByMainHand(playerInventory.cells[cellDragID].inventoryItem.itemData.itemObject, playerInventory.cells[cellDragID].inventoryItem);
                }
            }
            else if(_inventoryItem.itemData.isCookable && id == playerInventory.campFireCell.id ||
            _inventoryItem.itemData.isCombustible && id == playerInventory.campFireCombustibleCell.id)
            {
                playerUI.campFire.AddItemForCoock(playerUI.coockingTimeFill);
            }
            playerInventory.cells[cellDragID].isUsedCell = true;
            itemIsDrag = false;
            playerInventory.SetDescriptionText(inventoryItem);
        }
        SetCellItemUI();
    }
    public void ReturnDragItem() //3eme
    {
        if(itemIsDrag)
        {
            playerInventory.cells[id].itemImage.sprite = spriteDragImage;
            if(playerInventory.cells[id].isUsedCell)
            {
                playerInventory.cells[id].inventoryItem.stackNumber += dragStackNumber;
            }
            else
                playerInventory.cells[id].isUsedCell = true;

            SetCellItemUI();
        }
        else
        {
            if(id == playerInventory.primaryHandItem.id && !playerInventory.primaryHandItem.isUsedCell)
            {
                playerInventory.DeleteItemInHand();
            }
            if(id == playerInventory.armorItem.id)
            {
                player.SetArmor();
            }
            cellDragID = -1;
            spriteDragImage = null;
        }
        Destroy(playerInventory.cells[id].itemDragClone);
        itemIsDrag = false;
    }

    public void OnEnter()//si objet alors affiche une petite fenêtre avec la desc de l'objet
    {
        if(!itemIsDrag && inventoryItem != null)
        {  
            SetDescriptionText();
            playerInventory.SetDescriptionPanelPosition(transform.position, GetComponent<RectTransform>());
        }
    }

    void SetDescriptionText()
    {
        if(!itemIsDrag && inventoryItem != null)
        {  
            playerInventory.SetDescriptionText(inventoryItem);
        }
    }

    public void OnExit()
    {
        GameObject descriptionPanelTr = playerInventory.objectDescriptionPanel;
        descriptionPanelTr.SetActive(false);
    }
}