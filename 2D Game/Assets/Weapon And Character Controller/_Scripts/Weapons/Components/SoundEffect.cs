using static Inventory.Item;

namespace Bardent.Weapons.Components
{
    public class SoundEffect : WeaponComponent<SoundEffectData, AttackSoundEffect>
    {
        private void HandleAttackSoundEffect()
        {
            if (weaponData.Type == WeaponType.Sword)
                Invoke("PlaySoundEffect", currentAttackData.Delay);
        }

        private void PlaySoundEffect() => AudioManager.Instance.SwordSoundEffect.Play();

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
