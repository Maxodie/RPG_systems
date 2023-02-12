using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerUseSkill : MonoBehaviour
{
    [SerializeField] PlayerUse playerUse;
    [SerializeField] PlayerSkillsManager playerSkillsManager;
    [SerializeField] PlayerCaracteristiqueStats pcs;
    [SerializeField] Player player;
    [SerializeField] PlayerUI playerUI;
    bool useSkillIsCharging = false;

    [SerializeField] Transform useSkillEffectTr;

    [SerializeField] KeyCode[] useKeys;
    [HideInInspector] public Skill[] useSkills;

    public int routinePlayerUseSkillCurentPlayNumber = 0;

    Dictionary<string, GameObject> activeSkillSprite = new Dictionary<string, GameObject>();

    void Start() 
    {
        useSkills = new Skill[useKeys.Length];
    }

    void Update() 
    {
        for(int i=0; i<useKeys.Length; i++)
        {
            if(Input.GetKeyDown(useKeys[i]))
            {
                UseSkill(i);
            }
        }
    }

    public void SetActiveSkill(Skill[] _activeSkill)
    {
        useSkills = _activeSkill;
    }

    public void SetActiveSkillKey(KeyCode[] _newUseKeys)
    {
        useKeys = _newUseKeys;
    }

    void UseSkill(int id)
    {
        if(useSkills[id] == null || !PlayerUI.canOpenPanel || !player.isAlive || !useSkills[id].isActiveSkill || useSkillIsCharging || !useSkills[id].canUse ||
        player.currentMana-useSkills[id].manaCost < 0)
            return;
        
        useSkillIsCharging = true;
        routinePlayerUseSkillCurentPlayNumber++;
        StartCoroutine(UseSkillRoutine(useSkills[id]));
    }

    IEnumerator UseSkillRoutine(Skill _skill)
    {
        SkillUI _skillUI = playerSkillsManager.GetSkillUI(_skill.skillType);//reset le visuel du rechargement du skill
        _skillUI.rechargeFill.fillAmount = 0;
        _skillUI.rechargeFill.color = _skillUI.unactivableColor;

        playerUse.canUse = false;
        PlayerUI.canOpenPanel = false;
        _skill.canUse = false;
        
        MakeParticle(_skill.startSkillEffects, _skill.activeTimeOfActiveSkill);
        
        player.canIncreaseMana = false;
        float resultMana = player.currentMana - _skill.manaCost;
        float wait = _skill.activeTimeOfActiveSkill;
        while(wait > 0)//Attendre que le sort se prépare et que le mana soit utilisé
        {
            player.currentMana -= _skill.manaCost/_skill.activeTimeOfActiveSkill*Time.deltaTime;
            wait -= Time.deltaTime;
            yield return null;
        }
        player.currentMana = resultMana;//pour avoir un resultat plus précis

        MakeParticle(_skill.duringSkillEffects, _skill.duringActiveTime);
        useSkillIsCharging = false;

        if(_skill.isInstantiateSkill)
            SetInstantiateSkillStats(_skill);
        else if(_skill.duringActiveTime > 0)
        {
            GameObject go = Instantiate(playerUI.activeSkillSpriteGo, playerUI.activeSkillSpriteContentTr);
            go.GetComponent<Image>().sprite = _skill.activeSprite;
            activeSkillSprite.Add(_skill.skillName, go);
        }

        pcs.SetSkillStats(_skill);

        playerUse.canUse = true;
        PlayerUI.canOpenPanel = true;
        player.canIncreaseMana = true;
        yield return new WaitForSeconds(_skill.duringActiveTime);
        routinePlayerUseSkillCurentPlayNumber--;

        playerSkillsManager.AddSkillXp(_skill);
        pcs.ResetSkillBonusStats(_skill);
        if(player.currentPlayerLife > pcs.GetMaxPlayerLife(false))
            player.currentPlayerLife = pcs.GetMaxPlayerLife(false);

        pcs.pcsUI.SetPointsCaracteristiqueStatsUI();
        StartCoroutine(ReloadActiveSkillRoutine(_skill));
    }

    IEnumerator ReloadActiveSkillRoutine(Skill _skill)
    {
        if(!_skill.isInstantiateSkill)
        {
            if(activeSkillSprite.ContainsKey(_skill.skillName))
                Destroy(activeSkillSprite[_skill.skillName]);
            activeSkillSprite.Remove(_skill.skillName);
        }
        SkillUI _skillUI = playerSkillsManager.GetSkillUI(_skill.skillType);

        while(!_skill.canUse)
        {
            _skillUI.rechargeFill.fillAmount += (1/_skill.reloadActiveSkill)*Time.deltaTime;

            if(_skillUI.rechargeFill.fillAmount >= 1f)
            {
                _skillUI.rechargeFill.color = _skillUI.activableColor;
                _skill.canUse = true;
            }

            yield return null;
        }
    }

    void SetInstantiateSkillStats(Skill _skill)
    {
        Transform handTr = playerUse.playerPickAndDropItem.handTransform;
        Quaternion rot = _skill.instantiateSkillPrefab.transform.rotation;
        if(_skill.isChangeRotation)
            rot = Quaternion.LookRotation(handTr.forward);
        GameObject clone = Instantiate(_skill.instantiateSkillPrefab, handTr.position, rot);
        if(_skill.isFollowPlayer)
            clone.transform.SetParent(useSkillEffectTr);
        
        InstantiateSkillStats instantiateSkillStats = clone.GetComponent<InstantiateSkillStats>();
        Rigidbody rb = clone.GetComponent<Rigidbody>();
        if(rb)
            rb.velocity = player.cam.transform.forward * _skill.instantiateSkillSpeed * Time.deltaTime;
            
        instantiateSkillStats.skillDamage = _skill.instantiateSkillDamage;
        Destroy(clone, _skill.instantiateSkillDestroyTime);
    }

    void MakeParticle(GameObject particles, float destroyTime)
    {
        GameObject _particulesClone = Instantiate(particles, useSkillEffectTr);
        Destroy(_particulesClone, destroyTime);
    }
}
