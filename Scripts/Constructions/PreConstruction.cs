using UnityEngine;

public class PreConstruction : MonoBehaviour
{
    PlayerConstructor playerConstructor;
    [SerializeField]
    float constructRange;
    [SerializeField]
    ConstructionType constructionType;
    [SerializeField]
    GameObject constructionPrefab;
    bool canBuild = true;

    [SerializeField]
    LayerMask groundMask;
    Transform camTr;
    Transform playerTr;
    [SerializeField]
    float rotateSpeed;

    [SerializeField]
    Renderer[] rend;//le renderer 
    Color canConstructColor;//la couleur quand on peut poser l'objet (pour la sauvegarder)
    [SerializeField]
    Color cannotConstructColor;//la couleur quand on peut poser l'objet (pour la sauvegarder)

    void Start() 
    {
        playerConstructor = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerConstructor>();
        camTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().cam.transform;
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;

        canConstructColor = rend[0].material.color;
    }

    void Update() 
    {
        Position();
        GroundedCheck();
        if(Input.GetKey(InputManager.instance.preContructionRotate))
        {
            Rotate();
        }
        if(Input.GetKeyDown(InputManager.instance.inHandUse))
        {
            BuildConstruction();
        }
    }

    void Position()
    {
        RaycastHit hit;
        if(Physics.Raycast(camTr.position, camTr.forward, out hit, constructRange, groundMask))
        {
            transform.position = hit.point;
        } 
        else
        {
            transform.position = new Vector3(camTr.position.x+constructRange*camTr.forward.x, camTr.position.y, 
            camTr.position.z+constructRange*camTr.forward.z);
        } 
    }

    void GroundedCheck()
    {
        RaycastHit hit;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y+0.2f, transform.position.z);
        if(Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, groundMask))//sur le terrain
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
        else //sous le terrain
        {
            transform.position = playerTr.position;
        }
    }

    void Rotate()
    {
        transform.Rotate(0, rotateSpeed*Time.deltaTime, 0);
    }

    void BuildConstruction()
    {
        if(canBuild && playerConstructor.constructCampFire == null)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y+0.2f, transform.position.z);
            GameObject go = Instantiate(constructionPrefab, transform.position, transform.rotation);
            //GameObject.FindGameObjectWithTag("GameManager").GetComponent<CombineMeshes>().Combine(go, false, null);
            switch(constructionType)
            {
                case ConstructionType.FireCamp:
                    playerConstructor.constructCampFire = go;
                    break;
            }
            DestroyPreConstruct();
        }
    }

    public void DestroyPreConstruct()
    {
        playerConstructor.canPreConstruct = true;
        PlayerUI.canOpenPanel = true;
        Destroy(gameObject);
    }

    void OnTriggerStay(Collider other) 
    {
        canBuild = false;
        SetColor(cannotConstructColor);
    }
    void OnTriggerExit(Collider other)
    {
        canBuild = true;
        SetColor(canConstructColor);
    }

    void SetColor(Color color)
    {
        for(int i=0; i<rend.Length; i++)
        {
            rend[i].material.SetColor("_Color", color);
        }
    }

    enum ConstructionType
    {
        FireCamp
    }
}
