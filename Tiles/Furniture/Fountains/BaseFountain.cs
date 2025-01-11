using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Fountains
{
    public abstract class BaseFountain : ModTile
    {
		internal virtual int DropType => ModContent.ItemType<BriarFountainItem>();

		public sealed override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;

			TileID.Sets.InteractibleByNPCs[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Width = 5;
			TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.Origin = new Point16(2, 3);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 18];
			TileObjectData.newTile.StyleLineSkip = 1;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(75, 139, 166));
			RegisterItemDrop(DropType);

			DustType = DustID.Stone;
            AnimationFrameHeight = 72;
            AdjTiles = [TileID.WaterFountain];
        }

		public sealed override void NearbyEffects(int i, int j, bool closer)
		{
			if (Main.netMode != NetmodeID.Server && Framing.GetTileSafely(i, j).TileFrameY >= AnimationFrameHeight)
				Main.LocalPlayer.GetSpiritPlayer().fountainsActive["BRIAR"] = 4;
		}

		public sealed override void AnimateTile(ref int frame, ref int frameCounter) => frame = Main.tileFrame[TileID.WaterFountain];

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            int animate = tile.TileFrameY >= AnimationFrameHeight ? (Main.tileFrame[Type] * AnimationFrameHeight) : 0;
			Vector2 pos = new Vector2(i * 16, j * 16) + new Vector2(0, 2) - Main.screenPosition + zero;

			spriteBatch.Draw(TextureAssets.Tile[Type].Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY + animate, 16, 16), Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override bool RightClick(int i, int j)
        {
			SoundEngine.PlaySound(SoundID.Waterfall, new(i * 16, j * 16));
            //ToggleTile(i, j);
            return true;
        }

		public sealed override void HitWire(int i, int j) => ToggleTile(i, j);

		public sealed override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = DropType;
        }

		private void ToggleTile(int i, int j)
		{
			int x = i - Framing.GetTileSafely(i, j).TileFrameX / 18 % 5;
			int y = j - Framing.GetTileSafely(i, j).TileFrameY / 18 % 4;

			for (int l = x; l < x + 5; l++)
			{
				for (int m = y; m < y + 4; m++)
				{
					Tile tile = Framing.GetTileSafely(l, m);
					if (tile.HasTile && tile.TileType == Type)
					{
						if (tile.TileFrameY < 72)
							tile.TileFrameY += 72;
						else
							tile.TileFrameY -= 72;
					}

					if (Wiring.running)
						Wiring.SkipWire(l, m);
				}
			}

			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(-1, x, y, 5, 4);
		}
	}

	public abstract class BaseFountainItem : ModItem
	{
		internal virtual int PlaceType => ModContent.TileType<BriarFountain>();

		public override void SetDefaults()
		{
			Item.width = 52;
			Item.height = 58;
			Item.maxStack = Item.CommonMaxStack;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 16;
			Item.useTime = 16;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 4, 0, 0);
			Item.createTile = PlaceType;
		}
	}
}