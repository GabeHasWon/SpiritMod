using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using System;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System.Linq;
using SpiritMod.Tiles;
using Terraria.GameContent;
using System.Collections.Generic;

namespace SpiritMod.Mechanics.Fathomless_Chest
{
	[TileTag(TileTags.Indestructible)]
	public class Fathomless_Chest : ModTile
	{
		private static float Timer => (float)Math.Sin(Main.GlobalTimeWrappedHourly) + 0.5f;

		public override void SetStaticDefaults()
		{
			Main.tileSolidTop[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 1200;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.Origin = new Point16(0, 2);
			Main.tileOreFinderPriority[Type] = 1000;
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Fathomless Vase");
			Main.tileSpelunker[Type] = true;
			AddMapEntry(new Color(112, 216, 238), name);
			TileID.Sets.DisableSmartCursor[Type] = true;
			Main.tileLighted[Type] = true;

			HitSound = SoundID.DD2_SkeletonDeath;
			ItemDrop = ModContent.ItemType<Black_Stone_Item>();
			DustType = -1;
		}
		public override bool CanExplode(int i, int j) => false;

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0f;
			g = 0.2f + (1 * (Timer / 6));
			b = 0.5f + (1 * (Timer / 9));
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

		public override bool RightClick(int i, int j)
		{
			WorldGen.KillTile(i, j, false, false, false);
			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
			return true;
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			TileUtilities.BlockActuators(i, j);
			return base.TileFrame(i, j, ref resetFrame, ref noBreak);
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.cursorItemIconID = ModContent.ItemType<Fathomless_Chest_Item>();
			player.cursorItemIconText = string.Empty;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override bool KillSound(int i, int j, bool fail) => fail;

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Player player = Main.player[Player.FindClosest(new Vector2(i * 16, j * 16), 100, 100)];

			if (player.whoAmI == Main.myPlayer)
			{
				int count = ChanceEffectManager.effectIndex.Count;
				int randomEffectCounter = Main.rand.Next(count);

				while (!ChanceEffectManager.effectIndex[randomEffectCounter].Selectable(new Point16(i, j)))
					randomEffectCounter = Main.rand.Next(count);

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.FathomlessData, 4);
					packet.Write((byte)randomEffectCounter);
					packet.Write((byte)player.whoAmI);
					packet.Write((ushort)i);
					packet.Write((ushort)j);
					packet.Send();
				}
				else //This prevents the effect from triggering twice for the specified player in multiplayer
				{
					ChanceEffectManager.effectIndex[randomEffectCounter].Trigger(player, new Point16(i, j));
				}
			}
		}

		public static void ConvertTiles(int i, int j, int size, Dictionary<int, int> pair, float density = 1f)
		{
			for (int k = i - size; k <= i + size; k++)
			{
				for (int l = j - size; l <= j + size; l++)
				{
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt(size * size + size * size))
					{
						int type = Main.tile[k, l].TileType;

						//Set up basic conversion types
						if (TileID.Sets.Conversion.Stone[type])
							type = TileID.Stone;
						if (type == 0 || type == 2)
							type = TileID.Dirt;
						if (TileID.Sets.Conversion.Ice[type])
							type = TileID.IceBlock;

						int convertTo = pair.GetValueOrDefault(type, -1); //Does type have a valid pair present in the dictionary?

						if (convertTo == -1 || Main.rand.NextFloat() <= density) //The tile isn't a match
							continue;

						Main.tile[k, l].TileType = (ushort)convertTo;
						WorldGen.SquareTileFrame(k, l, true);
						NetMessage.SendTileSquare(-1, k, l, 1);
					}
				}
			}
		}

		public static bool CheckTileRange(int i, int j, int[] tiletypes, int size)
		{
			for (int k = i - size; k <= i + size; k++)
			{
				for (int l = j - size; l <= j + size; l++)
				{
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt(size * size + size * size))
					{
						int type = Main.tile[k, l].TileType;

						if (tiletypes.Contains(type))
							return true;
					}
				}
			}
			return false;
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (tile.TileFrameX % 32 == 0 && tile.TileFrameY == 0)
			{
				Texture2D texture = TextureAssets.Extra[59].Value;
				Vector2 center = new Vector2((i * 16) - (int)Main.screenPosition.X + 16, (j * 16) - (int)Main.screenPosition.Y + 24) + zero;

				spriteBatch.Draw(texture, center, null, new Color(90, 179, 255, 0) * Timer, 0f, texture.Size() / 2, .4f, SpriteEffects.None, 0f);
				Utilities.DrawGodray.DrawGodrays(spriteBatch, center, Color.SeaGreen * Timer, 26, 18, 3);
			}

			int left = i - tile.TileFrameX / 18;
			int top = j - tile.TileFrameY / 18;
			int spawnX = left * 16;
			int spawnY = top * 16;

			if (Main.rand.NextBool(20))
			{
				Dust dust = Main.dust[Dust.NewDust(new Vector2(spawnX, spawnY), 16 * 2, 16 * 3, DustID.DungeonSpirit, 0.0f, 0.0f, 150, new Color(), 0.3f)];
				dust.fadeIn = 0.75f;
				dust.velocity *= 0.1f;
				dust.noLight = true;
				dust.noGravity = true;
			}
			return true;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

			spriteBatch.Draw(glow, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * Timer * .5f);
		}
	}

	internal class Fathomless_Chest_Item : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fathomless Vase");
			Tooltip.SetDefault("You aren't supposed to have this!");
		}

		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = 999;
			Item.useTurn = true;
			Item.value = 0;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.rare = ItemRarityID.Yellow;
			Item.createTile = ModContent.TileType<Fathomless_Chest>();
		}
	}
}