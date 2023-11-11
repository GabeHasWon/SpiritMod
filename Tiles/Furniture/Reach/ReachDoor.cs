using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture.Reach;
using SpiritMod.Tiles.BaseTile;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture.Reach;

public class ReachDoorOpen : BaseDoorOpen
{
	public override int Item => ModContent.ItemType<ReachDoorItem>();

	public override int CloseID => ModContent.TileType<ReachDoorClosed>();

	public override void StaticDefaults(LocalizedText defaultName) => AddMapEntry(new Color(179, 146, 107), defaultName);
}

public class ReachDoorClosed : BaseDoorClosed
{
	public override int Item => ModContent.ItemType<ReachDoorItem>();

	public override int OpenID => ModContent.TileType<ReachDoorOpen>();

	public override void StaticDefaults(LocalizedText defaultName) => AddMapEntry(new Color(179, 146, 107), defaultName);
}