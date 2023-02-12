using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class CompassMarker : MonoBehaviour
{
    [HideInInspector] public bool isDefaultMarker = false;
    CompassSystem compassSystem;

    [HideInInspector] public Image image;
    public Sprite icone;

    public Sprite mapIcone;

    [HideInInspector] public Vector2 position{
        get{return new Vector2(transform.position.x, transform.position.z);}
    }

    void Start() 
    {
        GetComponent<SpriteRenderer>().sprite = mapIcone;
        compassSystem = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<CompassSystem>();
        if(!isDefaultMarker)
            compassSystem.AddMapAndcompassMarkerIcone(this);
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
            compassSystem.DeleteDefaultMarkerPoint(this);
    }
}
