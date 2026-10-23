# GrapplingHookTweaks

Replaces the double jump needed to reel yourself in with the **Grappling Hook**: a key combination
of your choice, an automatic pull when the hook is fired with the special attack, and a pull even
while you are standing on the ground.

## What it changes

Vanilla behaviour:

* fire the hook with the **attack key** (left click by default): it latches and you are pulled
  toward the point right away;
* fire it with the **special attack**: it latches but leaves you hanging, so you have to press
  **jump twice** — once to jump from the ground you are still standing on, once more in the air —
  to be pulled toward the point.

With this mod:

* a **key combination** (configurable, **Left Alt** by default) reels you in, on the ground as
  well as in the air: no more double jump;
* the vanilla jump trigger is **replaced** by default, so a jump press while hanging no longer
  pulls you in by accident (set `Allow jump to pull` to `true` to keep both);
* **automatic pull**: when the hook is fired with the special attack, you are pulled in
  immediately, exactly like with the attack key (set `Auto pull on special launch` to `false` for
  the vanilla behaviour);
* **ground pull**: the game only reels you in once you are off the ground, so a hook fired on flat
  ground in front of you stays still — the reel-in now lifts you off your feet first (set
  `Ground pull` to `false` for the vanilla behaviour).

While the combination is pressed, the attack is swallowed, so it never fires a second hook that
would break the line you are hanging from. Keyboard and mouse only: the combination is not
available on a gamepad.

# Configuration

Settings can be changed in game (ConfigurationManager) or in
`BepInEx\config\orax.GrapplingHookTweaks.cfg`.

| Setting | Default | Description |
| --- | --- | --- |
| Enabled | `true` | Master switch: off means vanilla behaviour. |
| Debug logging | `false` | Log every reel-in, for troubleshooting. |
| Pull shortcut | `LeftAlt` | Key combination that reels you in: any keyboard key or mouse button, alone or with modifiers. Default is Left Alt alone; set it to `None` to disable it entirely. |
| Allow jump to pull | `false` | Keep the vanilla jump trigger as well as the shortcut. |
| Auto pull on special launch | `true` | Reel in automatically when the hook was fired with the special attack. |
| Ground pull | `true` | Reel in while standing on the ground: the reel-in lifts you off your feet first, the way the game does when the hook lands above you. |

# Troubleshooting

Turn `Debug logging` on and read `BepInEx\LogOutput.log`: every press of the shortcut, every
attached hook and every swallowed attack is reported there, together with the reason when nothing
happened — which key went down, whether the modifiers were held, whether a hook is attached and
what keeps the shortcut from firing.

# Installation

This mod is client-side only. You do not need to install it on the server.

## Using a mod manager (recommended)
You can install the mod using Vortex, R2ModMan or Gale.

## Manual
Install BepInExPack for Valheim.
Copy the DLL file to `Steam\steamapps\common\Valheim\BepInEx\plugins\`

# Misc.

Source code: [GitHub repository](https://github.com/orax-Valheim-mods/)
