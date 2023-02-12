using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Quest", menuName = "Survival/Quest", order = 0)]
public class QuestScriptableObject : ScriptableObject
{
    public bool questIsStart = false;
    public List<QuestStep> questSteps = new List<QuestStep>();
}

[System.Serializable]
public class QuestStep
{
    [HideInInspector] public bool isStart = false;//si quête démaré
    [HideInInspector] public bool canFinish = false;//si peut être fini (en allant voir le pnj)
    [HideInInspector] public bool isFinish = false;//si l'étape est fini

    public string pnjStartPhrase;//la phrase que le pnj dit au début avant de donner la quête
    public string pnjInQuestPhrase;//la phrase que le pnj dit pendant la quête
    public string pnjEndPhrase;//la phrase que le pnj dit à la fin de la quête (avec les récompenses)

    public int numberEnemyToKill = 1;//le nombre current d'enemie à tuer
    public int maxNumberEnemyToKill;//le nombre total d'enemie à tuer (pour le ui questFollowItem)
    public GameObject enemyToKillPrefab;//l'enemie à tuer

    public bool isWinCoin;//Si gagne des coin (player.coin) après mission
    public int coinWon;//nombre de coin gagné

    public GameObject itemWon;//item gagné après la mission
    
    public bool isWinSkill;//si oui peut gagné un skill
    public PlayerSkillsManager.SkillsIDType skillWon;//le skill gagné
}
