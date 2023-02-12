using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
#if UNITY_EDITOR
[CustomEditor(typeof(QuestScriptableObject), true)]
public class QuestScriptableObjectEditor : Editor
{
    QuestScriptableObject questScriptableObject;
    bool securModifyStep = false;
    bool[] flags = new bool[0];

    public override void OnInspectorGUI()
    { 
        questScriptableObject = (QuestScriptableObject)target;

        List<QuestStep> questSteps = questScriptableObject.questSteps;
        securModifyStep = EditorGUILayout.Toggle("modifier le nombre d'étapes", securModifyStep);
        int numberStep = questSteps.Count;
        if(securModifyStep)
            numberStep = EditorGUILayout.IntField("Étapes: ", questSteps.Count);
        
        if(flags.Length != numberStep)
        {
            flags = new bool[numberStep];
            for(int i=0; i<flags.Length; i++)
                flags[i] = true;
        }
        

        while(numberStep < questSteps.Count)
            questSteps.RemoveAt(questSteps.Count-1);
        while(numberStep > questSteps.Count)
            questSteps.Add(null);
        
        GUIStyle style = new GUIStyle(EditorStyles.textArea);
        style.wordWrap = true;
        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        for(int x=0; x< numberStep; x++)
        {
            if(questSteps[x] == null || flags.Length != numberStep)
                return;

            flags[x] = EditorGUILayout.Toggle("Étape:" + x, flags[x], myFoldoutStyle);
            
            if(flags[x])
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Phrase début de quête:");
                questSteps[x].pnjStartPhrase = EditorGUILayout.TextArea(questSteps[x].pnjStartPhrase, style);
                EditorGUILayout.LabelField("Phrase en cours de quête:");
                questSteps[x].pnjInQuestPhrase = EditorGUILayout.TextArea(questSteps[x].pnjInQuestPhrase, style);
                EditorGUILayout.LabelField("Phrase fin de quête:");
                questSteps[x].pnjEndPhrase = EditorGUILayout.TextArea(questSteps[x].pnjEndPhrase, style);

                questSteps[x].enemyToKillPrefab = (GameObject)EditorGUILayout.ObjectField("l'enemie à tuer:", questSteps[x].enemyToKillPrefab, typeof(GameObject), false);
                if(questSteps[x].enemyToKillPrefab)
                {
                    questSteps[x].numberEnemyToKill = EditorGUILayout.IntField("Le nombre à tuer:", questSteps[x].numberEnemyToKill);
                    questSteps[x].maxNumberEnemyToKill = questSteps[x].numberEnemyToKill;
                }

                questSteps[x].isWinCoin = EditorGUILayout.Toggle("si coins à gagner:", questSteps[x].isWinCoin);
                if(questSteps[x].isWinCoin)
                    questSteps[x].coinWon = EditorGUILayout.IntField("Coins à gagner:", questSteps[x].coinWon);
                
                questSteps[x].itemWon = (GameObject)EditorGUILayout.ObjectField("Item gagné:", questSteps[x].itemWon, typeof(GameObject), false);
                
                questSteps[x].isWinSkill = EditorGUILayout.Toggle("peut gagner un skill:", questSteps[x].isWinSkill);
                if(questSteps[x].isWinSkill)
                    questSteps[x].skillWon = (PlayerSkillsManager.SkillsIDType)EditorGUILayout.EnumPopup("Skill gagné:", questSteps[x].skillWon);

                EditorGUI.indentLevel--;
            }
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(questScriptableObject);
            EditorSceneManager.MarkSceneDirty(GameObject.FindGameObjectWithTag("GameManager").scene);
        }
    }
}
#endif