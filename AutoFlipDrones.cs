using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Auto Flip Drones", "WhiteThunder", "1.0.0")]
    [Description("Automatically flips upside-down RC drones when a player takes control of them at a computer station.")]
    internal class AutoFlipDrones : CovalencePlugin
    {
        private const string PermissionUse = "autoflipdrones.use";

        private void Init()
        {
            permission.RegisterPermission(PermissionUse, this);
        }

        private void OnBookmarkControlStarted(ComputerStation computerStation, BasePlayer player, string bookmarkName, Drone drone)
        {
            var rootEntity = GetRootEntity(drone);
            var rootTransform = rootEntity.transform;

            if (Vector3.Dot(Vector3.up, rootTransform.up) <= 0f
                && !AutoFlipWasBlocked(drone, player))
            {
                if (drone != rootEntity)
                {
                    // Special handling for resized drones.
                    rootTransform.position -= rootTransform.InverseTransformPoint(drone.transform.position) * 2;
                }
                rootTransform.rotation = Quaternion.identity;
            }
        }

        private static bool AutoFlipWasBlocked(Drone drone, BasePlayer player)
        {
            object hookResult = Interface.CallHook("OnDroneAutoFlip", drone, player);
            return hookResult is bool && (bool)hookResult == false;
        }

        private static BaseEntity GetRootEntity(Drone drone)
        {
            // Recurse no more than this many parents just in case cycles are possible.
            var triesLeft = 5;

            BaseEntity validParent = drone;
            while (triesLeft-- > 0)
            {
                var parent = validParent.GetParentEntity();
                if (parent == null)
                    break;

                validParent = parent;
            }

            return validParent;
        }
    }
}
