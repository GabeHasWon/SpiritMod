using Microsoft.Xna.Framework;
using SpiritMod.Items.Material;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.FloranSet
{
	public class FloranOreTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSpelunker[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			TileID.Sets.Ore[Type] = true;

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(30, 200, 25), name);
			HitSound = SoundID.Tink;
			DustType = DustID.GrassBlades;
			MinPick = 40;
		}
	}
}