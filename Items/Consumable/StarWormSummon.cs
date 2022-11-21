using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Material;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	public class StarWormSummon : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starplate Beacon");
			Tooltip.SetDefault("Non-consumable\nUse at an Astralite Beacon to summon the Starplate Voyager");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Consumable/StarWormSummon_Glow");
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.LightRed;
			Item.maxStack = 1;
			Item.noMelee = true;
			Item.consumable = false;
			Item.autoReuse = false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<StarEnergy>(), 2);
			recipe.AddIngredient(ItemID.Wire, 10);
            recipe.AddIngredient(ItemID.FallenStar, 4);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
