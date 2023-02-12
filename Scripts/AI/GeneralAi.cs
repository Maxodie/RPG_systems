using UnityEngine;
using UnityEngine.AI;

public class GeneralAi : MonoBehaviour
{
    public AiStatsScriptableObject aiData;
    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent agent;
    FieldOfViewAi fov;

    float currentTimeToChangeDestination = 0f;

    float basicSpeed;
    float currentLife;

    PlayerSkillsManager playerSkillsManager;

    Vector3 firstPos;
    Vector3 target;

    [SerializeField] Transform spawnItemTr;

    void Start()
    {
        //GameObject.FindGameObjectWithTag("GameManager").GetComponent<CombineMeshes>().Combine(gameObject, false, null);

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        fov = GetComponent<FieldOfViewAi>();
        firstPos = transform.position;
        currentLife = aiData.maxLife;
        basicSpeed = agent.speed;
        playerSkillsManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkillsManager>();
    }

    void Update()
    {
        if(CanChangeTarget())
        {
            if(fov != null)
            {
                if(fov.canSeePlayer)
                    return;//empèche de changer la target si il y a un fov ai script et que un joueur est repéré
            }
            SetTarget();
            SettimeBeforeChangePosition();
        }
        currentTimeToChangeDestination -= 1 * Time.deltaTime;
    }

    bool CanChangeTarget()
    {
        if(currentTimeToChangeDestination <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SettimeBeforeChangePosition()
    {
        float r = Random.Range(aiData.minTimeToChangeDestination, aiData.maxTimeToChangeDestination);
        currentTimeToChangeDestination = r;
    }

    void SetTarget()
    {
        CalculatePosition();
        NavMeshHit hit;
        if(NavMesh.SamplePosition(target, out hit, 1f, NavMesh.AllAreas))
        {
            agent.destination = target;
        }
        else
        {
            SetTarget();
        }
        agent.speed = basicSpeed;
    }

    void CalculatePosition()
    {
        float x;
        float y;
        float z;
        x = Random.Range(firstPos.x-aiData.maxDistanceAfterSpawnCanMove, firstPos.x+aiData.maxDistanceAfterSpawnCanMove);
        
        y = Random.Range(firstPos.y-aiData.maxDistanceAfterSpawnCanMove, firstPos.y+aiData.maxDistanceAfterSpawnCanMove);

        z = Random.Range(firstPos.z-aiData.maxDistanceAfterSpawnCanMove, firstPos.z+aiData.maxDistanceAfterSpawnCanMove);
        target = new Vector3(x, y, z);
    }

    public void TakeDamage(float _amount, Transform transformWhoHit, ItemData.WeaponType weaponType)
    {
        if(aiData.vulnerableWeapon != ItemData.WeaponType.NoWeapon && aiData.vulnerableWeapon == weaponType)
        {
            _amount *= 1+aiData.vulnerablepct/100;
        }
        currentLife -= _amount;
        Vector3 directionToTarget = transformWhoHit.position - transform.position;
        if(aiData.isEscapeWheneHit)
        {
            agent.speed *= aiData.escapeSpeedMultiplicator;
            Quaternion lookRotation = Quaternion.LookRotation(-directionToTarget);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * agent.speed).eulerAngles; // calcule rotation pour regarder joueur
            transform.rotation = Quaternion.Euler(rotation);
            agent.destination = _amount * aiData.escapeSpeedMultiplicator * transformWhoHit.forward;
            SettimeBeforeChangePosition();
        }
        else if(aiData.isChasing && fov != null)
        {
            fov.ChangeFOVAttack(transformWhoHit);
        }
        if(currentLife <= 0)
        {
            Death(weaponType);
        }
    }

    void Death(ItemData.WeaponType weaponType)
    {
        QuestScriptableObject[] questdb = GameManager.dataBase.questsData;
        foreach(QuestScriptableObject quest in questdb)
        {
            if(!quest.questIsStart)
                continue;
            foreach(QuestStep step in quest.questSteps)
            {
                if(!step.isStart || step.canFinish)
                    continue;
                if(step.enemyToKillPrefab.GetComponent<GeneralAi>().aiData.AIName == aiData.AIName)
                {
                    step.numberEnemyToKill--;
                    GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<QuestUI>().ActualiseQuestFollowText();
                }
            }
        }
        //le skill gagné
        if(weaponType == aiData.requireWeaponType || aiData.requireWeaponType == ItemData.WeaponType.NoWeapon)
        {
            if(aiData.isWinSkill)
                playerSkillsManager.AddSkill(aiData.skillToAddAtDeath);
        }
        //l'item gagné
        Instantiate(aiData.itemDropAtDeath, spawnItemTr.position, aiData.itemDropAtDeath.transform.rotation); //ajouter transfomr pour spawn d'item
        //coins gagnés
        int winCoin = Random.Range(aiData.minCoinWonAtDeath, aiData.maxCoinWinAtDeath+1);//+1 car pour int exclusif
        playerSkillsManager.GetComponent<Player>().ChangeCoinCount(winCoin, false);
        Destroy(gameObject);
    }
}
