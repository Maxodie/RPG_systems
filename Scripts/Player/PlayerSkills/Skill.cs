using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Skill", menuName = "Survival/Skill", order = 0)]
public class Skill : ScriptableObject//stats et info des skills (pas oublier de changer l'editor window, dans l'upgrade, le ui dans skillPanelManager, les caracteristiques et après modification)
{
    public PlayerSkillsManager.SkillCategoriesType skillCategorieType;
    public PlayerSkillsManager.SkillsIDType skillType;
    public string skillName;
    
    public bool isActiveSkill;
    public bool canUse = true;//si oui et si isActiveSkill oui alors peut être utiliser(modifier dans script)
    public Sprite activeSprite;
    public GameObject startSkillEffects;//l'effet au début qui dure activeTimeOfActiveSkill
    public GameObject duringSkillEffects;//l'effet pendant le skill qui dure duringActiveTime
    public float activeTimeOfActiveSkill;//le temps durant on ne peut rien faire avant l'activation des stats
    public float manaCost = 10f;//Le cout en mana du skill actif
    public bool isPermanentStatsSkill;//si oui ne peut pas utiliser des objet et le changement des stats se font après duringActiveTime, surtout utiliser pour les soin et les sorts
    public float duringActiveTime;//si isActiveSkill utilisé comme temps avant de terminer le skill
    public float reloadActiveSkill;//temps de recharge d'un skill actif

    public bool isInstantiateSkill;//si le skill est ex: une boule de feu
    public GameObject instantiateSkillPrefab;//la prefab du skill
    public float instantiateSkillSpeed;//la vitesse du skill
    public float instantiateSkillDamage;//les dégats du skill
    public float instantiateSkillDestroyTime;//Temps de destruction du skill
    public bool isUpgradingDestroyTime = false;//si oui augmente le temps avant de se détruir par amélioration
    public bool isChangeRotation = true;//si oui change la rotation(egale au joueur)
    public bool isFollowPlayer = false;//si oui se met dans un transform du joueur;

    public int skillLvl = 0;//LVL du skill actuel
    public float currentXp = 0f;//l'xp du skill actuel
    public float reachXp = 100f;//reachXp += 50 * skillLvl/((skillLvl+1)/2) pour chaque lvl sup
    public float addXpPerUsing = 10f;//l'xp augmenté par utilisation de la compétence
    public float additionalUpradeStats = 1f;//le nombre de stats par lequel le skill va s'additioné

    public float swordDamage;//dégâts supplémentaire si passif avec les armes de type sword en %
    public float increasedPlayerLife;//l'augmentation de la vie du joueur
    public bool canIncreaseUpperThanCurrentHealth;//si oui augmente la vie max sinon soin classique
    public float increasePlayerDodge;//le pourcentage d'ésquive des dégâts subis
    public float increasePlayerStrength;//le pourcentage de force
    public float increasePlayerSpeed;//le pourcentage de vitesse suplémentaire
}
