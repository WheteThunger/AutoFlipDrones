using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Auto Flip Drones", "WhiteThunder", "1.1.0")]
    [Description("Automatically flips upside-down RC drones when hit with a hammer or taken control of at a computer station.")]
    internal class AutoFlipDrones : CovalencePlugin
    {
        private const string PermissionUse = "autoflipdrones.use";

        private void Init()
        {
            permission.RegisterPermission(PermissionUse, this);
        }

        private void OnBookmarkControlStarted(ComputerStation computerStation, BasePlayer player, string bookmarkName, Drone drone)
        {
            MaybeFlipDrone(player, drone);
        }

        private void OnHammerHit(BasePlayer player, HitInfo info)
        {
            var drone = info.HitEntity as Drone;
            if (drone == null)
                return;

            MaybeFlipDrone(player, drone);
        }

        private bool AutoFlipWasBlocked(Drone drone, BasePlayer player)
        {
            return Interface.CallHook("OnDroneAutoFlip", drone, player) is false;
        }

        private void MaybeFlipDrone(BasePlayer player, Drone drone)
        {
            if (!permission.UserHasPermission(player.UserIDString, PermissionUse))
                return;

            var droneTransform = drone.transform;

            if (Vector3.Dot(Vector3.up, droneTransform.up) > 0.1f)
                return;

            if (AutoFlipWasBlocked(drone, player))
                return;

            droneTransform.rotation = Quaternion.identity;
        }
    }
}
