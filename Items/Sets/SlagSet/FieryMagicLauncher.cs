using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SlagSet
{
	public class FieryMagicLauncher : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slag Breath");
			Tooltip.SetDefault("Expels spurts of magical flame\nCritical hits shower enemies in damaging sparks");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Sets/SlagSet/FieryMagicLauncher_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 21;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 12;
			Item.width = 32;
			Item.height = 26;
			Item.useTime = 4;
			Item.useAnimation = 16;
			Item.reuseDelay = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 1;
			Item.value = Item.sellPrice(0, 0, 40, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item34;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<FieryFlareMagic>();
			Item.shootSpeed = 6f;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.4f, .12f, .028f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override Vector2? HoldoutOffset() => new Vector2(-5, 0);
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 5f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			if (player.wet) {
				return false;
			}
			Vector2 vel = velocity.RotatedByRandom(MathHelper.Pi / 18);
			player.itemRotation = MathHelper.WrapAngle(vel.ToRotation() - ((player.direction < 0) ? MathHelper.Pi : 0));

			for (int j = 0; j < 6; j++) {
				int dust = Dust.NewDust(position, 0, 0, DustID.Torch);
				Main.dust[dust].velocity = vel.RotatedByRandom(MathHelper.Pi / 12) * Main.rand.NextFloat(1.3f, 2.2f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].scale = Main.rand.NextFloat(1.6f, 2f);
			}
			Projectile.NewProjectile(source, position + new Vector2(-8, 8), vel, type, damage, knockback, player.whoAmI, 0f, 0f);
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CarvedRock>(), 14);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}