using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class QuestUI : MonoBehaviour
{
    [SerializeField] PlayerInventory playerInventory;
    bool canClosePanel = false;

    public GameObject questPanel;
    public Image itemImage;
    public Text coinWonText;
    public Text skillWonNameText;

    QuestStep currentOpenedPanelStep;
    QuestGuiver currentOpenedPanelQuestGuiver;

    public GameObject acceptQuestButtonGo;
    public Text questDialogueText;

    [SerializeField] Transform questFollowItemContent;
    List<QuestFollowItemUI> questFollowItemUIs = new List<QuestFollowItemUI>();

    [SerializeField] GameObject questFollowItemPrefab;

    InventoryItem currentQuestInventoryItem = null;

    void Start()
    {
        questPanel.SetActive(false);
    }

    void Update()
    {
        if(questPanel.activeSelf)
            if(Input.GetKeyDown(InputManager.instance.escape) || Input.GetKeyDown(InputManager.instance.useKey))
                if(canClosePanel)
                    ToggleQuestPanel();
    }

    public void SetItem(InventoryItem inventoryItem, Skill skill, QuestStep questStep, QuestGuiver questGuiver)
    {
        currentQuestInventoryItem = inventoryItem;
        currentOpenedPanelQuestGuiver = questGuiver;
        currentOpenedPanelStep = questStep;
        if(!questStep.isStart)
        {
            acceptQuestButtonGo.SetActive(true);
        }
        else
            acceptQuestButtonGo.SetActive(false);

        if(questStep.isWinCoin)
        {
            coinWonText.gameObject.SetActive(true);
            coinWonText.text = questStep.coinWon + " Coins à gagner";
        }
        else
            coinWonText.gameObject.SetActive(false);

        if(inventoryItem != null)
        {
            itemImage.transform.parent.gameObject.SetActive(true);
            itemImage.sprite = inventoryItem.itemData.itemIcone;
        }
        else
            itemImage.transform.parent.gameObject.SetActive(false);
        
        if(skill)
        {
            skillWonNameText.gameObject.SetActive(true);
            skillWonNameText.text = "Le skill '" + skill.skillName + "' sera obtenue";
        }
        else
            skillWonNameText.gameObject.SetActive(false);
    }

    public void AddQuestFollowItem(QuestStep questStep, string pnjName)
    {
        GameObject clone = Instantiate(questFollowItemPrefab, questFollowItemContent);
        QuestFollowItemUI questFollowItem = clone.GetComponent<QuestFollowItemUI>();
        questFollowItemUIs.Add(questFollowItem);
        questFollowItem.pnjNameText.text = "de " + pnjName;
        questFollowItem.pnjName = pnjName;
        questFollowItem.questStep = questStep;

        
        questFollowItem.descritpionText.text = SetDescriptionText(questStep, pnjName);
    }

    string SetDescriptionText(QuestStep questStep, string pnjName)
    {
        string descritpion = "";
        
        if(questStep.enemyToKillPrefab)
        {
            int numberAlreadyKilled = questStep.maxNumberEnemyToKill-questStep.numberEnemyToKill;
            descritpion += numberAlreadyKilled + " sur " + questStep.maxNumberEnemyToKill + " " + questStep.enemyToKillPrefab.GetComponent<GeneralAi>().aiData.AIName;
            if(numberAlreadyKilled > 1)
                descritpion += " tués";
            else
                descritpion += " tué";
        }

        if(questStep.canFinish)
            descritpion += "\n(Quête terminé!)";//utilisation de   (alt+255) pour éviter le retour à la ligne

        return descritpion;
    }

    public void ActualiseQuestFollowText()
    {
        foreach(QuestFollowItemUI questFollow in questFollowItemUIs)
        {
            questFollow.descritpionText.text = SetDescriptionText(questFollow.questStep, questFollow.pnjName);
        }
    }

    public void RemoveQuestFollowItem(QuestStep questStep)
    {
        foreach(QuestFollowItemUI questFollow in questFollowItemUIs)
        {
            if(questFollow.questStep == questStep)
            {
                questFollowItemUIs.Remove(questFollow);
                Destroy(questFollow.gameObject);
                return;
            }
        }
    }

    public void AcceptQuestBttuon()
    {
        currentOpenedPanelQuestGuiver.AcceptQuest(currentOpenedPanelStep);
        acceptQuestButtonGo.SetActive(false);
    }

    public void ToggleQuestPanel()
    {
        if(!questPanel.activeSelf)
            StartCoroutine(WaitClosePanel(questPanel));
        else
            canClosePanel = false;
        
            questPanel.SetActive(!questPanel.activeSelf);
            GetComponent<PlayerUI>().constructionPanel.SetActive(false);
            playerInventory.objectDescriptionPanel.SetActive(false);
            SetTogglePanelParameter(questPanel.activeSelf);
    }
    
    IEnumerator WaitClosePanel(GameObject panel)//Pour ne pas l'ouvrir juste après l'avoir fermé dans PlayerPickAndDropItem
    {
        yield return new WaitForSeconds(0.1f);
        canClosePanel = true;
    }

    void SetTogglePanelParameter(bool isActive)
    {
        PlayerUI.canOpenPanel = !isActive;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().canMove = !isActive;
        GameManager.ToggleCursorStats(isActive);
    }

    public void MouseEnter()
    {
        if(currentQuestInventoryItem == null)
            return;

        playerInventory.SetDescriptionPanelPosition(itemImage.transform.position, itemImage.GetComponent<RectTransform>());
        playerInventory.SetDescriptionText(currentQuestInventoryItem);
    }

    public void MouseExit() 
    {
        playerInventory.objectDescriptionPanel.SetActive(false);
    }
}
