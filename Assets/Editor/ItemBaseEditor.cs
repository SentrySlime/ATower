using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemBase))]
public class ItemBaseEditor : Editor
{
    private bool showHealth = true;
    private bool showDefense = true;
    private bool showMovement = true;
    private bool showDamage = true;
    private bool showAmmo = true;
    private bool showWeaponSocket = true;
    private bool showMisc = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemDescription"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemIcon"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemMesh"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemMaterial"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isDevilItem"));

        DrawFoldout("Health", ref showHealth, new string[]
        {
            "maxHp", "oneMaxHP", "hpOnHit", "hpOnCritHit", "hpOnKill", "helpingHand", "hpRegen", "hpRegenOnEnemyHit", 
        });

        DrawFoldout("Defense", ref showDefense, new string[]
        {
            "damageReductionPercent", "damageIgnoreChance"
        });

        DrawFoldout("Movement", ref showMovement, new string[]
        {
            "moveSpeed", "extraJumps"
        });

        DrawFoldout("Damage", ref showDamage, new string[]
        {
            "damage", "critChance", "canExplodeEnemies", "moneyIsPower"
        });

        DrawFoldout("Ammo", ref showAmmo, new string[]
        {
            "ammoRefills", "reloadAmount", "maxMagazineSize", "reloadSpeed", "returnAmmoOnkill", "hasAlternateFastReload", "heartboundRounds"
        });

        DrawFoldout("Weapon Socket", ref showWeaponSocket, new string[]
        {
            "fireBallChance"
        });

        DrawFoldout("Misc", ref showMisc, new string[]
        {
            "hasIncreasedMoneyDrops", "increasedMoneyDrops", "moneyIsHealth"
        });

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawFoldout(string title, ref bool toggle, string[] fields)
    {
        toggle = EditorGUILayout.Foldout(toggle, title, true);
        if (toggle)
        {
            EditorGUI.indentLevel++;
            foreach (string field in fields)
            {
                SerializedProperty prop = serializedObject.FindProperty(field);
                if (prop != null)
                {
                    EditorGUILayout.PropertyField(prop);
                }
            }
            EditorGUI.indentLevel--;
        }
    }
}
