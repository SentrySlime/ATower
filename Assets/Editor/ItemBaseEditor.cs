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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("goldCost"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isDevilItem"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("interactSFX"));

        DrawFoldout("Health", ref showHealth, new string[]
        {
            "addedHealth", "oneMaxHP", "hpOnHit", "hpOnCritHit", "hpOnKill", "hpOnEliteKill" ,"helpingHand", "hpRegen", "hpRegenOnEnemyHit", "healCap", "canOverheal", "onlyEliteKillHeal", "healOnReload", "overkillDamageHeal"
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
            "damage", "increasedDamage", "critChance", "canExplodeEnemies", "moneyIsPower", "hpIsPower", "chainLightningDamage"
        });

        DrawFoldout("Ammo", ref showAmmo, new string[]
        {
            "ammoRefills", "reloadAmount", "maxMagazineSize", "reloadSpeed", "returnAmmoOnkill", "hasAlternateFastReload", "heartboundRounds", "maxAmmo", "bandolierEffect"
        });

        DrawFoldout("Weapon Socket", ref showWeaponSocket, new string[]
        {
            "fireBallChance", "accuracy" , "chainLightningTargets"
        });

        DrawFoldout("Misc", ref showMisc, new string[]
        {
            "hasIncreasedMoneyDrops", "increasedMoneyDrops", "moneyIsHealth", "thorns"
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
