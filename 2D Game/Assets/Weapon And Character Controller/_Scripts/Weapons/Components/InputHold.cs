using Bardent.Utilities;
using Krisnat;
using System.Threading;
using UnityEngine;

namespace Bardent.Weapons.Components
{
    public class InputHold : WeaponComponent <InputHoldData , AttackInputHold>
    {
        private Animator anim;

        private bool input;

        private bool minHoldPassed;
        private bool attackStarted;
        private float attackHoldTime;
        private bool cooldown;
        private bool canStopCooldown;
        private CoreSystem.Movement movement;

        protected override void HandleEnter()
        {
            base.HandleEnter();

            minHoldPassed = false;
        }

        private void HandleCurrentInputChange(bool newInput)
        {
            input = newInput;

            SetAnimatorParameter();
        }

        private void HandleMinHoldPassed()
        {
            minHoldPassed = true;

            SetAnimatorParameter();
        }

        private void SetAnimatorParameter()
        {
            if (input)
            {
                anim.SetBool("hold", input);
                return;
            }

            if (minHoldPassed)
            {
                anim.SetBool("hold", false);
            }
        }

        private void Attack(CombatInputs combatInput)
        {
            if (currentAttackData != null && !cooldown)
            {
                EndHold();

                cooldown = true;
                Invoke("StopCooldown", 0.1f);

                foreach (var data in currentAttackData.inputHoldAttackDatas)
                {
                    if (data.Projectile != null)
                    {
                        var projectile = Instantiate(data.Projectile).GetComponent<Arrow>();

                        switch (attackHoldTime)
                        {
                            case var holdTime when holdTime < currentAttackData.FirstStageChargeTime:
                                Debug.Log(1);
                                projectile.SetArrowStats(data.Damage, data.Speed / 5f, movement.FacingDirection, data.Offset, Core);
                                break;
                            case var holdTime when holdTime < currentAttackData.SecondStageChargeTime && holdTime > currentAttackData.FirstStageChargeTime:
                                Debug.Log(2);
                                projectile.SetArrowStats(data.Damage, data.Speed / 3.5f, movement.FacingDirection, data.Offset, Core);
                                break;
                            case var holdTime when holdTime < currentAttackData.ThirdStageChargeTime && holdTime > currentAttackData.SecondStageChargeTime:
                                Debug.Log(3);
                                projectile.SetArrowStats(data.Damage, data.Speed / 2f, movement.FacingDirection, data.Offset, Core);
                                break;
                            case var holdTime when holdTime < currentAttackData.FinalStageChargeTime && holdTime > currentAttackData.ThirdStageChargeTime:
                                Debug.Log(3);
                                projectile.SetArrowStats(data.Damage, data.Speed / 1.5f, movement.FacingDirection, data.Offset, Core);
                                break;
                            case var holdTime when holdTime > currentAttackData.FinalStageChargeTime:
                                Debug.Log(4);
                                projectile.SetArrowStats(data.Damage, data.Speed, movement.FacingDirection, data.Offset, Core);
                                break;
                        }
                    }
                }
            }
        }

        private void StopCooldown()
        {
            if(canStopCooldown == true)
            {
                canStopCooldown = false;
                cooldown = false;
            }
            else canStopCooldown = true;
        }

        private void StartHold(CombatInputs combatInput) => attackStarted = true;
        private void EndHold() => attackStarted = false;

        private void Update()
        {
            if (attackStarted)
            {
                attackHoldTime += Time.deltaTime;
            }
            else
            {
                attackHoldTime = 0;
            }
        }
        protected override void Awake()
        {
            base.Awake();

            anim = GetComponentInChildren<Animator>();
            movement = Core.GetCoreComponent<CoreSystem.Movement>();

            weapon.OnCurrentInputChange += HandleCurrentInputChange;
            eventHandler.OnMinHoldPassed += HandleMinHoldPassed;
            PlayerInputHandler.Instance.OnAttackCancelled += Attack;
            PlayerInputHandler.Instance.OnAttackStarted += StartHold;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            weapon.OnCurrentInputChange -= HandleCurrentInputChange;
            eventHandler.OnMinHoldPassed -= HandleMinHoldPassed;
            PlayerInputHandler.Instance.OnAttackCancelled -= Attack;
        }
    }
}