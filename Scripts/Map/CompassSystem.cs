using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CompassSystem : MonoBehaviour
{
    [SerializeField] RawImage rawCompass;
    Transform playerTr;
    PlayerUI playerUI;

    [SerializeField] Transform mapMarkerPointSpawnTr;//la tr où apparait les marker sur la map

    [SerializeField] GameObject compassMarkerIconePrefab;//prefab d'un marker sur la boussole
    [SerializeField] GameObject spaceMarkerPrefab;//prefab d'un marker sur la map
    [SerializeField] int maxMarkPoint = 5;
    [SerializeField] float maxDistance = 20;
    
    [HideInInspector] public List<CompassMarker> defaultMarkPoints = new List<CompassMarker>();
    [HideInInspector] public List<CompassMarker> compassMarkers = new List<CompassMarker>();

    float compassUnit;

    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        compassUnit = rawCompass.rectTransform.rect.width / 360f;
    }

    void Update()
    {
        rawCompass.uvRect = new Rect(playerTr.eulerAngles.y/360f, 0, 1, 1);

        foreach(CompassMarker compassMarker in compassMarkers)
        {
            compassMarker.image.rectTransform.anchoredPosition = GetCompassMarkerPosOnCompass(compassMarker);

            float dst = Vector2.Distance(new Vector2(playerTr.position.x, playerTr.position.z), compassMarker.position);
            float scale = 0f;

            if(dst < maxDistance)
                scale = 1f - dst / maxDistance;
            
            if(scale < 0.5f && scale > 0)
                scale = 0.5f;
            
            compassMarker.image.rectTransform.localScale = Vector3.one * scale;
        }
        
        if(playerUI.miniMapPanel.activeSelf)
        {
            if(Input.GetKeyDown(InputManager.instance.mouseZero))
            {
                RaycastHit hit =  CheckMapPosWithClick();
                if(!RaycastHit.Equals(hit, new RaycastHit()))
                {
                    if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Map"))
                    {
                        DeleteDefaultMarkerPoint(hit.transform.GetComponentInChildren<CompassMarker>());
                        return;
                    }
                }
                CanMakeMark();
            }
        }
    }

    RaycastHit CheckMapPosWithClick()
    {
        Camera cam = playerUI.miniMapCameraObject.GetComponent<Camera>();
        RectTransform markerRectTransform = mapMarkerPointSpawnTr.gameObject.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(markerRectTransform, Input.mousePosition, null, out Vector2 localClick);
        localClick.y = (markerRectTransform.rect.yMin * -1) - (localClick.y * -1);
        localClick.x = (markerRectTransform.rect.xMin * -1) - (localClick.x * -1);

        Vector2 viewportClick = new Vector2(localClick.x / markerRectTransform.rect.size.x, localClick.y / markerRectTransform.rect.size.y);

        Ray ray = cam.ViewportPointToRay(new Vector3(viewportClick.x, viewportClick.y, 0));
        if (Physics.Raycast(ray, out RaycastHit hit)) 
            return hit;
        else
            return new RaycastHit();
    }

    void CanMakeMark()//verifie si click sur un point de la carte
    {
        RaycastHit hit = CheckMapPosWithClick();
        if(!RaycastHit.Equals(hit, new RaycastHit())) 
            MakeMark(hit);//si oui fait un marker
    }

    void MakeMark(RaycastHit hit)//créé un marker par defaut
    {
        if(defaultMarkPoints.Count+1 > maxMarkPoint)
            DeleteDefaultMarkerPoint(defaultMarkPoints[0]);
        
        GameObject compassMarkerClone = Instantiate(spaceMarkerPrefab, hit.point, Quaternion.identity);
        CompassMarker compassMarker = compassMarkerClone.GetComponentInChildren<CompassMarker>();
        compassMarker.isDefaultMarker = true;
        AddMapAndcompassMarkerIcone(compassMarker);//ajoute l'icone du marker sur la boussole et la map
    }

    public void AddMapAndcompassMarkerIcone(CompassMarker compassMarker)//ajoute une icone sur la boussole/map avec un compassMarker (n'importe quel objet)
    {
        GameObject markerClone = Instantiate(compassMarkerIconePrefab, rawCompass.transform);//Set l'icone dans la boussole
        compassMarker.image = markerClone.GetComponent<Image>();
        compassMarker.image.sprite = compassMarker.icone;

        compassMarkers.Add(compassMarker);
        if(compassMarker.isDefaultMarker)
            defaultMarkPoints.Add(compassMarker);
    }

    public void DeleteDefaultMarkerPoint(CompassMarker compassMarker)//suprime un marker avec comme id l'objet
    {
        if(compassMarker.isDefaultMarker)
        {
            defaultMarkPoints.Remove(compassMarker);
            if(compassMarker.transform.parent)
                Destroy(compassMarker.transform.parent.gameObject);
        }

        compassMarkers.Remove(compassMarker);
        Destroy(compassMarker.image.gameObject);
        Destroy(compassMarker.gameObject);
    }

    Vector2 GetCompassMarkerPosOnCompass(CompassMarker compassMarker)
    {
        Vector2 playerPos = new Vector2(playerTr.position.x, playerTr.position.z);
        Vector2 playerFwd = new Vector2(playerTr.forward.x, playerTr.forward.z);

        float angle = 0;
        if(compassMarker)
            angle = Vector2.SignedAngle(compassMarker.position - playerPos, playerFwd);

        return new Vector2(compassUnit * angle, 0f);
    }
}
