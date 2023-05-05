using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.SlagSet;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SlingHammerSubclass
{
	public class SlagHammer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slag Breaker");
			Tooltip.SetDefault("Hold down and release to throw the Hammer like a boomerang\nCan be wound up to deal increased damage");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.useStyle = 100;
			Item.width = 40;
			Item.height = 32;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item1;
			Item.DamageType = DamageClass.Melee;
			Item.channel = true;
			Item.noMelee = true;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.shootSpeed = 8f;
			Item.knockBack = 5f;
			Item.damage = 40;
			Item.value = Item.sellPrice(0, 0, 60, 0);
			Item.rare = ItemRarityID.Orange;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ModContent.ProjectileType<SlagHammerProj>();
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => 
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0 && player.ownedProjectileCounts[ModContent.ProjectileType<SlagHammerProjReturning>()] == 0;

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CarvedRock>(), 16);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}

	public class SlagHammerProj : SlingHammerProj
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slag Breaker");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
		}
		protected override int Width => 60;
		protected override int Height => 60;
		protected override int ChargeTime => 50;
		protected override float ChargeRate => 0.7f * Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee);
		protected override int ThrownProj => ModContent.ProjectileType<SlagHammerProjReturning>();
		protected override float DamageMult => 1.5f;
		protected override int ThrowSpeed => 16;

		public override void AI()
		{
			int dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Flare);
			Main.dust[dust].velocity *= -1f;
			Main.dust[dust].noGravity = true;
			Vector2 vector2_1 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
			vector2_1.Normalize();
			Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
			Main.dust[dust].velocity = vector2_2;
			vector2_2.Normalize();
			Vector2 vector2_3 = vector2_2 * 34f;
			Main.dust[dust].position = Projectile.Center - vector2_3;

			base.AI();
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.NextBool(6))
				target.AddBuff(BuffID.OnFire, 180);
		}

		public override void PostDraw(Color lightColor)
		{
			if (Projectile.ai[0] > 10)
			{
				float sineAdd = (float)Math.Sin(alphaCounter) + 2.5f;
				Main.spriteBatch.Draw(TextureAssets.Extra[49].Value, Projectile.Center - Main.screenPosition, null, new Color((int)(16.5f * sineAdd), (int)(5.5f * sineAdd), (int)(0 * sineAdd), 0), 0f, new Vector2(50, 50), 0.25f * (sineAdd + 1), SpriteEffects.None, 0f);
			}
		}

		public override void SafeModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => damage = (int)(damage * 0.75f);

		public override void ModifyHitPvp(Player target, ref int damage, ref bool crit) => damage = (int)(damage * 0.75f);
	}

	public class SlagHammerProjReturning : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slag Breaker");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 45;
			Projectile.height = 45;
			Projectile.aiStyle = 3;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 700;
			Projectile.extraUpdates = 1;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Player player = Main.player[Projectile.owner];
			if (Projectile.tileCollide)
			{
				player.GetModPlayer<MyPlayer>().Shake += 8;
				SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);
			}
			if (Main.rand.NextBool(6))
				target.AddBuff(BuffID.OnFire, 120, true);
			{
				int n = 4;
				int deviation = Main.rand.Next(0, 300);
				for (int i = 0; i < n; i++)
				{
					float rotation = MathHelper.ToRadians(270 / n * i + deviation);
					Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(rotation);
					perturbedSpeed.Normalize();
					perturbedSpeed.X *= 2.5f;
					perturbedSpeed.Y *= 2.5f;
					Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center.X, Projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ProjectileID.Spark, Projectile.damage / 2, 2, Projectile.owner);
				}
			}
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (Projectile.tileCollide)
				damage = (int)(damage * 1.5);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Player player = Main.player[Projectile.owner];
			player.GetModPlayer<MyPlayer>().Shake += 8;
			SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);
			return base.OnTileCollide(oldVelocity);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
			{
				float opacity = (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type];
				opacity *= 0.5f;
				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * opacity, Projectile.oldRot[i],
					TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}
			return true;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			width = height /= 2;
			return true;
		}

		public override void PostDraw(Color lightColor)
		{
			float sineAdd = (float)Math.Sin(alphaCounter) + 2.5f;
			Main.spriteBatch.Draw(TextureAssets.Extra[49].Value, Projectile.Center - Main.screenPosition, null, new Color((int)(16.5f * sineAdd), (int)(5.5f * sineAdd), (int)(0 * sineAdd), 0), 0f, new Vector2(50, 50), 0.25f * (sineAdd + 1), SpriteEffects.None, 0f);
		}

		float alphaCounter = 0;
		public override void AI()
		{
			alphaCounter += 0.08f;
			int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Flare);
			Main.dust[d].noGravity = true;
		}
	}
}