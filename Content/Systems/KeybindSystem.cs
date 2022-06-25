using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace Octopus.Content.Systems
{
		// Acts as a container for keybinds registered by this mod.
		public class KeybindSystem : ModSystem
		{
				public static ModKeybind WaterSpitKeybind { get; private set; }
				public static ModKeybind InkBombKeybind { get; private set; }

				public static ModKeybind OrangeFortKeybind { get; private set; }

				public override void Load()
				{
						// Registers a new keybind
						WaterSpitKeybind = KeybindLoader.RegisterKeybind(Mod, "Spit Water", Keys.Z);
						InkBombKeybind = KeybindLoader.RegisterKeybind(Mod, "Fire Ink Bomb", Keys.X);
						OrangeFortKeybind = KeybindLoader.RegisterKeybind(Mod, "Summon Orange Fort", Keys.C);
				}
				public override void Unload()
				{
						WaterSpitKeybind = null;
						InkBombKeybind = null;
						OrangeFortKeybind = null;
				}
		}
}
