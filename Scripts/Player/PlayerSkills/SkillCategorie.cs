using UnityEngine;
using System.Collections.Generic;

[System.Serializable, CreateAssetMenu(fileName = "SkillCategorie", menuName = "Survival/SkillCategorie", order = 0)]
public class SkillCategorie: ScriptableObject//catégorie de skill comme combatant, survivant, assassin
{
    public PlayerSkillsManager.SkillCategoriesType categorieType;
    public string categorieName;
    public bool categorieIsUse;
    public List<Skill> skills = new List<Skill>();

    public int categorieLvl = 0;//l'xp actuel
    public float currentCategorieXp = 0f;//l'xp requis pour passer le lvl suivant
    public float addXpPerLvlSkill = 75f;//l'xp gagné pour un skill de lvl 1
    public float reachCategorieXp = 100f;//reachCategorieXp += 25 * categorieLvl/((categorieLvl+1)/2) pour chaque lvl sup
}

