using AAModClassic.Music;
using Terraria;
using Terraria.ModLoader;

namespace SulfurAAAddon.Content.Unimush.World.Biomes
{
    public class UnimushroomBiome : ModBiome
    {
        public override string BestiaryIcon => "SulfurAAAddon/Content/Unimush/World/Biomes/UnimushroomBiome_Icon";

        public override string MapBackground => "SulfurAAAddon/Content/Unimush/World/Biomes/Backgrounds/UnimushroomMap";

        public override string BackgroundPath => "SulfurAAAddon/Content/Unimush/World/Biomes/Backgrounds/UnimushroomMap";

        public override bool IsBiomeActive(Player player)
        {
            bool active = AddonAAWorld.unimushTiles > 100;
            return active;
        }

        public override int Music => MusicManagementSystem.MusicSlots["Unimushroom_Surface"];

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<UnimushroomSurfaceBgStyle>();

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<UnimushroomUgBgStyle>();
    }

    public class UnimushroomSurfaceBgStyle : ModSurfaceBackgroundStyle
    {
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Unimush/World/Biomes/Backgrounds/UnimushroomBG3");
        }

        public override int ChooseMiddleTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Unimush/World/Biomes/Backgrounds/UnimushroomBG2");
        }

        public override int ChooseFarTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Unimush/World/Biomes/Backgrounds/UnimushroomBG1");
        }
    }

    public class UnimushroomUgBgStyle : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Unimush/World/Biomes/Backgrounds/UnimushroomUG2");
            textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Unimush/World/Biomes/Backgrounds/UnimushroomUG1");
            textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Unimush/World/Biomes/Backgrounds/UnimushroomUG4");
            textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Unimush/World/Biomes/Backgrounds/UnimushroomUG3");
        }
    }
}