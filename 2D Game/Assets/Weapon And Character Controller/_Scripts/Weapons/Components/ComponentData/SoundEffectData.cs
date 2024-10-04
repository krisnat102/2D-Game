namespace Bardent.Weapons.Components
{
    public class SoundEffectData : ComponentData<AttackSoundEffect>
    {
        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(SoundEffect);
        }
    }
}
