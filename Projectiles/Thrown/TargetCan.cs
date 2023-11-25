using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Thrown
{
	public class TargetCan : ModProjectile
	{
		public bool struck = false;

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Shuriken);
			Projectile.width = Projectile.height = 25;
			Projectile.damage = 0;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 2;
		}

		public override void AI()
		{
			if (struck)
				return;

			float collisionPoint = 0f;
			Rectangle hitbox = Projectile.getRect();

			foreach(Projectile proj in Main.projectile)
				if (proj.active && proj.whoAmI != Projectile.whoAmI && proj.IsRanged() && proj.friendly && (proj.width <= 6 || proj.height <= 6) && Collision.CheckAABBvLineCollision(hitbox.TopLeft(), hitbox.Size(), proj.Center, proj.Center + (proj.velocity * (proj.extraUpdates + 1)), 10, ref collisionPoint))
				{
					ImpactFX();
					struck = true;
					Projectile.damage = 110;
					Projectile.velocity = proj.velocity * 2;
					proj.active = false;
					CombatText.NewText(hitbox, new Color(255, 155, 0, 100), Language.GetTextValue("Mods.SpiritMod.Misc.Bullseye"));
					break;
				}
		}

		public override void OnKill(int timeLeft) => ImpactFX();

		private void ImpactFX()
		{
			for (int i = 0; i < 15; i++)
				Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Tin, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default, 0.7f);
			
			SoundEngine.PlaySound(SoundID.NPCHit4 with { PitchVariance = 0.5f }, Projectile.Center);
		}
	}
}