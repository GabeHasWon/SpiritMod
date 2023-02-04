using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using SpiritMod.Mechanics.Trails;

namespace SpiritMod.Items.Sets.GraniteSet.GraniteFlail
{
	public class GraniteFragment : ModProjectile, ITrailProjectile
	{
		private int Timer
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int timerMax = 100;

		public void DoTrailCreation(TrailManager tManager) 
			=> tManager.CreateTrail(Projectile, new GradientTrail(Color.Cyan, Color.Blue * .7f), new RoundCap(), new DefaultTrailPosition(), 10f, 100f, new ImageShader(Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_1").Value, Vector2.One, 0.75f, 0.1f));

		public override void SetStaticDefaults() => DisplayName.SetDefault("Granite Fragment");

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 320;
			Projectile.extraUpdates = 1;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
		{
			if (Timer < timerMax)
				Timer++;
			if (Projectile.timeLeft < 20) //Fade out
				Projectile.alpha += 255 / 20;

			if (Main.rand.NextBool(20 - (int)MathHelper.Clamp(Projectile.velocity.Length(), 0, 20)))
			{
				Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Vector2.Zero, 0, default, Main.rand.NextFloat(0.5f, 1.0f));
				dust.noGravity = true;
			}

			Projectile.velocity.Y += 0.1f;
			Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
			Projectile.rotation += Projectile.velocity.X / 12 * Projectile.direction;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.Length() > 1.5f)
			{
				for (int i = 0; i < 3; i++)
				{
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Flare_Blue);
					dust.velocity = -oldVelocity.RotatedByRandom(1f);
				}
			}
			Projectile.velocity.Y = -(oldVelocity.Y * .5f);
			Projectile.velocity.X *= 0.95f;

			return false;
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Flare_Blue);
				dust.velocity = -Projectile.velocity.RotatedByRandom(1f);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			int yFrames = 2;

			for (int i = 0; i < 2; i++)
			{
				Color color = (i > 0) ? Color.LightBlue * (float)(1f - (float)((float)Timer / timerMax)) : lightColor;
				Rectangle frame = new Rectangle(0, texture.Height / yFrames * i, texture.Width, (texture.Height / yFrames) - 2);

				Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition, frame, Projectile.GetAlpha(color), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}