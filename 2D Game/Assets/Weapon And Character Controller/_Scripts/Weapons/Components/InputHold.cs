using Bardent.CoreSystem;
using Krisnat;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Inventory.Item;

namespace Bardent.Weapons.Components
{
    public class InputHold : WeaponComponent <InputHoldData , AttackInputHold>
    {
        private bool input = false;
        private bool minHoldPassed = false;
        private bool attackStarted = false;
        private bool cooldown = false;
        private bool lockMovement = false;
        private bool potentialLockMovement = false;
        private int oldFacingDirection;
        private int flipped = 0;
        private float attackHoldTime;
        private Animator animator;
        private CoreSystem.Movement movement;
        private Slider bowSlider;
        private CollisionSenses collisionSenses;
        private WeaponDataSO currentWeaponData;
        private const float COOLDOWN_DURATION = 0.3f;
        private Stats stats;

        private Stats Stats =>
            stats ? stats : Core.GetCoreComponent(ref stats);

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
            if (movement.IsHanging || (movement.IsCrouching && collisionSenses.Ceiling)) return;

            if (currentAttackData != null && !cooldown && currentWeaponData.Type == WeaponType.Bow && Stats.stam.CurrentValue > 5)
            {
                if (attackHoldTime < currentAttackData.MinimalHoldTime)
                {
                    EndHold();
                    return;
                }

                bowSlider = UIManager.Instance.BowChargeTimeSlider;

                StartCoroutine(CooldownCoroutine());

                AudioManager.instance.PlayWhooshSound(0.8f, 1.2f);

                foreach (var data in currentAttackData.inputHoldAttackData)
                {
                    if (data.Projectile != null)
                    {
                        var projectile = Instantiate(data.Projectile).GetComponent<Arrow>();
                        
                        switch (attackHoldTime)
                        {
                            case var holdTime when holdTime < currentAttackData.PerfectShotChargeTimeUpperRange && holdTime > currentAttackData.PerfectShotChargeTimeLowerRange:
                                projectile.SetArrowStats(data.Damage * 1.5f, data.Speed * 1.1f, data.Piercing, movement.FacingDirection, data.Offset, Core, 6);
                                break;
                            case var holdTime when holdTime < currentAttackData.FirstStageChargeTime:
                                projectile.SetArrowStats(data.Damage / 5f, data.Speed / 5f, data.Piercing, movement.FacingDirection, data.Offset, Core, 1);
                                break;

                            case var holdTime when holdTime < currentAttackData.SecondStageChargeTime && holdTime > currentAttackData.FirstStageChargeTime:
                                projectile.SetArrowStats(data.Damage / 3.5f, data.Speed / 2f, data.Piercing, movement.FacingDirection, data.Offset, Core, 2);
                                break;

                            case var holdTime when holdTime < currentAttackData.ThirdStageChargeTime && holdTime > currentAttackData.SecondStageChargeTime:
                                projectile.SetArrowStats(data.Damage / 2f, data.Speed / 1.5f, data.Piercing, movement.FacingDirection, data.Offset, Core, 3);
                                break;

                            case var holdTime when holdTime < currentAttackData.FinalStageChargeTime && holdTime > currentAttackData.ThirdStageChargeTime:
                                projectile.SetArrowStats(data.Damage / 1.5f, data.Speed / 1.2f, data.Piercing, movement.FacingDirection, data.Offset, Core, 4);
                                break;

                            case var holdTime when holdTime > currentAttackData.FinalStageChargeTime:
                                projectile.SetArrowStats(data.Damage, data.Speed, data.Piercing, movement.FacingDirection, data.Offset, Core, 5);
                                break;
                        }
                    }
                }

                HandleAttackCost();
            }
            
            EndHold();
        }

        private void StartHold(CombatInputs combatInput)
        {
            if (movement.IsHanging || (movement.IsCrouching && collisionSenses.Ceiling)) return;
            if (combatInput == CombatInputs.primary)
            {
                currentWeaponData = Core.transform.parent.Find("PrimaryWeapon").GetComponent<WeaponGenerator>().Data;
            }
            else if (combatInput == CombatInputs.secondary)
            {
                currentWeaponData = Core.transform.parent.Find("SecondaryWeapon").GetComponent<WeaponGenerator>().Data;
            }

            if(currentWeaponData.Type == WeaponType.Bow)
            {
                attackStarted = true;
                if (collisionSenses.Ground)
                {
                    lockMovement = true;
                }
                else
                {
                    potentialLockMovement = true;
                }
            }
        }
        public void EndHold() 
        {
            attackStarted = false;
            lockMovement = false;
            potentialLockMovement = false;

            flipped = 0;
        }

        private IEnumerator CooldownCoroutine()
        {
            cooldown = true;

            yield return new WaitForSeconds(COOLDOWN_DURATION);

            cooldown = false;
        }

        private void HandleAttackCost()
        {
            if (currentAttackData != null && currentAttackData.StamCost)
            {
                Stats.stam.Decrease(currentAttackData.AttackCost);
                Stats.stam.StopRegen(currentAttackData.AttackRecoveryTime);
            }
        }

        private void FlipPlayer() => movement.Flip();
        
        private void Update()
        {
            if (movement.FacingDirection != oldFacingDirection)
            {
                bowSlider.gameObject.transform.localScale *= -1;
            }
            oldFacingDirection = movement.FacingDirection; 

            if (attackStarted)
            {
                if (PlayerInputHandler.Instance.AttackInputs[(int)CombatInputs.primary])
                {
                    EndHold();
                }

                attackHoldTime += Time.deltaTime;

                bowSlider.value = attackHoldTime;
                bowSlider.gameObject.SetActive(true);
            }
            else
            {
                attackHoldTime = 0;
                bowSlider.gameObject.SetActive(false);
            }

            if(potentialLockMovement && collisionSenses.Ground)
            {
                Core.transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
            if(lockMovement)
            {
                Core.transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }

            if(lockMovement && PlayerInputHandler.Instance.NormInputX < 0 && movement.FacingDirection == 1 && flipped != -1)
            {
                FlipPlayer();
                flipped = -1;
            }
            if (lockMovement && PlayerInputHandler.Instance.NormInputX > 0 && movement.FacingDirection == -1 && flipped != 1)
            {
                FlipPlayer();
                flipped = 1;
            }

        }
        protected override void Awake()
        {
            base.Awake();

            animator = GetComponentInChildren<Animator>();
            movement = Core.GetCoreComponent<CoreSystem.Movement>();
            collisionSenses = Core.GetCoreComponent<CollisionSenses>();
            bowSlider = UIManager.Instance.BowChargeTimeSlider;

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