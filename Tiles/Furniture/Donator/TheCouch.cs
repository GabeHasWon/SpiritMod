using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Donator
{
	public class TheCouch : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2); //this style already takes care of direction for us
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.Direction = Terraria.Enums.TileObjectDirection.None;
			TileObjectData.addTile(Type);

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("The Couch");
			AddMapEntry(new Color(200, 200, 200), name);
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.CanBeSleptIn[Type] = true;
			TileID.Sets.InteractibleByNPCs[Type] = true;
			TileID.Sets.IsValidSpawnPoint[Type] = true;
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (closer) {
				Player player = Main.LocalPlayer;
				if (!player.dead) {
					player.AddBuff(ModContent.BuffType<CouchPotato>(), 60, true);
				}
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;
	}
}