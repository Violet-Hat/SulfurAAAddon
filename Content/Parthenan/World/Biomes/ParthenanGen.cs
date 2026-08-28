using AAModClassic;
using AAModClassic._Removed.Content.Parthenan.__Hardmode.Items.Tiles.Decoration;
using AAModClassic._Removed.Content.Parthenan.__Hardmode.Items.Tiles.Decoration.Ancient;
using AAModClassic.Base.BaseMod.Base;
using AAModClassic.Utilities;
using SulfurAAAddon.Content.Parthenan.World.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static AAModClassic.Utilities.WorldGenUtils;

namespace SulfurAAAddon.Content.Parthenan.World.Biomes
{
    public class ParthenanTexGenAssets : ModSystem
    {
        //Paths
        public readonly string ClassicParthenanPath = "AAModClassic/_Unreleased/Content/Parthenan/World/Biomes/";
        public readonly string AddonParthenanPath = "SulfurAAAddon/Content/Parthenan/World/Biomes/";

        //Small worlds
        public static TexGenData ParthenanTileData;
        public static TexGenData ParthenanWallData;

        //Medium+ worlds
        public static TexGenData BigParthenanTileData;
        public static TexGenData BigParthenanWallData;

        public override void OnModLoad()
        {
            ParthenanTileData = TexGen.GetTextureForGen(ClassicParthenanPath + "ParthenanGen");
            ParthenanWallData = TexGen.GetTextureForGen(ClassicParthenanPath + "ParthenanGen_Walls");

            BigParthenanTileData = TexGen.GetTextureForGen(AddonParthenanPath + "BigParthenanGen");
            BigParthenanWallData = TexGen.GetTextureForGen(AddonParthenanPath + "BigParthenanGen_Walls");
        }
    }

    public class ParthenanGen : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            int attempts = 0;
            int maxAttempts = 5000;

            bool isSmallWorld = GetWorldSize() == 1;

            //Data to use
            TexGenData tileData = isSmallWorld ? ParthenanTexGenAssets.ParthenanTileData : ParthenanTexGenAssets.BigParthenanTileData;
            TexGenData wallData = isSmallWorld ? ParthenanTexGenAssets.ParthenanWallData : ParthenanTexGenAssets.BigParthenanWallData;

            Point placementPoint = origin;
            do
            {
                //AAMod.instance.Logger.Info("Attempting to Place Parthenan at: " + placementPoint);

                bool canGenerateInLocation = true;

                if (!structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, tileData.Width, tileData.Height), WorldGenUtils.AllTilesAllowed, 0))
                {
                    //AAMod.instance.Logger.Info("Parthenan Placement Failed, Encountered a Pre-Existing Structure");
                    canGenerateInLocation = false;
                }

                if (canGenerateInLocation)
                {
                    int fullX = placementPoint.X + tileData.Width;
                    int fullY = placementPoint.Y + tileData.Height;

                    for (int x = placementPoint.X; x < fullX; x++)
                    {
                        for (int y = placementPoint.Y; y < fullY; y++)
                        {
                            if (Framing.GetTileSafely(x, y).HasTile)//ShouldAvoidLocation(new Point(x, y), attempts > 1000, attempts > 4000))
                            {
                                canGenerateInLocation = false;
                                break;
                            }
                        }
                        if (!canGenerateInLocation)
                            break;
                    }
                }

                if (canGenerateInLocation)
                {
                    AAMod.instance.Logger.Info("Parthenan successfully placed after " + attempts + " attempts.");
                    break;
                }

                int radius = 200 + attempts / 2;
                int targetX = Math.Clamp(origin.X + WorldGen.genRand.Next(-radius, radius), 40, Main.maxTilesX - (40 + tileData.Width));
                int targetY = origin.Y;
                placementPoint = new Point(targetX, targetY);

            } while (attempts++ < maxAttempts);

            WorldGenUtils.AddProtectedStructure(new Rectangle(origin.X, origin.Y, tileData.Width, tileData.Height), 5);

            //this handles generating the actual tiles, but you still need to add things like treegen etc. I know next to nothing about treegen so you're on your own there, lol.
            Dictionary<Color, int> colorToTile = new Dictionary<Color, int>();
            colorToTile[new Color(0, 255, 0)] = ModContent.TileType<FulguritePlating_Tile>();
            colorToTile[new Color(255, 0, 0)] = ModContent.TileType<FulguriteBrick_Tile>();
            colorToTile[new Color(0, 0, 255)] = ModContent.TileType<StormCloud_Tile>();
            colorToTile[new Color(255, 0, 255)] = ModContent.TileType<FulgurGlass_Tile>();
            colorToTile[new Color(0, 255, 255)] = ModContent.TileType<FulguritePlatingPlatform_Tile>();
            colorToTile[new Color(150, 150, 150)] = -2; //turn into air
            colorToTile[Color.Black] = -1; //don't touch when genning		

            Dictionary<Color, int> colorToWall = new Dictionary<Color, int>();
            colorToWall[new Color(0, 255, 0)] = ModContent.WallType<FulguritePlating_Wall>();
            colorToWall[new Color(255, 0, 255)] = ModContent.WallType<FulgurGlass_Wall>();
            colorToWall[new Color(0, 0, 255)] = ModContent.WallType<FulguriteBrick_Wall>();
            colorToWall[Color.Black] = -1; //don't touch when genning

            HashSet<int> protectedTiles = [
                ModContent.TileType<FulguritePlating_Tile>(),
                ModContent.TileType<FulguriteBrick_Tile>(),
                ModContent.TileType<StormCloud_Tile>(),
                ModContent.TileType<FulgurGlass_Tile>(),
                ModContent.TileType<FulguritePlatingPlatform_Tile>()
            ];

            HashSet<int> protectedWalls = [
                ModContent.WallType<FulguritePlating_Wall>(),
                ModContent.WallType<FulgurGlass_Wall>(),
                ModContent.WallType<FulguriteBrick_Wall>()
            ];

            TexGen gen = TexGen.GetTexGenerator(tileData, colorToTile, wallData, colorToWall, unbreakableTiles: protectedTiles, unbreakableWalls: protectedWalls);

            gen.Generate(placementPoint.X, placementPoint.Y, true, true);

            if(isSmallWorld)
            {
                WorldGen.PlaceObject(placementPoint.X + 37, placementPoint.Y + 47, (ushort)ModContent.TileType<AncientDataBank_Tile>());
                WorldGen.PlaceChest(placementPoint.X + 32, placementPoint.Y + 47, (ushort)ModContent.TileType<StormChest_Tile>());
                WorldGen.PlaceChest(placementPoint.X + 41, placementPoint.Y + 47, (ushort)ModContent.TileType<StormChest_Tile>());
            }
            else
            {
                WorldGen.PlaceObject(placementPoint.X + 174, placementPoint.Y + 59, (ushort)ModContent.TileType<AncientDataBank_Tile>());
                WorldGen.PlaceChest(placementPoint.X + 168, placementPoint.Y + 59, (ushort)ModContent.TileType<StormChest_Tile>());
                WorldGen.PlaceChest(placementPoint.X + 179, placementPoint.Y + 59, (ushort)ModContent.TileType<StormChest_Tile>());
            }
            
            return true;
        }
    }
}