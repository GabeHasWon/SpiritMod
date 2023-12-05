using Microsoft.Xna.Framework;
using SpiritMod.Items.ByBiome.Briar.Placeable.Furniture;
using SpiritMod.NPCs;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace SpiritMod.Tiles.Furniture.Pylons
{
	internal class BriarPylonTile : SimplePylonTile<BriarPylonItem>
	{
		internal override string MapKeyName => "Mods.SpiritMod.MapObject.BriarPylon";
		internal override Condition CanBeSold => SpiritConditions.InBriar;

		public override void StaticDefaults(LocalizedText name) => AddMapEntry(new Color(217, 250, 49), name);

		public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => Biomes.BiomeTileCounts.InBriar;
		public override bool IsSold(int npcType, Player player, bool npcHappyEnough) => npcHappyEnough && Biomes.BiomeTileCounts.InBriar;
	}
}