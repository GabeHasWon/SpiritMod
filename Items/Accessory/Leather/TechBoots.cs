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
			if (!hideVisual)
			{
				int chance = (int)(10 - (10f - (int)(player.statLife / (float)(player.statLifeMax2 / 10f))));
				if (player.velocity.X != 0 && Main.rand.NextBool(System.Math.Max(chance, 1)))
				{
					Dust dust = Dust.NewDustDirect(player.position + new Vector2(0, player.height - 4), player.width, 0, DustID.Electric);
					dust.velocity = Vector2.Zero;
					dust.noGravity = true;
				}
			}
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) =>
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<LeatherBoots>(), 1);
			recipe.AddIngredient(ModContent.ItemType<TechDrive>(), 5);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

		private class TechBootsPlayer : ModPlayer
		{
			public bool hasBoots = false;

			public override void ResetEffects() => hasBoots = false;

			public override void PostUpdateRunSpeeds()
			{
				if (hasBoots)
				{
					float speedBonus = (10f - (int)(Player.statLife / (float)(Player.statLifeMax2 / 10f))) * .05f;
					float accelBonus = (10f - (int)(Player.statLife / (float)(Player.statLifeMax2 / 10f))) * .15f;

					Player.maxRunSpeed *= speedBonus + 1.1f;
					Player.accRunSpeed *= speedBonus + 1.05f;
					Player.runAcceleration *= accelBonus + 1.25f;
				}
			}
		}
	}
}
