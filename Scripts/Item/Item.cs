using UnityEngine;

public class Item : MonoBehaviour
{
    PlayerPickAndDropItem pickAndDropItem;

    public InventoryItem inventoryItem = new InventoryItem();
    public ItemData itemData;
    public int startStackNumber;

    public AnimationClip useClip;
    [HideInInspector] public bool isStart = true;

    public Collider[] colliders;

    void Start()
    {
        pickAndDropItem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPickAndDropItem>();
        if(isStart)
        {
            SetStart();
        }
    }

    public void SetStart()
    {
        if(!itemData.isStackable)
            startStackNumber = 1;//verifie si peut stack et sinon le met à 1 pour éviter tout problème

        inventoryItem.useNumber = itemData.maxUseNumber;
        inventoryItem.itemData = itemData;
        inventoryItem.stackNumber = startStackNumber;
    }
}
