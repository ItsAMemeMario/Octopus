using Terraria.GameContent.Creative;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Octopus.Content.Mounts;
using Octopus.Content.Players;

namespace Octopus.Content.Items
{
		public class KetchupBall : ModItem
		{
				public static int equipSlotHead;
				public static int equipSlotBody;
				public static int equipSlotLegs;
				public override void Load()
				{
						// The code below runs only if we're not loading on a server
						if (Main.netMode == NetmodeID.Server)
								return;

						// Add equip textures
						EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this, name: "InvisiblePlayer");
						EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this, name: "InvisiblePlayer");
						EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this, name: "InvisiblePlayer");
				}

				// Called in SetStaticDefaults
				private void SetupDrawing()
				{
						// Since the equipment textures weren't loaded on the server, we can't have this code running server-side
						if (Main.netMode == NetmodeID.Server)
								return;

						equipSlotHead = EquipLoader.GetEquipSlot(Mod, "InvisiblePlayer", EquipType.Head);
						equipSlotBody = EquipLoader.GetEquipSlot(Mod, "InvisiblePlayer", EquipType.Body);
						equipSlotLegs = EquipLoader.GetEquipSlot(Mod, "InvisiblePlayer", EquipType.Legs);
				}

				public override void SetStaticDefaults()
				{
						Tooltip.SetDefault("Reject humanity...");
						CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;

						SetupDrawing();
				}

				public override void SetDefaults()
				{
						Item.CloneDefaults(ItemID.SlimySaddle);
						Item.accessory = true;
						Item.mountType = ModContent.MountType<Mounts.OctopusForm>();
				}

				public override void AddRecipes()
				{
						Recipe recipe = CreateRecipe();
						recipe.AddIngredient(ItemID.DirtBlock, 0);
						recipe.Register();
				}
		}
}