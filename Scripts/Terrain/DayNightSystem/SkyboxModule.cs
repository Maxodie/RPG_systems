using UnityEngine;

[RequireComponent(typeof(DayNightSystem))]
public class SkyboxModule : MonoBehaviour
{
    DayNightSystem dayNightSystem;
    [SerializeField] Gradient skyTint;
    [SerializeField] Gradient groundColor;
    [SerializeField] AnimationCurve sunSize;//10= petit pas éclérant; 1= grand éclairant
    [SerializeField] AnimationCurve atmosphereThickness;//0=sombre/lourd; 1=plein jour/éclairé
    [SerializeField] AnimationCurve exposure;
    Material sky;

    void Start()
    {
        dayNightSystem = GetComponent<DayNightSystem>();
        sky = RenderSettings.skybox;
    }

    void Update() 
    {
        //changer la couleur du fond la couleur de la lumière du sol
        sky.SetColor("_SkyTint", skyTint.Evaluate(dayNightSystem.time));
        sky.SetColor("_GroundColor", groundColor.Evaluate(dayNightSystem.time));

        //changer la taille du soleil
        sky.SetFloat("_SunSizeConvergence", sunSize.Evaluate(dayNightSystem.time));

        //changer l'épesseur de l'atmosphere et son exposure
        sky.SetFloat("_AtmosphereThickness", atmosphereThickness.Evaluate(dayNightSystem.time));
        sky.SetFloat("_Exposure", exposure.Evaluate(dayNightSystem.time));

    }
}
