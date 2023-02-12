using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public GameObject defaultDialoguePanel;//panel de base (dialogue)
    public Text dialogueText;//text de base (dialogue)
    [SerializeField] int maxStringPerPanel = 50;//nombre de caracter max pour chaque menu de dialogue
    [SerializeField] int makeAnotherPanelLimit = 5;//nombre min de caracter pour faire un autre menu dialogue

    bool isDialoging = false;
    bool isDone = false;

    void Start()
    {
        defaultDialoguePanel.SetActive(false);
    }

    public void MakeDialogue(string dialogueString , Text _dialogueText, GameObject _dialoguePanel) 
    {
        if(!isDialoging)
            StartCoroutine(WaitDialogue(dialogueString, _dialogueText, _dialoguePanel));
    }

    //_dialogue panel optionel (panel contrôlé si renseigné)
    IEnumerator WaitDialogue(string dialogueString, Text _dialogueText, GameObject _dialoguePanel)//peut utiliser le text et le panel de base
    {
        isDialoging = true;
        if(_dialoguePanel)
            _dialoguePanel.SetActive(true);
        string[] dialogueWord = dialogueString.Split(' ');
        char[] dialogueChar = dialogueString.ToCharArray();

        int dialoguePanelNumber = Mathf.CeilToInt((float)dialogueString.Length/maxStringPerPanel);
        if(dialoguePanelNumber == 0)
            dialoguePanelNumber = 1;

        int charCount = 0;
        int totalCharCount = 0;
        int wordCount = 0;
        for(int i=0; i<dialoguePanelNumber; i++)
        {
            if(wordCount >= dialogueWord.Length)
                continue;
            string phrase = "";
            for(int x=0; x<dialogueWord.Length; x++)
            {
                if(wordCount > x)
                    continue;

                foreach(char _char in dialogueWord[x])
                {
                     charCount++;

                    if(charCount == maxStringPerPanel)
                    {
                        int restCharNumber = 0;
                        for(int y=charCount-1; y<dialogueChar.Length; y++)
                        {
                            restCharNumber++;
                        }
                        if(restCharNumber <= makeAnotherPanelLimit)
                        {
                            charCount --;
                            break;
                        }
                    }
                }
                if(charCount <= maxStringPerPanel)
                {
                    phrase += dialogueWord[x] + " ";
                    wordCount++;
                }
            }
            charCount = 0;
            totalCharCount += charCount;
            //phrase.Insert(0,  + " \n");
            _dialogueText.text = phrase;
            yield return WaitForInput();
        }

        StartCoroutine(WaitDialogue(dialogueString, _dialogueText, _dialoguePanel));
    }
    public void CloseDialogue(GameObject _dialoguePanel)
    {
        StopAllCoroutines();
        isDialoging = false;
        if(_dialoguePanel)
            _dialoguePanel.SetActive(false);
    }

    public void ContinueDialogueButton()
    {
        isDone = true;
    }

    IEnumerator WaitForInput()
    {
        while(!isDone)
        {
            if(Input.GetKeyDown(InputManager.instance.continueDialogue))
            {
                isDone = true;
            }
            yield return null;
        }
        isDone = false;
    }
}
