using SulfurAAAddon.Content.Unimush.World.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace SulfurAAAddon.Conversions
{
    public class UnimushroomConversion : ModBiomeConversion
    {
        public override void PostSetupContent()
        {
            TileLoader.RegisterConversion(TileID.Grass, Type, ModContent.TileType<UnifiedMycelium_Tile>());
            WallLoader.RegisterConversion(WallID.Grass, Type, ModContent.WallType<UnifiedMushroomWall_Wall>());
        }
    }
}