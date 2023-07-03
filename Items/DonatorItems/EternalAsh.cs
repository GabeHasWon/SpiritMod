using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.DonatorItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.DonatorItems
{
	public class EternalAsh : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eternal Ashes");
			Tooltip.SetDefault("Summons a Phoenix Minion to rain down fireballs on your foes ");
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.QueenSpiderStaff);
			Item.damage = 41;
			Item.mana = 15;
			Item.width = 40;
			Item.height = 40;
			Item.value = Item.sellPrice(0, 0, 70, 0);
			Item.rare = ItemRarityID.Pink;
			Item.knockBack = 3.5f;
			Item.UseSound = SoundID.Item25;
			Item.shoot = ModContent.ProjectileType<PhoenixMinion>();
			Item.shootSpeed = 0f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, Main.MouseWorld, velocity, type, damage, knockback, player.whoAmI);
			player.UpdateMaxTurrets();

			return false;
		}

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe(1);
			modRecipe.AddIngredient(ItemID.FireFeather, 1);
			modRecipe.AddIngredient(ItemID.SoulofNight, 3);
			modRecipe.AddIngredient(ItemID.PixieDust, 20);
			modRecipe.AddTile(TileID.MythrilAnvil);
			modRecipe.Register();
		}
	}
}