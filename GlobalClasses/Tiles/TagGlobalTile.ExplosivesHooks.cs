using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.Tiles;

public partial class TagGlobalTile : GlobalTile
{
	public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
	{
		Tile tileAbove = Framing.GetTileSafely(i, j - 1);

		if (_indestructibles.Contains(type) || _indestructibles.Contains(tileAbove.TileType)) //Check for indestructibles
			return false;
		if (_indestructiblesUngrounded.Contains(type)) //Check for floating indesctructibles
			return false;
		return true;
	}

	public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
	{
		Tile tileAbove = Framing.GetTileSafely(i, j - 1);

		if (_indestructibles.Contains(tileAbove.TileType) && type != tileAbove.TileType && TileID.Sets.Falling[type])
			return false;
		if (_indestructiblesUngrounded.Contains(type))
			return false;
		return true;
	}

	public override bool PreHitWire(int i, int j, int type)
	{
		Tile tileAbove = Framing.GetTileSafely(i, j - 1);

		if (_indestructibles.Contains(tileAbove.TileType) && type != tileAbove.TileType)
		{
			Tile tile = Main.tile[i, j];
			tile.IsActuated = false;
		}
		return true;
	}

	public override bool Slope(int i, int j, int type) => !_indestructibles.Contains(type);

	public override bool CanExplode(int i, int j, int type)
	{
		Tile tileAbove = Framing.GetTileSafely(i, j - 1);

		if (_indestructibles.Contains(tileAbove.TileType) || _indestructibles.Contains(type))
			return false;
		if (_indestructiblesUngrounded.Contains(type))
			return false;
		return true;
	}
}
