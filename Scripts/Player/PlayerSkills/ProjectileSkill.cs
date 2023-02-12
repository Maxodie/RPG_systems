using UnityEngine;

public class ProjectileSkill : MonoBehaviour
{
    InstantiateSkillStats stats;

    float damage;
    [SerializeField] GameObject destroyEffect;

    void Start() 
    {
        stats = GetComponent<InstantiateSkillStats>();
        damage = stats.skillDamage;
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("TransparentBarrier"))
            return;
        
        if(other.CompareTag("GeneralAi"))
        {
            GeneralAi ai = other.GetComponent<GeneralAi>();
            ai.TakeDamage(damage, transform, ItemData.WeaponType.NoWeapon);
        }

        DestroyObject(other.transform.position-transform.position);
    }

    void DestroyObject(Vector3 dir)
    {
        GameObject effectIns = Instantiate(destroyEffect, transform.position, Quaternion.LookRotation(dir));
        Destroy(effectIns, 2f);

        Destroy(gameObject);
    }
}
