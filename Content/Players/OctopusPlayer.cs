using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Octopus.Content.Mounts;
using Octopus.Content.Projectiles;
using Octopus.Content.Systems;
using System;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Audio;
using Octopus.Content.Buffs;
using Octopus.Content.Items;

namespace Octopus.Content.Players
{
  public class OctopusPlayer : ModPlayer
  {
    public bool isInOctopusForm = false;

    private int HeadTurn = 1;

    private const int InkBombCoolDown = 24;
    public int InkBombDelay = 0;
    private int OrangeFortSummonDelay = 0;

    private const int DoubleJumpCooldown = 30;
    private int DoubleJumpDelay = 0;

    private const int DashCooldown = 50;
    //private const int DashDuration = 35;
    private const float DashSpeed = 10f;
    private int DashDelay = 0;
    //private int DashTimer = 0;
    private int DashDir = 0;

    public override void ResetEffects()
    {
      if (isInOctopusForm && DoubleJumpDelay == 0)
      {
        Player.hasJumpOption_Fart = true;
        Player.canJumpAgain_Fart = true;
      }

      isInOctopusForm = false;

      if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[2] < 15)
      {
        DashDir = 1;
      }
      else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[3] < 15)
      {
        DashDir = -1;
      }
      else
      {
        DashDir = 0;
      }
    }

    public override void FrameEffects()
    {
      // TODO: Need new hook, FrameEffects doesn't run while paused.
      if (isInOctopusForm)
      {
        Player.head = EquipLoader.GetEquipSlot(Mod, "InvisiblePlayer", EquipType.Head);
        Player.body = EquipLoader.GetEquipSlot(Mod, "InvisiblePlayer", EquipType.Body);
        Player.legs = EquipLoader.GetEquipSlot(Mod, "InvisiblePlayer", EquipType.Legs);

        ArmorIDs.Head.Sets.DrawHead[KetchupBall.equipSlotHead] = false;
        ArmorIDs.Body.Sets.HidesTopSkin[KetchupBall.equipSlotBody] = true;
        ArmorIDs.Body.Sets.HidesArms[KetchupBall.equipSlotBody] = true;
        ArmorIDs.Legs.Sets.HidesBottomSkin[KetchupBall.equipSlotLegs] = true;

      }
      if (InkBombDelay > 0)
      {
        Player.direction = HeadTurn;
        InkBombDelay -= 1;
      }
    }

    public override void PreUpdateMovement()
    {
      if (isInOctopusForm)
      {
        // Fart
        if (DoubleJumpDelay == 0 && Player.oldVelocity.Y != 0 && Player.velocity.Y != 0 && Player.controlJump)
        {
          DoubleJumpDelay = DoubleJumpCooldown;
        }
        if (DoubleJumpDelay > 0)
        {
          DoubleJumpDelay--;
        }

        // Dash
        if (DashDir != 0 && DashDelay == 0)
        {
          Vector2 newVelocity = Player.velocity;

          switch (DashDir)
          {
            // Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
            case -1 when Player.velocity.X > -DashSpeed:
            case 1 when Player.velocity.X < DashSpeed:
              {
                newVelocity.X = DashDir * DashSpeed;
                // start our dash
                DashDelay = DashCooldown;
                //DashTimer = DashDuration;
                Player.velocity = newVelocity;
                break;
              }
          }

          // Fart
          SoundEngine.PlaySound(SoundID.Item16, Player.Center);
          // Poop
          Vector2 poopPosition = new Vector2(Player.position.X - 10f, Player.Center.Y - DashDir * 10f);
          Projectile.NewProjectile(Player.GetSource_FromThis(), poopPosition, Player.velocity, ProjectileID.ToiletEffect, 0, 0f, Owner: Player.whoAmI);

        }
        else if (DashDelay > 0)
        {
          DashDelay--;
        }
      }
    }

    public override void PreUpdate()
    {
      if (isInOctopusForm)
      {
        // Squirt water when WaterSpitKeyBind is held
        if (KeybindSystem.WaterSpitKeybind.Current)
        {
          Vector2 shootPosition = new Vector2(Player.Center.X - Player.direction * 12f, Player.Center.Y + Player.velocity.Y * 2f);
          if ((int)Main.time % 6 == 0)
          {
            SoundEngine.PlaySound(SoundID.Item13);
            Projectile.NewProjectile(Player.GetSource_FromThis(), shootPosition.X, shootPosition.Y, Player.direction * 8f + Player.velocity.X * 0.3f, -1f, ProjectileID.WaterStream, 15, 5f, Player.whoAmI);
          }
        }
        // Fire ink bomb when InkBombKeyBind is pressed or held
        else if (InkBombDelay == 0 && (KeybindSystem.InkBombKeybind.JustPressed || KeybindSystem.InkBombKeybind.Current))
        {
          SoundEngine.PlaySound(SoundID.NPCHit25);
          if (Main.MouseWorld.X >= Player.Center.X)
          {
            HeadTurn = 1;
          }
          else
          {
            HeadTurn = -1;
          }
          Vector2 shootPosition = new Vector2(Player.position.X + HeadTurn * 10f + 10f + Player.velocity.X * 2f, Player.position.Y + 14f + Player.velocity.Y * 2f);
          Vector2 shootVelocity = Main.MouseWorld - shootPosition;
          shootVelocity = (shootVelocity / shootVelocity.Length()) * 16f;
          Projectile.NewProjectile(Player.GetSource_FromThis(), shootPosition, shootVelocity, ModContent.ProjectileType<InkBomb>(), 100, 5f, Player.whoAmI);
          InkBombDelay = InkBombCoolDown;
        }
        if (InkBombDelay > 0)
        {
          AnimateInkBombSpit();
        }
        // Summon orange fort minion when OrangeFortKeybind is pressed
        if (OrangeFortSummonDelay == 0 && KeybindSystem.OrangeFortKeybind.JustPressed)
        {
          SoundEngine.PlaySound(SoundID.Item78, Player.position);

          // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
          Player.AddBuff(ModContent.BuffType<OrangeFortBuff>(), 2);

          // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
          var projectile = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.position + new Vector2(2, 20), default, ModContent.ProjectileType<OrangeFort>(), 10, 5, owner: Player.whoAmI, ai0: Player.ownedProjectileCounts[ModContent.ProjectileType<OrangeFort>()]);
          projectile.originalDamage = 10;
          OrangeFortSummonDelay = 30;
        }
        if (OrangeFortSummonDelay > 0)
        {
          OrangeFortSummonDelay--;
        }
      }
    }

    public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
    {
      if (isInOctopusForm)
      {
        SoundEngine.PlaySound(SoundID.NPCHit25, Player.position);
      }
    }

    private void AnimateInkBombSpit()
    {
      switch ((InkBombDelay + 1) / 2)
      {
        case 12:
          Player.mount._frame = 1;
          break;
        case 11:
          Player.mount._frame = 1;
          break;
        case 10:
          Player.mount._frame = 3;
          break;
        case 9:
          Player.mount._frame = 3;
          break;
        case 8:
          Player.mount._frame = 3;
          break;
        default:
          Player.mount._frame = 0;
          break;
      }
    }
  }
}