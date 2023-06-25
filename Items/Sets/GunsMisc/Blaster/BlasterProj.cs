using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Particles;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles;
using System;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class BlasterProj : SubtypeProj
	{
		private const int chargeMax = 40;

		private int ShotIndex
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

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
			Projectile.timeLeft = chargeMax;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (player.whoAmI == Main.myPlayer)
			{
				Projectile.velocity = player.DirectionTo(Main.MouseWorld);
				Projectile.netUpdate = true;
			}
			int direction = Math.Sign(Projectile.velocity.X);
			if (direction == 0)
				direction = player.oldDirection;

			player.ChangeDir(direction);

			Projectile.Center = player.Center + new Vector2(44f, -6 * player.direction).RotatedBy(Projectile.velocity.ToRotation());
			Projectile.rotation = Projectile.velocity.ToRotation();

			player.heldProj = Projectile.whoAmI;
			player.itemRotation = MathHelper.WrapAngle(Projectile.rotation + ((player.direction < 0) ? MathHelper.Pi : 0));

			var dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(Projectile.timeLeft + 2f, 0).RotatedByRandom(MathHelper.TwoPi), Dusts[Main.rand.Next(Dusts.Length)]);
			dust.velocity = new Vector2(Projectile.timeLeft / (float)chargeMax * Main.rand.NextFloat(3f, 7f), 0).RotatedBy((Projectile.Center - dust.position).ToRotation());
			dust.noGravity = true;
			dust.scale = Main.rand.NextFloat(0.5f, 0.8f);

			player.itemTime = player.itemAnimation = 14;
		}

		public override bool ShouldUpdatePosition() => false;

		public override void Kill(int timeLeft)
		{
			Player player = Main.player[Projectile.owner];
			float magnitude = 15f;
			player.GetModPlayer<MyPlayer>().Shake += 4;

			for (int i = 0; i < 2; i++)
			{
				ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center, GetColor(Subtype), (i > 0) ? 40 : 60, 20, PulseCircle.MovementType.OutwardsQuadratic)
				{
					Angle = Projectile.rotation,
					ZRotation = .6f,
					Velocity = Projectile.velocity * ((i > 0) ? -0.2f : -1.2f)
				});
			}
			for (int i = 0; i < 12; i++)
			{
				var dust = Dust.NewDustPerfect(Projectile.Center, Dusts[Main.rand.Next(Dusts.Length)]);
				dust.velocity = Vector2.Zero + (Vector2.Normalize(Main.MouseWorld - (player.Center - new Vector2(4, 4))).RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(2f, 6f) * 3f);
				dust.noGravity = true;
				dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
			}

			if (Main.netMode != NetmodeID.Server)
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/EnergyBlastMedium") with { PitchVariance = 0.1f, Volume = 0.6f }, player.Center);

			if (Projectile.owner == player.whoAmI)
			{
				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * magnitude, ShotIndex, Projectile.damage, Projectile.knockBack, Projectile.owner);
				if (proj.ModProjectile is SubtypeProj)
					(proj.ModProjectile as SubtypeProj).Subtype = Subtype;

				Projectile.netUpdate = true;
			}
		}

		public override bool? CanDamage() => false;

		public override bool DoAudiovisuals => false;
	}
}