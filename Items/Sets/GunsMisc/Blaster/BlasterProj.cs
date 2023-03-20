using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Particles;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class BlasterProj : SubtypeProj
	{
		private int Charge
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int chargeMax = 40;

		private int ShotIndex
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		private bool fired = false;

		Vector2 Direction => Main.player[Projectile.owner].DirectionTo(Main.MouseWorld);

		public override string Texture => SpiritMod.EMPTY_TEXTURE;
		public override void SetStaticDefaults() => DisplayName.SetDefault("Blaster");

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 2;
			Projectile.ignoreWater = true;
		}

		public override bool PreAI()
		{
			Player player = Main.player[Projectile.owner];
			player.ChangeDir(Main.MouseWorld.X > player.Center.X ? 1 : -1);

			player.itemTime = 14;
			player.itemAnimation = 14;

			Projectile.position = player.Center + new Vector2(44f, -6 * player.direction).RotatedBy(Direction.ToRotation()) - (Projectile.Size / 2);
			player.itemRotation = MathHelper.WrapAngle(Direction.ToRotation() + ((player.direction < 0) ? MathHelper.Pi : 0));

			int[] dustType = Dusts;

			if (Charge < chargeMax)
			{
				var dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(chargeMax - Charge + 2f, 0).RotatedByRandom(MathHelper.TwoPi), dustType[Main.rand.Next(dustType.Length)]);
				dust.velocity = new Vector2((1f - (Charge / (float)chargeMax)) * Main.rand.NextFloat(3f, 7f), 0).RotatedBy((Projectile.Center - dust.position).ToRotation());
				dust.noGravity = true;
				dust.scale = Main.rand.NextFloat(0.5f, 0.8f);

				Projectile.timeLeft = 2;
				Charge++;
			}
			else if (!fired)
			{
				fired = true;

				float magnitude = 15f;
				player.GetModPlayer<MyPlayer>().Shake += 4;

				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/EnergyBlastMedium") with { PitchVariance = 0.1f, Volume = 0.6f }, player.Center);
				
				for (int i = 0; i < 3; i++)
				{
					ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center, GetColor(Subtype), 40 + (i * 15), 20 - i, PulseCircle.MovementType.OutwardsQuadratic)
					{
						Angle = Direction.ToRotation(),
						ZRotation = 0.65f,
						Velocity = Direction * (i + .5f)
					});
				}

				for (int i = 0; i < 12; i++)
				{
					var dust = Dust.NewDustPerfect(Projectile.Center, dustType[Main.rand.Next(2)]);
					dust.velocity = Vector2.Zero + (Vector2.Normalize(Main.MouseWorld - (player.Center - new Vector2(4, 4))).RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(2f, 6f) * 3f);
					dust.noGravity = true;
					dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
				}

				Blaster.FireVisuals(Projectile.Center, Direction * magnitude, Subtype);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Direction * magnitude, ShotIndex, Projectile.damage, Projectile.knockBack, Projectile.owner);
					if (proj.ModProjectile is SubtypeProj)
						(proj.ModProjectile as SubtypeProj).Subtype = Subtype;

					Projectile.netUpdate = true;
				}
			}
			return true;
		}

		public override bool? CanDamage() => false;
	}
}