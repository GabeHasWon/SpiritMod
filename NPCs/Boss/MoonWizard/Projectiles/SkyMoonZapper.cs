using Microsoft.Xna.Framework;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Boss.MoonWizard.Projectiles
{
	public class SkyMoonZapper : ModProjectile
	{
		private readonly int timeLeftMax = 100;

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(32);
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.damage = 13;
			Projectile.timeLeft = timeLeftMax;
			Projectile.alpha = 255;
		}

		public override void AI()
        {
			if (Projectile.timeLeft == timeLeftMax)
			{
				//Scan for tile collision
				for (int i = 0; i < 80; i++)
				{
					Projectile.Center += new Vector2(0, 16);
					if (WorldGen.SolidTile(Framing.GetTileSafely(Projectile.Center)))
						break;
				}
			}

			//Telegraph dusts
			for (int i = 0; i < 2; i++)
			{
				float magnitude = Main.rand.NextFloat();
				Vector2 pos = Vector2.Lerp(Projectile.Center - (Vector2.UnitY * (16 * 30)), Projectile.Center, magnitude);
				Dust.NewDustPerfect(pos + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(3)), 206,
					Vector2.UnitY * Main.rand.NextFloat(Projectile.timeLeft / timeLeftMax) * 3, Scale: magnitude);
			}
		}

		public override void OnKill(int timeLeft)
		{
			float vol = MathHelper.Clamp(1f - (Main.LocalPlayer.Distance(Projectile.Center) / 400f), 0, 1);
			if (vol > .1f)
				SoundEngine.PlaySound(SoundID.DD2_LightningBugZap with { Volume = vol, MaxInstances = 3 }, Projectile.Center);

			if (Projectile.owner == Main.myPlayer)
			{
				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MoonThunder>(), Projectile.damage, 0);
				proj.hostile = true;
				proj.friendly = true;
				proj.netUpdate = true;
			}
			for (int i = 0; i < 20; i++)
			{
				Dust.NewDustPerfect(Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(32)), Terraria.ID.DustID.Electric, (Vector2.UnitY * -Main.rand.NextFloat(3f)).RotatedByRandom(.5f), Scale: Main.rand.NextFloat());
				if (i < 3)
					ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(10)), Vector2.UnitY * -Main.rand.NextFloat(), Color.LightBlue with { A = 0 } * .15f, Main.rand.NextFloat(.8f, 1.2f), 20));
			}
		}

		public override Color? GetAlpha(Color lightColor) => null;

		public override void PostDraw(Color lightColor)
        {
        }
    }
}