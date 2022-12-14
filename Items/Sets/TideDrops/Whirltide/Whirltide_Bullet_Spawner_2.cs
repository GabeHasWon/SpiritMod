using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.TideDrops.Whirltide
{
	public class Whirltide_Bullet_Spawner_2 : ModProjectile
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Whirltide Bullet Spawner");

		public override void SetDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.aiStyle = 1;
			AIType = ProjectileID.WoodenArrowFriendly;
			Projectile.hide = true;
			Projectile.scale = 1f;
			Projectile.timeLeft = 60;
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;

		public override void AI()
		{
			Projectile.velocity.Y = 100f;
			Projectile.velocity.X = 0f;
		}

		public override void Kill(int timeLeft)
		{
			Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.position.X, Projectile.position.Y - 8, 0f, -2.5f, ModContent.ProjectileType<Whirltide_Water_Explosion>(), 13, 8.5f, 0);
			SoundEngine.PlaySound(SoundID.LiquidsWaterLava);
		}
	}
}