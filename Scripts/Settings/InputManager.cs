using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Reflection;

public class InputManager : MonoBehaviour
{
    //ajouter key changeable: add variable KeyCode, dupliquer le boutton settingsUI, mettre le text dans le boutton avec même nom que variable
    public static InputManager instance;

    [Header("---Changeable")]
    public KeyCode moveForward = KeyCode.Z;//Avancer
    public KeyCode moveBack = KeyCode.S;//Reculer
    public float verticalMove {//Le mouvement vertical (pour playerController)
        get
        {
            if(Input.GetKey(moveForward))
                return 1f;
            else if(Input.GetKey(moveBack))
                return -1f;
            else
                return 0f;
        }
    }
    public KeyCode moveLeft = KeyCode.Q;//Gauche
    public KeyCode moveRight = KeyCode.D;//Droite
    public float horizontalMove {//Le mouvement horizontal (pour playerController)
        get
        {
            if(Input.GetKey(moveRight))
                return 1f;
            else if(Input.GetKey(moveLeft))
                return -1f;
            else
                return 0f;
        }
    }

    public KeyCode run = KeyCode.LeftShift;//Courir
    public KeyCode jump = KeyCode.Space;//sauter

    [Header("Utility")]
    public KeyCode useKey = KeyCode.F;//Utilisation (prendre item, parler aux pnj, ex..)
    public KeyCode inHandUse = KeyCode.Mouse0;//Utiliser objet dans la main (arme, potion, poser pré-construction, ex..)
    public KeyCode dropItem = KeyCode.X;//Drop des item
    public KeyCode continueDialogue = KeyCode.J;//passer à la phrase suivante dans un dialogue (pour dialogueSystem)
    public KeyCode preContructionRotate = KeyCode.R;//rotater la pré-construction (pour PreConstruction)

    [Header("Panel")]
    public KeyCode inventory = KeyCode.I;//Inventaire
    public KeyCode skillPanel = KeyCode.U;//Le Menu des skill
    public KeyCode caracteristiquePanel = KeyCode.C;//Le menu des caractéristiques
    public KeyCode map = KeyCode.O;//La map
    public KeyCode constructionPanel = KeyCode.A;//Le menu de construction

    FieldInfo[] fields = new FieldInfo[typeof(InputManager).GetFields().Length];
    [SerializeField] GameObject[] changeInputUIText;//tout les bouttons pour changer les touches

    [Header("---Jamais changé")]
    public KeyCode mouseZero = KeyCode.Mouse0;//Toujours KeyCode.mouse0
    public KeyCode escape = KeyCode.Escape;//Toujour KeyCode.Escape
    public KeyCode leftShift = KeyCode.LeftShift;//Toujours shift gauche

    void Awake()
    {
        #region Singleton
        {
            if(instance)
            {
                Debug.LogError("Plusieurs instances d' InputManager existent");
                return;
            }

            instance = this;
        }
        #endregion

        fields = GetType().GetFields();
        SetAllChangeableKeysButtonUI();
    }

    void SetAllChangeableKeysButtonUI()
    {

        foreach(GameObject go in changeInputUIText)
        {
            string stringKey = go.GetComponent<Text>().text;
            Text childText = null;

            for(int i=0; i<go.transform.childCount; i++)
            {
                childText = go.transform.GetChild(i).GetComponent<Text>();
                if(childText)
                    break;
            }

            for(int i=0; i<fields.Length; i++)
                if(fields[i].Name == stringKey)
                    childText.text = fields[i].GetValue(this).ToString();
        }
    }

    public void ChangeInput(GameObject button)
    {
        string text = button.GetComponent<Text>().text;
        for(int y=0; y<fields.Length; y++)
        {
            if(GetKey(y) == text)
            {
                Text childText = null;
                for(int i=0; i<button.transform.childCount; i++)
                {
                    childText = button.transform.GetChild(i).GetComponent<Text>();
                    if(childText)
                        childText.text = "...";
                }
                StartCoroutine(WaitForChangeKey(GetKey(y), childText));
            }
        }
    }

    IEnumerator WaitForChangeKey(string stringKey, Text text)
    {
        bool isDone = false;
        while(!isDone)
        {
            foreach(KeyCode key in Enum.GetValues(typeof(KeyCode)))
                if(Input.GetKeyDown(key))
                {
                    isDone = true;
                    for(int i=0; i<fields.Length; i++)
                        if(fields[i].GetValue(this).GetType().Equals(typeof(KeyCode)))
                            if((KeyCode)fields[i].GetValue(this) == key)
                                ChangeActiveSkillKey(GetKey(i), KeyCode.None);
                    
                    ChangeActiveSkillKey(stringKey, key);
                    SetAllChangeableKeysButtonUI();

                    text.text = key.ToString();
                }
                
            yield return null;
        }
    }

    void ChangeActiveSkillKey(string stringKey, KeyCode newKey)
    {
        FieldInfo field = GetType().GetField(stringKey);
        field.SetValue(this, newKey);
    }

    string GetKey(int id)
    {
        return fields.GetValue(id).ToString().Substring(20);//20 est égale au UnityEngine.KeyCode(+espace)
    }
}