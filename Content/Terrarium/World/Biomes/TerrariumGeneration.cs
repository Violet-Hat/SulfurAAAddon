using AAModClassic._Content.Terrarium.World.Tiles;
using AAModClassic.Base.BaseMod.Base;
using AAModClassic.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SulfurAAAddon.Content.Terrarium.World.Tiles;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace SulfurAAAddon.Content.Terrarium.World.Biomes
{
    public class TerrariumTexGenAssets : ModSystem
    {
        //Paths
        public readonly string ClassicTerrariumPath = "AAModClassic/_Content/Terrarium/World/Biomes/";
        public readonly string AddonTerrariumPath = "SulfurAAAddon/Content/Terrarium/World/Biomes/";

        //Data
        public static TexGenData TerrariumSmallDeletionData;
        public static TexGenData TerrariumMediumDeletionData;

        public static TexGenData TerrariumSmallTileData;
        public static TexGenData TerrariumMediumTileData;

        public static TexGenData TerrariumSmallWallData;
        public static TexGenData TerrariumMediumWallData;

        public static TexGenData TerrariumSmallLiquidData;
        public static TexGenData TerrariumMediumLiquidData;

        public override void OnModLoad()
        {
            TerrariumSmallDeletionData = TexGen.GetTextureForGen(ClassicTerrariumPath + "TerrariumDelete");
            TerrariumMediumDeletionData = TexGen.GetTextureForGen(AddonTerrariumPath + "TerrariumMedDelete");

            TerrariumSmallTileData = TexGen.GetTextureForGen(ClassicTerrariumPath + "Terrarium");
            TerrariumMediumTileData = TexGen.GetTextureForGen(AddonTerrariumPath + "TerrariumMed");

            TerrariumSmallWallData = TexGen.GetTextureForGen(ClassicTerrariumPath + "TerrariumWalls");
            TerrariumMediumWallData = TexGen.GetTextureForGen(AddonTerrariumPath + "TerrariumMedWalls");

            TerrariumSmallLiquidData = TexGen.GetTextureForGen(AddonTerrariumPath + "TerrariumLiquid");
            TerrariumMediumLiquidData = TexGen.GetTextureForGen(AddonTerrariumPath + "TerrariumMedLiquid");
        }
    }

    public class TerrariumDelete : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            //this handles generating the actual tiles, but you still need to add things like treegen etc. I know next to nothing about treegen so you're on your own there, lol.

            int worldSize = WorldGenUtils.GetWorldSize();
            int biomeRadius = worldSize == 3 ? 400 : worldSize == 2 ? 300 : 200;

            Dictionary<Color, int> colorToTile = new Dictionary<Color, int>
            {
                [new Color(0, 255, 0)] = -2,
                [Color.Black] = -1 //don't touch when genning		
            };

            Dictionary<Color, int> colorToWall = new Dictionary<Color, int>();
            colorToTile[new Color(0, 255, 0)] = -2;
            colorToTile[Color.Black] = -1; //don't touch when genning	

            TexGenData Terrasphere;
            if (worldSize == 1)
                Terrasphere = TerrariumTexGenAssets.TerrariumSmallDeletionData;
            else
                Terrasphere = TerrariumTexGenAssets.TerrariumMediumDeletionData;

            TexGen gen = TexGen.GetTexGenerator(Terrasphere, colorToTile, Terrasphere, colorToWall);
            Point newOrigin = new Point(origin.X, origin.Y); //biomeRadius);

            WorldUtils.Gen(newOrigin, new Shapes.Circle(biomeRadius), Actions.Chain(new GenAction[] //remove all fluids in sphere...
            {
                new WorldGenUtils.InWorld(),
                new Modifiers.RadialDither(biomeRadius - 5, biomeRadius),
                new Actions.SetLiquid(0, 0)
            }));
            WorldUtils.Gen(new Point(origin.X - (gen.width / 2), origin.Y - 20), new Shapes.Rectangle(gen.width, gen.height), Actions.Chain(new GenAction[] //remove all fluids in the volcano...
            {
                new WorldGenUtils.InWorld(),
                new Actions.SetLiquid(0, 0)
            }));
            gen.Generate(origin.X - (gen.width / 2), origin.Y, true, true);

            return true;
        }
    }

    public class TerrariumGeneration : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            //this handles generating the actual tiles, but you still need to add things like treegen etc. I know next to nothing about treegen so you're on your own there, lol.
            int worldSize = WorldGenUtils.GetWorldSize();
            int biomeRadius = worldSize == 3 ? 400 : worldSize == 2 ? 300 : 200;

            Dictionary<Color, int> colorToTile = new Dictionary<Color, int>
            {
                [new Color(0, 255, 0)] = ModContent.TileType<TerraCrystal_Tile>(),
                [new Color(255, 0, 255)] = ModContent.TileType<PermeableTerraWood_Tile>(),
                [new Color(255, 255, 0)] = ModContent.TileType<TerraLeaves_Tile>(),
                [new Color(255, 0, 0)] = ModContent.TileType<TerrariumDirt>(),
                [new Color(255, 100, 0)] = ModContent.TileType<TerrariumStone>(),
                [new Color(0, 255, 255)] = ModContent.TileType<TerrariumGrass>(),
                [new Color(0, 0, 255)] = -2, //turn into air
                [Color.Black] = -1 //don't touch when genning		
            };

            HashSet<int> protectedTiles = [
                ModContent.TileType<TerraCrystal_Tile>(),
                ModContent.TileType<PermeableTerraWood_Tile>(),
                ModContent.TileType<TerraLeaves_Tile>(),
                ModContent.TileType<TerrariumDirt>(),
                ModContent.TileType<TerrariumStone>(),
                ModContent.TileType<TerrariumGrass>()
            ];

            Dictionary<Color, int> colorToWall = new Dictionary<Color, int>
            {
                [new Color(0, 255, 0)] = -2,
                [Color.Black] = -1 //don't touch when genning				
            };

            TexGenData Terrasphere = null;
            TexGenData TerraWalls = null;
            TexGenData TerraLiquid = null;

            if (Terrasphere == null)
            {
                if (worldSize == 1)
                {
                    Terrasphere = TerrariumTexGenAssets.TerrariumSmallTileData;

                    TerraWalls = TerrariumTexGenAssets.TerrariumSmallWallData;

                    TerraLiquid = TerrariumTexGenAssets.TerrariumSmallLiquidData;
                }
                else
                {
                    Terrasphere = TerrariumTexGenAssets.TerrariumMediumTileData;

                    TerraWalls = TerrariumTexGenAssets.TerrariumMediumWallData;

                    TerraLiquid = TerrariumTexGenAssets.TerrariumMediumLiquidData;
                }
            }

            WorldGenUtils.AddProtectedStructure(new Rectangle(origin.X, origin.Y, Terrasphere.Width, Terrasphere.Height), 20);

            TexGen gen = TexGen.GetTexGenerator(Terrasphere, colorToTile, TerraWalls, colorToWall, TerraLiquid, unbreakableTiles: protectedTiles);
            Point newOrigin = new Point(origin.X, origin.Y); //biomeRadius);

            WorldUtils.Gen(newOrigin, new Shapes.Circle(biomeRadius), Actions.Chain(new GenAction[] //remove all fluids in sphere...
            {
                new WorldGenUtils.InWorld(),
                new Modifiers.RadialDither(biomeRadius - 5, biomeRadius),
                new Actions.SetLiquid(0, 0)
            }));
            WorldUtils.Gen(new Point(origin.X - (gen.width / 2), origin.Y - 20), new Shapes.Rectangle(gen.width, gen.height), Actions.Chain(new GenAction[] //remove all fluids in the volcano...
            {
                new WorldGenUtils.InWorld(),
                new Actions.SetLiquid(0, 0)
            }));
            gen.Generate(origin.X - (gen.width / 2), origin.Y, true, true);

            return true;
        }
    }
}