using SDG.Unturned;
using UnityEngine;
using Rocket.Unturned.Player;
using Rocket.API;

namespace AdvancedDamageSystem
{
    public class RaycastHelper
    {
        public static Transform Raycast(IRocketPlayer rocketPlayer, float distance)
        {
            UnturnedPlayer player = (UnturnedPlayer)rocketPlayer;
            if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit hit, distance, RayMasks.BARRICADE_INTERACT | RayMasks.BARRICADE))
            {
                Transform transform = hit.transform;

                return transform;
            }
            return null;
        }
    }
}
