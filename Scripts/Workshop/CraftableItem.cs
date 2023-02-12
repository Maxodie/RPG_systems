using UnityEngine;

[CreateAssetMenu(fileName = "CraftableItem", menuName = "Survival/CraftableItem", order = 0)]
public class CraftableItem : ScriptableObject
{
    [Header("Infos Générale")]
    public ItemData craftedItem;
    public int categorieLvlRequired;
    public PlayerSkillsManager.SkillCategoriesType categoriesType;//stocké quand instantié
    
    [Header("Achetable")]
    public bool isBuyable = true;
    public int requiredCoinsNumber;

    [Header("Fabricable")]
    public bool isCraftable = true;
    public ItemData[] requiredItems;
    public int[] requireItemNumbers;

    [HideInInspector] public WorkshopItemUI workshopItemUI;
    [HideInInspector] public WorkshopItemUI shopWorkshopItemUI;//si achetable stocké quand instantié
}
