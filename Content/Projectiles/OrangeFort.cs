using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Octopus.Content.Buffs;
using Octopus.Content.Projectiles;

namespace Octopus.Content.Projectiles
{
  public class OrangeFort : ModProjectile
  {
    private const int Width = 16;
    private const int Height = 10;
    private const float MaxDistanceFromTarget = 1600f;

    private const int ShootCoolDown = 10;
    private int ShootDelay;
    private int HeadTurn = 1;

    private int stackOrder;

    public override void SetStaticDefaults()
    {
      DisplayName.SetDefault("Example Minion");
      // Sets the amount of frames this minion has on its spritesheet
      Main.projFrames[Projectile.type] = 1;
      // This is necessary for right-click targeting
      ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

      Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

      ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
      ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
    }

    public sealed override void SetDefaults()
    {
      Projectile.width = Width;
      Projectile.height = Height;
      DrawOffsetX = Width / 2 - 12;
      DrawOriginOffsetY = Height / 2 - 11;
      Projectile.tileCollide = false; // Makes the minion go through tiles freely

      // These below are needed for a minion weapon
      Projectile.friendly = false; // Stops minion from dealing contact damage
      Projectile.minion = true; // Declares this as a minion (has many effects)
      Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
      Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
      Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
    }

    // Minion breaks fragile tiles like grass or pots
    public override bool? CanCutTiles()
    {
      return false;
    }

    // Minion does not do contact damage
    public override bool MinionContactDamage()
    {
      return false;
    }

    public override void AI()
    {
      Player owner = Main.player[Projectile.owner];
      // stackOrder is set to the current number of orange fort minions on the player
      stackOrder = (int)Projectile.ai[0];

      if (!CheckActive(owner))
      {
        return;
      }

      Positioning(owner);
      if (ShootDelay == 0)
      {
        AttackNearest(owner);
      }
      if (ShootDelay > 0)
      {
        AnimateShooting();
        ShootDelay--;
      }
    }

    // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
    private bool CheckActive(Player owner)
    {
      if (owner.dead || !owner.active)
      {
        owner.ClearBuff(ModContent.BuffType<OrangeFortBuff>());

        return false;
      }

      if (owner.HasBuff(ModContent.BuffType<OrangeFortBuff>()))
      {
        Projectile.timeLeft = 2;
      }

      return true;
    }

    private void Positioning(Player owner)
    {
      int octopusHeadOffset; // offset to help align orange forts to the top of the octopus' head
      switch (owner.mount._frame)
      {
        case 0:
          octopusHeadOffset = 8;
          break;
        case 1:
          octopusHeadOffset = 10;
          break;
        case 2:
          octopusHeadOffset = 8;
          break;
        case 3:
          octopusHeadOffset = 6;
          break;
        case 4:
          octopusHeadOffset = 10;
          break;
        default:
          octopusHeadOffset = 20; // unusual value to help debug
          break;
      }

      float posX = owner.position.X + 1;
      float posY = owner.position.Y - octopusHeadOffset - Height * stackOrder;
      Projectile.position = new Vector2(posX, posY);

      Projectile.direction = Projectile.spriteDirection = owner.direction;

      Projectile.frame = 0;
    }

    private void AttackNearest(Player owner)
    {
      NPC target = default; // empty value so the code will compile
      bool foundTarget = false;

      // Target the NPC marked by whips if applicable
      if (owner.HasMinionAttackTargetNPC)
      {
        target = Main.npc[owner.MinionAttackTargetNPC];
        if (Vector2.Distance(target.Center, Projectile.Center) <= MaxDistanceFromTarget)
        {
          foundTarget = true;
        }
      }

      // Manually search for target
      if (!foundTarget)
      {
        float distanceBetween;
        float minDistance = 0f;
        NPC npc;
        for (int i = 0; i < Main.maxNPCs; i++)
        {
          npc = Main.npc[i];
          if (npc.CanBeChasedBy())
          {
            distanceBetween = Vector2.Distance(npc.Center, Projectile.Center);
            bool closest = distanceBetween < minDistance;
            bool inRange = distanceBetween <= MaxDistanceFromTarget;
            bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);

            if (inRange && (closest || !foundTarget) && lineOfSight)
            {
              target = npc;
              minDistance = distanceBetween;
              foundTarget = true;
            }
          }
        }
      }

      // If target is found, shoot projectile at target
      if (foundTarget)
      {
        SoundEngine.PlaySound(SoundID.Item11, Projectile.position);
        Vector2 shootVelocity = target.Center - Projectile.Center;
        shootVelocity.Normalize();
        shootVelocity *= 18f;
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity, ModContent.ProjectileType<OrangeSeed>(), 20, 2, Projectile.owner);

        ShootDelay = ShootCoolDown;
        HeadTurn = target.Center.X >= Projectile.Center.X ? 1: -1;
      }
    }

    private void AnimateShooting()
    {
      Projectile.direction = Projectile.spriteDirection = HeadTurn;
      switch ((ShootDelay + 1) / 2)
      {
        case 5:
          Projectile.position.X -= 2f * HeadTurn;
          break;
        case 4:
          Projectile.position.X -= 1f * HeadTurn;
          break;
      }
    }
  }
}
