using Microsoft.Xna.Framework;
using SpiritMod.Items.ByBiome.Spirit.Placeables.Furniture;
using SpiritMod.NPCs;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace SpiritMod.Tiles.Furniture.Pylons
{
	internal class SpiritPylonTile : SimplePylonTile<SpiritPylonItem>
	{
		internal override string MapKeyName => "Mods.SpiritMod.MapObject.SpiritPylon";
		internal override Condition CanBeSold => SpiritConditions.InSpirit;

		public override void StaticDefaults(LocalizedText name) => AddMapEntry(new Color(77, 147, 255), name);

		public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => Biomes.BiomeTileCounts.InSpirit;
		public override bool IsSold(int npcType, Player player, bool npcHappyEnough) => npcHappyEnough && Biomes.BiomeTileCounts.InSpirit;
	}
}