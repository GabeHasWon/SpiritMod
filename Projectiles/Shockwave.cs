using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class Shockwave : ModProjectile
	{
		bool boom = false;
		private readonly float distortStrength = 300f;

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Shockwave");

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = false;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 35;
			Projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			if (!boom) {
				if (Main.netMode != NetmodeID.Server && !Filters.Scene["Shockwave"].IsActive()) {
					Filters.Scene.Activate("Shockwave", Projectile.Center).GetShader().UseColor(10, 5, 15).UseTargetPosition(Projectile.Center);
				}
				boom = true;
			}
			if (Main.netMode != NetmodeID.Server && Filters.Scene["Shockwave"].IsActive()) {
				float progress = (35f - Projectile.timeLeft) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
				Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
			}
			return false;
		}

		public override void OnKill(int timeLeft)
		{
			if (Main.netMode != NetmodeID.Server && Filters.Scene["Shockwave"].IsActive()) {
				Filters.Scene["Shockwave"].Deactivate();
			}
		}
	}
}
