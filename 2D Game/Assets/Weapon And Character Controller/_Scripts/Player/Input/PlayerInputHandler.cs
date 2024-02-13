using Bardent.Weapons;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    #region OtherVariables
    public static PlayerInputHandler Instance { get; private set; }

    private bool stopAllInputs = false;
    private bool stopAttack = false;

    private PlayerInput playerInput;
    private Camera cam;

    public delegate void AttackCancelledHandler(CombatInputs combatInput);
    public event AttackCancelledHandler OnAttackCancelled;

    public delegate void AttackStartedHandler(CombatInputs combatInput);
    public event AttackCancelledHandler OnAttackStarted;

    [SerializeField]
    private float inputHoldTime = 0.2f;

    public bool StopAllInputs { get => stopAllInputs; set => stopAllInputs = value; }
    public bool StopAttack { get => stopAttack; set => stopAttack = value; }
    #endregion

    #region StartTimeVariables
    private float jumpInputStartTime;
    private float dashInputStartTime;
    #endregion

    #region VariableInputs
    public Vector2 RawMovementInput { get; private set; }
    public Vector2 RawDashDirectionInput { get; private set; }
    public Vector2Int DashDirectionInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }
    public bool GrabInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool DashInputStop { get; private set; }
    public bool InventoryInput { get; private set; }
    public bool SpellInventoryInput { get; private set; }
    public bool CharacterTabInput { get; private set; }
    public bool MenuInput { get; private set; }
    public bool SpellInput { get; private set; }
    public bool AbilityInput { get; private set; }
    public bool UseInput { get; private set; }
    public bool SwitchSpell1Input { get; private set; }
    public bool SwitchSpell2Input { get; private set; }
    public bool SwitchAbility1Input { get; private set; }
    public bool SwitchAbility2Input { get; private set; }
    public Vector2 MousePosition { get; private set; }

    public bool[] AttackInputs { get; private set; }
    #endregion

    #region UnityMethods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        int count = Enum.GetValues(typeof(CombatInputs)).Length;
        AttackInputs = new bool[count];

        cam = Camera.main;
    }

    private void Update()
    {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();
    }
    #endregion

    #region Inputs
    public void OnPrimaryAttackInput(InputAction.CallbackContext context)
    {
        var weapon = GetComponentsInChildren<WeaponGenerator>()[0].Data;
        if (context.started && !StopAllInputs && !StopAttack && weapon != null)
        {
            AttackInputs[(int)CombatInputs.primary] = true;
            OnAttackStarted?.Invoke(CombatInputs.primary);
        }

        if (context.canceled && !StopAllInputs && !StopAttack && weapon != null)
        {
            AttackInputs[(int)CombatInputs.primary] = false;
            OnAttackCancelled?.Invoke(CombatInputs.primary);
        }
    }

    public void OnSecondaryAttackInput(InputAction.CallbackContext context)
    {
        var weapon = GetComponentsInChildren<WeaponGenerator>()[1].Data;
        if (context.started && !StopAllInputs && !StopAttack && weapon != null)
        {
            AttackInputs[(int)CombatInputs.secondary] = true;            
            OnAttackStarted?.Invoke(CombatInputs.secondary);
        }

        if (context.canceled && !StopAllInputs && !StopAttack && weapon != null)
        {
            AttackInputs[(int)CombatInputs.secondary] = false;
            OnAttackCancelled?.Invoke(CombatInputs.secondary);
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();

        NormInputX = Mathf.RoundToInt(RawMovementInput.x);
        NormInputY = Mathf.RoundToInt(RawMovementInput.y);

    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            JumpInput = true;
            JumpInputStop = false;
            jumpInputStartTime = Time.time;
        }

        if (context.canceled && !StopAllInputs)
        {
            JumpInputStop = true;
        }
    }

    public void OnGrabInput(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            GrabInput = true;
        }

        if (context.canceled && !StopAllInputs)
        {
            GrabInput = false;
        }
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            DashInput = true;
            DashInputStop = false;
            dashInputStartTime = Time.time;
        }
        else if (context.canceled && !StopAllInputs)
        {
            DashInputStop = true;
        }
    }

    public void OnDashDirectionInput(InputAction.CallbackContext context)
    {
        if (!StopAllInputs)
        {
            RawDashDirectionInput = context.ReadValue<Vector2>();

            if (playerInput.currentControlScheme == "Keyboard")
            {
                RawDashDirectionInput = cam.ScreenToWorldPoint((Vector3)RawDashDirectionInput) - transform.position;
            }

            DashDirectionInput = Vector2Int.RoundToInt(RawDashDirectionInput.normalized);
        }
    }
    public void OnInventoryInput(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            InventoryInput = true;
        }
    }

    public void OnSpellInventoryInput(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            SpellInventoryInput = true;
        }
        if (context.canceled && !StopAllInputs)
        {
            SpellInventoryInput = false;
        }
    }

    public void OnCharacterTabInput(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            CharacterTabInput = true;
        }
        if (context.canceled && !StopAllInputs)
        {
            CharacterTabInput = false;
        }
    }

    public void OnMenuInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            MenuInput = true;
        }
        if (context.canceled)
        {
            MenuInput = false;
        }
    }

    public void OnSpellInput(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            SpellInput = true;
        }
        if (context.canceled && !StopAllInputs)
        {
            SpellInput = false;
        }
    }

    public void OnAbilityInput(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            AbilityInput = true;
        }
        if (context.canceled && !StopAllInputs)
        {
            AbilityInput = false;
        }
    }

    public void OnUseInput(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            UseInput = true;
        }
        if (context.canceled && !StopAllInputs)
        {
            UseInput = false;
        }
    }

    public void OnSwitchSpell1Input(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            SwitchSpell1Input = true;
        }
        if (context.canceled && !StopAllInputs)
        {
            SwitchSpell1Input = false;
        }
    }

    public void OnSwitchSpell2Input(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            SwitchSpell2Input = true;
        }
        if (context.canceled && !StopAllInputs)
        {
            SwitchSpell2Input = false;
        }
    }

    public void OnSwitchAbility1Input(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            SwitchAbility1Input = true;
        }
        if (context.canceled && !StopAllInputs)
        {
            SwitchAbility1Input = false;
        }
    }

    public void OnSwitchAbility2Input(InputAction.CallbackContext context)
    {
        if (context.started && !StopAllInputs)
        {
            SwitchAbility2Input = true;
        }
        if (context.canceled && !StopAllInputs)
        {
            SwitchAbility2Input = false;
        }
    }

    public void MousePositionInput(InputAction.CallbackContext context)
    {
        MousePosition = context.ReadValue<Vector2>();
    }
    #endregion

    #region UseInput
    public void UseJumpInput() => JumpInput = false;
    public void UseDashInput() => DashInput = false;
    public void UseUseInput() => UseInput = false;
    public void UseInventoryInput() => InventoryInput = false;
    public void UseSpellInventoryInput() => SpellInventoryInput = false;
    public void UseCharacterTabInput() => CharacterTabInput = false;
    public void UseMenuInpit() => MenuInput = false;
    public void UseSwitchSpell1Input() => SwitchSpell1Input = false;
    public void UseSwitchSpell2Input() => SwitchSpell2Input = false;
    public void UseSwitchAbility1Input() => SwitchAbility1Input = false;
    public void UseSwitchAbility2Input() => SwitchAbility2Input = false;
    #endregion

    #region CheckInputs
    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }

    private void CheckDashInputHoldTime()
    {
        if (Time.time >= dashInputStartTime + inputHoldTime)
        {
            DashInput = false;
        }
    }
    #endregion
}

public enum CombatInputs
{
    primary,
    secondary
}
