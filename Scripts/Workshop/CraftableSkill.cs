using UnityEngine;

[CreateAssetMenu(fileName = "CraftableSkill", menuName = "Survival/CraftableSkill", order = 0)]
public class CraftableSkill : ScriptableObject
{
    [Header("Infos Générale")]
    public Skill craftedSkill;
    public int categorieLvlRequired;
    [HideInInspector] public WorkshopItemUI workshopItemUI;

    [Header("Item(s) requis")]
    public ItemData[] requiredItems;
    public int[] requireItemNumbers;
}
