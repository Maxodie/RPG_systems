using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] PlayerUse playerUse;
    [SerializeField] PlayerCaracteristiqueStats pcs;
    [SerializeField] Player player;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] PlayerPickAndDropItem playerPickAndDropItem;
    [SerializeField] PlayerSkillsManager playerSkillsManager;
    [SerializeField]
    float punchDamage = 5f;
    [SerializeField]
    float punchRange = 5f;
    [SerializeField]
    LayerMask hitMask;
    [SerializeField]
    Camera cam;

    public void PrepareAttack()
    {
        if(!player.isAlive || !PlayerUI.canOpenPanel || !playerUse.canUse)
        {return;}

        float range = 0f;
        float damage = 0f;
        ItemData.WeaponType weaponType = ItemData.WeaponType.NoWeapon;
        if(playerUse.itemInHand != null)
        {
            ItemData _playerItemInHandItemData = playerUse.itemInHand.GetComponent<Item>().inventoryItem.itemData;
            weaponType = _playerItemInHandItemData.weaponType;
            if(_playerItemInHandItemData.weaponType != ItemData.WeaponType.NoWeapon)
            {
                range = _playerItemInHandItemData.attackRange;
                damage = _playerItemInHandItemData.attackDamage;
                switch (_playerItemInHandItemData.weaponType)
                {
                    case ItemData.WeaponType.Sword:
                        damage *= (1+pcs.GetSwordBonusDamage(true)/100);//les compétences passive, % de dégâts avec une épé
                    break;
                }
            }
            else
                return;
        }
        else
        {
            range = punchRange;
            damage = punchDamage;
        }
        GeneralAi generalAi = IsTuchEnemy(range);
        if(generalAi)
        {
            damage += Mathf.RoundToInt(Mathf.Sqrt(pcs.GetPlayerStrength(true))*1.2f);//calcule des dégâts que va resevoir l'enemi
            damage = Mathf.RoundToInt(damage);
            Attack(damage, generalAi, weaponType);
            if(playerUse.itemInHand != null)
            {
                Item _item = playerUse.itemInHand.GetComponent<Item>();
                if(_item.inventoryItem.itemData.weaponType != ItemData.WeaponType.NoWeapon)
                {
                    playerInventory.cells[playerInventory.primaryHandItem.id].ItemTakeDamage(1);
                    _item.inventoryItem.useNumber = playerInventory.cells[playerInventory.primaryHandItem.id].inventoryItem.useNumber;
                }
            }
        }
        
    }

    void Attack(float damage, GeneralAi enemy, ItemData.WeaponType weaponType)
    {
        enemy.TakeDamage(damage, transform, weaponType);
    }

    GeneralAi IsTuchEnemy(float range)
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, hitMask))
            if(hit.transform.CompareTag("GeneralAi"))
                return hit.transform.GetComponent<GeneralAi>();
        
        return null;
    }
}
