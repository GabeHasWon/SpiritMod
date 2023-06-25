using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using SpiritMod.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public abstract class SubtypeProj : ModProjectile
	{
		public enum Subtypes : byte
		{
			Fire = 0,
			Poison = 1,
			Frost = 2,
			Plasma = 3,
			Count
		}

		public byte Subtype { get; set; }

		public bool bouncy = false;

		/// <summary>
		/// Whether the projectile should do the audiovisual effects associated with its element, which can only run once
		/// </summary>
		public virtual bool DoAudiovisuals => true;
		private bool didDoAudiovisuals = false;

		public sealed override bool PreAI()
		{
			if (DoAudiovisuals && !didDoAudiovisuals)
			{
				if (Main.netMode != NetmodeID.Server)
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MaliwanShot1") with { MaxInstances = 3 }, Projectile.Center);

				switch (Subtype)
				{
					case (int)Subtypes.Fire:
						for (int i = 0; i < 10; i++)
						{
							if (i < 3)
								ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(-1.0f, 1.0f), Main.rand.NextFloat(-1.0f, 1.0f)), Color.Lerp(Color.DarkGray, Color.Orange, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.25f, 0.5f), 12));
							
							Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(2) ? DustID.Torch : DustID.Flare, null);
							dust.velocity = (Projectile.velocity * Main.rand.NextFloat(0.15f, 0.3f)).RotatedByRandom(1f);
							if (dust.type == DustID.Torch)
								dust.fadeIn = 1.1f;
							dust.noGravity = true;
						}
						break;
					case (int)Subtypes.Poison:
						for (int i = 0; i < 8; i++)
						{
							if (i < 3)
								ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(-1.0f, 1.0f), Main.rand.NextFloat(-1.0f, 1.0f)), Color.Lerp(Color.White, Color.LimeGreen, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.25f, 0.5f), 12));
							
							Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(2) ? DustID.FartInAJar : DustID.GreenTorch, null);
							dust.velocity = (Projectile.velocity * Main.rand.NextFloat(0.15f, 0.3f)).RotatedByRandom(1f);
							if (dust.type == DustID.GreenTorch)
								dust.fadeIn = 1.1f;
							dust.noGravity = true;
						}
						break;
					case (int)Subtypes.Frost:
						for (int i = 0; i < 8; i++)
						{
							if (i < 3)
								ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(-1.0f, 1.0f), Main.rand.NextFloat(-1.0f, 1.0f)), Color.Lerp(new Color(25, 236, 255), Color.White, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.5f, 1.0f), 14));
							
							Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(2) ? DustID.FrostHydra : DustID.GemSapphire, null);
							dust.velocity = new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * .5f, Main.rand.NextFloat(-1.0f, 1.0f) * .5f);
							dust.fadeIn = 1.1f;
							dust.noGravity = true;
						}
						break;
					case (int)Subtypes.Plasma:
						for (int i = 0; i < 8; i++)
						{
							if (i == 0)
								ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center, Color.Lerp(Color.Magenta, Color.White, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(20f, 40f), 14)
								{
									Angle = Projectile.velocity.ToRotation(),
									Velocity = Projectile.velocity * Main.rand.NextFloat(0.04f, 0.08f),
									ZRotation = 0.6f
								});

							Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(2) ? DustID.Pixie : DustID.PinkTorch, null);
							dust.velocity = (Projectile.velocity * Main.rand.NextFloat(0.2f, 0.8f)).RotatedByRandom(1.2f);
							dust.fadeIn = 1.1f;
							dust.noGravity = true;
						}
						break;
				}
				didDoAudiovisuals = true;
			}
			return true;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			int? debuffType = Debuff;
			if (debuffType != null)
				target.AddBuff(debuffType.Value, 200);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (bouncy)
				ProjectileExtensions.Bounce(Projectile, oldVelocity);

			return !bouncy;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(Subtype);
			writer.Write(bouncy);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Subtype = reader.ReadByte();
			bouncy = reader.ReadBoolean();
		}

		internal static Color GetColor(int index)
		{
			return index switch
			{
				(int)Subtypes.Poison => new Color(22, 245, 140),
				(int)Subtypes.Frost => new Color(135, 190, 225),
				(int)Subtypes.Plasma => new Color(255, 85, 235),
				_ => new Color(255, 163, 0)
			};
		}

		protected int? Debuff
			=> Subtype switch
			{
				(int)Subtypes.Poison => BuffID.Poisoned,
				(int)Subtypes.Frost => BuffID.Frostburn,
				(int)Subtypes.Plasma => ModContent.BuffType<Shocked>(),
				_ => BuffID.OnFire
			};

		protected int[] Dusts
			=> Subtype switch
			{
				(int)Subtypes.Poison => new int[] { DustID.FartInAJar, DustID.GreenTorch },
				(int)Subtypes.Frost => new int[] { DustID.FrostHydra, DustID.IceTorch },
				(int)Subtypes.Plasma => new int[] { DustID.Pixie, DustID.PinkTorch },
				_ => new int[] { DustID.SolarFlare, DustID.Torch }
			};
	}
}