using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Particles;
using SpiritMod.Projectiles.Bullet.Blaster;
using System.IO;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class BlasterProj : SubtypeProj
	{
		private int charge;
		private readonly int chargeMax = 40;

		private int ShotIndex
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private int ChargeShotIndex
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

			Projectile.frame = (int)((float)charge / chargeMax * Main.projFrames[Projectile.type]);

			GetVFX(out Color fxColor, out int[] dustType);

			if (player.channel)
			{
				if (charge < chargeMax)
				{
					if (charge >= 8)
					{
						var dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(chargeMax - charge + 2f, 0).RotatedByRandom(MathHelper.TwoPi), dustType[Main.rand.Next(2)]);
						dust.velocity = new Vector2((1f - (charge / (float)chargeMax)) * Main.rand.NextFloat(3f, 7f), 0).RotatedBy((Projectile.Center - dust.position).ToRotation());
						dust.noGravity = true;
						dust.scale = Main.rand.NextFloat(0.5f, 0.8f);
					}
					charge++;
				}
				if ((charge + 1) == chargeMax)
					SoundEngine.PlaySound(SoundID.MaxMana, Projectile.position);

				Projectile.timeLeft = 2;
			}
			else if (!fired)
			{
				fired = true;

				int damage = Projectile.damage;
				float knockback = Projectile.knockBack;
				float magnitude = player.HeldItem.shootSpeed + 2f;

				if (charge >= chargeMax)
				{
					if (ChargeShotIndex == ModContent.ProjectileType<BigBeam>())
					{
						damage = (int)(Projectile.damage * .6f); //The projectile's piercing and range compensate for this
						knockback = 1f;
					}
					else
					{
						damage = (int)(damage * 2.5f);
						knockback *= 2f;
					}

					magnitude *= 2f;
					player.GetModPlayer<MyPlayer>().Shake += 4;

					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/EnergyBlastMedium") with { PitchVariance = 0.1f, Volume = 0.6f }, player.Center);
					ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center, fxColor, 50, 20, PulseCircle.MovementType.OutwardsQuadratic)
					{
						Angle = Direction.ToRotation(),
						ZRotation = 0.6f,
						Velocity = Direction
					});
					for (int i = 0; i < 12; i++)
					{
						var dust = Dust.NewDustPerfect(Projectile.Center, dustType[Main.rand.Next(2)]);
						dust.velocity = Vector2.Zero + (Vector2.Normalize(Main.MouseWorld - (player.Center - new Vector2(4, 4))).RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(2f, 6f) * 3f);
						dust.noGravity = true;
						dust.scale = (float)(charge / chargeMax) * Main.rand.NextFloat(0.8f, 1.2f);
					}
				}
				else 
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MaliwanShot1") with { MaxInstances = 2 }, player.Center);

				Blaster.FireVisuals(Projectile.Center, Direction * magnitude, Subtype);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Direction * magnitude, (charge >= chargeMax) ? ChargeShotIndex : ShotIndex, damage, knockback, Projectile.owner);
					if (proj.ModProjectile is SubtypeProj)
						(proj.ModProjectile as SubtypeProj).Subtype = Subtype;
				}
				Projectile.netUpdate = true;
			}
			return true;
		}

		private void GetVFX(out Color color, out int[] dust)
		{
			switch (Subtype)
			{
				case 1:
					color = Color.LimeGreen;
					dust = new int[] { DustID.FartInAJar, DustID.GreenTorch };
					break;
				case 2:
					color = Color.LightBlue;
					dust = new int[] { DustID.FrostHydra, DustID.IceTorch };
					break;
				case 3:
					color = Color.Magenta;
					dust = new int[] { DustID.Pixie, DustID.PinkTorch };
					break;
				default:
					color = Color.Orange;
					dust = new int[] { DustID.SolarFlare, DustID.Torch };
					break;
			}
		}

		public override void SendExtraAI(BinaryWriter writer) => writer.Write(charge);
		public override void ReceiveExtraAI(BinaryReader reader) => charge = reader.Read();
	}
}