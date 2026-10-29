Replaces the double jump needed to reel yourself in with the **Grappling Hook**:
a custom key combination, an automatic pull when fired with the special attack,
and a pull that works even while standing on the ground.

# What it changes

Vanilla behavior:

- Fire the hook with the **attack key** (left-click by default): it latches and
  you are pulled toward the point right away.
- Fire it with the **special attack**: it latches but leaves you hanging,
  meaning you have to press **jump twice** (once to leave the ground, once in
  the air) to be pulled toward the point.

With this mod:

- A **key combination** (configurable, **Left Alt** by default) reels you in
  both on the ground and in the air—no more double jump needed.
- The vanilla jump trigger is **replaced** by default, so pressing jump while
  hanging no longer pulls you in by accident (set `Allow jump to pull` to `true`
  to keep the vanilla behavior).
- **Automatic pull**: when the hook is fired with the special attack, you are
  pulled in immediately, just like with the attack key (set
  `Auto pull on special launch` to `false` for the vanilla behavior).
- **Ground pull**: the game normally only reels you in once you are off the
  ground, but this mod allows the reel-in to lift you straight off flat ground
  (set `Ground pull` to `false` for the vanilla behavior).

# Configuration

Settings can be changed in-game (using ConfigurationManager) or directly in
`BepInEx\config\orax.GrapplingHookTweaks.cfg`.

# Installation

This mod is client-side only. You do not need to install it on the server.

## Using a mod manager (recommended)

You can install the mod using Vortex or Gale.

## Manual

Install BepInExPack for Valheim, then copy the DLL file to
`Steam\steamapps\common\Valheim\BepInEx\plugins\`.

# Misc.

GitHub repositories of my Valheim mods: https://github.com/orax-Valheim-mods/
