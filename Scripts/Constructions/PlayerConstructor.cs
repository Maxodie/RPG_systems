using UnityEngine;

public class PlayerConstructor : MonoBehaviour
{
    [HideInInspector]
    public PlayerUI playerUI;
    [HideInInspector]
    public GameObject constructCampFire;
     GameObject preConstructionCampFire;
    [HideInInspector]
    public bool canPreConstruct = true;

    [HideInInspector]
    public GameObject objectToConstructAtClose;

    void Start() 
    {
        playerUI =  GetComponent<PlayerUI>();
    }

    void Update()
    {
        if(Input.GetKeyDown(InputManager.instance.escape))
        {
            DeletePreConstruction();
        }
    }

    void DeletePreConstruction()
    {
        if(!canPreConstruct)
        {
            if(preConstructionCampFire != null)
            {
                PreConstruction preC = preConstructionCampFire.GetComponent<PreConstruction>();
                preC.DestroyPreConstruct();
            }
        }
    }

    public void OnClick(GameObject constructionPrefab)
    {
        PreConstruct(constructionPrefab);
        playerUI.ToggleConstructorPanel();
    }

    public void PreConstruct(GameObject constructionPrefab)
    {
        if(canPreConstruct && constructCampFire == null)
        {
            PlayerUI.canOpenPanel = false;
            GameObject go = Instantiate(constructionPrefab, transform.position, transform.rotation);
            preConstructionCampFire = go;
            objectToConstructAtClose = null;
            canPreConstruct = false;
        }
    }

    public void OnExit()
    {
        objectToConstructAtClose = null;
    }

    public void OnEnter(GameObject constructionPrefab)
    {
        objectToConstructAtClose = constructionPrefab;
    }
}
