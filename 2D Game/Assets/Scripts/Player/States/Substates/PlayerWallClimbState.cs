using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallClimbState : PlayerTouchingWallState
{
    private float timeTracker;
    private Vector2 holdPosition;

    public PlayerWallClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        holdPosition = player.transform.position;

        timeTracker = 0;

        Debug.Log("wallClimb");
    }

    public override void Exit()
    {
        base.Exit();

        timeTracker = 0;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            timeTracker = Time.deltaTime;

            holdPosition.y = holdPosition.y + timeTracker * playerData.wallClimbVelocity;

            player.transform.position = holdPosition;
            player.SetVelocityY(0f);

            if (yInput != 1)
            {
                stateMachine.ChangeState(player.WallGrabState);
            }
        }
    }
}
