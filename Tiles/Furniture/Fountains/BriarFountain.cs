using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture.Fountains;

public class BriarFountain : BaseFountain
{
	internal override int DropType => ModContent.ItemType<BriarFountainItem>();
	//internal override int WaterStyle => ModContent.GetInstance<ReachWaterStyle>().Slot;
}

[Sacrifice(1)]
public class BriarFountainItem : BaseFountainItem
{
	internal override int PlaceType => ModContent.TileType<BriarFountain>();
}