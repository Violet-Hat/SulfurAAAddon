using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SulfurAAAddon.Content.Terrarium.World.Tiles
{
    public class TerrariumDirt : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            HitSound = SoundID.Dig;
            DustType = DustID.Dirt;
            AddMapEntry(new Color(151, 107, 75));

            Main.tileMerge[Type][ModContent.TileType<TerrariumStone>()] = true;
            Main.tileMerge[Type][ModContent.TileType<TerrariumGrass>()] = true;
        }
    }
}