using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Material
{
	public class SynthMaterial : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Discharge Tubule");
			Tooltip.SetDefault("'The colorful tubes are filled with energized gas'");
		}

		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 42;
			Item.value = 100;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = 999;
			ItemID.Sets.ItemIconPulse[Item.type] = true;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		
		public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(10);
            recipe.AddIngredient(ModContent.ItemType<Items.Sets.CoilSet.TechDrive>(), 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
