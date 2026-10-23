using UnityEngine;

namespace GrapplingHookTweaks.Grapple
{
    /// <summary>
    /// Remembers how the local player last started an attack, so a grappling point
    /// that attaches a moment later knows whether it was fired with the special
    /// attack. The hook needs a moment to fly to its target, hence the time window:
    /// a launch is only remembered while it can still be the one being attached.
    /// </summary>
    internal static class SpecialLaunchTracker
    {
        /// <summary>Longer than any hook flight (the hook range is about 40 metres).</summary>
        private const float WindowSeconds = 5f;

        private static float _launchedAt = -1f;

        public static bool LaunchedWithSpecial =>
            _launchedAt >= 0f && Time.time - _launchedAt <= WindowSeconds;

        public static void Record(bool secondaryAttack)
        {
            _launchedAt = secondaryAttack ? Time.time : -1f;
        }

        public static void Clear()
        {
            _launchedAt = -1f;
        }
    }
}
