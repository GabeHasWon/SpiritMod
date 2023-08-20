using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using SpiritMod.Dusts;

namespace SpiritMod.Items.Sets.MarbleSet
{
	public class MarbleOre : ModTile
	{
		private const int minPick = 65;

		public override void SetStaticDefaults()
		{
			Main.tileSpelunker[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileOreFinderPriority[Type] = 390;

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(227, 191, 75), name);
			HitSound = SoundID.Tink;
			DustType = DustID.GoldCoin;
		}

		public override bool CanExplode(int i, int j) => false;

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => NPC.downedBoss2;

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (Main.rand.NextBool(1100))
			{
				int glyphnum = Main.rand.Next(10);
				DustHelper.DrawDustImage(new Vector2(i * 16, j * 16), ModContent.DustType<MarbleDust>(), 0.05f, "SpiritMod/Effects/Glyphs/Glyph" + glyphnum, 1f);
			}

			MinPick = NPC.downedBoss2 ? minPick : 10000; //Blockswap precaution
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (.073f, .066f, .044f);
	}
}