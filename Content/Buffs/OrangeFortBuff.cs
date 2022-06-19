using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Octopus.Content.Projectiles;
using Octopus.Content.Players;

namespace Octopus.Content.Buffs
{
		public class OrangeFortBuff : ModBuff
		{
				public override void SetStaticDefaults()
				{
						DisplayName.SetDefault("Orange Fort");
						Description.SetDefault("The tower of ruthless creatures obliterates your enemies!");

						Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
						Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
				}
				public override void Update(Player player, ref int buffIndex)
				{
						// If the minions exist, reset the buff time, otherwise remove the buff from the player
						if (player.ownedProjectileCounts[ModContent.ProjectileType<OrangeFort>()] > 0)
						{
								player.buffTime[buffIndex] = 18000;
						}
						else
						{
								player.DelBuff(buffIndex);
								buffIndex--;
						}
				}
		}
}
