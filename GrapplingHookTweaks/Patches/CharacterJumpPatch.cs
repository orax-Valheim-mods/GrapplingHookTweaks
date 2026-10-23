using GrapplingHookTweaks.Config;
using HarmonyLib;

namespace GrapplingHookTweaks.Patches
{
    /// <summary>
    /// The reel-in of the vanilla game lives in the jump branch of
    /// <see cref="Character.Jump"/>: while hanging, a jump press further than
    /// <see cref="GrapplingPoint.m_jumpPullMaxDist"/> from the point costs stamina
    /// and calls <see cref="GrapplingPoint.Pull"/>. On the ground the same press is
    /// a normal jump instead, which is why the player has to press jump twice when
    /// the hook was fired from the ground.
    ///
    /// Unless <c>AllowJumpToPull</c> is on, the grappling point is hidden for the
    /// duration of the call: the vanilla jump branch then never sees a grapple and
    /// cannot pull, while a jump pressed on the ground keeps working normally. The
    /// point is handed from the prefix to the postfix with <c>__state</c>, so nested
    /// calls restore their own value.
    /// </summary>
    [HarmonyPatch]
    internal static class CharacterJumpPatch
    {
        [HarmonyPatch(typeof(Character), nameof(Character.Jump))]
        private static void Prefix(Character __instance, out GrapplingPoint __state)
        {
            __state = null;

            PluginConfig settings = GrapplingHookTweaks.Instance?.Settings;
            if (settings == null || !settings.Enabled || settings.AllowJumpToPull)
            {
                return;
            }

            if (__instance != Player.m_localPlayer)
            {
                return;
            }

            __state = GrapplingPoint.m_localGrappler;
            if (__state != null)
            {
                GrapplingPoint.m_localGrappler = null;
            }
        }

        [HarmonyPatch(typeof(Character), nameof(Character.Jump))]
        private static void Postfix(GrapplingPoint __state)
        {
            if (__state != null)
            {
                GrapplingPoint.m_localGrappler = __state;
            }
        }
    }
}
