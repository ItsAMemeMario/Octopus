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

namespace Octopus.Content.Projectiles
{
		public class InkDroplet : ModProjectile
		{
				public override void SetStaticDefaults()
				{
						// Total animation frame count
						Main.projFrames[Projectile.type] = 3;
				}
				public override void SetDefaults()
				{
						Projectile.CloneDefaults(ProjectileID.StyngerShrapnel);
						AIType = ProjAIStyleID.ThrownProjectile;

						Projectile.width = 8; // The width of projectile hitbox
						Projectile.height = 8; // The height of projectile hitbox

						DrawOffsetX = -14;
						DrawOriginOffsetY = -6;
						DrawOriginOffsetX = 4;

						Projectile.friendly = true; // Can the projectile deal damage to enemies?
						Projectile.DamageType = DamageClass.Magic; // Is the projectile shot by a ranged weapon?
						Projectile.ignoreWater = true; // Is the projectile's speed influenced by water?
				}

				public override void AI()
				{
						// animate projectile
						if (++Projectile.frameCounter >= 6)
						{
								Projectile.frameCounter = 0;
								Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
						}

						// rotate sprite
						Projectile.rotation = Projectile.velocity.ToRotation();

						// spawn dust
						int dustIndex = Dust.NewDust(Projectile.Center, 0, 0, DustID.Ash, Alpha: 150, newColor: Color.Black);
						Main.dust[dustIndex].noGravity = true;
				}

				public override void Kill(int timeLeft)
				{
						SoundEngine.PlaySound(SoundID.NPCHit18, Projectile.position);
						int dustIndex;
						for (int i = 0; i < 5; i++)
						{
								dustIndex = Dust.NewDust(Projectile.Center, 0, 0, DustID.Ash, Alpha: 150, newColor: Color.Black);
								Main.dust[dustIndex].noGravity = true;
						}
				}
		}
}
