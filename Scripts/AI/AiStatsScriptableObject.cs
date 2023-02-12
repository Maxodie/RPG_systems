using UnityEngine;

[CreateAssetMenu(fileName = "AiStatsScriptableObject", menuName = "Survival/AiStatsScriptableObject", order = 0)]
public class AiStatsScriptableObject : ScriptableObject 
{
    [Header("Stats")]
    public string AIName;
    public float maxLife = 50f;
    public ItemData.WeaponType vulnerableWeapon;
    public float vulnerablepct;

    [Header("Chasing")]
    public bool isChasing;
    public bool isEscapeWheneHit;
    public float escapeSpeedMultiplicator = 1.3f;

    [Header("Move")]
    public float maxTimeToChangeDestination = 5f;
    public float minTimeToChangeDestination = 2f;
    public float maxDistanceAfterSpawnCanMove = 10f;

    [Header("Récompence")]
    public GameObject itemDropAtDeath;
    public int minCoinWonAtDeath;//le nombre de coin min gagnable
    public int maxCoinWinAtDeath;//le nombre de coin max gagnable
    public bool isWinSkill = true;//si gagne un skill
    public PlayerSkillsManager.SkillsIDType skillToAddAtDeath;
    public ItemData.WeaponType requireWeaponType;//si sur noWeapon alors pas besoin d'arme spécial
}
