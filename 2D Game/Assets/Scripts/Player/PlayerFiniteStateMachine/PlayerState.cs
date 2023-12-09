using UnityEngine;

public class PlayerState : MonoBehaviour
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;

    protected bool isAnimationFinished;

    protected float startTime;

    private string animBoolName;

    public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        DoCheck();
        player.Animator.SetBool(animBoolName, true);
        startTime = Time.time;
        isAnimationFinished = false;
    }

    public virtual void Exit() => player.Animator.SetBool(animBoolName, false);

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() => DoCheck();

    public virtual void DoCheck() { }
    
    public virtual void AnimationTrigger() { }

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;
}
