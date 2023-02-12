using UnityEngine;
using System.Collections;

public class QuestGuiver : MonoBehaviour
{
    public string pnjName;

    [SerializeField] QuestScriptableObject questData;
    public Transform[] spawnEnemyTr;//la pos de l'enemie (instantié)
    PlayerUI playerUI;
    QuestUI questUI;
    PlayerPickAndDropItem playerPickAndDropItem;
    DialogueSystem dialogueSystem;
    PlayerSkillsManager playerSkillsManager;

    void Start()
    {
        //laisser les getComponent car peut être instantié (rapport à l'heur?, à l'avancement?)
        playerSkillsManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkillsManager>();
        playerUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerUI>();
        playerPickAndDropItem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPickAndDropItem>();
        dialogueSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DialogueSystem>();
        questUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<QuestUI>();

        questData.questIsStart = false;
        foreach(QuestStep step in questData.questSteps)//!!\\A SUPPRIMER POUR FINAL BUILD//!!\\
        {
            step.isFinish = false;
            step.canFinish = false;
            step.isStart = false;
        }////////
    }

    void StartQuest()
    {
        for(int i=0; i<questData.questSteps.Count; i++)
        {
            if(!questData.questSteps[i].isFinish && !questData.questSteps[i].canFinish)
            {
                InventoryItem _inventoryItem = null;
                if(questData.questSteps[i].itemWon)
                {
                    Item item = questData.questSteps[i].itemWon.GetComponent<Item>();
                    item.SetStart();//set l'inventoryItem de l'item
                    _inventoryItem = item.inventoryItem;
                }
                Skill _skill = null;
                if(questData.questSteps[i].isWinSkill)
                    _skill = playerSkillsManager.GetSkillWithType(questData.questSteps[i].skillWon);

                questUI.SetItem(_inventoryItem, _skill, questData.questSteps[i], this);

                if(!questUI.questPanel.activeSelf)
                    questUI.ToggleQuestPanel();

                if(questData.questSteps[i].isStart)//si la quête est déjà fini oui commencé
                {
                    dialogueSystem.MakeDialogue(questData.questSteps[i].pnjInQuestPhrase, questUI.questDialogueText, null);
                    break;
                }
                
                dialogueSystem.MakeDialogue(questData.questSteps[i].pnjStartPhrase, questUI.questDialogueText, null);
                break;
            }
        }
    }

    public void AcceptQuest(QuestStep questStep)
    {
        if(questStep.isStart)
            return;
        for(int y=0; y<questStep.numberEnemyToKill ; y++)
        {
            int randInt = Random.Range(0, spawnEnemyTr.Length);
            Transform randTr = spawnEnemyTr[randInt];
            Vector3 pos = randTr.position;
            if(Physics.Raycast(randTr.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                pos.y = hit.point.y;
            
            GameObject _clone = Instantiate(questStep.enemyToKillPrefab, pos, randTr.rotation);
            StartCoroutine(WaitForKillEnemies(questStep));
        }
        questUI.AddQuestFollowItem(questStep, pnjName);//ajouter le suivit de quête
        questData.questIsStart = true;
        questStep.isStart = true;
    }

    IEnumerator WaitForKillEnemies(QuestStep step)
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        bool isDone = false;
        while(!isDone)
        {
            if(step.numberEnemyToKill <= 0)
                isDone = true;
            
            yield return wait;
        }
        FinishStep(step);
    }

    void FinishStep(QuestStep step)
    {
        step.canFinish = true;
        questData.questIsStart = false;
        questUI.ActualiseQuestFollowText();
    }

    void FinishQuest()
    {
        for(int i=0; i<questData.questSteps.Count; i++)
        {
            if(questData.questSteps[i].canFinish && !questData.questSteps[i].isFinish)
            { 
                InventoryItem inventoryItem = null;
                GameObject _item = questData.questSteps[i].itemWon;
                if(_item)
                    inventoryItem = _item.GetComponent<Item>().inventoryItem;
                Skill _skillWon = null;

                if(questData.questSteps[i].isWinCoin)
                    playerPickAndDropItem.GetComponent<Player>().ChangeCoinCount(questData.questSteps[i].coinWon, false);

                if(questData.questSteps[i].isWinSkill)
                {
                    _skillWon = playerSkillsManager.GetSkillWithType(questData.questSteps[i].skillWon);
                    playerSkillsManager.AddSkill(questData.questSteps[i].skillWon);
                }
                questUI.SetItem(inventoryItem, _skillWon, questData.questSteps[i], this);
                questUI.ToggleQuestPanel();
                dialogueSystem.MakeDialogue(questData.questSteps[i].pnjEndPhrase, questUI.questDialogueText, null);
                questUI.RemoveQuestFollowItem(questData.questSteps[i]);//supprimer le suivit de quête
                
                if(questData.questSteps[i].itemWon)
                {
                    GameObject _clone = Instantiate(questData.questSteps[i].itemWon, playerPickAndDropItem.transform.position, playerPickAndDropItem.transform.rotation);
                    Item item = _clone.GetComponent<Item>();
                    item.SetStart();
                    playerPickAndDropItem.PickItem(_clone, inventoryItem);
                }

                questData.questSteps[i].isFinish = true;
                return;
            }
        }
    }

    public void Talk()
    {
        if(!PlayerUI.canOpenPanel)
            return;

        for(int i=0; i<questData.questSteps.Count; i++)
        {
            if(questData.questSteps[i].isFinish)
                continue;

            if(!questData.questSteps[i].canFinish)
                StartQuest();
            else
                FinishQuest();
            
            break;
        }
    }
}
