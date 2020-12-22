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

            Allow_gun_damage = true;
            Allow_melee_damage = true;
            Allow_vehicle_damage = true;
        }

        public bool Enabled;

        public int Percentage_break_leg;

        public int Percentage_drop_gun;

        public float Slow_after_damage_time;

        public int Max_damage_per_vehicle_crash;

        public bool Allow_gun_damage;
        public bool Allow_melee_damage;
        public bool Allow_vehicle_damage;
    }
}
