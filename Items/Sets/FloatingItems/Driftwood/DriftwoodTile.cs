using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace SpiritMod.Items.Sets.FloatingItems.Driftwood
{
	public class DriftwoodTileItem : FloatingItem
	{
		public override float SpawnWeight => 1f;
		public override float Weight => base.Weight * 0.9f;
		public override float Bouyancy => base.Bouyancy * 1.05f;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Driftwood Block");
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<DriftwoodTile>());
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.White;
			Item.maxStack = 999;
		}
	}

	public class DriftwoodTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			Main.tileMerge[Type][TileID.WoodBlock] = true;
			Main.tileMerge[TileID.WoodBlock][Type] = true;

			Main.tileMerge[Type][TileID.Sand] = true;
			Main.tileMerge[TileID.Sand][Type] = true;

			AddMapEntry(new Color(138, 79, 45));
		}

		public override bool CanExplode(int i, int j) => true;
	}
}