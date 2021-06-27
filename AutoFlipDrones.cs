using Oxide.Core;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Auto Flip Drones", "WhiteThunder", "1.0.1")]
    [Description("Automatically flips upside-down RC drones when a player takes control of them at a computer station.")]
    internal class AutoFlipDrones : CovalencePlugin
    {
        [PluginReference]
        private Plugin DroneScaleManager;

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

        private bool AutoFlipWasBlocked(Drone drone, BasePlayer player)
        {
            object hookResult = Interface.CallHook("OnDroneAutoFlip", drone, player);
            return hookResult is bool && (bool)hookResult == false;
        }

        private BaseEntity GetRootEntity(Drone drone)
        {
            return drone.HasParent()
                ? DroneScaleManager?.Call("API_GetRootEntity", drone) as BaseEntity ?? drone
                : drone;
        }
    }
}
