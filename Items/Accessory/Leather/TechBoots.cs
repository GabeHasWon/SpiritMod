using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.CoilSet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.Leather
{
	[AutoloadEquip(EquipType.Shoes)]
	public class TechBoots : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Coiled Cleats");
			Tooltip.SetDefault("Slightly increases movement speed and acceleration\nGain a huge speed boost below half health");
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 20;
			Item.value = Item.buyPrice(0, 0, 70, 0);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.runAcceleration *= 1.25f;
			player.maxRunSpeed *= 1.05f;
			player.accRunSpeed *= 1.05f;

			if (player.statLife <= player.statLifeMax2 / 2) {
				player.maxRunSpeed *= 1.15f;
				player.accRunSpeed *= 1.15f;
				player.runAcceleration *= 1.75f;

				if (player.velocity.X != 0f) {
					int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + player.height - 4f), player.width, 0, DustID.Electric);
					Main.dust[dust].velocity *= 0f;
					Main.dust[dust].noGravity = true;
				}
			}
		}
		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) =>
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<LeatherBoots>(), 1);
			recipe.AddIngredient(ModContent.ItemType<TechDrive>(), 5);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
