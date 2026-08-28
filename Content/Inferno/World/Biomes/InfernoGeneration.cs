using AAModClassic._Content.Inferno.World.Tiles;
using AAModClassic.Base.BaseMod.Base;
using AAModClassic.Conversions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static AAModClassic.Utilities.WorldGenUtils;

namespace SulfurAAAddon.Content.Inferno.World.Biomes
{
    public class InfernoTexGenAssets : ModSystem
    {
        //Paths
        public readonly string ClassicInfernoPath = "AAModClassic/_Content/Inferno/World/Biomes/";
        public readonly string AddonInfernoPath = "SulfurAAAddon/Content/Inferno/World/Biomes/";

        //Small worlds
        public static TexGenData VolcanoTileData;
        public static TexGenData VolcanoWallData;
        public static TexGenData VolcanoLiquidData;

        //Medium+ worlds
        public static TexGenData BigVolcanoTileData;
        public static TexGenData BigVolcanoWallData;
        public static TexGenData BigVolcanoLiquidData;

        public override void OnModLoad()
        {
            VolcanoTileData = TexGen.GetTextureForGen(ClassicInfernoPath + "Volcano");
            VolcanoWallData = TexGen.GetTextureForGen(ClassicInfernoPath + "VolcanoWalls");
            VolcanoLiquidData = TexGen.GetTextureForGen(ClassicInfernoPath + "VolcanoLava");

            BigVolcanoTileData = TexGen.GetTextureForGen(AddonInfernoPath + "BigVolcano");
            BigVolcanoWallData = TexGen.GetTextureForGen(AddonInfernoPath + "BigVolcanoWalls");
            BigVolcanoLiquidData = TexGen.GetTextureForGen(AddonInfernoPath + "BigVolcanoLava");
        }
    }

    public class InfernoGeneration : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            //this handles generating the actual tiles, but you still need to add things like treegen etc. I know next to nothing about treegen so you're on your own there, lol.
            int worldSize = GetWorldSize();
            int biomeRadius = worldSize == 3 ? 260 : worldSize == 2 ? 220 : 180;

            bool isSmallWorld = worldSize == 1;

            //Data to use
            TexGenData tileData = isSmallWorld ? InfernoTexGenAssets.VolcanoTileData : InfernoTexGenAssets.BigVolcanoTileData;
            TexGenData wallData = isSmallWorld ? InfernoTexGenAssets.VolcanoWallData : InfernoTexGenAssets.BigVolcanoWallData;
            TexGenData liquidData = isSmallWorld ? InfernoTexGenAssets.VolcanoLiquidData : InfernoTexGenAssets.BigVolcanoLiquidData;

            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(255, 0, 0)] = ModContent.TileType<Torchstone_Tile>(),
                [new Color(0, 0, 255)] = TileID.Obsidian,
                [new Color(0, 255, 0)] = ModContent.TileType<ScorchedDynastyWoodUnsafe_Tile>(),
                [new Color(255, 255, 0)] = ModContent.TileType<ScorchedShingles_Tile>(),
                [new Color(255, 0, 255)] = ModContent.TileType<ScorchedPlatform_Tile>(),
                [new Color(150, 150, 150)] = -2, //turn into air
                [Color.Black] = -1 //don't touch when genning
            };

            HashSet<int> protectedTiles = [
                ModContent.TileType<ScorchedDynastyWoodUnsafe_Tile>(),
                ModContent.TileType<ScorchedShingles_Tile>(),
                ModContent.TileType<ScorchedPlatform_Tile>(),
            ];

            Dictionary<Color, int> colorToWall = new()
            {
                [new Color(255, 0, 0)] = ModContent.WallType<TorchstoneWall_Wall>(),
                [new Color(0, 0, 255)] = ModContent.WallType<ScorchedDynastyWoodWall_Wall>(),
                [Color.Black] = -1 //don't touch when genning				
            };

            TexGen gen = TexGen.GetTexGenerator(tileData, colorToTile, wallData, colorToWall, liquidData, unbreakableTiles: protectedTiles, unbreakableWalls: [ModContent.WallType<ScorchedDynastyWoodWall_Wall>()]);
            Point newOrigin = new(origin.X, origin.Y - 30); //biomeRadius

            WorldUtils.Gen(newOrigin, new Shapes.Circle(biomeRadius), Actions.Chain(
            //remove all fluids in sphere...
            [
                new InWorld(),
                new Modifiers.RadialDither(biomeRadius - 5, biomeRadius),
                new Actions.SetLiquid(1, 0)
            ]));
            WorldUtils.Gen(new Point(origin.X - (gen.width / 2), origin.Y - 20), new Shapes.Rectangle(gen.width, gen.height), Actions.Chain(
            //remove all fluids in the volcano...
            [
                new InWorld(),
                new Actions.SetLiquid(0, 0)
            ]));

            int volcanoOffset = isSmallWorld ? 80 : 140;
            int genX = origin.X - (gen.width / 2);
            int genY = origin.Y - volcanoOffset;
            gen.Generate(genX, genY, true, true);

            WorldUtils.Gen(newOrigin, new Shapes.Circle(biomeRadius), Actions.Chain(
            //convert tiles
            [
                new InWorld(),
                new Modifiers.RadialDither(biomeRadius - 5, biomeRadius), //this provides the 'blending' on the edges (except the top)
				new ConvertTile(ModContent.GetInstance<InfernoConversion>().Type) //actually place the tile
			]));

            //WorldGen.PlaceObject(genX + 65, genY + 4, Terraria.ModLoader.ModContent.TileType<DracoAltarS_Tile>());
            int offsetX = isSmallWorld ? 0 : 75;
            int offsetY = isSmallWorld ? 0 : 151;
            
            WorldGen.PlaceObject(genX + 24 + offsetX, genY + 307 + offsetY, ModContent.TileType<DragonEgg_Tile>());
            WorldGen.PlaceObject(genX + 33 + offsetX, genY + 313 + offsetY, ModContent.TileType<DragonEgg_Tile>());
            WorldGen.PlaceObject(genX + 46 + offsetX, genY + 314 + offsetY, ModContent.TileType<DragonEgg_Tile>());
            WorldGen.PlaceObject(genX + 57 + offsetX, genY + 316 + offsetY, ModContent.TileType<DragonEgg_Tile>());
            WorldGen.PlaceObject(genX + 67 + offsetX, genY + 316 + offsetY, ModContent.TileType<DragonEgg_Tile>());
            WorldGen.PlaceObject(genX + 78 + offsetX, genY + 317 + offsetY, ModContent.TileType<DragonEgg_Tile>());
            WorldGen.PlaceObject(genX + 87 + offsetX, genY + 315 + offsetY, ModContent.TileType<DragonEgg_Tile>());
            WorldGen.PlaceObject(genX + 96 + offsetX, genY + 312 + offsetY, ModContent.TileType<DragonEgg_Tile>());
            WorldGen.PlaceObject(genX + 103 + offsetX, genY + 307 + offsetY, ModContent.TileType<DragonEgg_Tile>());
            NetMessage.SendObjectPlacement(-1, genX + 24 + offsetX, genY + 307 + offsetY, (ushort)ModContent.TileType<DragonEgg_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 33 + offsetX, genY + 313 + offsetY, (ushort)ModContent.TileType<DragonEgg_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 46 + offsetX, genY + 314 + offsetY, (ushort)ModContent.TileType<DragonEgg_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 57 + offsetX, genY + 316 + offsetY, (ushort)ModContent.TileType<DragonEgg_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 67 + offsetX, genY + 316 + offsetY, (ushort)ModContent.TileType<DragonEgg_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 78 + offsetX, genY + 317 + offsetY, (ushort)ModContent.TileType<DragonEgg_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 87 + offsetX, genY + 315 + offsetY, (ushort)ModContent.TileType<DragonEgg_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 96 + offsetX, genY + 312 + offsetY, (ushort)ModContent.TileType<DragonEgg_Tile>(), 0, 0, -1, -1);
            NetMessage.SendObjectPlacement(-1, genX + 103 + offsetX, genY + 307 + offsetY, (ushort)ModContent.TileType<DragonEgg_Tile>(), 0, 0, -1, -1);

            for (int num = 0; num < Main.maxTilesX / 390; num++)
            {
                int xAxis = origin.X + WorldGen.genRand.Next(0, biomeRadius);
                int yAxis = origin.Y + WorldGen.genRand.Next(0, biomeRadius);
                for (int AltarX = xAxis - 45; AltarX < xAxis + 45; AltarX++)
                {
                    for (int AltarY = yAxis - 45; AltarY < yAxis + 45; AltarY++)
                    {
                        if (Main.rand.NextBool(15))
                        {
                            WorldGen.PlaceObject(AltarX, AltarY - 1, ModContent.TileType<DragonAltarUnsafe_Tile>());
                        }
                    }
                }
            }

            return true;
        }
    }

    public class InfernoDelete : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            //this handles generating the actual tiles, but you still need to add things like treegen etc. I know next to nothing about treegen so you're on your own there, lol.
            int worldSize = GetWorldSize();
            bool isSmallWorld = worldSize == 1;

            //Data to use
            TexGenData tileData = isSmallWorld ? InfernoTexGenAssets.VolcanoTileData : InfernoTexGenAssets.BigVolcanoTileData;

            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(255, 0, 0)] = -2,
                [new Color(0, 0, 255)] = -2,
                [new Color(0, 255, 0)] = -2,
                [new Color(0, 0, 255)] = -2,
                [new Color(255, 255, 0)] = -2,
                [new Color(255, 0, 255)] = -2,
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            TexGen gen = TexGen.GetTexGenerator(tileData, colorToTile);

            int volcanoOffset = isSmallWorld ? 80 : 140;
            int genX = origin.X - (gen.width / 2);
            int genY = origin.Y - volcanoOffset;
            gen.Generate(genX, genY, true, true);

            return true;
        }
    }
}