using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using AAModClassic;
using AAModClassic._CrossMod;
using AAModClassic.Utilities;
using AAModClassic.UI.World;
using AAModClassic.Base.BaseMod.Base;
using SulfurAAAddon.Content.Unimush.World.Tiles;
using SulfurAAAddon.Content.Inferno.World.Biomes;
using SulfurAAAddon.Content.Mire.World.Biomes;
using SulfurAAAddon.Content.Hoard.World.Biomes;
using SulfurAAAddon.Content.Parthenan.World.Biomes;
using SulfurAAAddon.Content.Terrarium.World.Biomes;

namespace SulfurAAAddon
{
    public class AddonAAWorld : ModSystem
    {
        //Instance
        readonly AAWorld instance = ModContent.GetInstance<AAWorld>();

        //tile ints
        public static int unimushTiles = 0;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            unimushTiles = tileCounts[ModContent.TileType<UnifiedMycelium_Tile>() ];
        }

        //World generation
        private static int RollInfernoX(int side)
        {
            return (Main.maxTilesX >= 8000)
                ? (side == 1 ? WorldGen.genRand.Next(2000, 2300) : (Main.maxTilesX - WorldGen.genRand.Next(2000, 2300)))
                : (side == 1 ? WorldGen.genRand.Next(1500, 1700) : (Main.maxTilesX - WorldGen.genRand.Next(1500, 1700)));
        }

        private static int RollMireX(int infernoSide)
        {
            return (Main.maxTilesX >= 8000)
                ? (infernoSide != 1 ? WorldGen.genRand.Next(2000, 2300) : (Main.maxTilesX - WorldGen.genRand.Next(2000, 2300)))
                : (infernoSide != 1 ? WorldGen.genRand.Next(1500, 1700) : (Main.maxTilesX - WorldGen.genRand.Next(1500, 1700)));
        }

        private static int FindBiomeSurfaceY(int x)
        {
            int y = (int)GenVars.worldSurfaceLow - 30;
            while (Main.tile[x, y] != null && !Main.tile[x, y].HasTile)
                y++;

            for (int l = x - 25; l < x + 25; l++)
            {
                for (int m = y - 6; m < y + 90; m++)
                {
                    if (Main.tile[l, m] != null && Main.tile[l, m].HasTile)
                    {
                        int type = Main.tile[l, m].TileType;
                        if (type == TileID.Cloud || type == TileID.RainCloud || type == TileID.Sunplate)
                        {
                            y++;
                        }
                    }
                }
            }

            return y;
        }

        private static Rectangle GetInfernoFootprint(Point origin)
        {
            int worldSize = WorldGenUtils.GetWorldSize();
            int biomeRadius = worldSize == 3 ? 260 : worldSize == 2 ? 220 : 180;

            bool isSmallWorld = worldSize == 1;

            int texWidth = isSmallWorld ? InfernoTexGenAssets.VolcanoTileData.Width : InfernoTexGenAssets.BigVolcanoTileData.Width;
            int texHeight = isSmallWorld ? InfernoTexGenAssets.VolcanoTileData.Height : InfernoTexGenAssets.BigVolcanoTileData.Height;

            int volcanoOffset = isSmallWorld ? 80 : 140;

            int halfWidth = Math.Max(biomeRadius, texWidth / 2);
            int top = origin.Y - biomeRadius;
            int bottom = Math.Max(origin.Y + biomeRadius, origin.Y - volcanoOffset + texHeight);

            return new Rectangle(origin.X - halfWidth, top, halfWidth * 2, bottom - top);
        }

        private static Rectangle GetMireFootprint(Point origin)
        {
            int worldSize = WorldGenUtils.GetWorldSize();
            int biomeRadius = worldSize == 3 ? 260 : worldSize == 2 ? 220 : 180;

            bool isSmallWorld = worldSize == 1;

            int texWidth = isSmallWorld ? MireTexGenAssets.LakeTileData.Width : MireTexGenAssets.BigLakeTileData.Width;
            int texHeight = isSmallWorld ? MireTexGenAssets.LakeTileData.Height : MireTexGenAssets.BigLakeTileData.Height;

            int lakeOffset = isSmallWorld ? 30 : 60;

            int halfWidth = Math.Max(biomeRadius, texWidth / 2);
            int top = origin.Y - biomeRadius;
            int bottom = Math.Max(origin.Y + biomeRadius, origin.Y - lakeOffset + texHeight);

            return new Rectangle(origin.X - halfWidth, top, halfWidth * 2, bottom - top);
        }

        private static (Point SurfacePoint, Point PlacementOrigin) FindSafeBiomeOrigin(Func<int> rollX, Func<Point, Rectangle> getFootprint, StructureMap structures, string biomeNameForLog, int maxAttempts = 300)
        {
            Point fallbackSurface = default;
            Point fallbackOrigin = default;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                int x = rollX();
                int surfaceY = FindBiomeSurfaceY(x);
                Point surfacePoint = new Point(x, surfaceY);

                Point placementOrigin = surfacePoint;
                placementOrigin.Y = WorldGenUtils.GetFirstTileFloor(placementOrigin.X, placementOrigin.Y, true);

                if (attempt == 0)
                {
                    fallbackSurface = surfacePoint;
                    fallbackOrigin = placementOrigin;
                }

                Rectangle footprint = getFootprint(placementOrigin);

                if (structures.CanPlace(footprint, WorldGenUtils.AllTilesAllowed, 0))
                {
                    AAMod.instance.Logger.Info(biomeNameForLog + " placed successfully after " + attempt + " attempt(s).");
                    return (surfacePoint, placementOrigin);
                }
            }

            AAMod.instance.Logger.Warn(biomeNameForLog + " could not find a clear location after " + maxAttempts + " attempts; falling back to the first candidate, which may overlap another structure.");
            return (fallbackSurface, fallbackOrigin);
        }
        
        private void MireAndInferno(GenerationProgress progress)
        {
            if (ContentReplacementSystem.NeedToReplaceContent)
                return;

            instance.infernoSide = (Main.dungeonX > Main.maxTilesX / 2) ? (-1) : 1;

            progress.Message = Language.GetTextValue("Mods.AAModClassic.Common.AAWorldBuildChaos");
            progress.Message = Language.GetTextValue("Mods.AAModClassic.Common.AAWorldBuildInferno");

            var (infernoSurface, infernoOrigin) = FindSafeBiomeOrigin(() => RollInfernoX(instance.infernoSide), GetInfernoFootprint, GenVars.structures, "Inferno");

            instance.infernoPos.X = infernoSurface.X;
            instance.infernoPos.Y = infernoSurface.Y;
            instance.InfernoCenter = instance.infernoPos;

            InfernoGeneration infBiome = new();
            InfernoDelete infDelete = new();
            infDelete.Place(infernoOrigin, GenVars.structures);
            infBiome.Place(infernoOrigin, GenVars.structures);
            WorldGenUtils.AddProtectedStructure(GetInfernoFootprint(infernoOrigin), 20);

            progress.Message = Language.GetTextValue("Mods.AAModClassic.Common.AAWorldBuildMire");

            var (mireSurface, mireOrigin) = FindSafeBiomeOrigin(() => RollMireX(instance.infernoSide), GetMireFootprint, GenVars.structures, "Mire");

            instance.mirePos.X = mireSurface.X;
            instance.mirePos.Y = mireSurface.Y;
            instance.MireCenter = instance.mirePos;

            MireDelete mireDelete = new();
            MireGeneration mireBiome = new();
            mireDelete.Place(mireOrigin, GenVars.structures);
            mireBiome.Place(mireOrigin, GenVars.structures);
            WorldGenUtils.AddProtectedStructure(GetMireFootprint(mireOrigin), 20);
        }

        private static void Hoard(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.AAModClassic.Common.AAWorldBuildHoard");
            Point origin = new((int)(Main.maxTilesX * (ModLoader.HasMod("Remnants") ? 0.275f : 0.3f)), (int)(Main.maxTilesY * (ModLoader.HasMod("Remnants") ?  0.75f : 0.65f)));
            if (WorldTypeSystem.IsWorldOptionEnabled(AAWorldOption.Unreleased) && Main.dungeonX > Main.maxTilesX / 2)
                origin.X = (int)(Main.maxTilesX * (ModLoader.HasMod("Remnants") ? 0.675f : 0.7f));
            HoardGeneration biome = new();
            biome.Place(origin, GenVars.structures);
        }

        private static void ParthenanIsland(GenerationProgress progress)
        {
            progress.Message = "Storming the Parthenan";

            int ParthenanHeight = WorldGenUtils.GetWorldSize() == 1 ? 60 : ModLoader.HasMod("Remnants") ? 90 : 120;
            Point center = new Point((int)(Main.maxTilesX * 0.06f), center.Y = ParthenanHeight);
            ParthenanGen biome = new ParthenanGen();
            biome.Place(center, GenVars.structures);
        }

        private static void ReserveTerrarium(GenerationProgress progress)
        {
            if (WorldTypeSystem.IsWorldOptionEnabled(AAWorldOption.Unreleased) && !WorldTypeSystem.IsWorldOptionEnabled(AAWorldOption.Unofficial))
                return;
            progress.Message = Language.GetTextValue("Mods.AAModClassic.Common.AAWorldBuildTerrarium");
            Point origin = new((int)(Main.maxTilesX * 0.5f), (int)(Main.maxTilesY * 0.4f));

            if (ModLoader.HasMod("Spooky"))
                origin.Y += 150;

            TexGenData Terrasphere;
            if (WorldGenUtils.GetWorldSize() == 1)
                Terrasphere = TerrariumTexGenAssets.TerrariumSmallDeletionData;
            else
                Terrasphere = TerrariumTexGenAssets.TerrariumMediumDeletionData;

            WorldGenUtils.AddProtectedStructure(new Rectangle(origin.X, origin.Y, Terrasphere.Width, Terrasphere.Height), 20);

            AAWorld.terrariumCenter = origin;
        }

        private static void Terrarium(GenerationProgress progress)
        {
            if (WorldTypeSystem.IsWorldOptionEnabled(AAWorldOption.Unreleased) && !WorldTypeSystem.IsWorldOptionEnabled(AAWorldOption.Unofficial))
                return;
            progress.Message = Language.GetTextValue("Mods.AAModClassic.Common.AAWorldBuildTerrarium");

            new TerrariumDelete().Place(AAWorld.terrariumCenter, GenVars.structures);
            new TerrariumGeneration().Place(AAWorld.terrariumCenter, GenVars.structures);
        }

        //World generation task modifications
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            /*
            int TerrariumIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Terrarium"));
            if(TerrariumIndex > -1)
            {
                tasks[TerrariumIndex] = new PassLegacy("Terrarium", (progress, config) =>
                {
                    ReserveTerrarium(progress);
                });
            }
            */

            int ChaosIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Mire and Inferno"));
            if(ChaosIndex > -1)
            {
                tasks[ChaosIndex] = new PassLegacy("Mire and Inferno", (progress, config) =>
                {
                    MireAndInferno(progress);
                });
            }

            int HoardIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Hoard"));
            if(HoardIndex > -1)
            {
                tasks[HoardIndex] = new PassLegacy("Hoard", (progress, config) =>
                {
                    Hoard(progress);
                });
            }
            
            /*
            int TerrariumIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Equinox"));
            if(TerrariumIndex2 > -1)
            {
                tasks.Insert(TerrariumIndex2 + 1, new PassLegacy("Terrarium 2", delegate (GenerationProgress progress, GameConfiguration config)
                {
                    Terrarium(progress);
                }));
            }
            */

            //Unreleased
            if (WorldTypeSystem.IsWorldOptionEnabled(AAWorldOption.Unreleased))
            {
                int ParthenanIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Parthenan"));
                if(ParthenanIndex > -1)
                {
                    tasks[ParthenanIndex] = new PassLegacy("Parthenan", (progress, config) =>
                    {
                        ParthenanIsland(progress);
                    });
                }
            }
        }
    }
}