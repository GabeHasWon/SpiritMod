using SpiritMod.Skies.Overlays;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture.AuroraMonoliths
{
	public class BloodMoonAuroraMonolith : AuroraMonolith
	{
        internal override int DropType => ModContent.ItemType<BloodMoonAuroraMonolithItem>();
        internal override int AuroraType => AuroraOverlay.BLOODMOON;
    }

	public class BloodMoonAuroraMonolithItem : AuroraMonolithItem
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Blood Moon Aurora Monolith");
		public override int PlaceType => ModContent.TileType<BloodMoonAuroraMonolith>();
	}
}