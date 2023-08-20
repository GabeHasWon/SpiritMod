using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Summon.Dragon;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Summon
{
	public class JadeStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Staff of the Jade Dragon");
			// Tooltip.SetDefault("Summons two revolving ethereal dragons");
		}

		public override void SetDefaults()
		{
			Item.damage = 23;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 36;
			Item.width = 44;
			Item.height = 48;
			Item.useTime = 80;
			Item.useAnimation = 80;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.knockBack = 2.25f;
			Item.value = Item.buyPrice(gold: 10);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.NPCHit56;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<DragonHeadOne>();
			Item.shootSpeed = 6f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			//Spawn the staff visual
			Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<JadeStaffProj>(), damage, knockback, player.whoAmI);

			int dragonLength = 8;

			Vector2 offset = (Vector2.UnitX * 32).RotatedBy(velocity.ToRotation() + MathHelper.Pi);

			int latestprojectile = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DragonHeadOne>(), damage, knockback, player.whoAmI, velocity.X, velocity.Y); //bottom
			for (int i = 0; i < dragonLength; ++i)
				latestprojectile = Projectile.NewProjectile(source, position + (i * offset), Vector2.Zero, ModContent.ProjectileType<DragonBodyOne>(), damage, 0, player.whoAmI, 0, latestprojectile);
			
			Projectile.NewProjectile(source, position + (dragonLength * offset), Vector2.Zero, ModContent.ProjectileType<DragonTailOne>(), damage, 0, player.whoAmI, 0, latestprojectile);

			latestprojectile = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<DragonHeadTwo>(), damage, knockback, player.whoAmI, velocity.X, velocity.Y); //bottom
			for (int j = 0; j < dragonLength; ++j)
				latestprojectile = Projectile.NewProjectile(source, position + (j * offset), Vector2.Zero, ModContent.ProjectileType<DragonBodyTwo>(), damage, 0, player.whoAmI, 0, latestprojectile);
			
			Projectile.NewProjectile(source, position + (dragonLength * offset), Vector2.Zero, ModContent.ProjectileType<DragonTailTwo>(), damage, 0, player.whoAmI, 0, latestprojectile);
			return true;
		}
	}

	public class JadeStaffProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Staff of the Jade Dragon");
			ProjectileID.Sets.TrailCacheLength[Type] = 4;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 80;
			Projectile.friendly = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.ownerHitCheck = true;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			player.heldProj = Projectile.whoAmI;

			Projectile.timeLeft = 2;
			Projectile.Center = player.Center;
			Projectile.rotation +=  (0.02f + ((float)((float)player.itemAnimation / player.itemAnimationMax) * 0.12f)) * player.direction;

			Lighting.AddLight(Projectile.Center / 16, new Vector3(0.6f, 1.0f, 0.2f));

			for (int i = 0; i < 2; i++)
			{
				Vector2 dustPos = Projectile.Center + (Vector2.UnitX * (Projectile.Size.Length() * Main.rand.NextFloat(0.9f, 1.1f) * Projectile.scale / 2)).RotatedBy(-0.785f + (i * Math.PI) + Projectile.rotation);

				Dust dust = Dust.NewDustPerfect(dustPos, Main.rand.NextBool(2) ? DustID.AmberBolt : DustID.GemEmerald, (Vector2.UnitY * 2f).RotatedBy(Projectile.rotation), 0, default, Main.rand.NextFloat());
				dust.noGravity = true;
			}

			int fadeoutTime = 5;
			if (player.ItemAnimationEndingOrEnded)
			{
				Projectile.active = false;
				Projectile.netUpdate = true;
			}
			else if (player.itemAnimation < fadeoutTime)
			{
				Projectile.scale -= 0.05f;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
				Color trailCol = Projectile.GetAlpha(Color.White) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, drawPos, null, trailCol, Projectile.oldRot[k], texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}