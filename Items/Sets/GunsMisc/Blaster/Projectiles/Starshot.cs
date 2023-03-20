using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public class Starshot : SubtypeProj, ITrailProjectile
	{
		private const int timeLeftMax = 80;

		public void DoTrailCreation(TrailManager tManager)
		{
			Color color = Color.Orange;

			//Manually get the weapon's element color because Subtype is assigned to after DoTrailCreation is called
			Item heldItem = Main.player[Projectile.owner].HeldItem;
			if (heldItem.ModItem is Blaster)
				color = GetColor((heldItem.ModItem as Blaster).element);

			tManager.CreateTrail(Projectile, new StandardColorTrail(color), new RoundCap(), new DefaultTrailPosition(), 12f, 100f, new DefaultShader());
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starshot");
			Main.projFrames[Projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.alpha = 255;
			Projectile.timeLeft = timeLeftMax;
			Projectile.height = 8;
			Projectile.width = 8;
			AIType = ProjectileID.Bullet;
		}

		public override void AI()
		{
			Projectile.velocity *= 0.97f;
			Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X < 0) ? -1 : 1;
			Projectile.rotation += 0.1f * Projectile.direction;

			if (Projectile.alpha > 0)
				Projectile.alpha -= 255 / 20;
			if (Projectile.alpha < 0)
				Projectile.alpha = 0;

			if (Projectile.timeLeft <= (timeLeftMax / 8))
				Projectile.scale -= 0.1f;
			else
				Projectile.scale += 0.005f;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.velocity = -oldVelocity;
			return false;
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
			for (int i = 0; i < 12; i++)
			{
				Vector2 velocity = (new Vector2(Projectile.velocity.X, 0) * Main.rand.NextFloat(0.3f, 0.5f)).RotatedByRandom(MathHelper.TwoPi);
				if (timeLeft <= 0)
					velocity = (Projectile.velocity * Main.rand.NextFloat(0.6f, 1.0f)).RotatedByRandom(0.11f);

				int[] dustType = Dusts;
				Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, dustType[Main.rand.Next(dustType.Length)],
					velocity, 0, default, Main.rand.NextFloat(1.0f, 1.2f));
				dust.noGravity = true;
			}
		}

		public override Color? GetAlpha(Color lightColor) => GetColor(Subtype);

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, texture.Height / Main.projFrames[Type]);

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] + (Projectile.Size / 2) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
				Color color = (Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * .5f) with { A = 0 };
				float scale = Projectile.scale - (k / Projectile.oldPos.Length * .5f);

				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, frame, color, Projectile.rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
			}
			return false;
		}
	}
}
