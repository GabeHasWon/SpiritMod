using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.Tiles;
using SpiritMod.Items.Placeable;
using SpiritMod.NPCs.Pagoda.SamuraiGhost;
using SpiritMod.NPCs.Pagoda.Yuurei;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles
{
	public class SamuraiStatue : ModTile
	{

		public readonly int drawOffsetY = 2;

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(75, 139, 166));
			DustType = DustID.Stone;
			TileID.Sets.DisableSmartCursor[Type] = true;
			AdjTiles = new int[] { TileID.LunarMonolith };
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = drawOffsetY;

		public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<SamuraiStatueItem>());

		public override bool RightClick(int i, int j)
		{
			SoundEngine.PlaySound(SoundID.Mech, new(i * 16, j * 16));
			HitWire(i, j);
			return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<SamuraiStatueItem>();
		}

		public override void HitWire(int i, int j)
		{
			int x = i - Main.tile[i, j].TileFrameX / 18 % 2;
			int y = j - Main.tile[i, j].TileFrameY / 18 % 3;
			for (int l = x; l < x + 2; l++)
			{
				for (int m = y; m < y + 3; m++)
				{
					Tile tile = Main.tile[l, m];
					if (tile.HasTile)
					{
						if (tile.TileType == ModContent.TileType<SamuraiStatueActive>())
							tile.TileType = (ushort)ModContent.TileType<SamuraiStatue>();
						else
							tile.TileType = (ushort)ModContent.TileType<SamuraiStatueActive>();
					}
				}
			}
			if (Wiring.running)
			{
				Wiring.SkipWire(x, y);
				Wiring.SkipWire(x, y + 1);
				Wiring.SkipWire(x, y + 2);
				Wiring.SkipWire(x + 1, y);
				Wiring.SkipWire(x + 1, y + 1);
				Wiring.SkipWire(x + 1, y + 2);
			}
			NetMessage.SendTileSquare(-1, x, y + 1, 3);
		}
	}

	public class SamuraiStatueActive : SamuraiStatue
	{
		public override string Texture => $"SpiritMod/Tiles/{nameof(SamuraiStatue)}";

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (closer)
			{
				Player player = Main.player[Main.myPlayer];
				if (!player.dead)
					player.AddBuff(ModContent.BuffType<PagodaCurse>(), 8);
			}

			if (Main.rand.NextBool(1200) && NPC.CountNPCS(ModContent.NPCType<SamuraiPassive>()) + NPC.CountNPCS(ModContent.NPCType<PagodaGhostPassive>()) < 20)
			{
				Vector2 pos = new Vector2(i, j).ToWorldCoordinates() + new Vector2(Main.rand.NextFloat(350, 1000), 0).RotatedByRandom(MathHelper.TwoPi);

				while (Main.player.Take(Main.maxPlayers).Any(x => x.active && !x.dead && x.DistanceSQ(pos) < 200 * 200))
					pos = new Vector2(i, j).ToWorldCoordinates() + new Vector2(Main.rand.NextFloat(350, 1000), 0).RotatedByRandom(MathHelper.TwoPi);

				int type = Main.rand.NextBool() ? ModContent.NPCType<SamuraiPassive>() : ModContent.NPCType<PagodaGhostPassive>();
				NPC.NewNPC(new EntitySource_TileUpdate(i, j), (int)pos.X, (int)pos.Y, type);

				for (int v = 0; v < 6; ++v)
					Gore.NewGore(new EntitySource_TileUpdate(i, j), pos, Vector2.Zero, 99);
			} 
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

			if (tile.TileFrameX % 36 == 0 && tile.TileFrameY == 0)
			{
				Color color = Color.White;

				for (int o = 0; o < 3; o++)
				{
					Vector2 offset = o switch
					{
						0 => Vector2.Zero,
						1 => Vector2.UnitX * (float)Math.Sin(Main.GlobalTimeWrappedHourly) * -4f,
						_ => Vector2.UnitX * (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 4f
					};
					if (offset != Vector2.Zero)
						color = Color.White * .4f;
					offset += Main.rand.NextVector2Square(-0.25f, 0.25f);

					Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Back").Value, new Vector2(i * 16 - (int)Main.screenPosition.X - 2, j * 16 - (int)Main.screenPosition.Y - 2 + drawOffsetY) + zero + offset, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				}
			}
			return true;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			base.PostDraw(i, j, spriteBatch);

			Tile tile = Framing.GetTileSafely(i, j);

			if (tile.TileFrameX % 36 == 0 && tile.TileFrameY == 0 && Main.rand.NextBool(20))
			{
				int index3 = Dust.NewDust(new Vector2(i * 16, (float)(j * 16) + 48), 32, 0, DustID.Ash, 0.0f, 0f, 150, new Color(), Main.rand.NextFloat(0.5f, 1.0f));
				Main.dust[index3].fadeIn = 1.2f;
				Main.dust[index3].velocity = new Vector2(0, Main.rand.Next(-2, -1));
				Main.dust[index3].noLight = true;
				Main.dust[index3].noGravity = true;
			}

			float colorMod = MathHelper.Lerp(0f, 0.7f, (float)((Math.Sin(SpiritMod.GlobalNoise.Noise(i * 0.2f, j * 0.2f) * 3f + Main.GlobalTimeWrappedHourly * 1.3f) + 1f) * 0.5f));
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

			spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Shade").Value, new Vector2(i * 16, (float)(j * 16) + drawOffsetY) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Lighting.GetColor(i, j) * colorMod);
			for (int o = 0; o < 2; o++)
			{
				Vector2 offset = (Vector2.UnitX * (float)Math.Sin(Main.GlobalTimeWrappedHourly)) + Main.rand.NextVector2Square(-0.5f, 0.5f);
				spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_ShadeGlow").Value, new Vector2(i * 16, (float)(j * 16) + drawOffsetY) - Main.screenPosition + zero - offset, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * colorMod);
			}
		}
	}
}