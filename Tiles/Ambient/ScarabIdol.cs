using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Consumable.Quest;
using SpiritMod.NPCs.Boss.Scarabeus;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient
{
	public class ScarabIdol : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLighted[Type] = true;

			TileID.Sets.FramesOnKillWall[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = default;
			TileObjectData.newTile.AnchorTop = default;
			TileObjectData.newTile.AnchorWall = true;
			TileObjectData.addTile(Type);

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(245, 179, 66), name);
			RegisterItemDrop(ModContent.ItemType<ScarabIdolQuest>());
			DustType = DustID.GoldCoin;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (.245f, .179f, .066f);

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			if (!Main.dedServ)
				SoundEngine.PlaySound(SoundID.Zombie44, new Vector2(i, j) * 16);

			for (int n = 0; n < 5; n++)
			{
				int type = Main.rand.NextBool(2) ? ModContent.NPCType<Scarab>() : ModContent.NPCType<Scarab_Wall>();
				NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16 + (int)(Main.rand.NextFloat(-1.0f, 1.0f) * 30), j * 16 + (int)(Main.rand.NextFloat(-1.0f, 1.0f) * 30), type);
			}
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile t = Main.tile[i, j];
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (t.TileFrameX % 72 == 0 && t.TileFrameY == 0)
				Main.spriteBatch.Draw(TextureAssets.Extra[89].Value, new Vector2(i * 16 - (int)Main.screenPosition.X - 6, j * 16 - (int)Main.screenPosition.Y - 9) + zero, null, new Color(245, 179, 66, 100), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			return true;
		}
	}
}