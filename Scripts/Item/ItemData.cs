using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Survival/ItemData", order = 0)]
public class ItemData : ScriptableObject
{ 
    [Header("Information Principales")]
    public string itemName = "name"; //le nom
    public Sprite itemIcone; // l'image dans l'inventaire
    public GameObject itemObject; // l'objet
    public bool isStackable;//si oui peut être stacké jusqu'à X100
    public int maxUseNumber = 1; //nombre d'utilisation
    public bool isUnusable;//si ouinon ne peut rien faire
    public float usingTime = 0f; //temps d'utilisation d'un objet
    public float timeBeforeReuse = 0.5f; //temps avant de pouvoir réutiliser un item
    public ItemData dropedItemAtDestroy; // l'objet dropé quand détruit ||||(peut être vide)
    public Vector3 positionInHand; //la position de l'objet en main
    public Vector3 rotationInHand; //| la rotation
        
    [Header("Buff/Debuff Stats")]//les stats à par usenumber et armor peuvent être négatives
    public bool isConsomable; // si oui perd de l'usure avec utilisation dans playerUse et donne les stats suivantes
    public float hungerRecup = 0f; //récupération de faim
    public float healthRecup = 0f; //récupération de vie

    [Header("Attaque Stats")]
    public WeaponType weaponType;
    public float attackDamage = 5f; //dégats de l'attaque
    public float attackRange = 2; //range de l'attaque
    [Header("Défense Stats")]
    public bool isArmor; // si il peut être équipé comme arumre et perd de l'usure avec des attaque sur soi
    public float armor = 0f; //taux d'armure donné

    [Header("Cuisine")]
    public bool isCookable; // si oui peut être cuit ||||(require l'objet itemCoockingResult)
    public float coockingTime = 10f;//temps de cuisson (si isCoockable)
    public bool isCombustible; // si oui peut être utilisé comme combustible
    public int combustibleNumberOfTime; // le nombre de fois que peut être utilisé comme combustible avec le max de use number (seulement si isCombustible = true)
    public ItemData itemCoockingResult;// objet donné après cuisson

    public void DestroyItemUsed(GameObject go, PlayerPickAndDropItem playerPickAndDropItem, PlayerUse playerUse, InventoryItem inventoryItem)
    {
        if(dropedItemAtDestroy != null)
        {
            GameObject _clone = Instantiate(dropedItemAtDestroy.itemObject, playerPickAndDropItem.handTransform.position, playerPickAndDropItem.handTransform.rotation);
            Item _item = _clone.GetComponent<Item>();
            _item.SetStart();

            playerPickAndDropItem.playerInventory.IsItemFreeSlot(_item.inventoryItem, _clone);
        }
        inventoryItem.stackNumber--;
        if(inventoryItem.stackNumber <= 0)
            Destroy(go);
    }
    //*: si un moyen pour ne pas utiliser l'item dans la main quand il spawn après la destruction d'un autre est trouvé alors suprimmer ligne

    public enum WeaponType
    {
        NoWeapon,

        Sword,
        Dagger
    }
}
