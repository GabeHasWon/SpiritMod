using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GraniteSet
{
	public class GraniteOre : ModTile
	{
		private const int minPick = 65;
		private readonly bool canKill = NPC.downedBoss2;

		public override void SetStaticDefaults()
		{
			Main.tileSpelunker[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = false;
			Main.tileLighted[Type] = false;
			ItemDrop = ModContent.ItemType<GraniteChunk>();
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Enchanted Granite Chunk");
			AddMapEntry(new Color(30, 144, 255), name);
			HitSound = SoundID.Tink;
			MinPick = minPick;
			DustType = DustID.Electric;
		}

		public override bool CanExplode(int i, int j) => false;

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => canKill;

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (Main.rand.NextBool(255))
				Dust.NewDustPerfect(new Vector2(i * 16, j * 16), 226, new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-1.5f, 1.5f)), 0, default, 0.8f).noGravity = false;

			MinPick = canKill ? minPick : 10000; //Blockswap precaution
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.155f / 2;
			g = 0.215f / 2;
			b = .4375f / 2;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			var tex = ModContent.Request<Texture2D>(Texture + "_Glow", ReLogic.Content.AssetRequestMode.AsyncLoad).Value;
			Color colour = Color.White * 0.5f;
			GTile.DrawSlopedGlowMask(i, j, tex, colour, Vector2.Zero, false);
		}
	}
}