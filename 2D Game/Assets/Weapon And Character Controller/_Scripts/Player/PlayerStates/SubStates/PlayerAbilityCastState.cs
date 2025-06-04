using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityCastState : PlayerAbilityState
{
    private float abilitycastDuration = 0.5f;
    private float abilityCastTimer;
    
    public PlayerAbilityCastState(
        Player player,
        PlayerStateMachine stateMachine,
        PlayerData playerData,
        string animBoolName
    ) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        RuntimeAnimatorController ac = player.Anim.runtimeAnimatorController;

        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == "AbilityCast")
            {
                abilitycastDuration = clip.length;
                break;
            }
        }
        
        abilityCastTimer = abilitycastDuration;

        if (Mathf.Sign(PlayerInputHandler.Instance.MouseRelativeToPlayerPosition) != Mathf.Sign(Movement.FacingDirection)) Movement.Flip();
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        Movement.SetVelocityX(Movement.CurrentVelocity.x * 0.9f);

        abilityCastTimer -= Time.deltaTime;
        if (abilityCastTimer <= 0f)
        {
            ExitHandler();
        }
    }

    private void ExitHandler()
    {
        AnimationFinishTrigger();
        isAbilityDone = true;
    }
}