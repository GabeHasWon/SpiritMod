using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using SpiritMod.Items.Placeable.Tiles;
using Terraria.ObjectData;
using Terraria.GameContent.ObjectInteractions;
using Terraria.DataStructures;

namespace SpiritMod.Tiles.Furniture
{
	public class GlowplateChair : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.CanBeSatOnForNPCs[Type] = true;
			TileID.Sets.CanBeSatOnForPlayers[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
			TileObjectData.newTile.StyleMultiplier = 2; //same as above
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight; //allows me to place example chairs facing the same way as the player
			TileObjectData.addAlternate(1); //facing right will use the second texture style
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
			AddMapEntry(new Color(50, 50, 50), Language.GetText("MapObject.Chair"));
			TileID.Sets.DisableSmartCursor[Type] = true;
			DustType = -1;
			AdjTiles = new int[] { TileID.Chairs };
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Furniture/GlowplateChair_Glowmask").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), new Color(150, 150, 150, 100), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => FurnitureHelper.HasSmartInteract(i, j, settings);
		public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info) => FurnitureHelper.ModifySittingTargetInfo(i, j, ref info);
		public override bool RightClick(int i, int j) => FurnitureHelper.RightClick(i, j);
		public override void MouseOver(int i, int j) => FurnitureHelper.MouseOver(i, j, ModContent.ItemType<GlowplateChairItem>());
	}

	[Sacrifice(1)]
	public class GlowplateChairItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 28;
			Item.value = Item.value = Item.buyPrice(0, 0, 5, 0);
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<GlowplateChair>();
		}

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<TechBlockItem>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}