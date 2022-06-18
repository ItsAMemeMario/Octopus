using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace Octopus.Content.Systems
{
		// Acts as a container for keybinds registered by this mod.
		// See Common/Players/ExampleKeybindPlayer for usage.
		public class KeybindSystem : ModSystem
		{
				public static ModKeybind WaterSpitKeybind { get; private set; }
				public static ModKeybind InkBombKeybind { get; private set; }

				public override void Load()
				{
						// Registers a new keybind
						WaterSpitKeybind = KeybindLoader.RegisterKeybind(Mod, "Spit Water", Keys.LeftShift);
						InkBombKeybind = KeybindLoader.RegisterKeybind(Mod, "Fire Ink Bomb", Keys.RightShift);
				}

				// Please see ExampleMod.cs' Unload() method for a detailed explanation of the unloading process.
				public override void Unload()
				{
						// Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can help you figure out what's keeping it loaded.
						WaterSpitKeybind = null;
						InkBombKeybind = null;
				}
		}
}
