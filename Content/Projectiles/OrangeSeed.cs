using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Octopus.Content.Projectiles
{
  public class OrangeSeed : ModProjectile
  {
    public override void SetStaticDefaults()
    {
      DisplayName.SetDefault("Orange Seed");
    }

    public override void SetDefaults()
    {
      Projectile.CloneDefaults(ProjectileID.Bullet);

      Projectile.width = 4;
      Projectile.height = 2;

      Projectile.alpha = 240;
      DrawOffsetX = -8;
      DrawOriginOffsetY = -4;
      DrawOriginOffsetX = 2;
    }

    public override void AI()
    {
      FadeIn();

      Projectile.rotation = Projectile.velocity.ToRotation();
    }

    private void FadeIn()
    {
      if (Projectile.alpha > 0)
      {
        Projectile.alpha -= 80;
      }

      if (Projectile.alpha < 0)
      {
        Projectile.alpha = 0;
      }
    }

    public override void Kill(int timeLeft)
    {
      SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
    }
  }
}
