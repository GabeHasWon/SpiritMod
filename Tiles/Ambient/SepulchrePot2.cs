using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Ammo.Arrow;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient
{
	public class SepulchrePot2 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSpelunker[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.addTile(Type);

			DustType = DustID.Wraith;
			AddMapEntry(new Color(100, 100, 100), Language.GetText("MapObject.Pot"));
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
			=> offsetY = 2;

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = .05f;
			g = .120f;
			b = .061f;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
				zero = Vector2.Zero;

			int height = tile.TileFrameY == 36 ? 18 : 16;
			spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Ambient/SepulchrePot2_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			int potionitem = Main.rand.Next(new int[] { 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305 });
			if (Main.rand.NextBool(10))
				yield return new Item(potionitem) { stack = Main.rand.Next(1, 3) };

			int torchItem = Main.rand.Next(new int[] { 282, ItemID.CursedTorch });
			int ammoItem = Main.rand.Next(new int[] { ModContent.ItemType<SepulchreArrow>(), ItemID.WoodenArrow });
			int item = 0;
			int coins = ItemID.SilverCoin;
			int num = 0;
			switch (Main.rand.Next(5))
			{
				case 0:
					item = torchItem;
					num = Main.rand.Next(2, 10);
					break;
				case 1:
					item = ammoItem;
					num = Main.rand.Next(25, 50);
					break;
				case 2:
					item = 28;
					num = Main.rand.Next(1, 3);
					break;
				case 3:
					item = coins;
					num = Main.rand.Next(1, 3);
					break;
				case 4:
					item = ammoItem;
					num = Main.rand.Next(15, 20);
					break;
			}

			yield return new Item(item) { stack = num };
			yield return new Item(ammoItem) { stack = Main.rand.Next(10, 15) };
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			SoundEngine.PlaySound(SoundID.Shatter, new Vector2(i, j) * 16);

			for (int k = 0; k < 8; k++) {
				Dust.NewDust(new Vector2(i * 16, j * 16 - 10), 54, 16, DustID.Dirt, 0.0f, -1, 0, new Color(), 0.5f);//Leave this line how it is, it uses int division
				Dust.NewDust(new Vector2(i * 16, j * 16 - 10), 75, 16, DustID.Dirt, 0.0f, 0, 0, new Color(), 0.5f);//Leave this line how it is, it uses int division		
				
				if (Main.netMode != NetmodeID.Server)
					Gore.NewGore(new Terraria.DataStructures.EntitySource_TileBreak(i, j), new Vector2((int)i * 16 + Main.rand.Next(-10, 10), (int)j * 16 + Main.rand.Next(-10, 10)), new Vector2(-1, 1), Mod.Find<ModGore>("Pot1").Type, 1f);
			}
		}
	}
}