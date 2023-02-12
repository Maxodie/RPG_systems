using UnityEngine;

public class PlayerCaracteristiqueStats : MonoBehaviour
{
    public PlayerCaracteristiqueStatsUI pcsUI;
    [SerializeField] PlayerSkillsManager playerSkillsManager;
    [SerializeField] Player player;
    
    public int skillPoints = 0;
    
    //variableSkill[0]=skill passif, variableSkill[0]=skill actif;
    [Header("Caracteristiques Principals")]//peut être augmenté dans le menu des caracteristiques avec des points de compétence
    public float maxPlayerHealth = 50f;//vie max du joueur
    public float[] maxPlayerHealthSkills = new float[2]{0,0};//vie augmenté avec les skills
    public float GetMaxPlayerLife(bool isSkillsGetXP)
    {
        if(isSkillsGetXP)
        {
            foreach(Skill _skill in playerSkillsManager.GetAllUsedSkill())
            {
                if(_skill.increasedPlayerLife!=0 && playerSkillsManager.GetCategorie(_skill.skillCategorieType).categorieIsUse && !_skill.isActiveSkill)
                    playerSkillsManager.AddSkillXp(_skill);
            }
        }
        return maxPlayerHealth + maxPlayerHealthSkills[0] + maxPlayerHealthSkills[1];
    }

    float maxPlayerHunger = 100f;//nourriture max du joueur
    public float[] maxPlayerHungerSkills = new float[2]{0,0};//faim augmenté avec les skills
    public float GetMaxPlayerHunger()//aucun xp ajouté sur aucun skill car aucun skill utilise pour l'instant
    {
        return maxPlayerHunger + maxPlayerHungerSkills[0] + maxPlayerHungerSkills[1];
    }

    public float playerStrength = 1f;//la force du joueur
    public float[] playerStrengthSkills = new float[2]{0,0};//force augmenté avec les skills
    public float GetPlayerStrength(bool isSkillsGetXP)//aucun xp ajouté sur aucun skill car aucun skill utilise pour l'instant
    {
        if(isSkillsGetXP)
        {
            foreach(Skill _skill in playerSkillsManager.GetAllUsedSkill())
            {
                if(_skill.increasePlayerStrength!=0 && playerSkillsManager.GetCategorie(_skill.skillCategorieType).categorieIsUse && !_skill.isActiveSkill)
                    playerSkillsManager.AddSkillXp(_skill);
            }
        }
        return playerStrength + playerStrengthSkills[0] + playerStrengthSkills[1];
    }

    public float playerResistance = 1f;//la résistance du joueur
    public float[] playerResistanceSkills = new float[2]{0,0};//résistance augmenté avec les skills
    public float GetPlayerResistance()//aucun xp ajouté sur aucun skill car aucun skill utilise pour l'instant
    {
        return playerResistance + playerResistanceSkills[0] + playerResistanceSkills[1];
    }
    
    public float maxPlayerMana = 100f;//Mana max du joueur
    public float[] maxPlayerManaSkills = new float[2]{0,0};//mana augmenté avec les skills
    public float GetMaxPlayerMana()//aucun xp ajouté sur aucun skill car aucun skill utilise pour l'instant
    {
        return maxPlayerMana + maxPlayerManaSkills[0] + maxPlayerManaSkills[1];
    }

    [Header("Caracteristiques Secondaire")]//secondaire
    public float[] playerDodge = new float[2]{0,0};//l'ésquive augmenté avec les skills
    public float GetPlayerDodge(bool isSkillsGetXP)
    {
        if(isSkillsGetXP)
        {
            foreach(Skill _skill in playerSkillsManager.GetAllUsedSkill())
            {
                if(_skill.increasePlayerDodge!=0 && playerSkillsManager.GetCategorie(_skill.skillCategorieType).categorieIsUse && !_skill.isActiveSkill)
                    playerSkillsManager.AddSkillXp(_skill);
            }
        }
        return playerDodge[0] + playerDodge[1];
    }

    public float[] bonusSwordSkillDamage = new float[2]{0,0};//les dégats sup en % avec une épé
    public float GetSwordBonusDamage(bool isSkillsGetXP)
    {
        if(isSkillsGetXP)
        {
            foreach(Skill _skill in playerSkillsManager.GetAllUsedSkill())
            {
                if(_skill.swordDamage!=0 && playerSkillsManager.GetCategorie(_skill.skillCategorieType).categorieIsUse && !_skill.isActiveSkill)
                    playerSkillsManager.AddSkillXp(_skill);
            }
        }
        return bonusSwordSkillDamage[0] + bonusSwordSkillDamage[1];
    }

    public float[] bonusSpeedSkill = new float[2]{0,0};//la vitesse sup en %
    public float GetSpeedBonus(bool isSkillsGetXP)
    {
        if(isSkillsGetXP)
        {
            foreach(Skill _skill in playerSkillsManager.GetAllUsedSkill())
            {
                if(_skill.increasePlayerSpeed!=0 && playerSkillsManager.GetCategorie(_skill.skillCategorieType).categorieIsUse && !_skill.isActiveSkill)
                    playerSkillsManager.AddSkillXp(_skill);
            }
        }
        return bonusSpeedSkill[0] + bonusSpeedSkill[1];
    }

    public void AddPlayerStengthPoints(float _amount)//ajoute des stats dans playerStrength
    {
        if(skillPoints > 0)
        {
            playerStrength += _amount;
            skillPoints --;
            pcsUI.SetPointsCaracteristiqueStatsUI();
        }
    }

    public void AddPlayerLifePoints(float _amount)//ajoute des stats dans maxPlayerHealth
    {
        if(skillPoints > 0)
        {
            maxPlayerHealth += _amount;
            player.IncreaseCurrentPlayerStats(_amount, 0);
            skillPoints --;
            pcsUI.SetPointsCaracteristiqueStatsUI();
        }
    }

    public void AddPlayerResistancePoints(float _amount)//ajoute des stats dans playerResistance
    {
        if(skillPoints > 0)
        {
            playerResistance += _amount;
            skillPoints --;
            pcsUI.SetPointsCaracteristiqueStatsUI();
        }
    }

    public void AddPlayerManaPoints(float _amount)//ajoute des stats dans playerResistance
    {
        if(skillPoints > 0)
        {
            maxPlayerMana += _amount;
            skillPoints --;
            pcsUI.SetPointsCaracteristiqueStatsUI();
        }
    }

    public void SetSkillStats(Skill _skill)
    {
        int id = playerSkillsManager.IsActiveSkill(_skill);

        bonusSwordSkillDamage[id] += _skill.swordDamage;//ajoute les dégâts de la compétence sur les épé

        if(_skill.canIncreaseUpperThanCurrentHealth)//si oui augment la vie max et la vie current
            maxPlayerHealthSkills[id] += _skill.increasedPlayerLife;//ajouter de la vie max au joueur
        if(_skill.isPermanentStatsSkill)
            player.IncreaseCurrentPlayerStats(_skill.increasedPlayerLife, 0);//augmente la vie actuel
            
        playerDodge[id] += _skill.increasePlayerDodge;
        playerStrengthSkills[id] += _skill.increasePlayerStrength;
        bonusSpeedSkill[id] += _skill.increasePlayerSpeed;
        
        pcsUI.SetPointsCaracteristiqueStatsUI();
    }

    public void ResetSkillBonusStats(int isActive)//_skill pour savoir si reset les bonus des skill actifs ou passifs(0=passif, 1=Actif)
    {
        bonusSpeedSkill[isActive] = 0;
        bonusSwordSkillDamage[isActive] = 0;
        maxPlayerHealthSkills[isActive] = 0;
        playerDodge[isActive] = 0;
        playerResistanceSkills[isActive] = 0;
        playerStrengthSkills[isActive] = 0;
        maxPlayerHungerSkills[isActive] = 0;
        maxPlayerManaSkills[isActive] = 0;
    }

    public void ResetSkillBonusStats(Skill skill)
    {
        int isActiveSkill = playerSkillsManager.IsActiveSkill(skill);

        bonusSwordSkillDamage[isActiveSkill] -= skill.swordDamage;//dégats épé
        if(bonusSwordSkillDamage[isActiveSkill]<0) bonusSwordSkillDamage[isActiveSkill] = 0;

        maxPlayerHealthSkills[isActiveSkill] -= skill.increasedPlayerLife;//vie du joueur
        if(maxPlayerHealthSkills[isActiveSkill]<0) maxPlayerHealthSkills[isActiveSkill] = 0;

        playerDodge[isActiveSkill] -= skill.increasePlayerDodge;//esquive du joueur
        if(playerDodge[isActiveSkill]<0) playerDodge[isActiveSkill] = 0;

        playerStrengthSkills[isActiveSkill] -= skill.increasePlayerStrength;//la force du joueur
        if(playerStrengthSkills[isActiveSkill]<0) playerStrengthSkills[isActiveSkill] = 0;

        bonusSpeedSkill[isActiveSkill] -= skill.increasePlayerSpeed;//la vitesse du joueur
        if(bonusSpeedSkill[isActiveSkill]<0) bonusSpeedSkill[isActiveSkill] = 0;
    }
}
