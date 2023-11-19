using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.NPCs.ExplosiveBarrel;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.Fathomless_Chest.Entities
{
	public class Fathomless_Chest_Bomb : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int counterMax = 90;

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 48;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.timeLeft = 180;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
		}

		public override void AI()
		{
			if (++Counter >= counterMax)
			{
				Projectile.netUpdate = true;

				for (int i = 0; i < 50; i++)
				{
					Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, 3.0f), 0, default, Main.rand.NextFloat(0.5f, 1.5f));

					if (dust.scale < 1f)
						dust.fadeIn = 1.25f;
					if (Main.rand.NextBool(2))
						dust.noGravity = true;

					if (i < 3)
					{
						int type = (i > 0) ? ((i > 1) ? GoreID.Smoke1 : GoreID.Smoke2) : GoreID.Smoke3;

						Gore.NewGorePerfect(Entity.GetSource_Death(), Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, 10.0f)),
							Main.rand.NextVector2Unit(), type, Main.rand.NextFloat(1.0f, 1.5f));
					}
				}

				SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

				Projectile.NewProjectile(Entity.GetSource_Death(), Projectile.Center - new Vector2(0, 50), Vector2.Zero, ModContent.ProjectileType<BarrelExplosionLarge>(), 100, 8, Main.myPlayer);
				Projectile.active = false;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D glow = TextureAssets.Extra[59].Value;

			float quoteant = (float)((float)Counter / counterMax);

			Vector2 position = Projectile.Center - Main.screenPosition;
			float pulseTimer = (float)Math.Sin(Main.GlobalTimeWrappedHourly) + 0.5f;
			Color[] glowColor = new Color[] { Color.Lerp(Color.SeaGreen, Color.White, quoteant), Color.Lerp(new Color(90, 179, 255, 0), Color.White with { A = 0 }, quoteant) };

			Main.EntitySpriteDraw(glow, position, null, glowColor[1] * pulseTimer, 0f, glow.Size() / 2, .4f + (quoteant * .25f), SpriteEffects.None, 0);
			Utilities.DrawGodray.DrawGodrays(Main.spriteBatch, position, glowColor[0] * pulseTimer, 26, 18, 3);

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			int vFrames = 2;
			Rectangle frame = new Rectangle(0, 0, texture.Width, (texture.Height / vFrames) - 2);
			Vector2 offset = Main.rand.NextVector2Unit() * quoteant;

			for (int i = 0; i < 2; i++)
			{
				float flashTimer = (float)Math.Sin(Main.GlobalTimeWrappedHourly * (float)(Counter / 900f));
				Color color = (i > 0) ? Color.White * flashTimer : lightColor;

				if (i > 0)
					frame.Y += texture.Height / vFrames;

				Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center + offset - Main.screenPosition, frame, Projectile.GetAlpha(color),
					Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}