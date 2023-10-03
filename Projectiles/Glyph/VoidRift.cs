using Microsoft.Xna.Framework;
using SpiritMod.Items.Glyphs;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.GlobalClasses.Players;

namespace SpiritMod.Projectiles.Glyph
{
	public class VoidRift : ModProjectile
	{
		private readonly int timeLeftMax = VoidGlyph.CollapseDuration;

		public int TargetWhoAmI
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(10);
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.aiStyle = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = timeLeftMax;
		}

        public override void AI()
		{
			if ((Main.npc[TargetWhoAmI] is NPC npc) && npc.active)
			{
				Projectile.Center = npc.Center;
				Projectile.gfxOffY = npc.gfxOffY;
				float quoteant = (float)Projectile.timeLeft / timeLeftMax;

				if (Projectile.timeLeft % (10 + (Projectile.timeLeft / 100 * 10)) == 0)
				{
					Vector2 unit = Main.rand.NextVector2Unit();
					for (int i = 0; i < MathHelper.Max(1f - quoteant * 10, 4) ; i++)
						Dust.NewDustPerfect(Projectile.Center, DustID.CrystalPulse, (unit * Main.rand.NextFloat() * 4).RotatedByRandom(1f), 100, default, Main.rand.NextFloat(.5f, 1f)).noGravity = true;

					if ((Projectile.timeLeft < (timeLeftMax / 4)) && Main.rand.NextBool(2))
					{
						ParticleHandler.SpawnParticle(new PulseCircle(Projectile, Color.Magenta * .15f, 130, Math.Max(Projectile.timeLeft, 8), PulseCircle.MovementType.Inwards, Projectile.Center)
						{
							RingColor = Color.HotPink * .15f,
							Angle = Main.rand.NextFloat(-1f, 1f) + .785f,
							ZRotation = Main.rand.NextFloat(.4f, .9f)
						});
					}
				}
				if (Projectile.timeLeft == 25)
				{
					SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, Projectile.Center);
					for (int i = 0; i < 8; i++)
						Dust.NewDustPerfect(Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(30f, 50f)), DustID.CrystalPulse2, Vector2.Zero, 100, default, Main.rand.NextFloat(.8f, 1f)).noGravity = true;
				}
			}
			else Projectile.Kill();
		}

		public override void OnKill(int timeLeft)
		{
			if (timeLeft <= 0)
			{
				ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center, Color.Magenta * .3f, 100, 15, PulseCircle.MovementType.Inwards) { RingColor = Color.White * .3f });
				for (int i = 0; i < 30; i++)
					Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? DustID.PurpleCrystalShard : DustID.Shadowflame, Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 8, 100, default, Main.rand.NextFloat(1f, 2f)).noGravity = true;

				Vector2 unit = Main.rand.NextVector2Unit();
				for (int i = 0; i < 10; i++)
					Dust.NewDustPerfect(Projectile.Center, DustID.CrystalPulse, (unit * Main.rand.NextFloatDirection() * 10).RotatedByRandom(.25f), 100, default, Main.rand.NextFloat(1f, 2f)).noGravity = true;

				SoundEngine.PlaySound(SoundID.NPCDeath6 with { Volume = .5f }, Projectile.Center);
				SoundEngine.PlaySound(SoundID.NPCDeath11, Projectile.Center);
			}
			Main.LocalPlayer.GetModPlayer<GlyphPlayer>().voidStacks = 0;
		}

		public override bool? CanCutTiles() => false;

		public override bool? CanDamage() => Projectile.timeLeft <= 1;
	}
}