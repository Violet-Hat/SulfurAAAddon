using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SulfurAAAddon.Content.Terrarium.World.Tiles
{
    public class TerrariumStone : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            HitSound = SoundID.Tink;
            DustType = DustID.Stone;
            AddMapEntry(new Color(128, 128, 128));

            Main.tileMerge[Type][ModContent.TileType<TerrariumDirt>()] = true;
            Main.tileMerge[Type][ModContent.TileType<TerrariumGrass>()] = true;
        }
    }
}