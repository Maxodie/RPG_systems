using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    [Range(0f, 1f)]
    public float time;
    [SerializeField] float dayLength;//duration du jour en secondes
    [SerializeField] float nightLength;//duration de la nuit en secondes
    [SerializeField] float startTime = 0.245f;//0.245f = levé de soleil
    float dayTimeRate;
    float nightTimeRate;
    [SerializeField] Vector3 noon;//midi

    [Header("Soleil")]
    [SerializeField] Light sun;
    [SerializeField] Gradient sunColor;
    [SerializeField] AnimationCurve sunIntensity;

    [Header("Lune")]
    [SerializeField] Light moon;
    [SerializeField] Gradient moonColor;
    [SerializeField] AnimationCurve moonIntensity;

    [Header("Autres lumières")]
    [SerializeField] AnimationCurve lightingIntensityMultiplier;
    [SerializeField] AnimationCurve reflectionsIntensityMultiplier;

    void Start() 
    {
        dayTimeRate = 0.5f/dayLength;
        nightTimeRate = 0.5f/nightLength;
        time = startTime;
    }

    void Update() 
    {
        //augmenter le temps en fonction(vitesse) du joue et de la nuit
        if(sun.gameObject.activeInHierarchy)
            time += dayTimeRate * Time.deltaTime;
        else
        {
            time += nightTimeRate * Time.deltaTime;
        }

        if(time >= 1f)
            time = 0f;
        //rotation des astres
        sun.transform.eulerAngles = (time-0.25f)*noon*4f;
        moon.transform.eulerAngles = (time-0.75f)*noon*4f;

        //intensité des lumière
        sun.intensity = sunIntensity.Evaluate(time);
        moon.intensity = moonIntensity.Evaluate(time);

        //changer les couleurs
        sun.color = sunColor.Evaluate(time);
        moon.color = moonColor.Evaluate(time);

        //activer/désactiver le soleil
        if(sun.intensity == 0 && sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(false);
        else if(sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(true);

        //activer/desactiver la lune
        if(moon.intensity == 0 && moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(false);
        else if(moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(true);

        //intensité lumère et réfléction
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionsIntensityMultiplier.Evaluate(time);
    }
}
