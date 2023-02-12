using UnityEngine;
using System.Collections;

public class SpawnAiSystem : MonoBehaviour
{
    [Header("(Les points sont à poser HAUT!)")]
    [SerializeField] Transform spawnPointX;
    [SerializeField] Transform spawnPointZ;//les points de spawn

    [SerializeField] ObjectToSpawn[] objectToSpawns;//les objets à spawn
    [SerializeField] float spawnRate;//le temps de spawn

    void Start() 
    {
        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate);
        while(true)
        {
            SpawnObjectChance();
            yield return wait;
        }
    }

    void SpawnObjectChance()
    {
        float randomX = Random.Range(spawnPointZ.position.x, spawnPointX.position.x);
        float randomZ = Random.Range(spawnPointX.position.z, spawnPointZ.position.z);
        Vector3 spawnPos = new Vector3(randomX, transform.position.y, randomZ);

        float spawnChanceMin = 0;
        float maxChance = 0;
        for(int i=0; i<objectToSpawns.Length; i++)
        {
            maxChance += objectToSpawns[i].spawnChancePct;
        }
        float randomChanceSpawn = Random.Range(0, maxChance);
        foreach(ObjectToSpawn obj in objectToSpawns)
        {
            if(obj.spawnChancePct+spawnChanceMin >= randomChanceSpawn)
            {
                SpawnObject(spawnPos, obj.prefab);
                break;
            }
            spawnChanceMin += obj.spawnChancePct;
        }
    }

    void SpawnObject(Vector3 spawnPos, GameObject go)
    {
        RaycastHit hit;
        if(Physics.Raycast(spawnPos, Vector3.down, out hit, Mathf.Infinity))
        {
            spawnPos.y = hit.point.y;
            GameObject _cloneAi = Instantiate(go, spawnPos, Quaternion.identity);
            _cloneAi.transform.SetParent(transform);
        }
    }
}

[System.Serializable]
class ObjectToSpawn
{
    public GameObject prefab;
    public int spawnChancePct;
}
