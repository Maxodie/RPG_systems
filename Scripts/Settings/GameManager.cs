using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static DataBase dataBase;
    [SerializeField] string dataBasePath = "_DataBase/DataBase";

    void Awake()
    {
        dataBase = Resources.Load<DataBase>(dataBasePath);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        CheckIaNames();
    }

    void CheckIaNames()//vérifie le nom des ia pour que deux ia n'est pas le même nom
    {
        string[] aiNames = new string[dataBase.aiStatsData.Length];

        for(int i=0; i<dataBase.aiStatsData.Length; i++)
        {
            aiNames[i] = dataBase.aiStatsData[i].AIName;
            for(int y=0; y<aiNames.Length; y++)
            {
                if(y != i && aiNames[y] == dataBase.aiStatsData[i].AIName)
                    Debug.LogError("Deux IA on le même nom!: " + aiNames[y] + " et " + dataBase.aiStatsData[i].AIName);
            }
        }
    }  

    public static void SetLayerRecursively(GameObject go, string layerName)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    public static void ToggleCursorStats(bool isPanelActive)
    {
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Cursor.visible = isPanelActive;
        playerController.isCamCanMove = !isPanelActive;
        if(isPanelActive)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
