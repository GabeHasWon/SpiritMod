using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.FlailsMisc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GraniteSet.GraniteFlail
{
	public class GraniteFlail : BaseFlailItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Unstable Star");
			Tooltip.SetDefault("Builds momentum as it spins\nCreates a damaging blast when colliding at full momentum");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
			ItemID.Sets.ToolTipDamageMultiplier[Type] = 2;
		}

		public override void SafeSetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.sellPrice(0, 0, 80, 0);
			Item.rare = ItemRarityID.Green;
			Item.damage = 25;
			Item.knockBack = 7;
			Item.useTime = Item.useAnimation = 41;
			Item.shoot = ModContent.ProjectileType<GraniteMaceProj>();
			Item.shootSpeed = 12f;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.08f, .12f, .52f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<GraniteChunk>(), 18);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}