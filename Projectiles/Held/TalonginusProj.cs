using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Held
{
	public class TalonginusProj : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int lungeLength = 84;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Talonginus");
			ProjectileID.Sets.TrailCacheLength[Type] = 4;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

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
			float extraRotation = 0;

			if (Counter == halfTime) //The projectile is at the apex of its lunge
			{
				if (!Main.dedServ)
				{
					for (int i = 0; i < 3; i++)
					{
						Vector2 velocity = -(Projectile.velocity * 0.08f);

						ParticleHandler.SpawnParticle(new ImpactLine(Projectile.Center + (Projectile.velocity / 3.9f) + (Main.rand.NextVector2Unit() * 8), velocity, Color.Lerp(Color.FloralWhite, Color.Purple, Main.rand.NextFloat()) with { A = 100 }, new Vector2(.5f, Main.rand.NextFloat(0.9f, 1.5f)), 12, player)
						{
							Origin = ModContent.Request<Texture2D>(Texture).Size() / 2,
						});
					}
				}
			}
			else if (Counter > halfTime)
			{
				if (Main.rand.NextBool(4))
				{
					Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, (Projectile.velocity / 15).RotatedByRandom(0.5f), Scale: Main.rand.NextFloat(1.0f, 1.5f));
					dust.noGravity = true;
				}

				extraRotation = (float)((float)Counter - halfTime) / 35f * Projectile.direction;
				desiredVel = Vector2.Zero;
			}

			Projectile.rotation += extraRotation;
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVel, 0.25f);
			Projectile.Center = player.MountedCenter + Projectile.velocity + new Vector2(0, Projectile.height / 2 + (extraRotation * 35 * Projectile.direction));
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			SpriteEffects effects = (Projectile.direction < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			float rotation = Projectile.rotation + ((effects == SpriteEffects.FlipHorizontally) ? 0.785f : 2.355f);
			Vector2 origin = (effects == SpriteEffects.FlipHorizontally) ? new Vector2(texture.Width - (Projectile.width / 2), Projectile.height / 2) : Projectile.Size / 2;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, effects, 0);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), rotation, origin, Projectile.scale, effects, 0);

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
				if (effects == SpriteEffects.FlipHorizontally)
					drawPos.X -= texture.Width - Projectile.width;

				Color color = Projectile.GetAlpha(Color.White) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, drawPos, null, color, rotation, origin, Projectile.scale, effects, 0);
			}

			return false;
		}
	}
}