using GrapplingHookTweaks.Config;
using GrapplingHookTweaks.Grapple;
using HarmonyLib;

namespace GrapplingHookTweaks.Patches
{
    /// <summary>
    /// A hook fired with the attack key attaches and pulls the player right away,
    /// while one fired with the special attack only attaches: the player hangs until
    /// something reels them in (the jump press of the vanilla game). The two hooks
    /// are told apart by <see cref="GrapplingPoint.m_secondary"/>, which the prefab
    /// of the special attack carries: it switches the point into the "repell" state
    /// that never reels anyone in, and <see cref="GrapplingPoint.Pull"/> is what
    /// leaves that state again.
    ///
    /// When the automatic pull is on, the attachment of a special attack hook is
    /// turned into the case of the attack key, which is what the attack key does
    /// by itself. The recorded launch of <see cref="SpecialLaunchTracker"/> is kept
    /// as a second, independent signal, so that a hook whose prefab does not carry
    /// the flag still reels in.
    /// </summary>
    [HarmonyPatch]
    internal static class GrapplingPointActivatePatch
    {
        [HarmonyPatch(typeof(GrapplingPoint), nameof(GrapplingPoint.Activate))]
        private static void Postfix(GrapplingPoint __instance, Character character)
        {
            PluginConfig settings = GrapplingHookTweaks.Instance?.Settings;
            if (settings == null || !settings.Enabled)
            {
                return;
            }

            bool byLocalPlayer = character == Player.m_localPlayer;

            // Only the client that fired the hook owns its animation, which Pull needs.
            ZNetView nview = __instance.GetComponent<ZNetView>();
            bool ownedByLocalPlayer = nview != null && nview.IsOwner();

            bool specialProjectile = __instance.m_secondary;
            bool recordedLaunch = SpecialLaunchTracker.LaunchedWithSpecial;

            if (settings.DebugLogging)
            {
                GrapplingHookTweaks.Instance.LogInfo(
                    $"Hook attached: local player {byLocalPlayer}, owned {ownedByLocalPlayer}, "
                        + $"special attack hook {specialProjectile}, recorded launch {recordedLaunch}, "
                        + $"auto pull {settings.AutoPullOnSpecialLaunch}."
                );
            }

            if (!byLocalPlayer)
            {
                return;
            }

            SpecialLaunchTracker.Clear();

            if (!settings.AutoPullOnSpecialLaunch || !ownedByLocalPlayer)
            {
                return;
            }

            if (!specialProjectile && !recordedLaunch)
            {
                return;
            }

            GrapplePuller.ReelIn(Player.m_localPlayer, __instance, chargeStamina: false);

            if (settings.DebugLogging)
            {
                GrapplingHookTweaks.Instance.LogInfo(
                    "Hook fired with the special attack: pulled in automatically."
                );
            }
        }
    }
}
