using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Ambient
{
	public class UnstableIcicleProj : ModProjectile
	{
		public int Counter { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

		private int Size => Projectile.frame % 3;


		private const int WarnTime = 12;

		public override string Texture => "SpiritMod/Tiles/Ambient/UnstableIcicle";

		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 12;
			ProjectileID.Sets.TrailCacheLength[Type] = 5;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(16);
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
		}

		public override void AI()
		{
			Projectile.height = (16 * (Size + 1)) - 6;
			Projectile.tileCollide = Counter > (WarnTime + 10); //Allow tile collision after falling for 10 ticks

			if (++Counter > WarnTime)
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.UnitY * 15f, .08f);
				Projectile.rotation = 0;
			}
			else
			{
				Projectile.rotation = (float)(Math.Sin(Counter / 2) / 8f);
				Projectile.timeLeft++;

				Dust.NewDustDirect(Projectile.position, Projectile.width, 0, DustID.Snow, 0, 1, 80, default, Main.rand.NextFloat(.7f, 1.1f));
			}
		}

		public override void OnKill(int timeLeft)
		{
			int type = (Projectile.frame / 3) switch
			{
				1 => DustID.Ice_Purple,
				2 => DustID.Ice_Red,
				3 => DustID.Ice_Pink,
				_ => DustID.Ice
			};

			for (int i = 0; i < 10 * (Size + 1); i++)
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 0, 0, 100, default, Main.rand.NextFloat(.5f, 1f));

			SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = texture.Frame(Main.projFrames[Type], 1, Projectile.frame, 0, -2);
			Vector2 drawOffset = new Vector2(Projectile.width / 2, 0);
			Vector2 origin = new Vector2(frame.Width / 2, 0);

			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
			{
				float opacityMod = (ProjectileID.Sets.TrailCacheLength[Type] - i) / (float)ProjectileID.Sets.TrailCacheLength[Type] * .5f;
				Vector2 drawPosition = Projectile.oldPos[i] + drawOffset - Main.screenPosition;

				Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor) * opacityMod, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			}

			Main.EntitySpriteDraw(texture, Projectile.position + drawOffset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
