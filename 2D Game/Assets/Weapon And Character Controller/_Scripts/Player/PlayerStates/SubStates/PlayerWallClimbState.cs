using Bardent.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallClimbState : PlayerTouchingWallState {
	public PlayerWallClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
	}

    public override void Enter()
    {
        base.Enter();
	}

    public override void Exit()
    {
        base.Exit();
	}

	public override void LogicUpdate() {
		base.LogicUpdate();

		if (!isExitingState) {
			Movement?.SetVelocityY(playerData.wallClimbVelocity);

			if (yInput != 1) {
				stateMachine.ChangeState(player.WallGrabState);
			}

			if(Stats.Stam.CurrentValue == 0)
            {
				stateMachine.ChangeState(player.InAirState);
			}
		}

		float stamDecrease = playerData.wallClimbCost * Time.deltaTime;
		Stats.Stam.Decrease(stamDecrease);
	}
}
