using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.Tiles.BaseTile;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture;

public class SpiritDoorOpen : BaseDoorOpen
{
	public override int Item => ModContent.ItemType<SpiritDoorItem>();

	public override int CloseID => ModContent.TileType<SpiritDoorClosed>();

	public override void StaticDefaults(LocalizedText defaultName) => AddMapEntry(new Color(0, 0, 255), defaultName);
}

public class SpiritDoorClosed : BaseDoorClosed
{
	public override int Item => ModContent.ItemType<SpiritDoorItem>();

	public override int OpenID => ModContent.TileType<SpiritDoorOpen>();

	public override void StaticDefaults(LocalizedText defaultName) => AddMapEntry(new Color(0, 0, 255), defaultName);
}