using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs;
using SpiritMod.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Magic
{
	public class BloodFangs : ModProjectile
	{
		private float Counter
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int counterMax = 20;

		private float CloseCounter
		{
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blood Fangs");
			Main.projFrames[Type] = 2;
			ProjectileID.Sets.TrailCacheLength[Type] = 4;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 90;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.alpha = 255;
		}

		public override void OnSpawn(IEntitySource source) => Projectile.rotation = Main.rand.NextFloat(-0.3f, 0.3f);

		public override void AI()
		{
			if ((Counter + 1) == counterMax)
			{
				SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.Center);
				CloseCounter++;
			}

			if (Counter < counterMax)
				Counter++;
			if (CloseCounter > 0)
				CloseCounter -= 0.1f;

			//Fade effects
			int fadeTime = 8;
			if (Projectile.timeLeft > fadeTime)
			{
				if (Projectile.alpha > 0)
					Projectile.alpha -= 255 / fadeTime;
			}
			else
			{
				Projectile.alpha += 255 / fadeTime;
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<NightmareDust>());

			SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);
		}

		public override bool? CanDamage() => (Counter + 1) == counterMax;

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(5))
				target.AddBuff(ModContent.BuffType<SurgingAnguish>(), 180);
		}

		public override Color? GetAlpha(Color lightColor) => new Color(160, 160, 160, 100) * Projectile.Opacity;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			int biteDist = 20;

			for (int i = 0; i < 2; i++)
			{
				float mult = (i == 0) ? -1 : 1;
				int baseOffset = (int)((4 - (float)(CloseCounter * 5f)) * mult);

				Vector2 scale = new Vector2(1 + CloseCounter, 1) * Projectile.scale;
				Rectangle frame = new Rectangle(0, texture.Height / Main.projFrames[Type] * i, texture.Width, (texture.Height / Main.projFrames[Type]) - 2);

				float quoteant = (float)Counter / counterMax;
				float offsetLength = biteDist * (1f - quoteant) * mult;
				Vector2 offset = new Vector2(0, baseOffset + offsetLength).RotatedBy(Projectile.rotation) + new Vector2(0f, Projectile.gfxOffY);

				Main.EntitySpriteDraw(texture, Projectile.Center + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, scale, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}
