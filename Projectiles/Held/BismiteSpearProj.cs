using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.DoT;
using SpiritMod.Particles;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Held
{
	public class BismiteSpearProj : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int lungeLength = 36;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Bismite Pike");

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			player.ChangeDir(Projectile.direction);
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (!player.frozen)
				Counter++;

			if (Counter >= player.itemTimeMax || !player.active || player.dead)
				Projectile.Kill();

			int halfTime = player.itemTimeMax / 2;

			Vector2 desiredVel = new Vector2(lungeLength, 0).RotatedBy(Projectile.rotation);
			float rate = 0.5f;

			if (Counter == halfTime) //The projectile is at the apex of its lunge
			{
				if (!Main.dedServ)
				{
					for (int i = 0; i < 3; i++)
					{
						Vector2 velocity = -(Projectile.velocity * 0.08f);

						ParticleHandler.SpawnParticle(new ImpactLine(Projectile.Center - (Projectile.velocity * 1.85f) + (Main.rand.NextVector2Unit() * 8), -velocity, Color.Lerp(Color.LimeGreen, Color.Green, Main.rand.NextFloat()) with { A = 100 }, new Vector2(.5f, Main.rand.NextFloat(0.9f, 1.5f)), 12, player)
						{
							Origin = ModContent.Request<Texture2D>(Texture).Size() / 2,
						});
					}

					ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center - (Projectile.velocity / 3f), (Color.Lerp(Color.LimeGreen, Color.Green, Main.rand.NextFloat()) * 0.7f) with { A = 120 }, 40, 10)
					{
						ZRotation = 0.7f,
						Angle = Projectile.velocity.ToRotation() + MathHelper.Pi
					});
				}
			}
			else if (Counter > halfTime)
			{
				desiredVel = Vector2.Zero;
				rate = 0.1f;
			}

			for (int i = 0; i < 2; ++i)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Green, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 0f;
				Main.dust[dust].scale = 0.9f;
			}

			Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVel, rate);
			Projectile.Center = player.MountedCenter + Projectile.velocity;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.NextBool(5))
				target.AddBuff(ModContent.BuffType<FesteringWounds>(), 180);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			SpriteEffects effects = (Projectile.direction < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			float rotation = Projectile.rotation + ((effects == SpriteEffects.FlipHorizontally) ? 0.785f : 2.355f);
			Vector2 origin = (effects == SpriteEffects.FlipHorizontally) ? new Vector2(texture.Width - (Projectile.width / 2), Projectile.height / 2) : Projectile.Size / 2;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, effects, 0);

			return false;
		}
	}
}
