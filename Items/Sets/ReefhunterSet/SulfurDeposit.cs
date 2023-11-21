using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ReefhunterSet
{
	public class SulfurDeposit : ModItem
	{
		private int subID = -1; //Controls the in-world sprite for this item

		public override void SetDefaults()
		{
			Item.value = 100;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Blue;
			Item.width = 28;
			Item.height = 26;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (subID == -1)
				subID = Main.rand.Next(4);

			Texture2D tex = ModContent.Request<Texture2D>(Texture + "_World").Value;
			spriteBatch.Draw(tex, Item.position - Main.screenPosition, new Rectangle(0, 28 * subID, 28, 26), GetAlpha(lightColor) ?? lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
			return false;
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			subID = -1;
			return true;
		}

		public override void AddRecipes()
		{
			var recipe = Recipe.Create(ItemID.Grenade, 5);
			recipe.AddIngredient(this, 5);
			recipe.AddRecipeGroup("SpiritMod:CopperBars", 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();

			recipe = Recipe.Create(ItemID.Bomb, 3);
			recipe.AddIngredient(this, 3);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();

			recipe = Recipe.Create(ItemID.Dynamite, 3);
			recipe.AddIngredient(this, 6);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 2);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
