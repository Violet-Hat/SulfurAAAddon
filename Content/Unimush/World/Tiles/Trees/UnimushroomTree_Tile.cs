using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SulfurAAAddon.Content.Unimush.World.Tiles.Trees
{
    public class UnimushroomTree_Tile : ModTree
    {
        public override TreePaintingSettings TreeShaderSettings => new()
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        public override void SetStaticDefaults()
        {
            GrowsOnTileId = [ModContent.TileType<UnifiedMycelium_Tile>()];
        }

        public override int DropWood()
        {
            if (WorldGen.genRand.NextBool())
            {
                return ItemID.GlowingMushroom;
            }
            else
            {
                return ItemID.Mushroom;
            }
        }

        public override Asset<Texture2D> GetTexture()
        {
            return ModContent.Request<Texture2D>("SulfurAAAddon/Content/Unimush/World/Tiles/Trees/UnimushroomTree_Tile");
        }

        public override Asset<Texture2D> GetBranchTextures()
        {
            return ModContent.Request<Texture2D>("SulfurAAAddon/Content/Unimush/World/Tiles/Trees/UnimushroomTree_Branches");
        }

        public override Asset<Texture2D> GetTopTextures()
        {
            return ModContent.Request<Texture2D>("SulfurAAAddon/Content/Unimush/World/Tiles/Trees/UnimushroomTree_Top");
        }

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return TileID.MushroomPlants; //TODO: This was formerly trying to find "MushroomTree"...
        }
    }
}