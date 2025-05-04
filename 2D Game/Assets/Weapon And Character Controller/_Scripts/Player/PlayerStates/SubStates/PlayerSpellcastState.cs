using Bardent.Weapons;

public class PlayerSpellcastState : PlayerAbilityState
{
    public PlayerSpellcastState(
        Player player,
        PlayerStateMachine stateMachine,
        PlayerData playerData,
        string animBoolName
    ) : base(player, stateMachine, playerData, animBoolName)
    {
    }
}