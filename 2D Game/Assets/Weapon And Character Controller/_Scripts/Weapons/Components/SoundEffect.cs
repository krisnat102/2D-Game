using static Inventory.Item;

namespace Bardent.Weapons.Components
{
    public class SoundEffect : WeaponComponent<SoundEffectData, AttackSoundEffect>
    {
        private void HandleAttackSoundEffect()
        {
            if (weaponData.Type == WeaponType.Sword && currentAttackData.Delay != 0)
                Invoke("PlaySoundEffect", currentAttackData.Delay);
            else if (weaponData.Type == WeaponType.Sword)
                PlaySoundEffect();
        }

        private void PlaySoundEffect() => AudioManager.Instance.PlaySwordSound(0.9f, 1.1f);

        protected override void Start()
        {
            base.Start();

            eventHandler.OnAttackAction += HandleAttackSoundEffect;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            eventHandler.OnAttackAction -= HandleAttackSoundEffect;
        }
    }
}
