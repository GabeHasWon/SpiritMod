using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture.Driftwood;
using SpiritMod.Tiles.BaseTile;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture.Driftwood;

public class DriftwoodDoorClosed : BaseDoorClosed
{
	public override int Item => ModContent.ItemType<DriftwoodDoorItem>();

	public override int OpenID => ModContent.TileType<DriftwoodDoorOpen>();

	public override void StaticDefaults(LocalizedText defaultName) => AddMapEntry(new Color(165, 150, 0), defaultName);
}

public class DriftwoodDoorOpen : BaseDoorOpen
{
	public override int Item => ModContent.ItemType<DriftwoodDoorItem>();

	public override int CloseID => ModContent.TileType<DriftwoodDoorClosed>();

	public override void StaticDefaults(LocalizedText defaultName) => AddMapEntry(new Color(165, 150, 0), defaultName);
}