using GrapplingHookTweaks.Config;
using GrapplingHookTweaks.Grapple;
using HarmonyLib;

namespace GrapplingHookTweaks.Patches
{
    /// <summary>
    /// The walking code of the game steers the body toward the keys and brakes every
    /// other movement while the feet touch the ground, which cancels the reel-in of a
    /// hook fired on flat ground: that is why the vanilla game only reels a player in
    /// once they are off the ground.
    ///
    /// The lift of <see cref="GrapplePuller.KeepReelingOnGround"/> therefore runs here,
    /// as a prefix: the kick flags the player as off the ground before the walking code
    /// looks, so the very frame of the kick is not braked at all, and a player still
    /// standing keeps the speed of the pull up until they leave the ground.
    /// </summary>
    [HarmonyPatch]
    internal static class CharacterUpdateWalkingPatch
    {
        [HarmonyPatch(typeof(Character), nameof(Character.UpdateWalking))]
        private static void Prefix(Character __instance)
        {
            if (__instance != Player.m_localPlayer)
            {
                return;
            }

            PluginConfig settings = GrapplingHookTweaks.Instance?.Settings;
            if (settings == null)
            {
                return;
            }

            GrapplePuller.KeepReelingOnGround((Player)__instance, settings);
        }
    }
}
