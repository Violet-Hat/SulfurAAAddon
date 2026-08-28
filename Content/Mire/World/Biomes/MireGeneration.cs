using AAModClassic._Content.Mire.___PreHardmode.Items.Tiles.Decoration;
using AAModClassic._Content.Mire.___PreHardmode.Items.Tiles.Decoration.BogwoodFurniture;
using AAModClassic._Content.Mire.World.Tiles;
using AAModClassic.Base.BaseMod.Base;
using AAModClassic.Conversions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static AAModClassic.Utilities.WorldGenUtils;

namespace SulfurAAAddon.Content.Mire.World.Biomes
{
    public class MireTexGenAssets : ModSystem
    {
        //Paths
        public readonly string ClassicMirePath = "AAModClassic/_Content/Mire/World/Biomes/";
        public readonly string AddonMirePath = "SulfurAAAddon/Content/Mire/World/Biomes/";

        //Small worlds
        public static TexGenData LakeTileData;
        public static TexGenData LakeWallData;
        public static TexGenData LakeLiquidData;

        //Medium+ worlds
        public static TexGenData BigLakeTileData;
        public static TexGenData BigLakeWallData;
        public static TexGenData BigLakeLiquidData;

        public override void OnModLoad()
        {
            LakeTileData = TexGen.GetTextureForGen(ClassicMirePath + "RisingMoonLake");
            LakeWallData = TexGen.GetTextureForGen(ClassicMirePath + "RisingMoonLake_Walls");
            LakeLiquidData = TexGen.GetTextureForGen(ClassicMirePath + "RisingMoonLake_Liquid");

            BigLakeTileData = TexGen.GetTextureForGen(AddonMirePath + "BigRisingMoonLake");
            BigLakeWallData = TexGen.GetTextureForGen(AddonMirePath + "BigRisingMoonLake_Walls");
            BigLakeLiquidData = TexGen.GetTextureForGen(AddonMirePath + "BigRisingMoonLake_Liquid");
        }
    }

    public class MireGeneration : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            int worldSize = GetWorldSize();
            int biomeRadius = worldSize == 3 ? 260 : worldSize == 2 ? 220 : 180; //how deep the biome is (scaled by world size)

            bool isSmallWorld = worldSize == 1;

            //Data to use
            TexGenData tileData = isSmallWorld ? MireTexGenAssets.LakeTileData : MireTexGenAssets.BigLakeTileData;
            TexGenData wallData = isSmallWorld ? MireTexGenAssets.LakeWallData : MireTexGenAssets.BigLakeWallData;
            TexGenData liquidData = isSmallWorld ? MireTexGenAssets.LakeLiquidData : MireTexGenAssets.BigLakeLiquidData;

            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(0, 0, 255)] = ModContent.TileType<Depthstone_Tile>(),
                [new Color(255, 128, 0)] = ModContent.TileType<Darkmud_Tile>(),
                [new Color(0, 255, 255)] = ModContent.TileType<DepthMoss_Tile>(),
                [new Color(0, 255, 0)] = ModContent.TileType<AbyssGrass_Tile>(),
                [new Color(255, 0, 0)] = ModContent.TileType<AbyssWood_Tile>(),
                [new Color(128, 0, 0)] = ModContent.TileType<AbyssWoodSolid_Tile>(),
                [new Color(255, 255, 0)] = ModContent.TileType<AbyssVines_Tile>(),
                [new Color(255, 0, 255)] = ModContent.TileType<AbyssLeaves_Tile>(),
                [new Color(255, 100, 0)] = ModContent.TileType<BogwoodPlatform_Tile>(),
                [new Color(255, 0, 100)] = TileID.Obsidian,
                [new Color(150, 150, 150)] = -2, //turn into air
                [Color.Black] = -1 //don't touch when genning
            };

            HashSet<int> protectedTiles = [ ModContent.TileType<BogwoodPlatform_Tile>() ];

            Dictionary<Color, int> colorToWall = new()
            {
                [new Color(0, 0, 255)] = ModContent.WallType<DepthstoneWall_Wall>(),
                [Color.Black] = -1 //don't touch when genning
            };

            TexGen gen = TexGen.GetTexGenerator(tileData, colorToTile, wallData, colorToWall, liquidData, unbreakableTiles: protectedTiles);

            Point newOrigin = new(origin.X, origin.Y - 10); //biomeRadius);

            WorldUtils.Gen(new Point(origin.X - (gen.width / 2), origin.Y - 20), new Shapes.Rectangle(gen.width, gen.height), Actions.Chain(
            //remove all fluids in the mire...
            [
                new InWorld(),
                new Actions.SetLiquid(0, 0)
            ]));

            //convert tiles
            WorldUtils.Gen(newOrigin, new Shapes.Circle(biomeRadius), Actions.Chain(
            [
                new InWorld(),
                new Modifiers.RadialDither(biomeRadius - 5, biomeRadius), //this provides the 'blending' on the edges (except the top)
				new ConvertTile(ModContent.GetInstance<MireConversion>().Type) //actually place the tile
			]));

            int lakeOffset = isSmallWorld ? 30 : 60;
            int genX = origin.X - (gen.width / 2);
            int genY = origin.Y - lakeOffset;
            gen.Generate(genX, genY, true, true);

            int offsetX = isSmallWorld ? 0 : 100;
            int offsetY = isSmallWorld ? 0 : 228;

            WorldGen.PlaceObject(genX + 24 + offsetX, genY + 203 + offsetY, ModContent.TileType<HydraPod_Tile>());
            WorldGen.PlaceObject(genX + 43 + offsetX, genY + 211 + offsetY, ModContent.TileType<HydraPod_Tile>());
            WorldGen.PlaceObject(genX + 59 + offsetX, genY + 221 + offsetY, ModContent.TileType<HydraPod_Tile>());
            WorldGen.PlaceObject(genX + 81 + offsetX, genY + 223 + offsetY, ModContent.TileType<HydraPod_Tile>());
            WorldGen.PlaceObject(genX + 103 + offsetX, genY + 231 + offsetY, ModContent.TileType<HydraPod_Tile>());
            WorldGen.PlaceObject(genX + 124 + offsetX, genY + 222 + offsetY, ModContent.TileType<HydraPod_Tile>());
            WorldGen.PlaceObject(genX + 143 + offsetX, genY + 216 + offsetY, ModContent.TileType<HydraPod_Tile>());
            WorldGen.PlaceObject(genX + 161 + offsetX, genY + 214 + offsetY, ModContent.TileType<HydraPod_Tile>());
            WorldGen.PlaceObject(genX + 171 + offsetX, genY + 205 + offsetY, ModContent.TileType<HydraPod_Tile>());
            NetMessage.SendObjectPlacement(-1, genX + 24 + offsetX, genY + 204 + offsetY, ModContent.TileType<HydraPod_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 43 + offsetX, genY + 211 + offsetY, ModContent.TileType<HydraPod_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 59 + offsetX, genY + 221 + offsetY, ModContent.TileType<HydraPod_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 81 + offsetX, genY + 223 + offsetY, ModContent.TileType<HydraPod_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 103 + offsetX, genY + 231 + offsetY, ModContent.TileType<HydraPod_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 124 + offsetX, genY + 222 + offsetY, ModContent.TileType<HydraPod_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 143 + offsetX, genY + 216 + offsetY, ModContent.TileType<HydraPod_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 161 + offsetX, genY + 214 + offsetY, ModContent.TileType<HydraPod_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 171 + offsetX, genY + 205 + offsetY, ModContent.TileType<HydraPod_Tile>(), 0, 0, -1, -1);

            //WorldGen.PlaceObject(genX + 59, genY + 31, Terraria.ModLoader.ModContent.TileType<DreadAltarS_Tile>());		   

            for (int num = 0; num < Main.maxTilesX / 390; num++)
            {
                int xAxis = origin.X + WorldGen.genRand.Next(0, biomeRadius);
                int yAxis = origin.Y + WorldGen.genRand.Next(0, biomeRadius);
                for (int AltarX = xAxis - 45; AltarX < xAxis + 45; AltarX++)
                    for (int AltarY = yAxis - 45; AltarY < yAxis + 45; AltarY++)
                        if (Main.rand.NextBool(15))
                            WorldGen.PlaceObject(AltarX, AltarY - 1, ModContent.TileType<AbyssAltarUnsafe_Tile>());
            }
            return true;
        }
    }

    public class BogwoodCon : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            ushort LivingWood = (ushort)ModContent.TileType<LivingBogwood_Tile>(), LivingLeaves = (ushort)ModContent.TileType<LivingBogleaf_Tile>();

            ushort BogwoodWall = (ushort)ModContent.WallType<LivingBogwoodWall_Wall>(), LeafWall = (ushort)ModContent.WallType<LivingBogleafWall_Wall>();

            int worldSize = GetWorldSize();
            int biomeRadius = worldSize == 3 ? 260 : worldSize == 2 ? 220 : 180;
            Point newOrigin = new Point(origin.X, origin.Y - 10);

            WorldUtils.Gen(newOrigin, new Shapes.Circle(biomeRadius), Actions.Chain(new GenAction[] //Living Wood.
			{
                new InWorld(),
                new Modifiers.OnlyTiles(new ushort[]{ TileID.LivingMahogany, TileID.LivingWood}),
                new Modifiers.RadialDither(biomeRadius - 5, biomeRadius),
                new SetModTile(LivingWood, true, true)
            }));
            WorldUtils.Gen(newOrigin, new Shapes.Circle(biomeRadius), Actions.Chain(new GenAction[] //...and Living Leaves.
			{
                new InWorld(),
                new Modifiers.OnlyTiles(new ushort[]{ TileID.LivingMahoganyLeaves, TileID.LeafBlock}),
                new Modifiers.RadialDither(biomeRadius - 5, biomeRadius),
                new SetModTile(LivingLeaves, true, true)
            }));

            WorldUtils.Gen(newOrigin, new Shapes.Circle(biomeRadius), Actions.Chain(new GenAction[]
            {
                new InWorld(),
                new Modifiers.OnlyWalls(new ushort[]{ WallID.LivingWood }),
                new Modifiers.RadialDither(biomeRadius - 5, biomeRadius),
                new PlaceModWall(BogwoodWall, true)
            }));
            WorldUtils.Gen(newOrigin, new Shapes.Circle(biomeRadius), Actions.Chain(new GenAction[] //Walls
			{
                new InWorld(),
                new Modifiers.OnlyWalls(new ushort[]{ WallID.LivingLeaf }),
                new Modifiers.RadialDither(biomeRadius - 5, biomeRadius),
                new PlaceModWall(LeafWall, true)
            }));

            return true;
        }
        public static int GetWorldSize()
        {
            if (Main.maxTilesX == 4200) { return 1; }
            else if (Main.maxTilesX == 6400) { return 2; }
            else if (Main.maxTilesX == 8400) { return 3; }
            return 1; //unknown size, assume small
        }
    }

    public class MireDelete : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            //this handles generating the actual tiles, but you still need to add things like treegen etc. I know next to nothing about treegen so you're on your own there, lol.
            int worldSize = GetWorldSize();
            bool isSmallWorld = worldSize == 1;

            //Data to use
            TexGenData tileData = isSmallWorld ? MireTexGenAssets.LakeTileData : MireTexGenAssets.BigLakeTileData;

            Dictionary<Color, int> colorToTile = new Dictionary<Color, int>
            {
                [new Color(0, 0, 255)] = -2,
                [new Color(255, 128, 0)] = -2,
                [new Color(0, 255, 255)] = -2,
                [new Color(0, 255, 0)] = -2,
                [new Color(255, 0, 0)] = -2,
                [new Color(128, 0, 0)] = -2,
                [new Color(255, 255, 0)] = -2,
                [new Color(255, 0, 255)] = -2,
                [new Color(255, 100, 0)] = -2,
                [new Color(255, 0, 100)] = -2,
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            TexGen gen = TexGen.GetTexGenerator(tileData, colorToTile);
            
            int lakeOffset = (worldSize == 1) ? 30 : 60;
            int genX = origin.X - (gen.width / 2);
            int genY = origin.Y - lakeOffset;
            gen.Generate(genX, genY, true, true);

            return true;
        }
    }
}