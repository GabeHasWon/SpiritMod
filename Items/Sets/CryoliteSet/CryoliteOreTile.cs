using Microsoft.Xna.Framework;
using SpiritMod.Items.Material;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.CryoliteSet
{
	public class CryoliteOreTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSpelunker[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileMerge[Type][TileID.IceBlock] = true;
			Main.tileMerge[Type][TileID.SnowBlock] = true;
			Main.tileBlockLight[Type] = true;  //true for block to emit light
			Main.tileLighted[Type] = true;

			TileID.Sets.Ore[Type] = true;

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(40, 0, 205), name);
			HitSound = SoundID.Tink;
			MinPick = 65;
			DustType = DustID.BlueCrystalShard;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = .064f * 1.5f;
			g = .112f * 1.5f;
			b = .128f * 1.5f;
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			Player player = Main.player[Player.FindClosest(new Vector2(i, j).ToWorldCoordinates(0, 0), 16, 16)];

			if (player.inventory[player.selectedItem].type == ItemID.ReaverShark)
				return false;
			return true;
		}
		public override bool CanExplode(int i, int j) => false;
	}
}