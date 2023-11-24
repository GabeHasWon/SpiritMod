using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Thrown
{
	public class TargetBottle : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Shuriken);
			Projectile.width = Projectile.height = 25;
			Projectile.damage = 0;
			Projectile.DamageType = DamageClass.Ranged;
		}

		public override void AI()
		{
			float collisionPoint = 0f;
			Rectangle hitbox = Projectile.getRect();

			foreach (Projectile proj in Main.projectile)
				if (proj.active && proj.whoAmI != Projectile.whoAmI && proj.IsRanged() && proj.friendly && Collision.CheckAABBvLineCollision(hitbox.TopLeft(), hitbox.Size(), proj.Center, proj.Center + (proj.velocity * (proj.extraUpdates + 1)), 10, ref collisionPoint))
				{
					ImpactFX();
					Main.player[Projectile.owner].AddBuff(ModContent.BuffType<TrueMarksman>(), 210);
					Projectile.active = false;
					CombatText.NewText(hitbox, new Color(255, 155, 0, 100), "Bullseye!");
					break;
				}
		}

		public override void OnKill(int timeLeft) => ImpactFX();

		private void ImpactFX()
		{
			for (int i = 0; i < 15; i++)
				Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Glass, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default, 0.75f);

			SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
		}
	}
}