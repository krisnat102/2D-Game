using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerJumpState : PlayerAbilityState {
	private int amountOfJumpsLeft;

	public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
		amountOfJumpsLeft = playerData.amountOfJumps;
	}

	public override void Enter() {
		base.Enter();
		player.InputHandler.UseJumpInput();
		Movement?.SetVelocityY(playerData.jumpVelocity);
		isAbilityDone = true;
		amountOfJumpsLeft--;
		player.InAirState.SetIsJumping();

		Stats.Stam.Decrease(playerData.jumpCost);
	}

	public bool CanJump() {
		if (amountOfJumpsLeft > 0) {
			return true;
		} else {
			return false;
		}
	}

	public void ResetAmountOfJumpsLeft() => amountOfJumpsLeft = playerData.amountOfJumps;

	public void DecreaseAmountOfJumpsLeft() => amountOfJumpsLeft--;

    public override void Exit()
    {
        base.Exit();
		Stats.Stam.StopRegen(playerData.stamRecoveryTime);
	}
}
