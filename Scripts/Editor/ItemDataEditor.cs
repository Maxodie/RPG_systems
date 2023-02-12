using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
#if UNITY_EDITOR
[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{ 
    public override void OnInspectorGUI()
    { 
        ItemData itemData = (ItemData)target;

        var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};//le style des text
        style.richText = true;

        EditorGUILayout.LabelField("<b>Information Principales</b>", style, GUILayout.ExpandWidth(true));
        itemData.itemName = EditorGUILayout.TextField("Nom:", itemData.itemName);//le nom
        EditorGUILayout.BeginHorizontal();//
        EditorGUILayout.PrefixLabel("Icone:");
        itemData.itemIcone = (Sprite)EditorGUILayout.ObjectField(itemData.itemIcone, typeof(Sprite), allowSceneObjects: true);
        EditorGUILayout.EndHorizontal();//l'icone
        itemData.itemObject = (GameObject)EditorGUILayout.ObjectField("Objet", itemData.itemObject, typeof(GameObject), false);//l'objet
        itemData.isStackable = EditorGUILayout.Toggle("Stackable:", itemData.isStackable);//si peut être stacké X100 max
        itemData.isUnusable = EditorGUILayout.Toggle("N'es pas utilisable:", itemData.isUnusable);
        if(!itemData.isUnusable)
            itemData.maxUseNumber = EditorGUILayout.IntField("Nombre d'utilisation:", itemData.maxUseNumber);//numberUse
        else
            itemData.maxUseNumber = 0;

        if(itemData.weaponType == ItemData.WeaponType.NoWeapon)
            itemData.usingTime = EditorGUILayout.FloatField("Temps d'utilisation:", itemData.usingTime);//usingTime
        else
            itemData.usingTime = 0;
        itemData.timeBeforeReuse = EditorGUILayout.FloatField("Temps avant réutilisation:", itemData.timeBeforeReuse);//timeBeforeReuse
        itemData.dropedItemAtDestroy = (ItemData)EditorGUILayout.ObjectField("Objet obtenue après destruction:", itemData.dropedItemAtDestroy, typeof(ItemData), false);//l'objet donné quand détruit
        itemData.positionInHand = EditorGUILayout.Vector3Field("Position dans la main:", itemData.positionInHand);//position in hand
        itemData.rotationInHand = EditorGUILayout.Vector3Field("Rotation dans la main:", itemData.rotationInHand);//rotation in hand

        EditorGUILayout.LabelField("<b>Buff/Debuff Stats</b>", style, GUILayout.ExpandWidth(true));
        itemData.isConsomable = EditorGUILayout.Toggle("Is Comsomable:", itemData.isConsomable);//isConsomable
        if(itemData.isConsomable)
        {
            itemData.hungerRecup = EditorGUILayout.FloatField("Récup nourriture:", itemData.hungerRecup);//hunger recup
            itemData.healthRecup = EditorGUILayout.FloatField("Récup HP:", itemData.healthRecup);//health recup
        }

        EditorGUILayout.LabelField("<b>Attaque Stats</b>", style, GUILayout.ExpandWidth(true));
        itemData.weaponType = (ItemData.WeaponType)EditorGUILayout.EnumPopup("Type d'arme:", itemData.weaponType);//weaponType
        if(itemData.weaponType != ItemData.WeaponType.NoWeapon)
        {
            itemData.attackDamage = EditorGUILayout.FloatField("Dégâts:", itemData.attackDamage);//Damage
            itemData.attackRange = EditorGUILayout.FloatField("Porté:", itemData.attackRange);//Range
        }

        EditorGUILayout.LabelField("<b>Défense Stats</b>", style, GUILayout.ExpandWidth(true));
        itemData.isArmor = EditorGUILayout.Toggle("Is Armor", itemData.isArmor);//IsArmor
        if(itemData.isArmor)
        {
            itemData.armor = EditorGUILayout.FloatField("Protection:", itemData.armor);//Armor
        }
        
        EditorGUILayout.LabelField("<b>Cuisine</b>", style, GUILayout.ExpandWidth(true));
        itemData.isCookable = EditorGUILayout.Toggle("Is Coockable:", itemData.isCookable);//IsCoockable
        if(itemData.isCookable)
        {
            itemData.coockingTime = EditorGUILayout.FloatField("Temps de cuisson:", itemData.coockingTime);//CoockingTime
            itemData.itemCoockingResult = (ItemData)EditorGUILayout.ObjectField("Objet résultat de cuisson:", itemData.itemCoockingResult, typeof(ItemData), false);
        }

        itemData.isCombustible = EditorGUILayout.Toggle("isCombustible:", itemData.isCombustible);//isCombustible
        if(itemData.isCombustible)
        {
            itemData.combustibleNumberOfTime = EditorGUILayout.IntField("Nombre de fois combustible:", itemData.combustibleNumberOfTime);//combustibleNumberOfTime
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(itemData);
            EditorSceneManager.MarkSceneDirty(GameObject.FindGameObjectWithTag("GameManager").scene);
        }
    }
}
#endif