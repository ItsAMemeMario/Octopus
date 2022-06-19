using Terraria;
using Terraria.ModLoader;

namespace Octopus.Content.Buffs
{
  public class OctopusFormBuff : ModBuff
  {
    public override void SetStaticDefaults()
    {
      DisplayName.SetDefault("Octo");
      Description.SetDefault("Reject humanity...");
      Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
      Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
    }

    public override void Update(Player player, ref int buffIndex)
    {
      player.mount.SetMount(ModContent.MountType<Mounts.OctopusForm>(), player);
      player.buffTime[buffIndex] = 10; // reset buff time
    }
  }
}
