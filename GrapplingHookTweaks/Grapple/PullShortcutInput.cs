using BepInEx.Configuration;
using UnityEngine;

namespace GrapplingHookTweaks.Grapple
{
    /// <summary>
    /// Reads the configured combination. BepInEx's own <c>KeyboardShortcut</c> checks
    /// additionally that no other key of the keyboard is held, which rejects the
    /// shortcut as soon as the player keeps a movement or sprint key pressed: a
    /// gameplay shortcut has to work while running, so only the keys of the
    /// combination itself are checked here.
    /// </summary>
    internal static class PullShortcutInput
    {
        /// <summary>True on the frame the main key of the combination goes down.</summary>
        public static bool IsPressed(KeyboardShortcut shortcut)
        {
            return IsMainKeyDown(shortcut) && AreModifiersHeld(shortcut);
        }

        /// <summary>True while the whole combination is held down.</summary>
        public static bool IsHeld(KeyboardShortcut shortcut)
        {
            return IsMainKeyHeld(shortcut) && AreModifiersHeld(shortcut);
        }

        /// <summary>True on the frame the main key of the combination goes down.</summary>
        public static bool IsMainKeyDown(KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKeyDown(shortcut.MainKey);
        }

        /// <summary>True while the whole combination, main key included, is held.</summary>
        public static bool IsMainKeyHeld(KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKey(shortcut.MainKey);
        }

        /// <summary>True while every key but the main one of the combination is down.</summary>
        public static bool AreModifiersHeld(KeyboardShortcut shortcut)
        {
            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
