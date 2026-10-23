using System.Text;
using BepInEx.Configuration;
using UnityEngine;

namespace GrapplingHookTweaks.Config
{
    /// <summary>
    /// BepInEx backed settings of the mod. Values are read on every access, so
    /// changes applied in game (ConfigurationManager) or in the .cfg file take
    /// effect immediately, without restarting the game.
    /// </summary>
    public sealed class PluginConfig
    {
        private const string GeneralSection = "General";
        private const string PullSection = "Pull";

        private readonly ConfigEntry<bool> _enabled;
        private readonly ConfigEntry<bool> _debugLogging;
        private readonly ConfigEntry<KeyboardShortcut> _pullShortcut;
        private readonly ConfigEntry<bool> _allowJumpToPull;
        private readonly ConfigEntry<bool> _autoPullOnSpecialLaunch;
        private readonly ConfigEntry<bool> _groundPull;

        public PluginConfig(ConfigFile file)
        {
            _enabled = Bind(
                file,
                GeneralSection,
                nameof(Enabled),
                true,
                "Enable the grappling hook tweaks (key combination, automatic pull, ground pull). When off, the game behaves exactly like vanilla."
            );

            _debugLogging = Bind(
                file,
                GeneralSection,
                nameof(DebugLogging),
                false,
                "Log every reel-in (shortcut, automatic pull, swallowed attack). Verbose, intended for troubleshooting only."
            );

            _pullShortcut = Bind(
                file,
                PullSection,
                nameof(PullShortcut),
                new KeyboardShortcut(KeyCode.LeftAlt),
                new ConfigDescription(
                    "Key combination that reels the player toward the point the hook is attached to, "
                        + "replacing the double jump. Works on the ground and in the air. "
                        + "Default: Left Alt (pressed alone). Set it to None to disable it."
                )
            );

            _allowJumpToPull = Bind(
                file,
                PullSection,
                nameof(AllowJumpToPull),
                false,
                "Keep the vanilla trigger as well: pressing jump while hanging also reels the player in. "
                    + "When off (default), only the pull shortcut and the automatic pull can reel the player in."
            );

            _autoPullOnSpecialLaunch = Bind(
                file,
                PullSection,
                nameof(AutoPullOnSpecialLaunch),
                true,
                "Reel in automatically when the hook is fired with the special attack, "
                    + "exactly like the attack key already does. When off, the special attack "
                    + "leaves the player hanging until the pull shortcut is used."
            );

            _groundPull = Bind(
                file,
                PullSection,
                nameof(GroundPull),
                true,
                "Reel in even while standing on the ground. The game only reels you in "
                    + "while you are off the ground: it brakes every movement that does not "
                    + "come from your own keys as long as your feet touch the ground, so a "
                    + "hook fired on flat ground in front of you stays still. When on, the "
                    + "reel-in lifts you off your feet first, the way the game already does "
                    + "when the hook lands above you."
            );
        }

        public bool Enabled => _enabled.Value;

        public bool DebugLogging => _debugLogging.Value;

        public KeyboardShortcut PullShortcut => _pullShortcut.Value;

        public bool AllowJumpToPull => _allowJumpToPull.Value;

        public bool AutoPullOnSpecialLaunch => _autoPullOnSpecialLaunch.Value;

        public bool GroundPull => _groundPull.Value;

        /// <summary>
        /// Binds a setting under its readable key: the identifier given with nameof
        /// is turned into spaces and a single leading capital, so "PullShortcut"
        /// is stored and displayed as "Pull shortcut".
        /// </summary>
        private static ConfigEntry<T> Bind<T>(
            ConfigFile file,
            string section,
            string identifier,
            T defaultValue,
            string description
        )
        {
            return Bind(
                file,
                section,
                identifier,
                defaultValue,
                new ConfigDescription(description)
            );
        }

        private static ConfigEntry<T> Bind<T>(
            ConfigFile file,
            string section,
            string identifier,
            T defaultValue,
            ConfigDescription description
        )
        {
            return file.Bind(section, ReadableKey(identifier), defaultValue, description);
        }

        /// <summary>"PullShortcut" becomes "Pull shortcut".</summary>
        private static string ReadableKey(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return identifier;
            }

            StringBuilder key = new StringBuilder(identifier.Length + 4);
            key.Append(char.ToUpperInvariant(identifier[0]));
            for (int i = 1; i < identifier.Length; i++)
            {
                char character = identifier[i];
                if (char.IsUpper(character))
                {
                    key.Append(' ');
                }

                key.Append(char.ToLowerInvariant(character));
            }

            return key.ToString();
        }
    }
}
