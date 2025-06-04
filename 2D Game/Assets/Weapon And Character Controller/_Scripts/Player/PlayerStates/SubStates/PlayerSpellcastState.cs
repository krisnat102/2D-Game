using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellCastState : PlayerAbilityState
{
    private float spellcastDuration = 0.5f;
    private float spellcastTimer;
    
    public PlayerSpellCastState(
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
            if (clip.name == "SpellCast")
            {
                spellcastDuration = clip.length;
                break;
            }
        }
        
        spellcastTimer = spellcastDuration;
        
        if (Mathf.Sign(PlayerInputHandler.Instance.MouseRelativeToPlayerPosition) != Mathf.Sign(Movement.FacingDirection)) Movement.Flip();
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        Movement.SetVelocityX(Movement.CurrentVelocity.x * 0.9f);

        spellcastTimer -= Time.deltaTime;
        if (spellcastTimer <= 0f)
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