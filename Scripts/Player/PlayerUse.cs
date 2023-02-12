using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerUse : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] PlayerAttack playerAttack;
    public PlayerPickAndDropItem playerPickAndDropItem;
    public PlayerInventory playerInventory;
    public PlayerUI playerUI;

    [HideInInspector]
    public GameObject itemInHand;
    [HideInInspector]
    public bool canUse = true;

    void Start()
    {
        canUse = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(InputManager.instance.inHandUse))
        {
            UseItem();
        }
    }

    public void SetItemInHand(GameObject newItemInHand)
    {
        if(itemInHand != null)
        {
            if(itemInHand.GetComponent<Animator>())
                itemInHand.GetComponent<Animator>().Play("Idle");
        }
        StopAllCoroutines();
        itemInHand = newItemInHand;
        canUse = true;
    }

    void UseItem()//verify si peut utiliser lance l'anim et recupère les refs
    {
        if(itemInHand == null || !player.isAlive || !canUse || !PlayerUI.canOpenPanel)
        {
            if(itemInHand == null)
                playerAttack.PrepareAttack();
            return;
        }

        Item item = itemInHand.GetComponent<Item>();
        if(item.inventoryItem.itemData.isUnusable)
            return;

        Animator _anim = item.GetComponent<Animator>();
        if(item.inventoryItem.itemData.weaponType == ItemData.WeaponType.NoWeapon)
            _anim.SetFloat("Speed", item.useClip.length/item.inventoryItem.itemData.usingTime);
        _anim.SetTrigger("Use");
        
        StartCoroutine(UsingTime(item));
        
    }

    IEnumerator UsingTime(Item item)//temps avant que l'item soit utilisé
    {
        if(item.inventoryItem.itemData.weaponType != ItemData.WeaponType.NoWeapon)
            playerAttack.PrepareAttack();
        canUse = false;
        yield return new WaitForSeconds(item.inventoryItem.itemData.usingTime);
        UseStats(item);
    }

    void UseStats(Item item)//utilisation de l'item
    {
        ItemData itemData =  item.inventoryItem.itemData;
        StartCoroutine(TimeBeforeUse(itemData));
        if(itemData.isArmor)
        {
            if(!playerInventory.armorItem.isUsedCell && itemInHand != null)
                itemData.DestroyItemUsed(itemInHand, playerPickAndDropItem, this, item.inventoryItem);
            
            playerInventory.EquipeItemFromInventory(playerInventory.primaryHandItem.id, item.inventoryItem);
            return;
        }
        if(!itemData.isConsomable)
            return;
            
        float _foodIncreased = itemData.hungerRecup;
        float _healthIncreased = itemData.healthRecup;
        player.IncreaseCurrentPlayerStats(_healthIncreased, _foodIncreased);

        playerInventory.cells[playerInventory.primaryHandItem.id].ItemTakeDamage(1);//inventoryItem slot = à celui de l'item physique
        item.inventoryItem.useNumber = playerInventory.cells[playerInventory.primaryHandItem.id].inventoryItem.useNumber;
    }

    IEnumerator TimeBeforeUse(ItemData itemData)//temps avant de réutiliser un item
    {
        yield return new WaitForSeconds(itemData.timeBeforeReuse);
        canUse = true;
    }
}
