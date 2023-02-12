using UnityEngine;

[RequireComponent(typeof(PlayerUse))]
public class PlayerPickAndDropItem : MonoBehaviour
{
    [SerializeField] PlayerUse playerUse;
    public PlayerUI playerUI;
    public PlayerInventory playerInventory;
    [SerializeField] Player player;

    public Transform handTransform;
    public Camera cam;
    public float pickItemRange = 1.5f;
    public float dropRange;

    [SerializeField]
    string itemLayerName;
    [SerializeField] LayerMask obstructMask;

    RaycastHit itemHit;

    void Update()
    {
        if(Input.GetKeyDown(InputManager.instance.dropItem))
        {
            DropItem();
        }

        SetCanTake();

        if(Input.GetKeyDown(InputManager.instance.useKey))
            SetTake();
    }

    void SetCanTake()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, pickItemRange, obstructMask) &&
        PlayerUI.canOpenPanel)
        {
            itemHit = hit;
            if(hit.transform.CompareTag("Item"))
                playerUI.ToggleUsableText(true, "prendre " + hit.transform.GetComponent<Item>().itemData.itemName);
            else if(hit.transform.CompareTag("QuestGuiver"))
                playerUI.ToggleUsableText(true, "parler à " + hit.transform.GetComponent<QuestGuiver>().pnjName);
            else if(hit.transform.CompareTag("Workshop"))
                playerUI.ToggleUsableText(true, " ouvir l'atelier");
            else if(hit.transform.CompareTag("Shop"))
                playerUI.ToggleUsableText(true, " ouvir la boutique");
        }
        else if(!itemHit.Equals(new RaycastHit()))
        {
            playerUI.ToggleUsableText(false, "");
            itemHit = new RaycastHit();
        }
    }

    void SetTake()
    {
        if(!itemHit.Equals(new RaycastHit()))
        {
            if(itemHit.transform.CompareTag("Item"))
                TakeItem();
            else if(itemHit.transform.CompareTag("QuestGuiver"))
                itemHit.transform.GetComponent<QuestGuiver>().Talk();
            else if(itemHit.transform.CompareTag("Workshop") || itemHit.transform.CompareTag("Shop"))
            {
                WorkshopManager workshopManager = GameObject.FindGameObjectWithTag("WorkshopManager").GetComponent<WorkshopManager>();
                if(!workshopManager.workshopUI.workshopPanel.activeSelf && itemHit.transform.CompareTag("Workshop"))
                    workshopManager.OpenWorkshop();
                if(!workshopManager.workshopUI.shopPanel.activeSelf && itemHit.transform.CompareTag("Shop"))
                    workshopManager.OpenShop();
            }
        }
    }

    void TakeItem()
    {
        Item item = itemHit.transform.GetComponent<Item>();
        StopAllCoroutines();
        UseGroundedItem(item.inventoryItem, itemHit.transform.gameObject, itemHit.transform);//Methode pour mettre l'item dans la main/l'inventaire
    }

    public void DropItem()
    {
        if(playerUse.itemInHand != null)
        {
            Item item = playerUse.itemInHand.GetComponent<Item>();
            Rigidbody itemRb = item.GetComponent<Rigidbody>();

            itemRb.useGravity = true;
            itemRb.isKinematic = false;
            foreach(Collider col in item.colliders)
                col.enabled = true;

            item.transform.SetParent(null);
            item.transform.position = cam.transform.position;// + cam.transform.forward*dropRange;

            DeleteItemInHandCell();
            GameManager.SetLayerRecursively(playerUse.itemInHand, "ItemInGround");//le layer pour quand on pose un item ne prenne pas en compte son col
            playerUse.SetItemInHand(null);
        }
    }

    //Methode appelé sur player use pour raison pratique d'affichage du text info
    public void UseGroundedItem(InventoryItem inventoryItem, GameObject itemObj, Transform tr)
    {
        if(inventoryItem != null)
        {
            PickItem(itemObj, inventoryItem);
            playerUI.ToggleUsableText(false, "");
        }
    }
    
    public void PickItem(GameObject go, InventoryItem inventoryItem)
    {
        if(inventoryItem.itemData.isArmor && !playerInventory.armorItem.isUsedCell)
        {
            playerInventory.PutItemInInventory(inventoryItem, playerInventory.armorItem.id);
            Destroy(go);
            player.SetArmor();
        }
        else if(playerUse.itemInHand == null)
        {
            PickItemInHand(go.transform);
            playerInventory.PutItemInInventory(inventoryItem, playerInventory.primaryHandItem.id);//mettre l'objet dans l'inventair de la main
        }
        else
        {
            playerInventory.IsItemFreeSlot(inventoryItem, go);
        }
    }
    
    public void PickItemInHand(Transform tr)
    {
        if(playerUse.itemInHand == null)
        {
            Item item = tr.GetComponent<Item>();
            item.isStart = false;
            Rigidbody itemRb = item.GetComponent<Rigidbody>();
            itemRb.useGravity = false;
            itemRb.isKinematic = true;

            tr.SetParent(player.GetComponent<PlayerPickAndDropItem>().handTransform);
            
            tr.localPosition = item.inventoryItem.itemData.positionInHand;
            tr.localEulerAngles = item.inventoryItem.itemData.rotationInHand;
            tr.localScale = Vector3.one;
            foreach(Collider col in item.colliders)
                col.enabled = false;

            playerUse.SetItemInHand(tr.gameObject);
            //changer le layer de l'objet et des enfants
            GameManager.SetLayerRecursively(playerUse.itemInHand, itemLayerName);
            playerUI.ToggleUsableText(false, "");
        }
    }

    public void DeleteItemInHandCell()
    {
        if(playerUse.itemInHand != null)
        {
            playerInventory.DeleteItemInCell(playerInventory.primaryHandItem.id);
        }
    }
}