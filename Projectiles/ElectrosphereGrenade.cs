using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class ElectrosphereGrenade : ModProjectile
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Electrosphere Grenade");

		public override void SetDefaults()
		{
			Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 180;
			Projectile.width = 20;
			Projectile.height = 20;
		}

		public override void Kill(int timeLeft)
		{
			Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ProjectileID.Electrosphere, Projectile.damage, 0, Main.myPlayer);
			proj.friendly = true;
			proj.hostile = false;
			proj.timeLeft = 180;

			SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => Projectile.Kill();
	}
}