using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SulfurAAAddon.Content.Unimush.World.Tiles
{
    public class UnifiedMushroomWall_Wall : ModWall
	{
		public override void SetStaticDefaults()
		{
            Main.wallHouse[Type] = true;
			AddMapEntry(new Color(60, 14, 60));
            Terraria.ID.WallID.Sets.Conversion.Grass[Type] = true;
        }
    }
}