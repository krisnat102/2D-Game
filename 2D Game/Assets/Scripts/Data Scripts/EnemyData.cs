using UnityEngine;
using Inventory;
using Spells;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Enemy Data /Base Data")]
public class EnemyData : ScriptableObject
{
    [Header("General Stats")]
    public string enemyName;
    public float maxHP = 100f;
    [Tooltip("Higher value is lower attack speed")]
    public float attackSpeed = 1f;
    public float speed = 200f;
    public float damage = 20f;
    public float actionCooldown = 0.5f;

    [Header("Knockback")]
    [Tooltip("Higher value is higher knockback")]
    public float knockbackModifier = 5f;
    [Tooltip("Higher value is lower horizontal knockback")]
    public float knockbackWeight = 2f;

    [Header("UI")]
    [Tooltip("How long the delay is for the hp bar to appear")]
    public float hpBarDelay = 0f;

    [Header("Attack")]
    [Tooltip("True is physical damage, false is magical")]
    public bool damageType;
    public float attackAnimationLength = 0.3f;
    public float damageTriggerTime = 0.3f;
    public Rect HitBox;
    public LayerMask DetectableLayers;

    [Header("Attack Behaviour")]
    public bool fixRotationWhenAttacking;
    public bool fixLeftAttack;
    public bool rootWhenAttacking;
    public bool commitDirectionWhenAttacking;
    public bool moveWhenAttacking;
    public Vector2 direction;
    public Rect cancelMove;
    public float velocity;
    public float movementDelay;

    [Header("Dash")]
    public bool canDash;
    public float dashCooldown;
    public float dashStrength;
    public float dashDuration;

    [Header("Drops")]
    public int minCoinsDropped;
    public int maxCoinsDropped;
    [Range(0,1)]
    public float itemDropChance;
    [Range(0, 1)]
    public float spellDropChance;
    public Item itemDrop;
    public Spell spellDrop;

    [Header("Audio")]
    public float[] pitchVarianceAttack = new float[2];
    public float[] pitchVarianceRangedAttack = new float[2];
    public float[] pitchVarianceDamage = new float[2];

    [Header("Patrol")]
    public float patrolPauseTime;
    public float patrolSpeed;

    [Header("Behaviour")]
    public bool dummy = false;
    public bool lookAtPlayer;
    public bool facingDirection;
    public bool patrol;
    public float wakeUpTime;

    [Header("Boss")]
    public bool boss;
    public GameObject bossProjectile;
    public float rangedAttackSoundDelay;
    public GameObject bossSpecialProjectile;
    public float specialRangedAttackChargeTime;
    public float specialRangedAttackChargeExecutionTime;
    public float specialRangedAttackCooldown;
    public AudioClip bossMusic;

    [Header("Blood")]
    public GameObject bloodEffect;
    public Vector2 bloodOffset;

    [Header("Other")]
    public GameObject deathEffect;


    #region Variable Dependencies
    [Header("Ranged")]
    public bool ranged = false;
    [HideInInspector]
    public float aimDelay = 0.1f;
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
    //[System.Obsolete]
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
            script.aimDelay = EditorGUILayout.FloatField("Aim Delay", script.aimDelay);
            script.rangedSpeed = EditorGUILayout.FloatField("Projectile Speed", script.rangedSpeed);
            script.distanceOffset = EditorGUILayout.FloatField("Distance Offset", script.distanceOffset);
        }
    }
}
#endif
#endregion
