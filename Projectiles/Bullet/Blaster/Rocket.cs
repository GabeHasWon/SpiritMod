using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Bullet.Blaster
{
	public class Rocket : ModProjectile
	{
		private NPC target;

		private int? directNPCIndex;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Rocket");
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

		public override void AI()
		{
			if (Main.rand.NextBool(2))
			{
				Vector2 position = Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height));
				Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool(2) ? DustID.Smoke : DustID.Torch, null, 0, default, Main.rand.NextFloat(0.8f, 1.6f));
				dust.velocity = Projectile.velocity * .8f;
				dust.noGravity = true;
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

			//Manage homing capabilities
			float homingSpeed = 8f;
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
			ProjectileExtras.Explode(Projectile.whoAmI, 80, 80, delegate
			{
				if (!Main.dedServ)
					ExplodeEffect();
			});
		}

		private void ExplodeEffect()
		{
			SoundEngine.PlaySound(SoundID.Item14 with { PitchVariance = 0.1f }, Projectile.Center);
			for (int i = 0; i < 10; i++)
			{
				if (i < 3)
					ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(-1.0f, 1.0f), Main.rand.NextFloat(-1.0f, 1.0f)), Color.Lerp(Color.DarkGray, Color.Orange, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.5f, 1.0f), 14));
				Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(2) ? DustID.Torch : DustID.GemRuby, null);
				dust.velocity = new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * .5f, Main.rand.NextFloat(-1.0f, 1.0f) * .5f);
				dust.fadeIn = 1.1f;
				dust.noGravity = true;
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => directNPCIndex = target.whoAmI;
		public override bool? CanHitNPC(NPC target) => (target.whoAmI != directNPCIndex) ? null : false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			//Draw the projectile normally
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), 
				Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			//Draw a glowmask
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White),
				Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
