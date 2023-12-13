using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        public Vector2 RawMovementInput { get; private set; }
        public int NormInputX { get; private set; }
        public int NormInputY { get; private set; }
        public bool JumpInput { get; private set; }
        public bool CrouchInput { get; private set; }
        public bool DodgeInput { get; private set; }
        public bool SwordInput { get; private set; }
        public bool GunInput { get; private set; }
        public bool InventoryInput { get; private set; }
        public bool SpellInventoryInput { get; private set; }
        public bool MenuInput { get; private set; }
        public bool SpellInput { get; private set; }
        public bool AttackInput { get; private set; }
        public bool AbilityInput { get; private set; }
        public bool UseInput { get; private set; }
        public bool ReloadInput { get; private set; }
        public bool SwitchSpell1Input { get; private set; }
        public bool SwitchSpell2Input { get; private set; }
        public bool SwitchAbility1Input { get; private set; }
        public bool SwitchAbility2Input { get; private set; }
        public Vector2 MousePosition { get; private set; }

        [SerializeField] private float inputHoldTime = 0.2f;
        private float jumpInputStartTime;

        private void Update()
        {
            CheckJumpInputHoldTime();
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #region Inputs
        public void OnMoveInput(InputAction.CallbackContext context)
        {
            RawMovementInput = context.ReadValue<Vector2>();

            if (Mathf.Abs(RawMovementInput.x) > 0.5f)
            {
                NormInputX = Mathf.RoundToInt(RawMovementInput.x);
            }
            else
            {
                NormInputX = 0;
            }
            if (Mathf.Abs(RawMovementInput.y) > 0.5f)
            {
                NormInputY = Mathf.RoundToInt(RawMovementInput.y);
            }
            else
            {
                NormInputY = 0;
            }
        }

        public void OnJumpInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                JumpInput = true;
                jumpInputStartTime = Time.time;
            }
        }

        public void OnCrouchInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                CrouchInput = true;
            }
            if (context.canceled)
            {
                CrouchInput = false;
            }
        }

        public void OnDodgeInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                DodgeInput = true;
            }
            if (context.canceled)
            {
                DodgeInput = false;
            }
        }

        public void OnSwordInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                SwordInput = true;
            }
            if (context.canceled)
            {
                SwordInput = false;
            }
        }

        public void OnGunInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                GunInput = true;
            }
            if (context.canceled)
            {
                GunInput = false;
            }
        }

        public void OnInventoryInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                InventoryInput = true;
            }
        }

        public void OnSpellInventoryInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                SpellInventoryInput = true;
            }
            if (context.canceled)
            {
                SpellInventoryInput = false;
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
            if (context.started)
            {
                SpellInput = true;
            }
            if (context.canceled)
            {
                SpellInput = false;
            }
        }

        public void OnAttackInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                AttackInput = true;
            }
            if (context.canceled)
            {
                AttackInput = false;
            }
        }

        public void OnAbilityInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                AbilityInput = true;
            }
            if (context.canceled)
            {
                AbilityInput = false;
            }
        }

        public void OnUseInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                UseInput = true;
            }
            if (context.canceled)
            {
                UseInput = false;
            }
        }

        public void OnReloadInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ReloadInput = true;
            }
            if (context.canceled)
            {
                ReloadInput = false;
            }
        }

        public void OnSwitchSpell1Input(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                SwitchSpell1Input = true;
            }
            if (context.canceled)
            {
                SwitchSpell1Input = false;
            }
        }

        public void OnSwitchSpell2Input(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                SwitchSpell2Input = true;
            }
            if (context.canceled)
            {
                SwitchSpell2Input = false;
            }
        }

        public void OnSwitchAbility1Input(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                SwitchAbility1Input = true;
            }
            if (context.canceled)
            {
                SwitchAbility1Input = false;
            }
        }

        public void OnSwitchAbility2Input(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                SwitchAbility2Input = true;
            }
            if (context.canceled)
            {
                SwitchAbility2Input = false;
            }
        }

        public void MousePositionInput(InputAction.CallbackContext context)
        {
            MousePosition = context.ReadValue<Vector2>();
        }
        #endregion

        public void UseJumpInput() => JumpInput = false;
        public void UseInventoryInput() => InventoryInput = false;
        public void UseSpellInventoryInput() => SpellInventoryInput = false;
        public void UseMenuInpit() => MenuInput = false;

        private void CheckJumpInputHoldTime()
        {
            if (Time.time >= jumpInputStartTime + inputHoldTime)
            {
                JumpInput = false;
            }
        }
    }
}