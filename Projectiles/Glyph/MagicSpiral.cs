using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Glyph
{
	public class MagicSpiral : ModProjectile, ITrailProjectile
	{
		private readonly int timeLeftMax = 30;

		private float strength = 1f;
		private int direction = 1;

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(10);
			Projectile.penetrate = -1;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = timeLeftMax;

			direction = Main.rand.NextBool() ? -1 : 1;
			strength = Main.rand.NextFloat(.2f, 1f);
		}

		public void DoTrailCreation(TrailManager tManager)
		{
			tManager.CreateTrail(Projectile, new StandardColorTrail(Color.Cyan with { A = 0 }), new RoundCap(), new DefaultTrailPosition(), 8f, 100f);
			tManager.CreateTrail(Projectile, new StandardColorTrail(Color.White with { A = 0 }), new RoundCap(), new DefaultTrailPosition(), 2f, 80f);
		}

		public override void AI()
		{
			float rate = (Projectile.timeLeft > (timeLeftMax * .85f)) ? 0 : (1f - ((float)Projectile.timeLeft / timeLeftMax)) * strength;
			Projectile.velocity = Projectile.velocity.RotatedBy(rate * direction);
		}

		public override bool? CanDamage() => false;

		public override bool? CanCutTiles() => false;
	}
}