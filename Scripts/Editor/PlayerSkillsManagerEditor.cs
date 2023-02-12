using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
#if UNITY_EDITOR
[CustomEditor(typeof(Skill), true)]
public class PlayerSkillsManagerEditor : Editor
{
    Skill skill;
    bool changeStats = false;

    public override void OnInspectorGUI()
    {
        skill = (Skill)target;
        var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};

        EditorGUILayout.LabelField("SKILL", style, GUILayout.ExpandWidth(true));
        skill.skillCategorieType = (PlayerSkillsManager.SkillCategoriesType)EditorGUILayout.EnumPopup("Catégorie du skill:", skill.skillCategorieType);
        skill.skillType = (PlayerSkillsManager.SkillsIDType)EditorGUILayout.EnumPopup("Skill:", skill.skillType);//le skill dans l'enum (casi identique au nom)
        skill.skillName = EditorGUILayout.TextField("nom de compétence:", skill.skillName);
        EditorGUILayout.LabelField("Skill actif", style, GUILayout.ExpandWidth(true));
        skill.isActiveSkill = EditorGUILayout.Toggle("Compétence active:", skill.isActiveSkill);
        if(skill.isActiveSkill)
        {
            EditorGUI.indentLevel ++;
            if(!skill.isInstantiateSkill)
                skill.activeSprite = (Sprite)EditorGUILayout.ObjectField("Sprite du skill", skill.activeSprite, typeof(Sprite), false);
            skill.startSkillEffects = (GameObject)EditorGUILayout.ObjectField("Effets début du skill:", skill.startSkillEffects, typeof(GameObject), false);
            skill.duringSkillEffects = (GameObject)EditorGUILayout.ObjectField("Effets pendant le skill:", skill.duringSkillEffects, typeof(GameObject), false);
            skill.activeTimeOfActiveSkill = EditorGUILayout.FloatField("Temps d'activation:", skill.activeTimeOfActiveSkill);
            if(!skill.isInstantiateSkill)
                skill.duringActiveTime = EditorGUILayout.FloatField("duration du skill(/s):", skill.duringActiveTime);
            else
                skill.duringActiveTime = 0;
            skill.manaCost = EditorGUILayout.FloatField("Coût en mana:", skill.manaCost);
            if(skill.increasedPlayerLife!=0)
                skill.isPermanentStatsSkill = EditorGUILayout.Toggle("si c'est un heal (mettre la vie permanante):", skill.isPermanentStatsSkill);
            skill.reloadActiveSkill = EditorGUILayout.FloatField("Temps de recharge du skill(/s):", skill.reloadActiveSkill);
            EditorGUI.indentLevel --;
        }
        
        EditorGUILayout.LabelField("Skill instantié", style, GUILayout.ExpandWidth(true));
        skill.isInstantiateSkill = EditorGUILayout.Toggle("Skill instantié:", skill.isInstantiateSkill);
        if(skill.isInstantiateSkill)
        {
            skill.isActiveSkill = true;
            EditorGUI.indentLevel ++;
            skill.instantiateSkillPrefab = (GameObject)EditorGUILayout.ObjectField("Objet instantié:", skill.instantiateSkillPrefab, typeof(GameObject), false);
            skill.instantiateSkillSpeed = EditorGUILayout.FloatField("vitesse du skill:", skill.instantiateSkillSpeed);
            skill.instantiateSkillDamage = EditorGUILayout.FloatField("dégâts du skill:", skill.instantiateSkillDamage);
            skill.instantiateSkillDestroyTime = EditorGUILayout.FloatField("Temps avant destroy:", skill.instantiateSkillDestroyTime);
            skill.isUpgradingDestroyTime = EditorGUILayout.Toggle("Améliore temps avant destroy:", skill.isUpgradingDestroyTime);
            skill.isChangeRotation = EditorGUILayout.Toggle("changer la rotation d'instantiation:", skill.isChangeRotation);
            skill.isFollowPlayer = EditorGUILayout.Toggle("mettre dans un transform du joueur:", skill.isFollowPlayer);
            EditorGUI.indentLevel --;
        }

        EditorGUILayout.LabelField("Level", style, GUILayout.ExpandWidth(true));
        skill.addXpPerUsing = EditorGUILayout.FloatField("Xp add par utilisation:", skill.addXpPerUsing);
        skill.reachXp = EditorGUILayout.FloatField("Xp requis for lvl up:", skill.reachXp);
        skill.additionalUpradeStats = EditorGUILayout.FloatField("nb sup aux stats par lvl(!=0):", skill.additionalUpradeStats);

        EditorGUILayout.LabelField("Stats", style, GUILayout.ExpandWidth(true));
        if(!changeStats)
        {
            if(skill.swordDamage==0&&skill.increasedPlayerLife==0&&skill.increasePlayerDodge==0&&skill.increasePlayerStrength==0&&skill.increasePlayerSpeed==0)
                EditorGUILayout.LabelField("(Toutes les stats = 0)", style, GUILayout.ExpandWidth(true));
            else
                EditorGUILayout.LabelField("(Toutes les stats != 0)", style, GUILayout.ExpandWidth(true));
        }
        changeStats = EditorGUILayout.Toggle("Modifier les stats (flag):", changeStats);
        if(changeStats)
        {
            EditorGUI.indentLevel ++;
            skill.swordDamage = EditorGUILayout.FloatField("% damage avec épé:", skill.swordDamage);
            skill.increasedPlayerLife = EditorGUILayout.FloatField("Vie supplémentaire:", skill.increasedPlayerLife);
            if(skill.increasedPlayerLife != 0)
                skill.canIncreaseUpperThanCurrentHealth = EditorGUILayout.Toggle("+de vie max:", skill.canIncreaseUpperThanCurrentHealth);
            skill.increasePlayerDodge = EditorGUILayout.FloatField("% ésquive des dégâts:", skill.increasePlayerDodge);
            skill.increasePlayerStrength = EditorGUILayout.FloatField("% force en plus:", skill.increasePlayerStrength);
            skill.increasePlayerSpeed = EditorGUILayout.FloatField("% vitesse en plus:", skill.increasePlayerSpeed);
            EditorGUI.indentLevel --;
        }
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(skill);
            EditorSceneManager.MarkSceneDirty(GameObject.FindGameObjectWithTag("GameManager").scene);
        }
    }
}
#endif