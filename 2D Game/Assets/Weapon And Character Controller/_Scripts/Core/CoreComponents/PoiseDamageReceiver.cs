using Bardent.Interfaces;

namespace Bardent.CoreSystem
{
    public class PoiseDamageReceiver : CoreComponent, IPoiseDamageable
    {
        private Stats stats;


        public void DamagePoise(float amount)
        {
            stats.poise.Decrease(amount);
        }

        protected override void Awake()
        {
            base.Awake();

            stats = Core.GetCoreComponent<Stats>();
        }
    }
}