using Bardent.Utilities;
using Krisnat;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Bardent.Weapons.Components
{
    public class InputHold : WeaponComponent <InputHoldData , AttackInputHold>
    {
        private Animator animator;

        private bool input;

        private bool minHoldPassed;
        private bool attackStarted;
        private float attackHoldTime;
        private bool cooldown;
        private CoreSystem.Movement movement;
        private Slider bowSlider;
        private int oldFacingDirection;
        private const float COOLDOWN_DURATION = 0.3f;

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
                animator.SetBool("hold", input);
                return;
            }

            if (minHoldPassed)
            {
                animator.SetBool("hold", false);
            }
        }

        private void Attack(CombatInputs combatInput)
        {
            if (currentAttackData != null && !cooldown && weaponData.Type == "Bow")
            {
                bowSlider = UIManager.Instance.BowChargeTimeSlider;

                EndHold();

                StartCoroutine(CooldownCoroutine());

                foreach (var data in currentAttackData.inputHoldAttackDatas)
                {
                    if (data.Projectile != null)
                    {
                        var projectile = Instantiate(data.Projectile).GetComponent<Arrow>();

                        switch (attackHoldTime)
                        {
                            case var holdTime when holdTime < currentAttackData.PerfectShotChargeTimeUpperRange && holdTime > currentAttackData.PerfectShotChargeTimeLowerRange:
                                projectile.SetArrowStats(data.Damage * 1.5f, data.Speed * 1.1f, movement.FacingDirection, data.Offset, Core, 6);
                                break;
                            case var holdTime when holdTime < currentAttackData.FirstStageChargeTime:
                                projectile.SetArrowStats(data.Damage / 5f, data.Speed / 5f, movement.FacingDirection, data.Offset, Core, 1);
                                break;

                            case var holdTime when holdTime < currentAttackData.SecondStageChargeTime && holdTime > currentAttackData.FirstStageChargeTime:
                                projectile.SetArrowStats(data.Damage / 3.5f, data.Speed / 2f, movement.FacingDirection, data.Offset, Core, 2);
                                break;

                            case var holdTime when holdTime < currentAttackData.ThirdStageChargeTime && holdTime > currentAttackData.SecondStageChargeTime:
                                projectile.SetArrowStats(data.Damage / 2f, data.Speed / 1.5f, movement.FacingDirection, data.Offset, Core, 3);
                                break;

                            case var holdTime when holdTime < currentAttackData.FinalStageChargeTime && holdTime > currentAttackData.ThirdStageChargeTime:
                                projectile.SetArrowStats(data.Damage / 1.5f, data.Speed / 1.2f, movement.FacingDirection, data.Offset, Core, 4);
                                break;

                            case var holdTime when holdTime > currentAttackData.FinalStageChargeTime:
                                projectile.SetArrowStats(data.Damage, data.Speed, movement.FacingDirection, data.Offset, Core, 5);
                                break;
                        }
                    }
                }
            }
        }
        private IEnumerator CooldownCoroutine()
        {
            cooldown = true;

            yield return new WaitForSeconds(COOLDOWN_DURATION);

            cooldown = false;
        }

        private void StartHold(CombatInputs combatInput) => attackStarted = true;
        private void EndHold() => attackStarted = false;

        private void Update()
        {
            if (movement.FacingDirection != oldFacingDirection)
            {
                bowSlider.gameObject.transform.localScale *= -1;
            }
            oldFacingDirection = movement.FacingDirection; 

            if (attackStarted)
            {
                attackHoldTime += Time.deltaTime;

                bowSlider.gameObject.SetActive(true);
                bowSlider.value = attackHoldTime;
            }
            else
            {
                attackHoldTime = 0;
                bowSlider.gameObject.SetActive(false);
            }

            if(attackHoldTime > 0)
            {
                Core.transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                //if(attackHoldTime > currentAttackData.PerfectShotChargeTimeLowerRange && attackHoldTime < currentAttackData.PerfectShotChargeTimeUpperRange)
                {
                    
                }
            }
            
        }
        protected override void Awake()
        {
            base.Awake();

            animator = GetComponentInChildren<Animator>();
            movement = Core.GetCoreComponent<CoreSystem.Movement>();

            weapon.OnCurrentInputChange += HandleCurrentInputChange;
            eventHandler.OnMinHoldPassed += HandleMinHoldPassed;
            PlayerInputHandler.Instance.OnAttackCancelled += Attack;
            PlayerInputHandler.Instance.OnAttackStarted += StartHold;
            bowSlider = UIManager.Instance.BowChargeTimeSlider;
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