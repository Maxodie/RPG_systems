using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class SkillsPanelManager : MonoBehaviour
{
    [SerializeField] PlayerSkillsManager playerSkillsManager;
    [SerializeField] PlayerUseSkill playerUseSkill;
    [SerializeField]
    public SkillCategorieUIItem[] skillCategorieUIItems;
    
    [SerializeField]
    GameObject passiveSkillItemPrefab;
    [SerializeField]
    GameObject actifSkillItemPrefab;

    void Start() 
    {
        for(int i=0; i<skillCategorieUIItems.Length; i++)
        {
            skillCategorieUIItems[i].currentActiveSkill = new Skill[skillCategorieUIItems[i].activeSkillDropdowns.Length];
        }

        UpdateSkillCategorieUIItem();
    }

    public void UpdateSkillCategorieUIItem()//pour afficher le ui des catégories
    {
        for(int i=0; i<skillCategorieUIItems.Length; i++)
        {
            SkillCategorie _categorie = playerSkillsManager.GetCategorie(skillCategorieUIItems[i].skillCategoriesType);
            skillCategorieUIItems[i].descriptionText.text = "<u><b>Classe</b></u>\nNom: \n" + _categorie.categorieName + "\n";
            if(_categorie.categorieIsUse)
                skillCategorieUIItems[i].descriptionText.text += "<b>Utilisé</b>";
            
            skillCategorieUIItems[i].xpText.text = _categorie.currentCategorieXp + "/" + _categorie.reachCategorieXp;
            skillCategorieUIItems[i].xpFill.fillAmount = _categorie.currentCategorieXp/_categorie.reachCategorieXp;
            skillCategorieUIItems[i].lvlText.text = "Niveau: " + _categorie.categorieLvl;
        }
    }

    public void ChangeTab(int id)//pour changer l'onglet de classe
    {
        for(int i=0; i<skillCategorieUIItems.Length; i++)
            if(i == id)
                skillCategorieUIItems[i].page.SetActive(true);
            else
                skillCategorieUIItems[i].page.SetActive(false);
    }

    public void AddSkillUIItem(Skill skill)//pour ajouter un item(skill) visuelement dans une classe
    {
        for(int i=0; i<skillCategorieUIItems.Length; i++)
        {
            if(skillCategorieUIItems[i].skillCategoriesType == skill.skillCategorieType)
            {
                GameObject _clone = null;
                if(!skill.isActiveSkill)
                {
                    _clone = Instantiate(passiveSkillItemPrefab, skillCategorieUIItems[i].pagesContents);
                }
                else
                {
                    _clone = Instantiate(actifSkillItemPrefab, skillCategorieUIItems[i].pagesContents);
                }
                _clone.GetComponent<SkillUI>().skillIDType = skill.skillType;

                if(skill.isActiveSkill)//si c'est un skill actif
                {
                    SkillUI _skillUI = playerSkillsManager.GetSkillUI(skill.skillType);
                    _skillUI.rechargeFill.fillAmount = 1;
                    _skillUI.rechargeFill.color = _skillUI.activableColor;
                    UpdateEquipedActiveSkillDropdown(i);
                }

                UpdateSkillUI();
            }
        }
    }

    void UpdateEquipedActiveSkillDropdown(int skillCategorieUIItemsID)
    {
        foreach(Dropdown _dropdown in skillCategorieUIItems[skillCategorieUIItemsID].activeSkillDropdowns)//pour chaque dropdown de skill actif(les équipers)
        {
            _dropdown.ClearOptions();

            List<string> options = new List<string>();

            for(int i=0; i<playerSkillsManager.GetAllUsedSkill().Length; i++)
            {
                Skill _skill = playerSkillsManager.GetAllUsedSkill()[i];
                if(_skill.isActiveSkill && _skill.skillCategorieType == skillCategorieUIItems[skillCategorieUIItemsID].skillCategoriesType)
                {
                    string option = _skill.skillName;
                    options.Add(option);
                }
            }

            _dropdown.AddOptions(options);
            _dropdown.RefreshShownValue();
            ChangeActiveSkillDropdown(_dropdown);
        }
    }

    public void UpdateSkillUI()
    {
        foreach(Skill _skill in playerSkillsManager.GetAllUsedSkill())
        {
            SkillUI _skillUI = playerSkillsManager.GetSkillUI(_skill.skillType);
            _skillUI.titleText.text = "<b>" + _skill.skillName + "</b>";
            _skillUI.descriptiontext.text = SetSkillUIDescription(_skill); 
        }
    }

    public string SetSkillUIDescription(Skill _skill)
    {
        bool canSeeDurationTime = false;
        string descriptiontext = ""; 
        if(_skill.isInstantiateSkill)
        {
            if(_skill.instantiateSkillDamage != 0)
                descriptiontext += _skill.instantiateSkillDamage + " points de dégâts";
            if(_skill.isUpgradingDestroyTime)
                descriptiontext += _skill.instantiateSkillDestroyTime + "s avant destruction";
        }
        if(_skill.swordDamage!=0)
        {
            descriptiontext += "+" + _skill.swordDamage + "% de dégâts à l'épé\n";
            canSeeDurationTime = true;
        }
        if(_skill.increasedPlayerLife!=0)
        {
            if(_skill.canIncreaseUpperThanCurrentHealth)
            {
                descriptiontext += "+" + _skill.increasedPlayerLife + " points de vie en plus";
                canSeeDurationTime = true;
            }
            if(_skill.isPermanentStatsSkill)
                descriptiontext += "Soin de " + _skill.increasedPlayerLife + " points de vie";
        }
        if(_skill.increasePlayerDodge!=0)
        {
            descriptiontext += "+" + _skill.increasePlayerDodge + "% d'ésquive des dégâts";
            canSeeDurationTime = true;
        }
        if(_skill.increasePlayerStrength!=0)
        {
            descriptiontext += "+" + _skill.increasePlayerStrength + " points de force en plus";
            canSeeDurationTime = true;
        }
        if(_skill.increasePlayerSpeed!=0)
        {
            descriptiontext += "+" + _skill.increasePlayerSpeed + "% de vitesse en plus";
            canSeeDurationTime = true;
        }
        SkillUI _skillUI = playerSkillsManager.GetSkillUI(_skill.skillType);
        if(_skillUI)
        {
            if(canSeeDurationTime)
                AddDurationTimeActiveText(_skillUI.descriptiontext, _skill);
            _skillUI.xpFill.fillAmount = _skill.currentXp/_skill.reachXp;
            _skillUI.xpText.text = _skill.currentXp + "/" + _skill.reachXp;
        }
        
        return descriptiontext;
    }

    void AddDurationTimeActiveText(Text text, Skill _skill)
    {
        if(_skill.isActiveSkill)
            text.text += " pendant " + _skill.duringActiveTime + "s";
    }

    public void ActiveCategorieButton(int id)//bouton pour activer une catégorie
    {
        for(int i=0; i<playerSkillsManager.skillCategories.Length; i++)//pour toutes les catégories
        {
            if((int)playerSkillsManager.skillCategories[i].categorieType == id)//si l'id de l'enum des catégorie lié à la catégorie est la même que l'id sur le bouton
            {
                playerSkillsManager.ActiveCategorieButton(playerSkillsManager.skillCategories[i].categorieType);//envoi la catégorie(enum) lié à l'id
            }
        }
    }

    public void ChangeActiveSkillDropdown(Dropdown usedDropDown)//quand on change le dropDown pour équiper les skill actifs
    {
        Skill[] skillsIDTypes=null;
        for(int i=0; i<skillCategorieUIItems.Length; i++)
        {
            for(int y=0; y<skillCategorieUIItems[i].activeSkillDropdowns.Length; y++)//pour chaque dropdown d'équipement de skill actif
            {
                if(skillCategorieUIItems[i].activeSkillDropdowns[y].gameObject == usedDropDown.gameObject)//si true alor dropdown trouvé
                {
                    skillCategorieUIItems[i].currentActiveSkill[y] = playerSkillsManager.GetPossessSkillWithName(usedDropDown.options[usedDropDown.value].text);
                    skillsIDTypes = new Skill[skillCategorieUIItems[i].currentActiveSkill.Length];
                    for(int z=0; z<skillsIDTypes.Length; z++)
                    {
                        skillsIDTypes[z] = skillCategorieUIItems[i].currentActiveSkill[z];
                    }
                    if(playerSkillsManager.GetCategorie(skillCategorieUIItems[i].skillCategoriesType).categorieIsUse)
                        playerUseSkill.SetActiveSkill(skillsIDTypes);
                }
            }
        }
    }

    public void ChangeActiveSkillKeyButton(Button _activeSkillKeyButtons)
    {
        _activeSkillKeyButtons.GetComponentInChildren<Text>().text = "...";
        StartCoroutine(WaitForKeyPress(_activeSkillKeyButtons));
    }

    void ChangeActiveSkillKey(KeyCode key, Button _activeSkillKeyButtons)
    {
        foreach(SkillCategorieUIItem _skillCategorieUIItem in skillCategorieUIItems)
        {
            for(int i=0; i<_skillCategorieUIItem.activeSkillKeyButtons.Length; i++)
            {
                if(_skillCategorieUIItem.activeSkillKeyButtons[i].gameObject == _activeSkillKeyButtons.gameObject)
                {
                    for(int y=0; y<_skillCategorieUIItem.activeSkillKeys.Length; y++)
                    {
                        if(_skillCategorieUIItem.activeSkillKeys[y] == key)
                        {
                            _skillCategorieUIItem.activeSkillKeys[y] = KeyCode.None;
                            _skillCategorieUIItem.activeSkillKeyButtons[y].GetComponentInChildren<Text>().text = KeyCode.None.ToString();
                        }
                    }
                    _skillCategorieUIItem.activeSkillKeys[i] = key;
                    _skillCategorieUIItem.activeSkillKeyButtons[i].GetComponentInChildren<Text>().text = key.ToString();
                    if(playerSkillsManager.GetCategorie(_skillCategorieUIItem.skillCategoriesType).categorieIsUse)
                    {
                        playerUseSkill.SetActiveSkillKey(_skillCategorieUIItem.activeSkillKeys);
                    }
                }
            }
            
        }
    }

    IEnumerator WaitForKeyPress(Button _activeSkillKeyButtons)
    {
        bool done = false;
        while(!done) // essentially a "while true", but with a bool to break out naturally
        {
            foreach(KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if(Input.GetKeyDown(key))
                {
                    done = true; // breaks the loop
                    ChangeActiveSkillKey(key, _activeSkillKeyButtons);
                }
            }
                yield return null; // wait until next frame, then continue execution from here (loop continues)
        }
    }
}

[System.Serializable]
public class SkillCategorieUIItem//les info d'une categorie dans le ui
{
    public PlayerSkillsManager.SkillCategoriesType skillCategoriesType;
    public GameObject page;
    public Transform pagesContents;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI xpText;
    public Image xpFill;
    public TextMeshProUGUI lvlText;
    
    public Dropdown[] activeSkillDropdowns;
    [HideInInspector]public Skill[] currentActiveSkill;//taille toujours = à activeSkillDropdowns.Length
    public Button[] activeSkillKeyButtons;
    public KeyCode[] activeSkillKeys;
}
