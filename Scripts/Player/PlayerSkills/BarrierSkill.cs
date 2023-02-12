using UnityEngine;

public class BarrierSkill : MonoBehaviour
{
    void Start()
    {
        SetPos();
    }

    void SetPos()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
            if(hit.transform.CompareTag("Ground"))
                transform.position.Set(transform.position.x, hit.point.y, transform.position.z);
    }
}
