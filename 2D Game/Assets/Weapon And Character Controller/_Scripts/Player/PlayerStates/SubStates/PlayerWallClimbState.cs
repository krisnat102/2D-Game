using Bardent.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallClimbState : PlayerTouchingWallState
{
    public PlayerWallClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Stats.stam.StopRegen();
    }

    public override void Exit()
    {
        base.Exit();
        Stats.stam.StartRegen();
        Stats.stam.StopRegen(playerData.stamRecoveryTime);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            Movement?.SetVelocityY(playerData.wallClimbVelocity);

            if (yInput != 1)
            {
                stateMachine.ChangeState(player.WallGrabState);
            }

            if (Stats.stam.CurrentValue < 0.5)
            {
                stateMachine.ChangeState(player.InAirState);
            }
        }

        float stamDecrease = playerData.wallClimbCost * Time.deltaTime;
        Stats.stam.Decrease(stamDecrease);
    }
}
