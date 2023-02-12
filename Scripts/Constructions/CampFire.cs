using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CampFire : MonoBehaviour
{
    [SerializeField]
    LayerMask obstructionMask;
    [SerializeField]
    InventoryItem inventoryItemCoockCell;
    PlayerInventory playerInventory;

    [SerializeField]
    float distanceForDestroy = 10f;

    Image coockingTimeFill;
    bool isCoocking;

    PlayerUI playerUI;
    Transform playerTransform;
    PlayerPickAndDropItem playerPickAndDropItem;

    void Start() 
    {
        playerUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerUI>();
        playerUI.destroyCampFireButton.onClick.RemoveAllListeners();
        playerUI.destroyCampFireButton.onClick.AddListener(() => DestroyCampButton());
        playerTransform = playerUI.player.transform;
        playerPickAndDropItem = playerUI.player.GetComponent<PlayerPickAndDropItem>();
        playerInventory = playerUI.GetComponent<PlayerInventory>();
    }

    void Update() 
    {
        if(Vector3.Distance(playerTransform.position, transform.position) >= distanceForDestroy)
            DestroyCampButton();
    }

    public void AddItemForCoock(Image _coockingTimeFill)
    {
        inventoryItemCoockCell = playerInventory.campFireCell.inventoryItem;
        coockingTimeFill = _coockingTimeFill;
        if(CanCoock())
        {
            int _amount = 1;
            int _maxUseNumber = playerInventory.campFireCombustibleCell.inventoryItem.itemData.maxUseNumber;
            int _combustibleNumberOfTime = playerInventory.campFireCombustibleCell.inventoryItem.itemData.combustibleNumberOfTime;
            if(_combustibleNumberOfTime != 0)
                _amount = Mathf.CeilToInt((float)_maxUseNumber/_combustibleNumberOfTime);
            
            playerInventory.cells[playerInventory.campFireCombustibleCell.id].ItemTakeDamage(_amount);
            
            isCoocking = true;
            StartCoroutine(CoockRoutine());
        }
    }

    bool CanCoock()
    {
        InventoryItem _combustible = playerInventory.campFireCombustibleCell.inventoryItem;
        if(_combustible != null && inventoryItemCoockCell != null)
        {
            if(inventoryItemCoockCell.itemData.isCookable && _combustible.itemData.isCombustible)
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    IEnumerator CoockRoutine()
    {
        float time = inventoryItemCoockCell.itemData.coockingTime;
        float currentTime = 0f;

        while (isCoocking)
        {
            yield return null;
            if(currentTime >= time || playerInventory.campFireCell.inventoryItem == null)
            {
                isCoocking = false;
            }
            else
            {
                currentTime += 1* Time.deltaTime;
                coockingTimeFill.fillAmount = (float)currentTime/time;
            }
        }
        coockingTimeFill.fillAmount = 0;
        if(playerInventory.campFireCell.inventoryItem != null)
        {
            inventoryItemCoockCell.itemData = inventoryItemCoockCell.itemData.itemCoockingResult;
            inventoryItemCoockCell.useNumber = playerInventory.campFireCell.inventoryItem.useNumber;
            playerInventory.DeleteItemInCell(playerInventory.campFireCell.id);
            playerInventory.PutItemInInventory(inventoryItemCoockCell, playerInventory.campFireCell.id);
        }
        inventoryItemCoockCell = null;
    }

    //cherche si il y a un item dans le slot des combustibles et le détruit
    void OnMouseOver() 
    {
        bool distanceWithPlayer = Vector3.Distance(playerPickAndDropItem.transform.position, playerUI.player.cam.transform.position) <= playerPickAndDropItem.pickItemRange;
        if(!playerUI.campFirePanel.activeSelf && distanceWithPlayer)
        {
            playerUI.ToggleUsableText(true, "utiliser le feu de camp");
        }
        else
        {
            playerUI.ToggleUsableText(false, "");
            return;
        }

        if(Input.GetKeyDown(InputManager.instance.useKey))
        {
            playerUI.ToggleFireCamp(this);
            playerUI.ToggleUsableText(false, "");
            StartCoroutine(CloseRountine());
        }
    }

    void OnMouseExit() 
    {
        playerUI.ToggleUsableText(false, "");
    }

    IEnumerator CloseRountine()
    {
        yield return new WaitForSeconds(0.01f);//temps pour éviter qu'il se ouvre et se ferme tout de suite
        yield return WaitForKeyPress(InputManager.instance.useKey);
        playerUI.ToggleFireCamp(this);
    }

    IEnumerator WaitForKeyPress(KeyCode key)
    {
        bool done = false;
        while(!done) // essentially a "while true", but with a bool to break out naturally
        {
            if(Input.GetKeyDown(key))
            {
                done = true; // breaks the loop
            }
            yield return null; // wait until next frame, then continue execution from here (loop continues)
        }
    }

    void DestroyCampButton()
    {
        if(playerUI.campFirePanel.activeSelf)
            playerUI.ToggleFireCamp(this);

        InventoryItem _campFire = playerInventory.campFireCell.inventoryItem;
        if(_campFire != null)
        {
            GameObject _cloneCampFire = Instantiate(_campFire.itemData.itemObject, transform.position, transform.rotation);
            Item _item =  _cloneCampFire.GetComponent<Item>();
            _item.inventoryItem = _campFire;
            _item.isStart = false;
            playerInventory.DeleteItemInCell(playerInventory.campFireCell.id);

        }
        InventoryItem _combustibleCampFire = playerInventory.campFireCombustibleCell.inventoryItem;
        if(_combustibleCampFire != null)
        {
            GameObject _cloneCombustibleCampFire = Instantiate(_combustibleCampFire.itemData.itemObject, transform.position, transform.rotation);
            Item _item =  _cloneCombustibleCampFire.GetComponent<Item>();
            _item.inventoryItem = _combustibleCampFire;
            _item.isStart = false;
            playerInventory.DeleteItemInCell(playerInventory.campFireCombustibleCell.id);
        }
        Destroy(gameObject);
    }
}
