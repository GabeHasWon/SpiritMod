using Microsoft.Xna.Framework;
using SpiritMod.Buffs.DoT;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Yoyo
{
	public class GraspProj : ModProjectile
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.Handball.DisplayName");

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Valor);
			AIType = ProjectileID.Valor;
			Projectile.width = 16;
			Projectile.height = 18;
			Projectile.penetrate = 6;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(3))
				target.AddBuff(ModContent.BuffType<BloodCorrupt>(), 180);

			if (hit.Crit)
				target.AddBuff(BuffID.ShadowFlame, 180);
		}

		public override void AI()
		{
			Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 4;
			Projectile.velocity.X *= 1.005f;
			Dust newDust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, 0f, 0f, 0, default, 1f)];
			newDust.position = position;
			newDust.velocity = Projectile.velocity.RotatedBy(Math.PI / 2, default) * 0.33F + Projectile.velocity / 4;
			newDust.position += Projectile.velocity.RotatedBy(Math.PI / 2, default);
			newDust.fadeIn = 0.5f;
			newDust.noGravity = true;
			newDust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, 0f, 0f, 0, default, 1)];
			newDust.position = position;
			newDust.velocity = Projectile.velocity.RotatedBy(-Math.PI / 2, default) * 0.33F + Projectile.velocity / 4;
			newDust.position += Projectile.velocity.RotatedBy(-Math.PI / 2, default);
			newDust.fadeIn = 0.5F;
			newDust.noGravity = true;
		}
	}
}