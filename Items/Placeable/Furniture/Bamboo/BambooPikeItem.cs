using Microsoft.Xna.Framework;
using SpiritMod.Items.Material;
using SpiritMod.Tiles.Furniture.Bamboo;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable.Furniture.Bamboo
{
	[Sacrifice(1)]
	public class BambooPikeItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<BambooPikeTile>();
			Item.width = 36;
			Item.height = 36;
			Item.rare = ItemRarityID.White;
			Item.value = Item.sellPrice(copper: 5);
		}

		public override bool CanUseItem(Player player)
		{
			//Allow pikes to be placed when used on any tile in the stack
			Point tilePos = new Point((int)(Main.MouseWorld.X / 16), (int)(Main.MouseWorld.Y / 16));

			if (Framing.GetTileSafely(tilePos).TileType != Item.createTile) //Don't use this custom logic if possible
				return base.CanUseItem(player);

			while (Framing.GetTileSafely(tilePos).TileType == Item.createTile)
				tilePos.Y--;

			WorldGen.PlaceTile(tilePos.X, tilePos.Y, Item.createTile);
			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(-1, tilePos.X, tilePos.Y, 1);

			if (Item.stack == 1)
				Item.TurnToAir();
			else
				Item.stack--;

			return base.CanUseItem(player);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BambooBlock, 2);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}