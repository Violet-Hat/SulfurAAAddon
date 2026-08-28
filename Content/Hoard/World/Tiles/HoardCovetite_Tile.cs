using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SulfurAAAddon.Content.Hoard.World.Tiles
{
    public class HoardCovetite_Tile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(100, 51, 0));
            DustType = DustID.Gold;
            MinPick = 200;
        }
    }
}