using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Enemy Data /Base Data")]
public class EnemyData : ScriptableObject
{
    [Header("General Stats")]
    public int level = 1;
    public string enemyName;
    public float maxHP = 100f;
    public float attackSpeed = 1f;
    public float speed = 200f;
    public float damage = 20f;
    public float knockbackModifier = 5f;

    [Header("Offsets")]
    public float offsetX = 1.2f;
    public float offsetX2 = 1.2f;
    public float offsetY = -1f;

    [Header("Attack")]
    [Tooltip("True is physical damage, false is magical")]
    public bool damageType;
    public float attackAnimLength = 0.3f;
    public Rect HitBox;
    public LayerMask DetectableLayers;

    [Header("Drops")]
    public int minCoinsDropped;
    public int maxCoinsDropped;

    [Header("Behaviour")]
    public bool lookAtPlayer;
    public bool flipWhenPlayerIsRight;

    [Header("Other")]
    public GameObject deathEffect;
    public GameObject bloodEffect;

    #region Variable Dependencies
    [Header("Ranged")]
    //[HideInInspector]
    public bool ranged = false;
    [HideInInspector]
    public GameObject impactEffect;
    [HideInInspector]
    public float rangedDamage;
    [HideInInspector]
    public float rangedSpeed;
    [HideInInspector]
    public float distanceOffset;
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyData))]
public class Enemy_Editor : Editor
{
    [System.Obsolete]
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields

        EnemyData script = (EnemyData)target;

        // draw checkbox for the bool
        //script.ranged = EditorGUILayout.Toggle("Ranged", script.ranged);
        if (script.ranged) // if bool is true, show other fields
        {
            script.impactEffect = EditorGUILayout.ObjectField("Impact Effect", script.impactEffect, typeof(GameObject)) as GameObject;
            script.rangedDamage = EditorGUILayout.FloatField("Damage", script.rangedDamage);
            script.rangedSpeed = EditorGUILayout.FloatField("Projectile Speed", script.rangedSpeed);
            script.distanceOffset = EditorGUILayout.FloatField("Distance Offset", script.distanceOffset);
        }
    }
}
#endif
#endregion
