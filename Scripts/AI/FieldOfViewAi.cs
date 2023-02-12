using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AttackAi))]
public class FieldOfViewAi : MonoBehaviour
{
    AttackAi attackAi;
    GeneralAi generalAi;

    [SerializeField]
    float radius;
    [SerializeField, Range(0, 360)]
    float angle;
    float currentAngle;
    public Transform viewTr;

    [HideInInspector]
    public Player playerRef;//si multi remplir avec la liste des joueurs dans le gameManager
    [SerializeField]
    LayerMask targetMask;
    [SerializeField]
    LayerMask obstructionMask;

    [HideInInspector]
    public bool canSeePlayer;

    void Start()
    {
        currentAngle = angle;
        attackAi = GetComponent<AttackAi>();
        generalAi = GetComponent<GeneralAi>();
        StartCoroutine(FOVRoutine());
    }

    void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(viewTr.position, radius, targetMask);

        if(rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].GetComponent<Player>().cam.transform;
            Vector3 directionToTarget = (target.position - viewTr.position).normalized;

            if(Vector3.Angle(transform.forward, directionToTarget) < currentAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(viewTr.position, target.position);
                if(!Physics.Raycast(viewTr.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    playerRef = rangeChecks[0].GetComponent<Player>();
                }
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if(canSeePlayer)
        {
            canSeePlayer = false;
            currentAngle = angle;
        }
        
        if(!canSeePlayer)
            playerRef = null;
        
    }

    public void ChangeFOVAttack(Transform targetTr)
    {
        Vector3 directionToTarget = targetTr.transform.position - transform.position;
        currentAngle = 360;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * generalAi.agent.speed).eulerAngles; // calcule rotation pour regarder joueur
        transform.rotation = Quaternion.Euler(rotation); //effectue la rotation
    }

    IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }
}
