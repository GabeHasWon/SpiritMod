using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace SpiritMod.Items.Sets.SpiritSet
{
	public class SpiritGun : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spirit Burst");
			// Tooltip.SetDefault("Turns regular bullets into Spirit Bullets");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Sets/SpiritSet/SpiritGun_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 29;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 60;
			Item.height = 32;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 1;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 1, 08, 0);
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.Item36;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SpiritBullet>();
			Item.shootSpeed = 16f;
			Item.useAmmo = AmmoID.Bullet;

		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.06f, .16f, .22f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (type == ProjectileID.Bullet)
				type = ModContent.ProjectileType<SpiritBullet>();

			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;

			float spread = 15 * 0.0174f;//45 degrees converted to radians
			float baseSpeed = velocity.Length();
			double baseAngle = Math.Atan2(velocity.X, velocity.Y);
			double randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
			velocity.X = baseSpeed * (float)Math.Sin(randomAngle);
			velocity.Y = baseSpeed * (float)Math.Cos(randomAngle);
		}
		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<SpiritBar>(), 16);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}