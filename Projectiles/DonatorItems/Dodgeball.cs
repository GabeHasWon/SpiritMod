using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.DonatorItems
{
	public class Dodgeball : ModProjectile
	{
		public bool Collided { get => Projectile.ai[0] == 1; set => Projectile.ai[0] = value ? 1 : 0; }

		private readonly int timeLeftMax = 600;

		public override string Texture => "SpiritMod/Items/DonatorItems/DodgeBall";

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 4;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Throwing;
			Projectile.penetrate = 2;
			Projectile.timeLeft = timeLeftMax;
			Projectile.alpha = 255;
			Projectile.extraUpdates = 2;
		}

		public override void AI()
		{
			if (Collided && Projectile.timeLeft < 30)
				Projectile.alpha = Math.Min(Projectile.alpha + (255 / 30), 255);
			else
				Projectile.alpha = Math.Max(Projectile.alpha - (255 / 10), 0);

			if (Projectile.timeLeft < (timeLeftMax - 60))
				Projectile.velocity.Y += .1f; //Gravity effects

			Projectile.rotation += .2f * Projectile.direction;
		}

		public override void OnKill(int timeLeft)
		{
			if (timeLeft <= 0)
				return;
			for (int i = 0; i < 5; i++)
			{
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Honey);
				Main.dust[d].scale = .5f;
			}
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Projectile.penetrate > 1)
				return;

			Collided = true;
			Projectile.penetrate++;
			Projectile.velocity = (Vector2.Normalize(Projectile.velocity) * -Main.rand.NextFloat(.5f, 1f)) - (Vector2.UnitY * Main.rand.NextFloat(3f, 5f));
			Projectile.timeLeft = 60;

			ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center, (Color.LightYellow with { A = 0 }) * .1f, 80, 5, PulseCircle.MovementType.OutwardsSquareRooted) { RingColor = Color.White * .1f });
			for (int i = 0; i < 8; i++)
			{
				Vector2 linePos = Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(25f, 80f));
				ParticleHandler.SpawnParticle(new ImpactLine(linePos, linePos.DirectionTo(Projectile.Center) * 2.5f, Color.White * .4f, new Vector2(.5f, 1f), 8));
			}
			SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Item/Dodgeball") with { PitchVariance = .5f }, Projectile.Center);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (!Collided)
			{
				Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
				SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

				Collided = true;
				Projectile.timeLeft = 60;
				Projectile.velocity = (Vector2.Normalize(oldVelocity) * -Main.rand.NextFloat(.7f, .9f)) - (Vector2.UnitY * Main.rand.NextFloat(2f, 4f));
			}
			return false;
		}

		public override bool? CanDamage() => Collided ? false : null;

		public override bool? CanCutTiles() => Collided ? false : null;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			float stretch = MathHelper.Min(Projectile.velocity.Length() * .1f, .25f);
			Vector2 scale = new Vector2(1 + stretch, 1 - stretch) * Projectile.scale;

			for (int k = 0; k < Projectile.oldPos.Length; k++) {
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + (Projectile.Size / 2) + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, texture.Size() / 2, scale, SpriteEffects.None, 0f);
			}
			return false;
		}
	}
}
