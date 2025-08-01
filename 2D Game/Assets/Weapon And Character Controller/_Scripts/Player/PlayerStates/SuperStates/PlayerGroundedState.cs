﻿using System.Collections;
using System.Collections.Generic;
using Bardent.CoreSystem;
using Spells;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected int xInput;
    protected int yInput;

    protected bool isTouchingCeiling;

    protected Movement Movement
    {
        get => movement ?? core.GetCoreComponent(ref movement);
    }

    private Movement movement;

    private CollisionSenses CollisionSenses
    {
        get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses);
    }

    private CollisionSenses collisionSenses;

    private bool jumpInput;
    private bool grabInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isTouchingLedge;
    private bool dashInput;

    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData,
        string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
            isTouchingWall = CollisionSenses.WallFront;
            isTouchingLedge = CollisionSenses.LedgeHorizontal;
            isTouchingCeiling = CollisionSenses.Ceiling;
        }
    }

    public override void Enter()
    {
        base.Enter();

        player.JumpState.ResetAmountOfJumpsLeft();
        player.DashState.ResetCanDash();
        player.RollState.ResetCanRoll();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;
        grabInput = player.InputHandler.GrabInput;
        dashInput = player.InputHandler.DashInput;
        
        if (jumpInput && player.JumpState.CanJump() && !isTouchingCeiling)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if (Abilities.instance.IsSpellCastable())
        {
            stateMachine.ChangeState(player.SpellCastState);
        }
        else if (Abilities.instance.IsAbilityCastable())
        {
            stateMachine.ChangeState(player.AbilityCastState);
        }            
        else if (!isGrounded)
        {
            player.InAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.InAirState);
        }
        else if (isTouchingWall && grabInput && isTouchingLedge && Stats.stam.CurrentValue > 0.5f)
        {
            stateMachine.ChangeState(player.WallGrabState);
        }
        else if (dashInput && player.DashState.CheckIfCanDash() && !isTouchingCeiling && player.InputHandler.DashDirectionInput.y != 0)
        {
            stateMachine.ChangeState(player.DashState);
        }
        else if (dashInput && player.RollState.CheckIfCanRoll() && player.InputHandler.DashDirectionInput.y == 0)
        {
            stateMachine.ChangeState(player.RollState);
        }
        else if (player.InputHandler.AttackInputs[(int)CombatInputs.primary] && !isTouchingCeiling && Stats.stam.CurrentValue > 0.5f)
        {
            stateMachine.ChangeState(player.PrimaryAttackState);
        }
        else if (player.InputHandler.AttackInputs[(int)CombatInputs.secondary] && !isTouchingCeiling && Stats.stam.CurrentValue > 0.5f)
        {
            stateMachine.ChangeState(player.SecondaryAttackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}