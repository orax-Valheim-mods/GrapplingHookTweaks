using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using GrapplingHookTweaks.Config;
using GrapplingHookTweaks.Grapple;
using HarmonyLib;

namespace GrapplingHookTweaks
{
    /// <summary>
    /// Composition root of the mod: binds the settings, installs the Harmony patches
    /// and watches the reel-in shortcut. The shortcut has to be polled here because
    /// it is a key combination of the player's choosing (mouse buttons included) and
    /// therefore is not covered by any of the game's own input handling.
    /// </summary>
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    internal sealed class GrapplingHookTweaks : BaseUnityPlugin
    {
        public const string PluginGUID = "orax.GrapplingHookTweaks";
        public const string PluginName = "GrapplingHookTweaks";
        public const string PluginVersion = "0.1.0";

        internal static GrapplingHookTweaks Instance { get; private set; }

        /// <summary>
        /// Settings. Values are read on every access, so a change applied in game
        /// (ConfigurationManager) or in the .cfg file takes effect immediately.
        /// </summary>
        internal PluginConfig Settings { get; private set; }

        private Harmony _harmony;

        private bool _loggedPolling;

        private void Awake()
        {
            Instance = this;
            Settings = new PluginConfig(Config);

            _harmony = new Harmony(PluginGUID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo(
                $"{PluginName} {PluginVersion} loaded: shortcut {Settings.PullShortcut}, "
                    + $"jump trigger {(Settings.AllowJumpToPull ? "kept" : "replaced")}, "
                    + $"auto pull {(Settings.AutoPullOnSpecialLaunch ? "on" : "off")}, "
                    + $"ground pull {(Settings.GroundPull ? "on" : "off")}"
            );
        }

        /// <summary>Log from the patches, which cannot reach the protected logger.</summary>
        internal void LogInfo(string message)
        {
            Logger.LogInfo(message);
        }

        private void Update()
        {
            Player player = Player.m_localPlayer;
            if (player == null || !Settings.Enabled)
            {
                return;
            }

            if (!_loggedPolling)
            {
                _loggedPolling = true;
                Logger.LogInfo(
                    $"Shortcut polling started: {Settings.PullShortcut} "
                        + "(Debug logging in the config shows what a press does)."
                );
            }

            KeyboardShortcut shortcut = Settings.PullShortcut;
            if (!PullShortcutInput.IsMainKeyDown(shortcut))
            {
                return;
            }

            // One pull per press, even when the combination is held down.
            bool modifiersHeld = PullShortcutInput.AreModifiersHeld(shortcut);
            string blocked = BlockedReason();
            bool hookAttached = GrapplingPoint.m_localGrappler != null;

            if (Settings.DebugLogging)
            {
                Logger.LogInfo(
                    $"Shortcut main key down: modifiers held {modifiersHeld}, "
                        + $"hook attached {hookAttached}, blocked {blocked ?? "nothing"}."
                );
            }

            if (!modifiersHeld || blocked != null)
            {
                return;
            }

            if (GrapplePuller.TryPull(player, out string reason))
            {
                if (Settings.DebugLogging)
                {
                    Logger.LogInfo("Reeled in by the pull shortcut.");
                }
            }
            else if (Settings.DebugLogging)
            {
                Logger.LogInfo($"Pull shortcut pressed: {reason}.");
            }
        }

        /// <summary>
        /// The shortcut must stay inert while the player is typing or busy in a menu:
        /// a combination such as Alt + left click would otherwise be swallowed by a
        /// popup at the same time as it pulls the player. Returns what keeps the
        /// shortcut from firing, or null while it may fire.
        /// </summary>
        private static string BlockedReason()
        {
            if (Game.IsPaused())
            {
                return "the game is paused";
            }

            if (Menu.IsVisible())
            {
                return "the menu is open";
            }

            if (InventoryGui.IsVisible())
            {
                return "the inventory is open";
            }

            if (Console.IsVisible())
            {
                return "the console is open";
            }

            if (TextInput.IsVisible())
            {
                return "a text input is open";
            }

            if (Chat.instance != null && Chat.instance.HasFocus())
            {
                return "the chat has focus";
            }

            return null;
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
