using Bardent.CoreSystem;
using UnityEngine;

public class PlayerRollState : PlayerAbilityState
{
    public bool CanRoll { get; private set; }
    private bool isHolding;
    private bool rollInputStop;

    private float lastRollTime;

    private Vector2 rollDirection;
    private int rollDirectionInput;
    private Vector2 lastAIPos;

    public PlayerRollState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        CanRoll = false;
        player.InputHandler.UseDashInput();

        AudioManager.Instance.PlayDodgeRollSound(0.8f, 1.2f);

        isHolding = true;
        rollDirection = Vector2.right * Movement.FacingDirection;

        Stats.stam.Decrease(playerData.rollCost);
        Stats.stam.StopRegen(playerData.stamRecoveryTime);

        core.GetCoreComponent<DamageReceiver>().Invincible = true;
    }

    public override void Exit()
    {
        base.Exit();

        core.GetCoreComponent<DamageReceiver>().Invincible = false;

        ResetCanRoll();

        if (Movement?.CurrentVelocity.y > 0)
        {
            Movement?.SetVelocityY(Movement.CurrentVelocity.y * playerData.dashEndYMultiplier);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {

            player.Anim.SetFloat("yVelocity", Movement.CurrentVelocity.y);
            player.Anim.SetFloat("xVelocity", Mathf.Abs(Movement.CurrentVelocity.x));

            if (isHolding)
            {
                rollDirectionInput = player.InputHandler.DashDirectionInput.x;
                rollInputStop = player.InputHandler.DashInputStop;

                if (rollDirectionInput != 0)
                {
                    rollDirection = new Vector2(rollDirectionInput, 0);
                    rollDirection.Normalize();
                }

                if (rollInputStop || Time.unscaledTime >= startTime + playerData.maxHoldTime)
                {
                    isHolding = false;
                    Time.timeScale = 1f;
                    startTime = Time.time;
                    Movement?.CheckIfShouldFlip(Mathf.RoundToInt(rollDirection.x));
                    player.RB.drag = playerData.drag;
                    Movement?.SetVelocity(playerData.rollVelocity, rollDirection);
                    PlaceAfterImage();
                }
            }
            else
            {
                //dashDirection = new Vector2(dashDirection.x, dashDirection.y + 0.005f);
                Movement?.SetVelocity(playerData.rollVelocity, rollDirection);
                CheckIfShouldPlaceAfterImage();

                if (Time.time >= startTime + playerData.rollTime)
                {
                    player.RB.drag = 0f;
                    isAbilityDone = true;
                    lastRollTime = Time.time;
                }
            }
        }
    }

    private void CheckIfShouldPlaceAfterImage()
    {
        if (Vector2.Distance(player.transform.position, lastAIPos) >= playerData.distBetweenAfterImages)
        {
            PlaceAfterImage();
        }
    }

    private void PlaceAfterImage()
    {
        PlayerAfterImagePool.Instance.GetFromPool();
        lastAIPos = player.transform.position;
    }

    public bool CheckIfCanRoll()
    {
        return CanRoll && Time.time >= lastRollTime + playerData.rollCooldown;
    }

    public void ResetCanRoll() => CanRoll = true;

}