using Microsoft.Xna.Framework;
using SpiritMod.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Glyph
{
	public class RadiantEnergy : ModProjectile
	{
        private int TargetWhoAmI
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

		private readonly int timeLeftMax = 20;

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(10);
			Projectile.aiStyle = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = timeLeftMax;
		}

        public override void AI()
		{
			if ((Main.npc[TargetWhoAmI] is NPC npc) && npc.active)
			{
				Projectile.Center = npc.Center;
				Projectile.gfxOffY = npc.gfxOffY;
			}
			else Projectile.Kill();
		}

		public override bool? CanCutTiles() => false;

		public override bool? CanDamage() => false;

		public override bool PreDraw(ref Color lightColor)
		{
			float quoteant = (float)Projectile.timeLeft / timeLeftMax;
			DrawGodray.DrawGodrays(Main.spriteBatch, Projectile.Center - Main.screenPosition, Color.LightYellow, 40 * quoteant, 15 * quoteant, 5);
			return false;
		}
	}
}