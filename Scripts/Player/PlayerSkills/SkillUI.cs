using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [HideInInspector]
    public PlayerSkillsManager.SkillsIDType skillIDType;
    public Text titleText;
    public Text descriptiontext;

    public Image xpFill;
    public Text xpText;

    [Header("ACTIVE SKILL")]
    //SKILL ACTIFS
    public Image rechargeFill;
    public Color unactivableColor;
    public Color activableColor;
}
