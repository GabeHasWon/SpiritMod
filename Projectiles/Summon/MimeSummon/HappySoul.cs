using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Summon.MimeSummon
{
	public class HappySoul : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Soul of Happiness");
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

			if ((Counter = ++Counter % 45) == 0)
			{
				NPC target = null;
				int distance = 1000;
				foreach (NPC npc in Main.npc)
				{
					if (Projectile.Distance(npc.Center) < distance && npc.CanBeChasedBy(Projectile) && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
					{
						target = npc;
						distance = (int)Projectile.Distance(npc.Center);
					}
				}
				if (target != null)
				{
					Vector2 velocity = Projectile.DirectionTo(target.Center) * 9f;
					Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ModContent.ProjectileType<NovaBeam1>(), Projectile.damage, Projectile.knockBack / 2f, Projectile.owner, 0f, 0f).DamageType = DamageClass.Summon;
				}
			}
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
