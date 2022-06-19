using Octopus.Content.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;

namespace Octopus.Content.Projectiles
{
  public class InkBomb : ModProjectile
  {
    public static int Width { get; } = 8;
    public static int Height { get; } = 8;
    public override void SetStaticDefaults()
    {
      // Total animation frame count
      Main.projFrames[Projectile.type] = 3;
    }

    public override void SetDefaults()
    {
      Projectile.width = Width; // The width of projectile hitbox
      Projectile.height = Height; // The height of projectile hitbox
      Projectile.penetrate = -1;
      Projectile.damage = 100;

      DrawOffsetX = -(30 - Width / 2);
      DrawOriginOffsetY = -(20 - Height / 2);
      DrawOriginOffsetX = 6;

      Projectile.friendly = true; // Can the projectile deal damage to enemies?
      Projectile.DamageType = DamageClass.Magic; // Is the projectile shot by a ranged weapon?
      Projectile.ignoreWater = true; // Is the projectile's speed influenced by water?
    }

    public override void AI()
    {
      if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 2)
      {
        Projectile.tileCollide = false;
        // Set to transparent. This Projectile technically lives as  transparent for about 3 frames
        Projectile.alpha = 255;
        // change the hitbox size, centered about the original Projectile center. This makes the Projectile damage enemies during the explosion.
        Projectile.position = Projectile.Center;
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.Center = Projectile.position;
        Projectile.knockBack = 10f;
      }
      else
      {
        // slow down horizontally
        Projectile.velocity.X *= 0.99f;
        // affected by gravity
        Projectile.velocity.Y += 0.4f;

        // Loop through the 14 animation frames, spending 6 ticks on each
        // Projectile.frame — index of current frame

        if (++Projectile.frameCounter >= 6)
        {
          Projectile.frameCounter = 0;
          Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }

        // rotate sprite
        Projectile.rotation = Projectile.velocity.ToRotation();

        // spawn dust
        int dustIndex;
        for (int i = 0; i < 2; i++)
        {
          dustIndex = Dust.NewDust(Projectile.Center, 0, 0, DustID.Ash, Alpha: 150, newColor: Color.Black);
          Main.dust[dustIndex].noGravity = true;
        }
      }
    }

    public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
    {
      if (Main.expertMode || Main.masterMode)
      {
        if (target.type >= NPCID.EaterofWorldsHead && target.type <= NPCID.EaterofWorldsTail)
        {
          damage /= 5;
        }
      }
      Projectile.timeLeft = 2;
    }

    //public override void ModifyDamageHitbox(ref Rectangle hitbox)
    //{
    //		float scale = 2.5f;
    //		int oldWidth = hitbox.Width;
    //		int oldHeight = hitbox.Height;
    //		hitbox.Width = (int)(oldWidth * scale);
    //		hitbox.Height = (int)(oldHeight * scale);
    //		hitbox.X -= (hitbox.Width - oldWidth) / 2;
    //		hitbox.Y -= (hitbox.Height - oldHeight) / 2;
    //}
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
      Projectile.timeLeft = 2;
      return false;
    }

    public override void Kill(int timeLeft)
    {
      SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

      int goreIndex;
      // Black Smoke Gore
      for (int i = 0; i < 5; i++)
      {
        goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(1.8f, 2.8f)), 99);
        Main.gore[goreIndex].alpha = 60;
      }
      // Black Dust Effect
      int dustIndex;
      for (int i = 0; i < 50; i++)
      {
        dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ash, Alpha: 80, newColor: Color.Black, Scale: 2f);
        Main.dust[dustIndex].noGravity = true;
      }
      // Spawn child projectiles
      if (Projectile.owner == Main.myPlayer)
      {
        int splitCount = Main.rand.Next(3, 6);
        for (int i = 0; i < splitCount; i++)
        {
          int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Projectile.oldVelocity, new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f)), ModContent.ProjectileType<InkDroplet>(), 25, 5, Projectile.owner);
          Main.projectile[projIndex].maxPenetrate = 0;
        }
      }
      Vector2 center = Projectile.Center;
      Projectile.width = Width;
      Projectile.height = Height;
      Projectile.Center = center;
    }
  }
}