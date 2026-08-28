using AAModClassic;
using AAModClassic._Content.Desert.__Hardmode.Items.Quest;
using AAModClassic._Content.Hoard.World.Tiles;
using AAModClassic._Content.Parthenan.__Hardmode.Items.Weapons;
using AAModClassic._Content.Snow.__Hardmode.Items.Weapons;
using AAModClassic._Content.Underground.___PreHardmode.Items.Armor;
using AAModClassic.Base.BaseMod.Base;
using AAModClassic.Utilities;
using static AAModClassic.Utilities.WorldGenUtils;
using SulfurAAAddon.Content.Hoard.World.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace SulfurAAAddon.Content.Hoard.World.Biomes
{
    public class HoardTexGenAssets : ModSystem
    {
        //Path
        readonly string ClassicHoardPath = "AAModClassic/_Content/Hoard/World/Biomes/";
        readonly string AddonHoardPath = "SulfurAAAddon/Content/Hoard/World/Biomes/";

        //Data
        public static TexGenData HoardDeletionData;
        public static TexGenData HoardTileData;
        public static TexGenData HoardWallData;

        public static TexGenData BigHoardDeletionData;
        public static TexGenData BigHoardTileData;
        public static TexGenData BigHoardWallData;

        public override void OnModLoad()
        {
            HoardDeletionData = TexGen.GetTextureForGen(ClassicHoardPath + "GreedNestClear");
            HoardTileData = TexGen.GetTextureForGen(ClassicHoardPath + "GreedNest");
            HoardWallData = TexGen.GetTextureForGen(ClassicHoardPath + "GreedNestWalls");

            BigHoardDeletionData = TexGen.GetTextureForGen(AddonHoardPath + "BigGreedNestClear");
            BigHoardTileData = TexGen.GetTextureForGen(AddonHoardPath + "BigGreedNest");
            BigHoardWallData = TexGen.GetTextureForGen(AddonHoardPath + "BigGreedNestWalls");
        }
    }

    public class HoardGeneration : MicroBiome
    {
        private static bool ShouldAvoidLocation(Point p, bool leniant)
        {
            Tile tile = Framing.GetTileSafely(p);

            if ((!leniant && (tile.TileType == TileID.MushroomGrass || tile.TileType == TileID.JungleGrass)) ||
                tile.TileType == TileID.Sandstone ||
                tile.TileType == TileID.HardenedSand ||
                tile.TileType == TileID.SnowBlock ||
                tile.TileType == TileID.IceBlock ||
                tile.TileType == TileID.Ash ||
                tile.TileType == TileID.LihzahrdBrick ||         
                tile.TileType == TileID.BlueDungeonBrick ||
                tile.TileType == TileID.GreenDungeonBrick ||
                tile.TileType == TileID.PinkDungeonBrick)
            {
                //AAMod.instance.Logger.Info("Hoard Placement Failed, Encountered Tile of type: " + tile.TileType);
                return true;
            }

            return false;
        }

        public override bool Place(Point origin, StructureMap structures)
        {
            bool isSmallWorld = GetWorldSize() == 1;
            
            //Data to use
            TexGenData tileData = isSmallWorld ? HoardTexGenAssets.HoardTileData : HoardTexGenAssets.BigHoardTileData;
            TexGenData wallData = isSmallWorld ? HoardTexGenAssets.HoardWallData : HoardTexGenAssets.BigHoardWallData;
            TexGenData clearData = isSmallWorld ? HoardTexGenAssets.HoardDeletionData : HoardTexGenAssets.BigHoardDeletionData;

            int attempts = 0;
            int maxAttempts = 5000;
            Point placementPoint = origin;

            int assetWidth = tileData.Width;
            int assetHeight = tileData.Height;
            
            do
            {
                bool canGenerateInLocation = true;

                //AAMod.instance.Logger.Info("Attempting to Place Hoard at: " + placementPoint);

                for (int x = placementPoint.X; x < placementPoint.X + assetWidth; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + assetHeight; y++)
                    {
                        if (ShouldAvoidLocation(new Point(x, y), attempts > 2500))
                        {
                            canGenerateInLocation = false;
                            break;
                        }
                    }
                    if (!canGenerateInLocation)
                        break;
                }

                if (canGenerateInLocation && !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, assetWidth, assetHeight)))
                {
                    //AAMod.instance.Logger.Info("Hoard Placement Failed, Encountered a Pre-Existing Structure");
                    canGenerateInLocation = false;
                }

                if (canGenerateInLocation)
                {
                    AAMod.instance.Logger.Info("Hoard successfully placed after " + attempts + " attempts.");
                    origin = placementPoint;
                    break;
                }

                placementPoint = origin + new Point(WorldGen.genRand.Next(-1000, 600), WorldGen.genRand.Next(-200, 300));
            }
            while (attempts++ < maxAttempts);

            WorldGenUtils.AddProtectedStructure(new Rectangle(origin.X, origin.Y, assetWidth, assetHeight), 20);

            Dictionary<Color, int> colorToTile = new Dictionary<Color, int>
            {
                [new Color(255, 0, 0)] = -2,
                [new Color(255, 255, 255)] = -2, //turn into air
                [Color.Black] = -1 //don't touch when genning		
            };

            TexGen gen = TexGen.GetTexGenerator(clearData, colorToTile);
            gen.Generate(origin.X, origin.Y, true, true);

            colorToTile = new Dictionary<Color, int>
            {
                [new Color(255, 0, 0)] = ModContent.TileType<GreedStone_Tile>(),
                [new Color(0, 0, 255)] = ModContent.TileType<GreedBrick_Tile>(),
                [new Color(0, 255, 0)] = ModContent.TileType<HoardCovetite_Tile>(),
                [new Color(255, 0, 255)] = TileID.Chain,
                [new Color(255, 255, 255)] = -2,
                [Color.Black] = -1
            };

            Dictionary<Color, int> colorToWall = new Dictionary<Color, int>
            {
                [new Color(255, 0, 0)] = -2,
                [Color.Black] = -1
            };

            HashSet<int> protectedTiles = [
                ModContent.TileType<GreedStone_Tile>(),
                ModContent.TileType<GreedBrick_Tile>(),
            ];

            gen = TexGen.GetTexGenerator(tileData, colorToTile, wallData, colorToWall, unbreakableTiles: protectedTiles);
            gen.Generate(origin.X, origin.Y, true, true);

            WorldUtils.Gen(new Point(origin.X, origin.Y), new Shapes.Rectangle(gen.width, gen.height), Actions.Chain(new GenAction[]
            {
                new WorldGenUtils.InWorld(),
                new Actions.SetLiquid(0, 0)
            }));
            
            if(isSmallWorld)
            {
                HoardChest(origin.X + 19, origin.Y + 55);
                HoardChest(origin.X + 38, origin.Y + 67, 1);
                HoardChest(origin.X + 25, origin.Y + 34);
                HoardChest(origin.X + 41, origin.Y + 27);
                HoardChest(origin.X + 53, origin.Y + 38);
                HoardChest(origin.X + 49, origin.Y + 54);
                HoardChest(origin.X + 67, origin.Y + 70, 2);
                HoardChest(origin.X + 79, origin.Y + 61);
                HoardChest(origin.X + 72, origin.Y + 41);
                HoardChest(origin.X + 95, origin.Y + 45);
                HoardChest(origin.X + 107, origin.Y + 57);
                HoardChest(origin.X + 121, origin.Y + 33);
                HoardChest(origin.X + 131, origin.Y + 48, 3);
                HoardChest(origin.X + 130, origin.Y + 69);

                WorldGen.PlaceObject(origin.X + 80, origin.Y + 88, ModContent.TileType<GreedAltar_Tile>());
                NetMessage.SendObjectPlacement(-1, origin.X + 80, origin.Y + 88, ModContent.TileType<GreedAltar_Tile>(), 0, 0, -1, -1);
            }
            else
            {
                HoardChest(origin.X + 37, origin.Y + 49);
                HoardChest(origin.X + 52, origin.Y + 42, 1);
                HoardChest(origin.X + 152, origin.Y + 35);
                HoardChest(origin.X + 265, origin.Y + 47);
                HoardChest(origin.X + 88, origin.Y + 80);
                HoardChest(origin.X + 104, origin.Y + 99);
                HoardChest(origin.X + 114, origin.Y + 99, 2);
                HoardChest(origin.X + 170, origin.Y + 95);
                HoardChest(origin.X + 187, origin.Y + 95);
                HoardChest(origin.X + 198, origin.Y + 95);
                HoardChest(origin.X + 46, origin.Y + 165);
                HoardChest(origin.X + 148, origin.Y + 160);
                HoardChest(origin.X + 221, origin.Y + 149, 3);
                HoardChest(origin.X + 236, origin.Y + 153);

                WorldGen.PlaceObject(origin.X + 149, origin.Y + 125, ModContent.TileType<GreedAltar_Tile>());
                NetMessage.SendObjectPlacement(-1, origin.X + 149, origin.Y + 125, ModContent.TileType<GreedAltar_Tile>(), 0, 0, -1, -1);
            }

            return true;
        }

        public static void HoardChest(int x, int y, int specialItem = 0)
        {
            int PlacementSuccess = WorldGen.PlaceChest(x, y, (ushort)ModContent.TileType<GreedChest_Tile>(), false, 1);

            int[] GreedChestLoot = new int[] {

                ItemID.GoldenChair,
                ItemID.GoldenToilet,
                ItemID.GoldenDoor,
                ItemID.GoldenTable,
                ItemID.GoldenBed,
                ItemID.GoldenPiano,
                ItemID.GoldenDresser,
                ItemID.GoldenSofa,
                ItemID.GoldenSink,
                ItemID.GoldenBathtub,
                ItemID.GoldenClock,
                ItemID.GoldenLamp,
                ItemID.GoldenBookcase,
                ItemID.GoldenChandelier,
                ItemID.GoldenLantern,
                ItemID.GoldenCandelabra,
                ItemID.GoldenCandle,
                ItemID.GoldenChest,
                ItemID.GoldenWorkbench,
                ItemID.GoldWatch,
                ItemID.GoldDust,
                ItemID.AncientGoldHelmet,
                ItemID.GoldBunny,
                ItemID.GoldButterfly,
                ItemID.GoldFrog,
                ItemID.GoldGrasshopper,
                ItemID.SquirrelGold,
                ItemID.GoldBird,
                ItemID.GoldMouse,
                ItemID.GoldWorm,
                ItemID.GoldCrown,
                ItemID.GoldenKey,
                ItemID.Goldfish,
                ItemID.ReflectiveGoldDye,
                ItemID.GoldGreaves,
                ItemID.GoldHelmet,
                ItemID.FindingGold,
                ItemID.GoldChainmail,
                ItemID.GoldShortsword,
                ItemID.GoldBroadsword,
                ItemID.GoldBow,
                ItemID.GoldHammer,
                ItemID.GoldPickaxe,
                ItemID.GoldenCrate
            };

            int[] Loot = new int[]
            {
                ItemID.CoinGun,
                ItemID.Cutlass,
                ItemID.DiscountCard,
                ItemID.GoldRing,
                ItemID.LuckyCoin,
            };

            int[] Loot2 = new int[]
            {
                ModContent.ItemType<AncientGoldChestplate>(),
                ModContent.ItemType<AncientGoldLeggings>(),
            };

            if (PlacementSuccess >= 0)
            {
                Chest chest = Main.chest[PlacementSuccess];

                Item item0 = chest.item[0];
                UnifiedRandom genRand0 = WorldGen.genRand;
                int type;
                if (specialItem == 1)
                {
                    type = ModContent.ItemType<OdinsBlade>();
                }
                else if (specialItem == 2)
                {
                    type = ModContent.ItemType<RomulusTazesaber>();
                }
                else if (specialItem == 3)
                {
                    type = ModContent.ItemType<TheLifeAndEpicAdventuresOfAnubisTheWonderDog>();
                }
                else if (genRand0.Next(100) < 2f)
                {
                    type = Utils.Next(genRand0, Loot2);
                }
                else
                {
                    type = Utils.Next(genRand0, Loot);
                }

                item0.SetDefaults(type, false);

                chest.item[1].SetDefaults(ItemID.GoldBar);
                chest.item[1].stack = WorldGen.genRand.Next(70, 90);

                Item item = chest.item[2];
                item.SetDefaults(ItemID.FlaskofGold);
                chest.item[2].stack = WorldGen.genRand.Next(1, 4);

                chest.item[3].SetDefaults(ItemID.GoldCoin, false);
                chest.item[3].stack = WorldGen.genRand.Next(70, 90);

                for (int i = 0; i < 20; i++)
                {
                    chest.item[i + 4].SetDefaults(Utils.Next(WorldGen.genRand, GreedChestLoot));
                    if (chest.item[i + 4].maxStack > 1)
                    {
                        chest.item[i + 4].stack = WorldGen.genRand.Next(1, 3);
                    }
                }
            }

            NetMessage.SendObjectPlacement(-1, x, y, (ushort)ModContent.TileType<GreedChest_Tile>(), 1, 0, -1, -1);
        }
    }
}