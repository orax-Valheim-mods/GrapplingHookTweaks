using GrapplingHookTweaks.Config;
using GrapplingHookTweaks.Grapple;
using HarmonyLib;

namespace GrapplingHookTweaks.Patches
{
    /// <summary>
    /// Every attack of the game, special ones included, starts here, so this is where
    /// the mod records which key fired the next hook. Two things happen when the pull
    /// shortcut is pressed while a hook is attached: the launch is recorded (or
    /// forgotten) and the attack is swallowed, because firing another hook would
    /// break the line the player is hanging from instead of reeling them in.
    /// </summary>
    [HarmonyPatch]
    internal static class HumanoidStartAttackPatch
    {
        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.StartAttack))]
        private static bool Prefix(Humanoid __instance, bool secondaryAttack)
        {
            if (__instance != Player.m_localPlayer)
            {
                return true;
            }

            PluginConfig settings = GrapplingHookTweaks.Instance?.Settings;
            if (settings == null)
            {
                return true;
            }

            if (
                settings.Enabled
                && PullShortcutInput.IsHeld(settings.PullShortcut)
                && GrapplingPoint.m_localGrappler != null
            )
            {
                if (settings.DebugLogging)
                {
                    GrapplingHookTweaks.Instance.LogInfo(
                        "Attack swallowed: the pull shortcut was pressed while hanging."
                    );
                }

                SpecialLaunchTracker.Clear();
                return false;
            }

            bool recordedBefore = SpecialLaunchTracker.LaunchedWithSpecial;
            SpecialLaunchTracker.Record(secondaryAttack);

            if (settings.DebugLogging && recordedBefore != SpecialLaunchTracker.LaunchedWithSpecial)
            {
                GrapplingHookTweaks.Instance.LogInfo(
                    SpecialLaunchTracker.LaunchedWithSpecial
                        ? "Special attack started: the next hook reels in automatically."
                        : "Special attack forgotten: a normal attack was started."
                );
            }

            return true;
        }
    }
}
