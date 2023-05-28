using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Summon.MimeSummon
{
	public class SadSoul : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul of Sadness");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 13;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.sentry = true;
			Projectile.Size = new Vector2(20);
			Projectile.timeLeft = Projectile.SentryLifeTime;
			Projectile.friendly = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.aiStyle = -1;
		}

		public override void AI()
		{
			Projectile.alpha = (int)MathHelper.Min(200, Projectile.alpha + 5);

			if ((Counter = ++Counter % 40) == 0)
			{
				for (int i = 0; i < 2; i++)
				{
					Vector2 position = Projectile.Center + new Vector2((i > 0) ? 4 : -4, 8);
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, Vector2.UnitY * Main.rand.Next(8, 18), ModContent.ProjectileType<SadBeam>(), 13, Projectile.knockBack, Projectile.owner);
				}
			}

			if (Projectile.localAI[0] == 0f) {
				Projectile.localAI[0] = Projectile.Center.Y;
				Projectile.netUpdate = true; //localAI probably isnt affected by this... buuuut might as well play it safe
			}
			if (Projectile.Center.Y >= Projectile.localAI[0]) {
				Projectile.localAI[1] = -1f;
				Projectile.netUpdate = true;
			}
			if (Projectile.Center.Y <= Projectile.localAI[0] - 2f) {
				Projectile.localAI[1] = 1f;
				Projectile.netUpdate = true;
			}

			Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y + 0.009f * Projectile.localAI[1], -.75f, .75f);
		}

		public override Color? GetAlpha(Color lightColor) => new Color(252, 252, 252, Projectile.alpha);

		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawOrigin = TextureAssets.Projectile[Projectile.type].Value.Size() / 2;

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + (Projectile.Size / 2) + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}
