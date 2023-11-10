using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Driftwood
{
	public class DriftwoodDoorOpen : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileLavaDeath[Type] = true;
			Main.tileNoSunLight[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.OpenDoor, 0));
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(165, 150, 0), Language.GetText("MapObject.Door"));
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

			TileID.Sets.HousingWalls[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.CloseDoorID[Type] = ModContent.TileType<DriftwoodDoorClosed>();

			AdjTiles = new int[] { TileID.OpenDoor };
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
	}
}