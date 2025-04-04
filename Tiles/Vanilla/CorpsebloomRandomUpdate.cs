using SpiritMod.Tiles.Ambient;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Vanilla;

internal class CorpsebloomRandomUpdate : RandomUpdate
{
	public override void OnTick(int i, int j, int type)
	{
		if (type is TileID.CorruptGrass or TileID.Ebonstone && MyWorld.CorruptHazards < 20)
		{
			if (GTile.DirtAndDecor.Contains(Framing.GetTileSafely(i, j - 1).TileType) && GTile.DirtAndDecor.Contains(Framing.GetTileSafely(i, j - 2).TileType) && GTile.DirtAndDecor.Contains(Framing.GetTileSafely(i, j - 3).TileType) && (j > (int)Main.worldSurface - 100 && j < (int)Main.rockLayer - 20))
			{
				if (Main.rand.NextBool(450))
				{
					WorldGen.PlaceObject(i, j - 1, ModContent.TileType<Corpsebloom>());
					NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<Corpsebloom>(), 0, 0, -1, -1);
				}
				else if (Main.rand.NextBool(450))
				{
					WorldGen.PlaceObject(i, j - 1, ModContent.TileType<Corpsebloom1>());
					NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<Corpsebloom1>(), 0, 0, -1, -1);
				}
				else if (Main.rand.NextBool(450))
				{
					WorldGen.PlaceObject(i, j - 1, ModContent.TileType<Corpsebloom2>());
					NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<Corpsebloom2>(), 0, 0, -1, -1);
				}
			}
		}
	}
}
