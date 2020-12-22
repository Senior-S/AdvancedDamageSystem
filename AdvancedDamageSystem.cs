using Rocket.Core.Plugins;
using SDG.Unturned;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Player;
using System.Collections;
using UnityEngine;
using Random = System.Random;
using Steamworks;

namespace AdvancedDamageSystem
{
    public class AdvancedDamageSystem : RocketPlugin<Configuration>
    {
        protected override void Load()
        {
            Logger.Log(" Plugin loaded correctly!");
            Logger.Log(" More plugins: www.dvtserver.xyz");
            if (!Configuration.Instance.Enabled)
            {
                Logger.Log(" Plugin disabled! Please enable it in the config.");
                this.Unload();
                return;
            }

            Instance = this;

            DamageTool.damagePlayerRequested += DamagePlayerRequested;
            VehicleManager.onDamageVehicleRequested += OnDamageVehicle;
        }

        private void OnDamageVehicle(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedPlayer user = UnturnedPlayer.FromCSteamID(instigatorSteamID);
            if (user == null || pendingTotalDamage < 5) return;
            if (damageOrigin == EDamageOrigin.Vehicle_Collision_Self_Damage)
            {
                if (!Configuration.Instance.Allow_vehicle_damage) return;
                int damage = random.Next(1, Configuration.Instance.Max_damage_per_vehicle_crash);
                if (vehicle.checkDriver(instigatorSteamID))
                {
                    if (pendingTotalDamage > 5 && pendingTotalDamage < 35)
                    {
                        SlowPlayer(user.Player, 0.7f, 8f);
                        user.Player.life.serverSetLegsBroken(true);

                        user.Player.life.askDamage(byte.Parse(damage.ToString()), user.Player.transform.position, EDeathCause.VEHICLE, ELimb.SPINE, CSteamID.Nil, out EPlayerKill s);
                    }
                    if (pendingTotalDamage > 35)
                    {
                        SlowPlayer(user.Player, 0.7f, 15f);
                        user.Player.life.serverSetLegsBroken(true);
                        user.Player.life.askDamage(byte.Parse(damage.ToString()), user.Player.transform.position, EDeathCause.VEHICLE, ELimb.SPINE, CSteamID.Nil, out EPlayerKill s);
                    }
                }
                else if (user.Player.movement.getVehicle() == vehicle)
                {
                    if (pendingTotalDamage > 10 && pendingTotalDamage < 45)
                    {
                        SlowPlayer(user.Player, 0.8f, 5f);
                        user.Player.life.serverSetLegsBroken(true);

                        user.Player.life.askDamage(byte.Parse(damage.ToString()), user.Player.transform.position, EDeathCause.VEHICLE, ELimb.SPINE, CSteamID.Nil, out EPlayerKill s);
                    }
                    if (pendingTotalDamage > 45)
                    {
                        SlowPlayer(user.Player, 0.8f, 12f);
                        user.Player.life.serverSetLegsBroken(true);
                        user.Player.life.askDamage(byte.Parse(damage.ToString()), user.Player.transform.position, EDeathCause.VEHICLE, ELimb.SPINE, CSteamID.Nil, out EPlayerKill s);
                    }
                }
            }
        }

        private void DamagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            UnturnedPlayer user = UnturnedPlayer.FromPlayer(parameters.player);
            if (parameters.cause == EDeathCause.MELEE)
            {
                if (!Configuration.Instance.Allow_melee_damage) shouldAllow = false;
            }
            else if (parameters.cause == EDeathCause.GUN)
            {
                if (!Configuration.Instance.Allow_gun_damage)
                {
                    shouldAllow = false;
                    return;
                }
            }
            else if (parameters.cause == EDeathCause.VEHICLE)
            {
                if (!Configuration.Instance.Allow_vehicle_damage) return;
                if (parameters.damage >= 20)
                {
                    user.Player.life.serverSetLegsBroken(true);
                }
                else if (parameters.damage >= 50 && parameters.damage < user.Health)
                {
                    user.Player.life.serverSetLegsBroken(true);
                    user.Player.stance.stance = SDG.Unturned.EPlayerStance.PRONE;
                }
                else if (parameters.damage >= user.Health)
                {
                    user.Heal(100);
                    parameters.damage = 85;
                    user.Player.life.serverSetLegsBroken(true);
                    user.Player.stance.stance = SDG.Unturned.EPlayerStance.PRONE;
                }
            }
            if (parameters.damage >= user.Health) return;
            int per = random.Next(1, 101);
            switch (parameters.limb)
            {
                case ELimb.LEFT_LEG:
                    if (per > Configuration.Instance.Percentage_break_leg)
                    {
                        user.Player.life.serverSetLegsBroken(true);
                    }
                    break;
                case ELimb.RIGHT_LEG:
                    if (per > Configuration.Instance.Percentage_break_leg)
                    {
                        user.Player.life.serverSetLegsBroken(true);
                    }
                    break;
                case ELimb.LEFT_HAND:
                    if (per > Configuration.Instance.Percentage_drop_gun)
                    {
                        user.Player.equipment.dequip();
                        //user.Player.inventory.sendDropItem(user.Player.equipment.equippedPage, user.Player.equipment.equipped_x, user.Player.equipment.equipped_y);
                    }
                    break;
                case ELimb.RIGHT_HAND:
                    if (per > Configuration.Instance.Percentage_drop_gun)
                    {
                        user.Player.equipment.dequip();
                        //user.Player.inventory.sendDropItem(user.Player.equipment.equippedPage, user.Player.equipment.equipped_x, user.Player.equipment.equipped_y);
                    }
                    break;
            }
            if (parameters.damage > 10 && parameters.damage < 25)
            {
                StartCoroutine(SlowPlayer(user.Player, 0.8f, Configuration.Instance.Slow_after_damage_time));
            }
            else if (parameters.damage > 25 && parameters.damage < 50)
            {
                StartCoroutine(SlowPlayer(user.Player, 0.6f, Configuration.Instance.Slow_after_damage_time));
            }
            else if (parameters.damage > 50)
            {
                StartCoroutine(SlowPlayer(user.Player, 0.4f, Configuration.Instance.Slow_after_damage_time));
            }
        }

        private IEnumerator SlowPlayer(Player player, float SlowAmount, float time)
        {
            player.movement.sendPluginSpeedMultiplier(SlowAmount);
            yield return new WaitForSeconds(time);
            player.movement.sendPluginSpeedMultiplier(1);
            yield break;
        }

        protected override void Unload()
        {
            Logger.Log(" Plugin unloaded correctly!");
        }

        internal static AdvancedDamageSystem Instance;

        internal Random random = new Random();
    }
}