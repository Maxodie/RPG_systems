using UnityEngine;

[RequireComponent(typeof(FieldOfViewAi))]
[RequireComponent(typeof(GeneralAi))]
public class AttackAi : MonoBehaviour
{
    Player player;

    FieldOfViewAi fov;
    GeneralAi generalAi;

    [SerializeField]
    float attackRange = 1f;
    [SerializeField]
    float attackDamage = 10f;
    [SerializeField]
    float attackSpeed = 2f;
    float currentAttackSpeed;

    [SerializeField] LayerMask attackMask;

    bool IsInPlayerRange()
    {
        if(Vector3.Distance(transform.position, player.transform.position) <= attackRange)
            return true;
        else
            currentAttackSpeed = attackSpeed;
            return false;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        fov = GetComponent<FieldOfViewAi>();
        generalAi = GetComponent<GeneralAi>();
        currentAttackSpeed = attackDamage;
    }

    void Update()//est appelé dans le scipt fov quand il voit un joueur
    {
        if(fov.canSeePlayer)
            ChasingPlayer();
    }

    void ChasingPlayer()
    {
        player = fov.playerRef;
            if(player != null)//reverifie si le joueur est toujour à porté
            {
                currentAttackSpeed -= 1 * Time.deltaTime;
                if(IsInPlayerRange())//verifie si le joueur est à porté d'attack
                {
                    generalAi.agent.destination = transform.position;//arrete le déplacement
                    fov.ChangeFOVAttack(player.transform);

                    RaycastHit hit;
                    if(currentAttackSpeed <= 0f && Physics.Linecast(fov.viewTr.position, player.cam.transform.position, out hit, attackMask))
                    {
                        if(hit.transform.CompareTag("Player"))
                            Attack(player);//attack le joueur chase
                    }
                }
                else 
                    generalAi.agent.destination = player.transform.position;
            }
    }

    void Attack(Player player)
    {
        player.TakeDamage(attackDamage);
        currentAttackSpeed = attackSpeed;
    }
}
