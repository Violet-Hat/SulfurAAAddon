using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SulfurAAAddon.Content.Parthenan.World.Tiles
{
	public class FulguriteBrick_Wall : ModWall
	{
		public override void SetStaticDefaults()
		{
            Main.wallHouse[Type] = false;
			AddMapEntry(new Color(40, 0, 50));
		}
    }
}