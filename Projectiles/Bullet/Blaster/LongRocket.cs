using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Bullet.Blaster
{
	public class LongRocket : SubtypeProj, ITrailProjectile
	{
		private Color commonColor = Color.White;
		private int[] dustType = new int[2];
		private int debuffType;

		private NPC target;

		private int? directNPCIndex;

		public void DoTrailCreation(TrailManager tManager) => tManager.CreateTrail(Projectile, new GradientTrail(new Color(224, 108, 29) * 2.2f, Color.Red * .8f), new NoCap(), new DefaultTrailPosition(), 6f, 100f, new DefaultShader());

		public override void SetStaticDefaults() => DisplayName.SetDefault("Long Rocket");
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 80;
			Projectile.height = 8;
			Projectile.width = 8;
			AIType = ProjectileID.Bullet;
		}

		public override void OnSpawn(IEntitySource source)
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc.active && npc.CanBeChasedBy(Projectile) && Projectile.Distance(npc.Center) <= 500f)
				{
					target = npc;
					break;
				}
			}
		}

		public override bool PreAI()
		{
			switch (Subtype)
			{
				case 1:
					dustType = new int[] { DustID.FartInAJar, DustID.GreenTorch };
					commonColor = Color.LimeGreen;
					debuffType = BuffID.Poisoned;
					break;
				case 2:
					dustType = new int[] { DustID.FrostHydra, DustID.IceTorch };
					commonColor = Color.LightBlue;
					debuffType = BuffID.Frostburn;
					break;
				case 3:
					dustType = new int[] { DustID.Pixie, DustID.PinkTorch };
					commonColor = Color.Magenta;
					debuffType = BuffID.OnFire;
					break;
				default:
					dustType = new int[] { DustID.SolarFlare, DustID.Torch };
					commonColor = Color.Orange;
					debuffType = BuffID.OnFire;
					break;
			}
			return true;
		}

		public override void AI()
		{
			if (Main.rand.NextBool(2))
			{
				Vector2 position = Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height));
				Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool(2) ? DustID.Smoke : dustType[1], null, 0, default, Main.rand.NextFloat(0.8f, 1.6f));
				dust.velocity = Projectile.velocity * .8f;
				dust.noGravity = true;
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

			//Manage homing capabilities
			float homingSpeed = 12f;
			if (target != null)
			{
				if (target.active)
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * homingSpeed, 0.05f);
				else
					target = null;
			}
			else
			{
				foreach (NPC npc in Main.npc)
				{
					if (npc.active && npc.CanBeChasedBy(Projectile) && Projectile.Distance(npc.Center) <= 500f)
					{
						target = npc;
						break;
					}
				}

				//Fall down when moving slow enough and not homing in on a target
				if (Projectile.velocity.Length() < 1.8f)
					Projectile.velocity.Y += 0.1f;
			}
		}

		public override void Kill(int timeLeft)
		{
			ProjectileExtras.Explode(Projectile.whoAmI, 100, 100, delegate
			{
				if (!Main.dedServ)
					ExplodeEffect();
			});
		}

		private void ExplodeEffect()
		{
			SoundEngine.PlaySound(SoundID.Item14 with { PitchVariance = 0.1f }, Projectile.Center);
			int fireParticles = Main.rand.Next(4, 9);
			for (int i = 0; i < 10; i++)
			{
				if (i < 3)
					ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(-1.0f, 1.0f), Main.rand.NextFloat(-1.0f, 1.0f)), Color.Lerp(Color.DarkGray, Color.Orange, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.7f, 1.2f), 16));
				if (i < fireParticles)
				{
					float velLength = Main.rand.NextFloat(3, 5);
					Vector2 vel = Main.rand.NextVector2Unit() * velLength;
					ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center, vel, Color.White, commonColor * .25f, Main.rand.NextFloat(.2f, 0.6f), Main.rand.Next(15, 22)));
				}
				Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType[Main.rand.Next(dustType.Length)], null);
				dust.velocity = new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * .5f, Main.rand.NextFloat(-1.0f, 1.0f) * .5f);
				dust.fadeIn = 1.1f;
				dust.noGravity = true;
			}
			ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center, commonColor * .8f, 100, 15));
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			directNPCIndex = target.whoAmI;
			target.AddBuff(debuffType, 200);
		}

		public override bool? CanHitNPC(NPC target) => (target.whoAmI != directNPCIndex) ? null : false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle frame = GetDrawFrame(texture);

			//Draw the projectile normally
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), 
				Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			//Draw a glowmask
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White),
				Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
