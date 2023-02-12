using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public PlayerUse playerUse;
    public PlayerPickAndDropItem playerPickAndDropItem;
    [SerializeField] PlayerCaracteristiqueStats pcs;
    public Player player;
    PlayerUI playerUI;

    public string itemInInventoryTag;
    [HideInInspector]
    public PlayerInventoryCell[] cells;
    [SerializeField] int maxStackableNumber = 100;//100 est le nombre max stackable

    public GameObject dragItemPrefab;//pour changer l'item d'emplacement
    public Transform dragItemParentTransform;

    public GameObject objectDescriptionPanel;
    [SerializeField]
    TextMeshProUGUI descriptionText;

    [Header("MainInventaryCell")]
    public PlayerInventoryCell primaryHandItem;
    public PlayerInventoryCell armorItem;
    public PlayerInventoryCell campFireCell;
    public PlayerInventoryCell campFireCombustibleCell;

    void Start()
    {
        SetUpCells();
        playerUI = GetComponent<PlayerUI>();
        objectDescriptionPanel.SetActive(false);
    }

    void SetUpCells()
    {
        cells = new PlayerInventoryCell[GameObject.FindGameObjectsWithTag(itemInInventoryTag).Length];
        for(int i=0; i < cells.Length; i++)
        {
            cells[i] = GameObject.FindGameObjectsWithTag(itemInInventoryTag)[i].gameObject.GetComponent<PlayerInventoryCell>();
            cells[i].id = i;
            cells[i].SetPlayerInventory(this);
        }
    }

    public void EquipeItemFromInventory(int _cellId, InventoryItem inventoryItem)
    {
        if(inventoryItem.itemData.isArmor && armorItem.isUsedCell)
        {
            InventoryItem _inventoryItem = new InventoryItem();
            InventoryItem.MakeEqualTo(_inventoryItem, cells[armorItem.id].inventoryItem);
            DeleteItemInCell(armorItem.id);
            PutItemInInventory(cells[_cellId].inventoryItem, armorItem.id);
            PutItemInInventory(_inventoryItem, _cellId);
            return;
        }
        else if(playerUse.itemInHand == null || inventoryItem.itemData.isArmor && !armorItem.isUsedCell)
        {
            GameObject go = Instantiate(inventoryItem.itemData.itemObject, playerPickAndDropItem.handTransform);
            go.GetComponent<Item>().inventoryItem = inventoryItem;
            playerPickAndDropItem.PickItem(go, cells[_cellId].inventoryItem);
            DeleteItemInCell(_cellId);
            return;
        }

        if(primaryHandItem.inventoryItem.itemData == inventoryItem.itemData && inventoryItem.itemData.isStackable && 
        primaryHandItem.inventoryItem.useNumber == inventoryItem.useNumber)
        {
            AdditionalItemInInventory(inventoryItem, primaryHandItem.id);
            DeleteItemInCell(_cellId);
            return;
        }
        if(primaryHandItem.inventoryItem.itemData == inventoryItem.itemData && inventoryItem.itemData.isStackable && 
        primaryHandItem.inventoryItem.useNumber != inventoryItem.useNumber)
            return;

        Item item = playerUse.itemInHand.GetComponent<Item>();
        Destroy(playerUse.itemInHand);
        playerUse.SetItemInHand(null);
        EquipeItemFromInventory(_cellId, inventoryItem);
        PutItemInInventory(item.inventoryItem, _cellId);
        if(playerUI.campFire)
            playerUI.campFire.AddItemForCoock(playerUI.coockingTimeFill);
    }

    public void IsItemFreeSlot(InventoryItem inventoryItem, GameObject obj)
    {
        if(inventoryItem.itemData.isStackable)
        {
            foreach(PlayerInventoryCell cell in cells)//verifier une premiere fois si objet stackable
            {
                if(cell.inventoryItem != null && cell.inventoryItem.itemData == inventoryItem.itemData && inventoryItem.useNumber == cell.inventoryItem.useNumber && cell.inventoryItem.stackNumber < 100)
                {
                    AdditionalItemInInventory(inventoryItem, cell.id);
                    if(obj)
                        Destroy(obj);
                    return;
                }
            }
        }
        foreach(PlayerInventoryCell cell in cells)
        {
            if(cell.inventoryItem == null && campFireCell.id != cell.id && campFireCombustibleCell.id != cell.id)
            {
                if(armorItem.id == cell.id && !inventoryItem.itemData.isArmor || primaryHandItem.id == cell.id)
                    return;

                PutItemInInventory(inventoryItem, cell.id);
                if(obj)
                    Destroy(obj);
                return;
            }
        }
        if(primaryHandItem.inventoryItem == null)//pour vérifier en dernier si il y a de la place dans l'inventaire de la main
        {
            PutItemInInventory(inventoryItem, primaryHandItem.id);
            if(obj)
                playerPickAndDropItem.PickItemInHand(obj.transform);
        }
    }

    public void PutItemInInventory(InventoryItem inventoryItem, int _cellId)
    {
        if(cells[_cellId].inventoryItem == null)
            cells[_cellId].inventoryItem = new InventoryItem();
        InventoryItem.MakeEqualTo(cells[_cellId].inventoryItem, inventoryItem);
        cells[_cellId].itemImage.sprite = inventoryItem.itemData.itemIcone;
        cells[_cellId].SetCellItemUI();
        cells[_cellId].isUsedCell = true;
    }

    public void AdditionalItemInInventory(InventoryItem inventoryItem, int _cellId)
    {
        if(inventoryItem.stackNumber+cells[_cellId].inventoryItem.stackNumber < maxStackableNumber)
            cells[_cellId].inventoryItem.stackNumber += inventoryItem.stackNumber;
        else
        {
            int tooManyStackNumber = inventoryItem.stackNumber+cells[_cellId].inventoryItem.stackNumber-100;
            cells[_cellId].inventoryItem.stackNumber += inventoryItem.stackNumber-tooManyStackNumber;
            inventoryItem.stackNumber = tooManyStackNumber;
            IsItemFreeSlot(inventoryItem, null);
        }
        if(_cellId == primaryHandItem.id)
            InventoryItem.MakeEqualTo(playerUse.itemInHand.GetComponent<Item>().inventoryItem, cells[_cellId].inventoryItem);
        cells[_cellId].SetCellItemUI();
    }

    public void PickByMainHand(GameObject _go, InventoryItem inventoryItem)
    {
        if(playerUse.itemInHand)
        {
            Destroy(playerUse.itemInHand);
            playerUse.itemInHand = null;
        }
        GameObject go = Instantiate(_go, playerPickAndDropItem.handTransform);
        InventoryItem.MakeEqualTo(go.GetComponent<Item>().inventoryItem, inventoryItem);
        
        playerPickAndDropItem.PickItemInHand(go.transform);
    }

    public void DeleteItemInCell(int cellID)
    {
        cells[cellID].DeleteItem(false);
    }

    public void DeleteItemInHand()
    {
        playerPickAndDropItem.DeleteItemInHandCell();
        Destroy(playerUse.itemInHand);
    }

    public void SetDescriptionPanelPosition(Vector3 cellPos, RectTransform rectTr)
    {
        float transformSizeDeltaX = rectTr.sizeDelta.x;
        float transformSizeDeltaY = rectTr.sizeDelta.y;

        float descriptionSizeDeltaX = objectDescriptionPanel.GetComponent<RectTransform>().sizeDelta.x;
        float descriptionSizeDeltaY = objectDescriptionPanel.GetComponent<RectTransform>().sizeDelta.y;

        float panelPlacementXps = cellPos.x+transformSizeDeltaX/2+descriptionSizeDeltaX/2;
        float panelPlacementXng = cellPos.x-transformSizeDeltaX/2-descriptionSizeDeltaX/2;

        float panelPlacementYps = cellPos.y-transformSizeDeltaY/2+descriptionSizeDeltaY/2;
        float panelPlacementYng = cellPos.y+transformSizeDeltaY/2-descriptionSizeDeltaY/2;

        Vector3 pos = new Vector3(0,0, cellPos.z);
        //verifie si le panel sort de l'écrant && met le menu principalement vers le bas et vers la droite
        if(panelPlacementYng-descriptionSizeDeltaY/2 >= 0)
        {
            pos.y = panelPlacementYng;
        }
        else
        {
            pos.y = panelPlacementYps;
        }
        if(panelPlacementXps+descriptionSizeDeltaX/2 < Screen.width)
        {
            pos.x = panelPlacementXps;
        }
        else
        {
            pos.x = panelPlacementXng;
        }
        //créé l'objet et le met à côté de la case
        objectDescriptionPanel.transform.position = pos;
        objectDescriptionPanel.SetActive(true);
    }

    public void SetDescriptionText(InventoryItem inventoryItem)//la description d'un item
    {
        if(inventoryItem != null)
        {  
            ItemData _itemData = inventoryItem.itemData;
            string description = "";
            description += "<b><u>" + _itemData.itemName + "</u></b>\n";
            if(_itemData.maxUseNumber > 0)//infos générales
                description += "- " + inventoryItem.useNumber + "/" + _itemData.maxUseNumber + " utilisations restantes\n";
            
            if(_itemData.weaponType != ItemData.WeaponType.NoWeapon)//si arme
            {
                description += "\n<b><u>ARME:</u></b>\n";
                description += "- " + _itemData.attackDamage;
                switch (_itemData.weaponType)
                {
                    case ItemData.WeaponType.Sword:
                        if(pcs.bonusSwordSkillDamage[0]!=0||pcs.bonusSwordSkillDamage[1]!=0)
                            description += "<#68E71F>(+" + (Mathf.RoundToInt(inventoryItem.itemData.attackDamage*(1+(pcs.bonusSwordSkillDamage[0]+pcs.bonusSwordSkillDamage[1])/100))-inventoryItem.itemData.attackDamage) + ")</color>";
                        break;
                }
                
                description += " de dégâts\n";
                description += "- " + _itemData.attackRange + " de porté\n";
            }

            if(_itemData.isConsomable)//si comsomable
            {
                description += "\n<b><u>CONSOMABLE:</u></b>\n";
                //affiche si != 0 le changement de la vie 
                if(_itemData.healthRecup > 0)
                    description += "+";
                if(_itemData.healthRecup != 0)
                    description += _itemData.healthRecup + " de vie\n";
                //affiche si != 0 le changement de la faim 
                if(_itemData.hungerRecup > 0)
                    description += "+";
                if(_itemData.hungerRecup != 0)
                    description += _itemData.hungerRecup + " nutrition\n";
            }

            if(_itemData.isArmor)//si armur
            {
                description += "\n<b><u>ARMURE:</u></b>\n";
                description += "- " +  _itemData.armor + " de défense\n";
            }

            if(_itemData.isCookable)//si cuisinable
            {
                description += "\n<b><u>CUISINABLE:</u></b>\n";
                description += "- " +  _itemData.coockingTime + "s de temps de cuisson\n";
                description += "- " +  _itemData.itemCoockingResult.itemName + " est obtenu(e) après cuisson\n";
            }

            if(_itemData.isCombustible)//si peut servir de combustible
            {
                description += "\n<b><u>COMBUSTIBLE:</u></b>\n";
                int _combustibleTime =  Mathf.CeilToInt((float)inventoryItem.useNumber/((float)_itemData.maxUseNumber/_itemData.combustibleNumberOfTime));
                if(_combustibleTime == 0)
                    _combustibleTime = 1;
                description += "- Peut être utilisé " + _combustibleTime + " fois comme combustible";
            }

            descriptionText.text = description;
        }
    }
}
