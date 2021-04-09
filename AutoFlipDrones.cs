using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Auto Flip Drones", "WhiteThunder", "1.0.0")]
    [Description("Automatically flips upside-down drones when a player takes control.")]
    internal class AutoFlipDrones : CovalencePlugin
    {
        private const string PermissionUse = "autoflipdrones.use";

        private void Init()
        {
            permission.RegisterPermission(PermissionUse, this);
        }

        private void OnBookmarkControlStarted(ComputerStation computerStation, BasePlayer player, string bookmarkName, Drone drone)
        {
            if (Vector3.Dot(Vector3.up, drone.transform.up) <= 0f
                && !AutoFlipWasBlocked(drone, player))
            {
                drone.transform.rotation = Quaternion.identity;
            }
        }

        private static bool AutoFlipWasBlocked(Drone drone, BasePlayer player)
        {
            object hookResult = Interface.CallHook("OnDroneAutoFlip", drone, player);
            return hookResult is bool && (bool)hookResult == false;
        }
    }
}
