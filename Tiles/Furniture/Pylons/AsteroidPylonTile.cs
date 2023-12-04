using Microsoft.Xna.Framework;
using SpiritMod.Items.ByBiome.Asteroids.Placeables.Furniture;
using SpiritMod.NPCs;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace SpiritMod.Tiles.Furniture.Pylons
{
	internal class AsteroidPylonTile : SimplePylonTile<AsteroidPylonItem>
	{
		internal override string MapKeyName => "Mods.SpiritMod.MapObject.AsteroidPylon";
		internal override Condition CanBeSold => SpiritConditions.InAsteroids;

		public override void StaticDefaults(LocalizedText name) => AddMapEntry(new Color(153, 108, 111), name);

		public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => Biomes.BiomeTileCounts.InAsteroids;
		public override bool IsSold(int npcType, Player player, bool npcHappyEnough) => npcHappyEnough && Biomes.BiomeTileCounts.InAsteroids;
	}
}