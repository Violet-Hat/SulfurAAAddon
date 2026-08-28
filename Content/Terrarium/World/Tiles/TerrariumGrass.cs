using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SulfurAAAddon.Content.Terrarium.World.Tiles
{
    public class TerrariumGrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            HitSound = SoundID.Dig;
            DustType = DustID.Grass;
            AddMapEntry(new Color(28, 216, 94));

            Main.tileMerge[Type][ModContent.TileType<TerrariumDirt>()] = true;
            Main.tileMerge[Type][ModContent.TileType<TerrariumStone>()] = true;
        }
    }
}