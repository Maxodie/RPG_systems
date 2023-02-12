using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DataBase", menuName = "Survival/DataBase", order = 0)]
public class DataBase : ScriptableObject
{
    public CraftableSkill[] craftableSkills;
    public CraftableItem[] craftableItems;

    public QuestScriptableObject[] questsData;

    public AiStatsScriptableObject[] aiStatsData;

    public List<Skill> skillsDataList;
    public SkillCategorie[] skillCategories;
}
