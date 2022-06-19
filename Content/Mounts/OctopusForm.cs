using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Octopus.Content.Systems;
using Octopus.Content.Projectiles;
using Octopus.Content.Players;
using Octopus.Content.Items;
using Terraria.Audio;

namespace Octopus.Content.Mounts
{
  public class OctopusForm : ModMount
  {
    public override void SetStaticDefaults()
    {
      // Movement
      MountData.jumpHeight = 10; // How high the mount can jump.
      MountData.acceleration = 0.13f; // The rate at which the mount speeds up.
      MountData.jumpSpeed = 10f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is presssed.
      MountData.blockExtraJumps = false; // Determines whether or not you can use a double jump (like cloud in a bottle) while in the mount.
      MountData.constantJump = false; // Allows you to hold the jump button down.
      MountData.heightBoost = -22; // Height between the mount and the ground
      MountData.fallDamage = 1f; // Fall damage multiplier.
      MountData.runSpeed = 10f; // The speed of the mount
      MountData.dashSpeed = 10f; // The speed the mount moves when in the state of dashing.
      MountData.flightTimeMax = 0; // The amount of time in frames a mount can be in the state of flying.

      // Misc
      MountData.fatigueMax = 0;
      MountData.buff = ModContent.BuffType<Buffs.OctopusFormBuff>(); // The ID number of the buff assigned to the mount.

      // Effects
      MountData.spawnDust = DustID.Water; // The ID of the dust spawned when mounted or dismounted.

      // Frame data and player offsets
      MountData.totalFrames = 5; // Amount of animation frames for the mount
      MountData.playerYOffsets = Enumerable.Repeat(-15, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
      MountData.xOffset = 10;
      MountData.yOffset = 0;
      MountData.playerHeadOffset = 0;
      MountData.bodyFrame = 3;
      // Standing
      MountData.standingFrameCount = 4;
      MountData.standingFrameDelay = 12;
      MountData.standingFrameStart = 0;
      // Running
      MountData.runningFrameCount = 4;
      MountData.runningFrameDelay = 12;
      MountData.runningFrameStart = 0;
      // Flying
      MountData.flyingFrameCount = 0;
      MountData.flyingFrameDelay = 0;
      MountData.flyingFrameStart = 0;
      // In-air
      MountData.inAirFrameCount = 1;
      MountData.inAirFrameDelay = 12;
      MountData.inAirFrameStart = 4;
      // Idle
      MountData.idleFrameCount = 4;
      MountData.idleFrameDelay = 12;
      MountData.idleFrameStart = 0;
      MountData.idleFrameLoop = true;
      // Swim
      MountData.swimFrameCount = MountData.runningFrameCount;
      MountData.swimFrameDelay = MountData.runningFrameDelay;
      MountData.swimFrameStart = MountData.runningFrameStart;

      if (!Main.dedServ)
      {
        MountData.textureWidth = MountData.backTexture.Width() + 20;
        MountData.textureHeight = MountData.backTexture.Height();
      }
    }

    public override void UpdateEffects(Player player)
    {
      HidePlayer(player);

      // This code spawns some dust if we are moving fast enough.
      if (Math.Abs(player.velocity.X) > 2f)
      {
        Rectangle rect = player.getRect();
        Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Water);
      }
      //int test = Mount._frame;
    }

    private void HidePlayer(Player player)
    {
      player.GetModPlayer<OctopusPlayer>().isInOctopusForm = true;
      ArmorIDs.Head.Sets.DrawHead[KetchupBall.equipSlotHead] = false;
      ArmorIDs.Body.Sets.HidesTopSkin[KetchupBall.equipSlotBody] = true;
      ArmorIDs.Body.Sets.HidesArms[KetchupBall.equipSlotBody] = true;
      ArmorIDs.Legs.Sets.HidesBottomSkin[KetchupBall.equipSlotLegs] = true;
    }

    //private void ShootProjectiles(Player player)
    //{
    //		// Squirt water when WaterSpitKeyBind is held
    //		if (KeybindSystem.WaterSpitKeybind.Current)
    //		{
    //				Vector2 shootPosition = new Vector2(player.Center.X - player.direction * 12f, player.Center.Y + player.velocity.Y * 2f);
    //				if ((int)Main.time % 6 == 0) {
    //						SoundEngine.PlaySound(SoundID.Item13);
    //						Projectile.NewProjectile(player.GetSource_FromThis(), shootPosition.X, shootPosition.Y, player.direction * 8f + player.velocity.X * 0.3f, -1f, ProjectileID.WaterStream, 15, 5f, player.whoAmI);
    //				}
    //		}
    //		else if (headTurnTimer == 0 && KeybindSystem.InkBombKeybind.JustPressed)
    //		{
    //				SoundEngine.PlaySound(SoundID.NPCHit25);
    //				int headTurn;
    //				if (Main.MouseWorld.X >= player.Center.X)
    //				{
    //						headTurn = 1;
    //				} else {
    //						headTurn = -1;
    //				}
    //				Vector2 shootPosition = new Vector2(player.position.X + headTurn * 10f + 10f + player.velocity.X * 2f, player.position.Y + 14f + player.velocity.Y * 2f);
    //				Vector2 shootVelocity = Main.MouseWorld - shootPosition;
    //				shootVelocity = (shootVelocity / shootVelocity.Length()) * 16f;
    //				Projectile.NewProjectile(player.GetSource_FromThis(), shootPosition, shootVelocity, ModContent.ProjectileType<InkBomb>(), 50, 15f, player.whoAmI);
    //				headTurnTimer = headTurn * 15;
    //		}
    //}
  }
}