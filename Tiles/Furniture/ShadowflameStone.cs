using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Weapon.Magic.ShadowbreakWand;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture
{
	public class ShadowflameStone : ModTile
	{
		private static bool Looted(Tile tile) => tile.TileFrameY >= 54;

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileLighted[Type] = true;

			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Goblin Weapon Rack");
			AddMapEntry(Colors.RarityPurple, name);
			DustType = DustID.WoodFurniture;
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			if (!Looted(Framing.GetTileSafely(i, j)))
				(r, g, b) = (.51f, .25f, .72f);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			if (frameY >= 54) //This accomplishes the same thing as Looted() but is a special case
				return;
			Loot(i, j);
		}

		public override bool RightClick(int i, int j)
		{
			if (!Looted(Framing.GetTileSafely(i, j)))
			{
				Loot(i, j);
				HitWire(i, j);
			}
			return true;
		}

		private static void Loot(int i, int j)
		{
			SoundEngine.PlaySound(SoundID.Zombie7, new Vector2(i, j) * 16);

			for (int o = 0; o < 2; o++)
			{
				Vector2 randomPos = new Vector2(i * 16 + Main.rand.Next(-60, 60), j * 16);
				int npcType = NPCID.GoblinSorcerer;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					if (o == 0)
						Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 64, 32, ModContent.ItemType<ShadowflameStoneStaff>());

					int id = NPC.NewNPC(new EntitySource_TileBreak(i, j), (int)randomPos.X, (int)randomPos.Y, npcType);

					if (Main.netMode != NetmodeID.SinglePlayer)
						NetMessage.SendData(MessageID.SyncNPC, number: id);
				}
				else
				{
					if (o == 0)
						ItemUtils.NewItemWithSync(new EntitySource_TileBreak(i, j), Main.myPlayer, i * 16, j * 16, 48, 48, ModContent.ItemType<ShadowflameStoneStaff>());

					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.SpawnNPCFromClient, 3);
					packet.Write(npcType);
					packet.Write((int)randomPos.X);
					packet.Write((int)randomPos.Y);
					packet.Send();
				}
			}
		}

		public override void MouseOver(int i, int j)
		{
			if (Looted(Framing.GetTileSafely(i, j)))
				return;

			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<ShadowflameStoneStaff>();
		}

		public override void HitWire(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			int x = i - tile.TileFrameX / 18 % 3;
			int y = j - tile.TileFrameY / 18 % 3;

			if (!Looted(Framing.GetTileSafely(i, j)))
			{
				for (int l = x; l < x + 3; l++)
				{
					for (int m = y; m < y + 3; m++)
					{
						tile = Framing.GetTileSafely(l, m);
						if (tile.HasTile && tile.TileType == Type)
							tile.TileFrameY += 54;
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

				Wiring.SkipWire(x + 2, y);
				Wiring.SkipWire(x + 2, y + 1);
				Wiring.SkipWire(x + 2, y + 2);
			}
			NetMessage.SendTileSquare(-1, x, y + 1, 3);
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
				zero = Vector2.Zero;

			int height = tile.TileFrameY == 36 ? 18 : 16;
			spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Furniture/ShadowflameStone_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0);

			if (tile.TileFrameX % 54 == 0 && tile.TileFrameY == 0)
				spriteBatch.Draw(TextureAssets.Extra[60].Value, new Vector2(i * 16 - (int)Main.screenPosition.X - 42, j * 16 - (int)Main.screenPosition.Y - 36) + zero, null, new Color(169, 3, 252, 0), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		}
	}
}
