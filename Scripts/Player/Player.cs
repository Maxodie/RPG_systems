using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInventory playerInventory;
    [SerializeField] PlayerController playerController;
    public Camera cam;
    [SerializeField] PlayerCaracteristiqueStats pcs;

    [HideInInspector]
    public bool isAlive = true;
    public float currentPlayerLife;
    public float GetHealthPct()
    {
        return (float)currentPlayerLife / pcs.GetMaxPlayerLife(false);
    }

    public float currentPlayerHunger;
    [SerializeField]
    float decreaseHungerWithTime = 1;
    [SerializeField]
    float hungerDamage = 5;
    public float GetHungerPct()
    {
        return (float)currentPlayerHunger / pcs.GetMaxPlayerHunger();
    }

    public float currentPlayerArmor;

    [SerializeField]
    float increaseManaAmount = 0.5f;
    [HideInInspector]
    public bool canIncreaseMana = true;
    public float currentMana;
    public float GetManaPct()
    {
        return (float)currentMana / pcs.GetMaxPlayerMana();
    }

    public int coin;

    void Start() 
    {
        SetPlayerStats();
        canIncreaseMana = true;
    }

    void Update() 
    {
        DecreasePlayerStats(decreaseHungerWithTime);
        if(canIncreaseMana)
            IncreaseMana(increaseManaAmount);
    }

    void SetPlayerStats()
    {
        currentPlayerLife = pcs.GetMaxPlayerLife(false);
        currentPlayerHunger = pcs.GetMaxPlayerHunger();
        currentMana = pcs.GetMaxPlayerMana();
    }

    public void ChangeCoinCount(int amount, bool isNegative)
    {
        if(isNegative)
            coin -= amount;
        else
            coin += amount;
        playerInventory.GetComponent<PlayerUI>().SetCoinText();
    }

    void DecreasePlayerStats(float _hungerAmount)
    {
        if(currentPlayerHunger < 0)
        {
            TakeDamage(hungerDamage);
            return;
        }
        currentPlayerHunger -= _hungerAmount * Time.deltaTime;
    }

    void IncreaseMana(float _amount)
    {
        if(currentMana < pcs.GetMaxPlayerMana())
            currentMana += _amount * Time.deltaTime;
    }

    public void IncreaseCurrentPlayerStats(float _healthHealing, float _foodIncreased)
    {
        currentPlayerLife += _healthHealing;
        if(currentPlayerLife > pcs.GetMaxPlayerLife(false))
        {
            currentPlayerLife = pcs.GetMaxPlayerLife(false);
        }

        currentPlayerHunger += _foodIncreased;
        if(currentPlayerHunger > pcs.GetMaxPlayerHunger())
        {
            currentPlayerHunger = pcs.GetMaxPlayerHunger();
        }
    }

    public void SetArmor()
    {
        InventoryItem inventoryItem = playerInventory.armorItem.inventoryItem;
        if(inventoryItem != null)
        {
            currentPlayerArmor = inventoryItem.itemData.armor;
        }
        else
        {
            currentPlayerArmor = 0;
        }
    }

    public void TakeDamage(float _amount)
    {
        if(!isAlive)
        {return;}

        PlayerInventoryCell armorItemCell = playerInventory.armorItem;
        float _damage = _amount -(currentPlayerArmor/35+pcs.GetPlayerResistance()/10);
        _damage = Mathf.FloorToInt(_damage*(1-pcs.GetPlayerDodge(true)/100));
        if(armorItemCell.isUsedCell)
        {
            armorItemCell.ItemTakeDamage(1);
        }
        if(_damage > 0)
        {
            currentPlayerLife -= _damage;
            pcs.GetMaxPlayerLife(true);//augmente l'xp lié aux compétences qui donne de la vie
        }
        if(currentPlayerLife <= 0)
            Death();
    }

    void Death()
    {
        Debug.Log("bruh");
        playerController.canMove = false;
        isAlive = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}