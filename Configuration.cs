using Rocket.API;

namespace AdvancedDamageSystem
{
    public class Configuration : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            Enabled = true;

            Percentage_break_leg = 40;
            Percentage_drop_gun = 50;
            Slow_after_damage_time = 5;
            Max_damage_per_vehicle_crash = 35;
        }

        public bool Enabled;

        public int Percentage_break_leg;

        public int Percentage_drop_gun;

        public float Slow_after_damage_time;

        public int Max_damage_per_vehicle_crash;
    }
}
