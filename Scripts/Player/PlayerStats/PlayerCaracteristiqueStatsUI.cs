using UnityEngine;
using UnityEngine.UI;

public class PlayerCaracteristiqueStatsUI : MonoBehaviour
{
    [SerializeField] PlayerCaracteristiqueStats pcs;

    public Text skillPointsText;

    public Text strengthPointsText;
    public Text lifePointsText;
    public Text resistancePointsText;
    public Text manaPointsText;

    void Start() 
    {
        SetPointsCaracteristiqueStatsUI();
    }

    public void SetPointsCaracteristiqueStatsUI()
    {
        skillPointsText.text = "Points de compétence: " + pcs.skillPoints;
        
        strengthPointsText.text = pcs.playerStrength.ToString();
        if(pcs.playerStrengthSkills[0]!=0||pcs.playerStrengthSkills[1]!=0)
            strengthPointsText.text += " (+" + (pcs.playerStrengthSkills[0]+pcs.playerStrengthSkills[1]) + ")";//+Mathf.RoundToInt(Mathf.Sqrt(pcs.playerStrength)*1.2f) de dégâts par attaque
        
        lifePointsText.text = pcs.maxPlayerHealth.ToString();
        if(pcs.maxPlayerHealthSkills[0]!=0||pcs.maxPlayerHealthSkills[1]!=0)
            lifePointsText.text += " (+" + (pcs.maxPlayerHealthSkills[0]+pcs.maxPlayerHealthSkills[1]) + ")";

        resistancePointsText.text = pcs.playerResistance.ToString();
        if(pcs.playerResistanceSkills[0]!=0||pcs.playerResistanceSkills[1]!=0)
            resistancePointsText.text += " (+" + (pcs.playerResistanceSkills[0]+pcs.playerResistanceSkills[1]) + ")";//_damage = _amount -(currentPlayerArmor/35+pcs.playerResistance/10)
        
        manaPointsText.text = pcs.maxPlayerMana.ToString();
        if(pcs.maxPlayerManaSkills[0]!=0||pcs.maxPlayerManaSkills[1]!=0)
            manaPointsText.text += " (+" + (pcs.maxPlayerManaSkills[0]+pcs.maxPlayerManaSkills[1]) + ")";
    }

    public void AddPlayerStengthPointsButton(float _amount)//ajoute des stats dans playerStrength
    {
        pcs.AddPlayerStengthPoints(_amount);
    }

    public void AddPlayerLifePointsButton(float _amount)//ajoute des stats dans maxPlayerHealth
    {
        pcs.AddPlayerLifePoints(_amount);
    }

    public void AddPlayerResistancePointsButton(float _amount)//ajoute des stats dans playerResistance
    {
        pcs.AddPlayerResistancePoints(_amount);
    }

    public void AddPlayerManaPointsButton(float _amount)//ajoute des stats dans playerResistance
    {
        pcs.AddPlayerManaPoints(_amount);
    }
}
