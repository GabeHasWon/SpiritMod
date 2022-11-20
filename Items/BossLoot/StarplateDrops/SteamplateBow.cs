using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Material;
using SpiritMod.Projectiles.Arrow;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.StarplateDrops
{
	public class SteamplateBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starcharger");
			Tooltip.SetDefault("Left-click to shoot Positive Arrows\nRight-click to shoot Negative Arrows\nOppositely charged arrows explode upon touching each other");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/BossLoot/StarplateDrops/SteamplateBow_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 28;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 28;
			Item.height = 36;
			Item.useTime = 26;
			Item.useAnimation = 26;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.Shuriken;
			Item.useAmmo = AmmoID.Arrow;
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item5;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.autoReuse = true;
			Item.shootSpeed = 15f;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, texture, rotation, scale);
		}
		public override Vector2? HoldoutOffset() => new Vector2(-4, 0);
		public override bool AltFunctionUse(Player player) => true;

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (type == ProjectileID.WoodenArrowFriendly && player.altFunctionUse != 2)
				type = ModContent.ProjectileType<PositiveArrow>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			if (player.altFunctionUse == 2)
				Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<NegativeArrow>(), damage, knockback, player.whoAmI);
			else
				Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<CosmiliteShard>(), 17)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}