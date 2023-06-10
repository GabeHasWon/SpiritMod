using Mono.Cecil.Cil;
using MonoMod.Cil;
using SpiritMod.Tiles.Block;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Vanilla;

internal class VanillaAnchoringModifications : ILoadable
{
	public void Load(Mod mod) => IL.Terraria.WorldGen.CheckSunflower += WorldGen_CheckSunflower;

	private void WorldGen_CheckSunflower(ILContext il)
	{
		ILCursor c = new(il);

		if (!c.TryGotoNext(x => x.MatchCall<WorldGen>(nameof(WorldGen.SolidTile))))
			return;

		if (!c.TryGotoPrev(x => x.MatchStloc(0)))
			return;

		c.Emit(OpCodes.Pop);
		c.Emit(OpCodes.Ldloc_S, (byte)4);
		c.Emit(OpCodes.Ldloc_2);
		c.Emit(OpCodes.Ldloc_0);

		c.EmitDelegate((int i, int j, bool flag) => 
		{
			if (flag)
				return flag;

			int[] validTiles = TileObjectData.GetTileData(TileID.Sunflower, 0, 0).AnchorValidTiles;
			Tile tile = Main.tile[i, j + 4];
			return !tile.HasUnactuatedTile || !validTiles.Contains(tile.TileType);
		});
	}

	internal void ModifyTileAnchors()
	{
		ModifyTileAnchors(TileObjectData.GetTileData(TileID.Sunflower, 0, 0), false, ModContent.TileType<Stargrass>());
		ModifyTileAnchors(new int[] { TileID.BloomingHerbs, TileID.ImmatureHerbs, TileID.MatureHerbs, TileID.Plants, TileID.Plants2 }, false, ModContent.TileType<Stargrass>());
	}

	public void ModifyTileAnchors(int[] dataToGet, bool remove, params int[] modifications)
	{
		foreach (int id in dataToGet)
		{
			var data = TileObjectData.GetTileData(id, 0, 0);

			if (data is not null)
				ModifyTileAnchors(data, remove, modifications);
		}
	}

	public void ModifyTileAnchors(TileObjectData data, bool remove, params int[] modifications)
	{
		if (!remove)
		{
			var anchors = data.AnchorValidTiles.ToList();
			anchors.AddRange(modifications);
			data.AnchorValidTiles = anchors.ToArray();
		}
		else
		{
			var anchors = data.AnchorValidTiles.ToList();

			foreach (int item in modifications)
				anchors.Remove(item);

			data.AnchorValidTiles = anchors.ToArray();
		}
	}

	public void Unload()
	{
		ModifyTileAnchors(TileObjectData.GetTileData(TileID.Sunflower, 0, 0), true, ModContent.TileType<Stargrass>());
	}
}

public class AnchoringModificationsSystem : ModSystem
{
	public override void PostSetupContent() => ModContent.GetInstance<VanillaAnchoringModifications>().ModifyTileAnchors();
}