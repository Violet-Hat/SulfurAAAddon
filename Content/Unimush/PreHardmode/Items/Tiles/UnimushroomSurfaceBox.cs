using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using AAModClassic.Utilities.AbstractsLikeDigitalCircus;

namespace SulfurAAAddon.Content.Unimush.PreHardmode.Items.Tiles
{
    public class UnimushroomSurfaceBox : BaseAAItem, ILocalizedModType
	{
        public new string LocalizationCategory => "Items.Placeables.MusicBoxes";
		public override void SetStaticDefaults()
		{
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
        }

        public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<UnimushroomSurfaceBox_Tile>();
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.LightRed;
			Item.value = 10000;
			Item.accessory = true;
		}
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.MusicBox);
            recipe.AddIngredient(ItemID.Mushroom, 10);
            recipe.AddIngredient(ItemID.GlowingMushroom, 10);
            recipe.AddIngredient(ItemID.CobaltBar, 5);
            recipe.AddTile(TileID.Sawmill);
            recipe.Register();
        }
    }
}