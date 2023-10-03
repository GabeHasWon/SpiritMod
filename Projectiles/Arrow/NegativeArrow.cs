using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.Trails;
using Terraria;
using System.Linq;
using Terraria.Audio;
using Terraria.ID;
using SpiritMod.Particles;
using SpiritMod.Items.BossLoot.StarplateDrops.SteamplateBow;
using SpiritMod.Items.Accessory.UnstableTeslaCoil;

namespace SpiritMod.Projectiles.Arrow
{
	public class NegativeArrow : ModProjectile, ITrailProjectile
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Negative Arrow");

		public void DoTrailCreation(TrailManager tM) => tM.CreateTrail(Projectile, new StandardColorTrail(new Color(255, 113, 36, 0)), new RoundCap(), new ZigZagTrailPosition(3f), 8f, 250f);

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(16);
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.velocity.Y += .15f;

			var target = Main.projectile.Where(x => x.active && x.type == ModContent.ProjectileType<PositiveArrow>() && x.Distance(Projectile.Center) < 240).FirstOrDefault();
			if (target != default)
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * 10f, .08f);
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 8; i++)
			{
				Vector2 dustVel = Projectile.velocity * Main.rand.NextFloat();
				Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FireworkFountain_Yellow, dustVel.X, dustVel.Y).noGravity = true;
			}

			var projTarget = Main.projectile.Where(x => x.active && x.type == ModContent.ProjectileType<PositiveArrow>() && x.Distance(Projectile.Center) < (Projectile.Size.Length() * 2)).OrderBy(x => Vector2.Distance(x.Center, Projectile.Center)).FirstOrDefault();
			if (projTarget == default)
				return;

			ProjectileExtras.Explode(Projectile.whoAmI, 80, 80, delegate
			{
				SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/starHit") with { Pitch = 1f }, Projectile.Center);

				for (int i = 0; i < 30; i++)
				{
					Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 7f);
					if (Main.rand.NextBool(2))
						dust.noGravity = true;
				}

				DustHelper.DrawStar(new Vector2(Projectile.Center.X, Projectile.Center.Y), 226, pointAmount: 5, mainSize: 2.5f, dustDensity: 2, pointDepthMult: 0.3f, noGravity: true);
				ParticleHandler.SpawnParticle(new ElectricalBurst(Projectile.Center, 1, 0));
			});

			int npcsHit = 0;
			var npcTargets = Main.npc.Where(x => x.active && x.Distance(Projectile.Center) < 300 && x.CanBeChasedBy(Projectile) && Collision.CanHit(Projectile, x));
			foreach (NPC npc in npcTargets)
			{
				Projectile.NewProjectile(Entity.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Unstable_Tesla_Coil_Projectile>(), Projectile.damage, 0f, Projectile.owner, npc.position.X, npc.position.Y);
				if (++npcsHit >= 2)
					break;
			}

			projTarget.Kill();
		}
	}
}