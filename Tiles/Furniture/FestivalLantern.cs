using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture
{
	[Sacrifice(1)]
    public class FestivalLanternItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 22;
			Item.value = Item.sellPrice(0, 0, 40, 0);
			Item.value = Item.buyPrice(gold: 2);
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<FestivalLanternTile>();
		}

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.ChineseLantern, 1);
			recipe.AddIngredient(ItemID.IronBar, 1);
			recipe.AddTile(ModContent.TileType<ForagerTableTile>());
			recipe.Register();

			Recipe recipe1 = CreateRecipe(1);
			recipe1.AddIngredient(ItemID.ChineseLantern, 1);
			recipe1.AddIngredient(ItemID.LeadBar, 1);
			recipe1.AddTile(ModContent.TileType<ForagerTableTile>());
			recipe1.Register();
		}
	}
	public class FestivalLanternTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = default(AnchorData);
			TileObjectData.addTile(Type);
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
			DustType = -1;//ModContent.DustType<Pixel>();
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Festival Lantern");
            AddMapEntry(new Color(100, 100, 100), name);
            AdjTiles = new int[] { TileID.Torches };
            DustType = -1;
        }


        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = .255f * 1.5f;
            g = .243f * 1.5f;
            b = .074f * 1.5f;
        }
        public override void NearbyEffects(int i, int j, bool closer)
		{
			MyPlayer modPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
			modPlayer.ZoneLantern = true;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen) {
				zero = Vector2.Zero;
			}
			int height = tile.TileFrameY == 36 ? 18 : 16;
			spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Furniture/FestivalLantern_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}