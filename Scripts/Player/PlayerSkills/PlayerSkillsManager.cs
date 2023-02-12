using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[RequireComponent(typeof(PlayerCaracteristiqueStats))]
public class PlayerSkillsManager : MonoBehaviour
{
    [SerializeField] PlayerCaracteristiqueStats pcs;
    public SkillsPanelManager skillsPanelManager;
    [SerializeField] Player player;
    [SerializeField] PlayerUseSkill playerUseSkill;
    
    public SkillCategorie[] skillCategories;

    [HideInInspector] public DataBase dataBase;

    void Start() 
    {
        dataBase = GameManager.dataBase;
        skillCategories = dataBase.skillCategories;
        CheckIfSkillsAreSets();
        ActiveCategorieButton(skillCategories[0].categorieType);
        foreach(Skill skill in dataBase.skillsDataList)//à supprimer pour le build avec save (reset les stats pour les test in unity)
        {
            skill.canUse = true;
            skill.currentXp = 0;
            skill.skillLvl = 0;
        }
    }

    void Update() //TRICHE
    {//TRICHE
        if(Input.GetKeyDown(KeyCode.M))//ajouter un skill pour test//TRICHE
        {//TRICHE
            foreach(Skill skill in dataBase.skillsDataList)//TRICHE
            {//TRICHE
                AddSkill(skill.skillType);//TRICHE
            }//TRICHE
        }//TRICHE
    }//TRICHE

    void CheckIfSkillsAreSets()//verifie si il y a pas plusieur skills qui on le meme identifiant type ou si un skillsIDType n'est pas utilisé
    {
        foreach(SkillsIDType _skill in System.Enum.GetValues(typeof(SkillsIDType)))
        {
            bool canContinue = false;
            SkillsIDType _errorSkillNotAssigned = _skill;
            for(int i=0; i<dataBase.skillsDataList.Count; i++)
            {
                if(dataBase.skillsDataList[i].skillType == _skill)
                {
                    if(canContinue)
                        Debug.LogError("Le skill: " + _errorSkillNotAssigned + " est déjà set à un autre item que le skill: " + dataBase.skillsDataList[i].skillName);
                    canContinue = true;
                }
            }
            if(!canContinue)
            {
                Debug.LogError("Le skill: " + _errorSkillNotAssigned + " n'est pas relié à un item, veuillez le supprimer ou l'assigné à un skill");
            }
        }
        //verifie le si deux skill on le même nom
        List<string> skillName = new List<string>();
        foreach(Skill _skill in dataBase.skillsDataList)
        {
            for(int i=0; i<skillName.Count; i++)
            {
                if(skillName[i] == _skill.skillName)
                {
                    Debug.LogError("Deux skill on le même nom!: " + _skill.skillName);
                }
            }
            skillName.Add(_skill.skillName);
        }
    }

    public void AddSkill(SkillsIDType skillToAdd)//ajouter un skill au joueur et le stocker selon sa catégorie
    {
        foreach(Skill _skill in GetAllUsedSkill())//pour chaque catéorie
        {
            Debug.Log("0");
            if(skillToAdd == _skill.skillType)//si le skill est déjâ présent return
            {Debug.Log("2");
            return;}
        }
        Debug.Log("t");
        Skill _skillScriptToAdd = null;
        int _categorieWhereAddId = 0;
        for(int y=0; y<dataBase.skillsDataList.Count; y++)//for tout les skills existant
        {
            if(dataBase.skillsDataList[y].skillType == skillToAdd)//si le Type du skill(enum) == skilltoAdd(enum)
            {
                _skillScriptToAdd = dataBase.skillsDataList[y];//met le bon skill présent dans aucune catégorie dans la variable temporaire
                for(int z=0; z< skillCategories.Length; z++)
                {
                    if(skillCategories[z].categorieType == _skillScriptToAdd.skillCategorieType)//si c'est la catégorie du skill
                    {
                        _categorieWhereAddId = z;//set l'id de la catégorie du skill dans la variable temporaire _categorieWhereAddId
                    }
                }
            }
        }
        skillCategories[_categorieWhereAddId].skills.Add(_skillScriptToAdd);//ajoute le skill dans la bonne catégorie
        skillsPanelManager.AddSkillUIItem(_skillScriptToAdd);//ajoute le skill en UI
        SetPassiveSkillStats();//actualiser les stats des compétences
        GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<WorkshopUI>().UpdateAllUI();//update le ui de l'atelier
    }

    public Skill GetSkillWithType(SkillsIDType skillsIDType)
    {
        Skill _skill = null;
        foreach(Skill skill in dataBase.skillsDataList)
        {
            if(skill.skillType == skillsIDType)
                return _skill = skill;
        }
        return null;
    }

    public SkillCategorie GetCategorie(SkillCategoriesType type)//récupérer une catégorie par rapport à son type (enum)(surtout utilisé pour set un skill dans une catégorie dans le UI)
    {
        for(int i=0; i < skillCategories.Length; i++)
        {
            if(skillCategories[i].categorieType == type)
            {
                return skillCategories[i];
            }
        }
        return null;
    }

    public SkillUI GetSkillUI(PlayerSkillsManager.SkillsIDType skillIDType)//pour récupérer skillUI correspondant au skill
    {
        foreach(SkillCategorieUIItem _skillCategorieUIItems in skillsPanelManager.skillCategorieUIItems)//pour chaque categorieUI
        {
            for(int i=0; i<_skillCategorieUIItems.pagesContents.childCount; i++)//pour chaque skillsUI
            {
                SkillUI _skillUI = _skillCategorieUIItems.pagesContents.GetChild(i).GetComponent<SkillUI>();//recup le skillUI
                if(_skillUI.skillIDType == skillIDType)
                {
                    return _skillUI;
                }
            }
        }
        return null;
    }

    public void ActiveCategorieButton(SkillCategoriesType _skillCategorie)//pour activer une catégorie et désactiver toutes les autres
    {
        if(GetCategorie(_skillCategorie).categorieIsUse || playerUseSkill.routinePlayerUseSkillCurentPlayNumber > 0)
            return;

        for(int i=0; i<skillCategories.Length; i++)
        {
            if(skillCategories[i].categorieType == _skillCategorie)
            {
                skillCategories[i].categorieIsUse = true;
            }
            else
            {
                skillCategories[i].categorieIsUse = false;
            }
        }
        foreach(SkillCategorieUIItem skillCategorieUI in skillsPanelManager.skillCategorieUIItems)
        {
            if(skillCategorieUI.skillCategoriesType == _skillCategorie)
            {
                playerUseSkill.SetActiveSkill(skillCategorieUI.currentActiveSkill);
                playerUseSkill.SetActiveSkillKey(skillCategorieUI.activeSkillKeys);
            }
        }
        SetPassiveSkillStats();
        skillsPanelManager.UpdateSkillCategorieUIItem();
    }

    public void AddCategorieXp(SkillCategorie _skillCategorie, Skill _skill)//pour ajouter de l'xp à une catégorie
    {
        _skillCategorie.currentCategorieXp += Mathf.RoundToInt(_skillCategorie.addXpPerLvlSkill*(1+_skill.skillLvl/100));
        CheckCategorieLvlReach(_skillCategorie);
        skillsPanelManager. UpdateSkillUI();
        skillsPanelManager.UpdateSkillCategorieUIItem();
    }

    void CheckCategorieLvlReach(SkillCategorie _skillCategorie)//vérifier si la catégorie passe un lvl
    {
        if(_skillCategorie.currentCategorieXp >= _skillCategorie.reachCategorieXp)//si nouveau niveau
        {
            _skillCategorie.categorieLvl ++;
            _skillCategorie.currentCategorieXp -= _skillCategorie.reachCategorieXp;
            _skillCategorie.reachCategorieXp += 25 * _skillCategorie.categorieLvl/((_skillCategorie.categorieLvl+1)/2);//augmentation de l'xp à atteindre pour le next lvl
           
            pcs.skillPoints ++;
            pcs.pcsUI.SetPointsCaracteristiqueStatsUI();//actualise le ui des compétences
            GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<WorkshopUI>().UpdateAllUI();//update le ui de l'atelier
            CheckCategorieLvlReach(_skillCategorie);//recheck si pas nouveau niveau
        }
    }

    void SetPassiveSkillStats()//actualise les effets des compétences passives
    {
        //Set TOUT les les bonus lié au skills à 0
        pcs.ResetSkillBonusStats(0);//0=passif

        foreach (SkillCategorie _categorie in skillCategories)//pour chasue catégorie
        {
            if(!_categorie.categorieIsUse)//si catégorie pas utilisé annule
                continue;

            foreach (Skill _skill in _categorie.skills)//pour chaque skills dans la catégorie
            {
                if(_skill.isActiveSkill)//bloque les skills active
                    continue;

                pcs.SetSkillStats(_skill);
            }
        }

        if(player.currentPlayerLife > pcs.GetMaxPlayerLife(false))
            player.currentPlayerLife = pcs.GetMaxPlayerLife(false);
        
    }

    public void AddSkillXp(Skill skill)//ajouter de l'xp à un skill (xp à ajouter stocké dans les data du skill)
    {
        skill.currentXp += skill.addXpPerUsing;
        CheckSkillLvlReach(skill);
        skillsPanelManager.UpdateSkillUI();
    }

    public int IsActiveSkill(Skill _skill)
    {
        int id;
        if(!_skill.isActiveSkill)
            id = 0;
        else
            id = 1;
        
        return id;
    }

    public Skill GetPossessSkillWithName(string _name)
    {
        Skill returnSkill = null; 
        for(int i=0; i<GetAllUsedSkill().Length; i++)
        {
            if(GetAllUsedSkill()[i].skillName == _name)
            {
                returnSkill = GetAllUsedSkill()[i];
            }
        }
        return returnSkill;
    }

    void CheckSkillLvlReach(Skill skill)//vérifier si le skill passe un lvl
    {
        if(skill.currentXp >= skill.reachXp)//si nouveau niveau
        {
            skill.skillLvl ++;
            skill.currentXp -= skill.reachXp;
            skill.reachXp += 50 * skill.skillLvl/((skill.skillLvl+1)/2);//augmentation de l'xp à atteindre pour le next lvl
            //check les stats utilisé et l'augmente si oui
            float _additionalUpradePassiveStats;
            if(!skill.isActiveSkill)
                _additionalUpradePassiveStats = skill.additionalUpradeStats;
            else
                _additionalUpradePassiveStats = 0;

            if(skill.swordDamage!=0)
            {
                skill.swordDamage += skill.additionalUpradeStats;//ajoute les stats dans le skill
                pcs.bonusSwordSkillDamage[0] += _additionalUpradePassiveStats;//actualise(ajoute) les stats en jeu
            }
            if(skill.increasedPlayerLife!=0)
            {
                skill.increasedPlayerLife += skill.additionalUpradeStats;
                pcs.maxPlayerHealthSkills[0] += _additionalUpradePassiveStats;
            }
            if(skill.increasePlayerDodge!=0)
            {
                skill.increasePlayerDodge += skill.additionalUpradeStats;
                pcs.playerDodge[0] += _additionalUpradePassiveStats;
            }
            if(skill.increasePlayerStrength!=0)
            {
                skill.increasePlayerStrength += skill.additionalUpradeStats;
                pcs.playerStrengthSkills[0] += _additionalUpradePassiveStats;
            }
            if(skill.increasePlayerSpeed!=0)
            {
                skill.increasePlayerSpeed += skill.additionalUpradeStats;
                pcs.bonusSpeedSkill[0] += _additionalUpradePassiveStats;
            }
            if(skill.isInstantiateSkill)
            {
                if(skill.instantiateSkillDamage!=0)
                    skill.instantiateSkillDamage += skill.additionalUpradeStats;
                
                if(skill.isUpgradingDestroyTime)
                    skill.instantiateSkillDestroyTime += skill.additionalUpradeStats;
            }
            pcs.pcsUI.SetPointsCaracteristiqueStatsUI();
            AddCategorieXp(GetCategorie(skill.skillCategorieType), skill);
            CheckSkillLvlReach(skill);//recheck si pas nouveau niveau
        }
    }

    public enum SkillCategoriesType//les différents type de catégorie
    {
        Fighter,
        Survivor,
        Assassin
    }
    
    public Skill[] GetAllUsedSkill()
    {
        List<Skill> _skills = new List<Skill>();
        foreach(SkillCategorie _categorie in skillCategories)//pour chaque catégorie
        {
            foreach(Skill _skill in _categorie.skills)//pour chaque skill
            {
                _skills.Add(_skill);
            }
        }
        return _skills.ToArray();
    }

    public enum SkillsIDType//attention à bien référencer tout les skill dans skillsStats et de ne pas avoir deux fois le même
    {
        
        Epeiste,//Guerrier passif
        EffervescenceDePuissance,//Guerrier Actif
        DeterminationDeLEpeiste,//Guerrier Actif
        BouleDeFeu,//Guerrier instantié actif
        Esquive,//Assassin passif
        Vivalite,//Survivant passif
        Regeneration,//Survivant Actif
        BarriereSacree,//Survivant instantié actif
        LumiereSacree,//Survivant instantié actif
        VitesseDesOmbres//Assassin actif
    }
}
