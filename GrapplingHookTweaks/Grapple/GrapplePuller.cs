using GrapplingHookTweaks.Config;
using UnityEngine;

namespace GrapplingHookTweaks.Grapple
{
    /// <summary>
    /// Reels the local player toward the point the grappling hook is attached to.
    /// The stamina cost mirrors the one of the game: vanilla charges it in the jump
    /// branch of <see cref="Character.Jump"/>, which the pull shortcut replaces, so
    /// the shortcut charges it too. The automatic pull replaces the attack key
    /// trigger instead, and that trigger never charged stamina.
    /// </summary>
    internal static class GrapplePuller
    {
        /// <summary>
        /// Upward speed given to a player the mod lifts off the ground. The walking
        /// code of the game caps the vertical speed at 3 while the feet touch the
        /// ground, so anything at or above that still leaves the ground.
        /// </summary>
        private const float LiftOffSpeed = 4f;

        /// <summary>The point the player was last lifted off the ground for.</summary>
        private static GrapplingPoint _liftedFrom;

        /// <summary>
        /// Reels in on demand of the shortcut. Returns false and describes in
        /// <paramref name="reason"/> why nothing happened, which is what the
        /// troubleshooting log reports.
        /// </summary>
        public static bool TryPull(Player player, out string reason)
        {
            GrapplingPoint point = GrapplingPoint.m_localGrappler;
            if (point == null)
            {
                reason = "no hook is attached";
                return false;
            }

            if (player == null)
            {
                reason = "there is no local player";
                return false;
            }

            if (
                player.IsDead()
                || player.IsEncumbered()
                || player.InDodge()
                || player.IsKnockedBack()
                || player.IsStaggering()
            )
            {
                reason = "the player cannot reel in right now";
                return false;
            }

            // Vanilla only reels in when the player is farther than m_jumpPullMaxDist
            // (5 m), which decides whether a jump press pulls or jumps. The shortcut
            // is not a jump, so it only needs the point to be out of reach of the pull
            // itself: closer than that the player already hangs at the point.
            float distance = Vector3.Distance(point.transform.position, player.transform.position);
            if (distance <= point.m_closePullDist)
            {
                reason = $"the point is only {distance:0.0} m away, nothing to reel in";
                return false;
            }

            ReelIn(player, point, chargeStamina: true);
            reason = null;
            return true;
        }

        /// <summary>Starts (or restarts) the pull of the given grappling point.</summary>
        public static void ReelIn(Player player, GrapplingPoint point, bool chargeStamina)
        {
            if (chargeStamina)
            {
                if (!player.HaveStamina(player.m_jumpStaminaUsage) && Hud.instance != null)
                {
                    Hud.instance.StaminaBarEmptyFlash();
                }

                float stamina =
                    player.m_jumpStaminaUsage
                    - player.m_jumpStaminaUsage * player.GetEquipmentMovementModifier()
                    + player.m_jumpStaminaUsage * player.GetEquipmentJumpStaminaModifier();
                player.UseStamina(stamina);
            }

            point.Pull();
        }

        /// <summary>
        /// Keeps the reel-in alive while the player stands on the ground.
        ///
        /// <see cref="Character.UpdateWalking"/> steers the body toward the keys and
        /// brakes everything else for as long as the feet touch the ground, so the
        /// pull of a hook fired on flat ground is cancelled frame by frame: the player
        /// is only reeled in once off the ground, which is why a hook landing above
        /// them or a hook fired in mid air does work. With the option on, a grounded
        /// player is kicked toward the point the way the game kicks them when the
        /// point is above (<see cref="Character.ForceJump"/> also flags them as off
        /// the ground), and the speed is kept up until the game takes over in the air.
        /// </summary>
        public static void KeepReelingOnGround(Player player, PluginConfig settings)
        {
            GrapplingPoint point = GrapplingPoint.m_localGrappler;
            if (!settings.GroundPull || point == null || point.m_secondary || !player.IsOnGround())
            {
                // Nothing to keep up: the game reels an airborne player in by itself.
                _liftedFrom = null;
                return;
            }

            Vector3 towardPoint = point.transform.position - player.transform.position;
            float distance = towardPoint.magnitude;
            if (distance <= point.m_closePullDist)
            {
                return;
            }

            // The speed of the game itself, so a player on the ground is reeled in
            // exactly as fast as one in the air.
            float speed =
                point.Method == GrapplingPoint.GrapplingMethod.OneBoost
                    ? point.m_pullForce + point.m_distanceForceMultiplier * distance
                    : point.m_pullForce;
            Vector3 velocity = towardPoint / distance * speed;
            velocity.y = Mathf.Max(velocity.y, LiftOffSpeed);

            if (_liftedFrom == point)
            {
                // Still on the ground after the kick: keep the speed up, the walking
                // code brakes it otherwise.
                player.SetVelocity(velocity);
                return;
            }

            _liftedFrom = point;
            player.ForceJump(velocity, effects: false);

            if (settings.DebugLogging)
            {
                GrapplingHookTweaks.Instance.LogInfo(
                    $"Ground pull: lifted off the ground toward the point "
                        + $"({distance:0.0} m, {point.Method}, speed {speed:0.0})."
                );
            }
        }
    }
}
