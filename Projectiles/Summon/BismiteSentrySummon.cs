using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.NPCs.DiseasedSlime;
using SpiritMod.Particles;
using SpiritMod.Projectiles.Magic;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Summon
{
	public class BismiteSentrySummon : ModProjectile
	{
		public ref float Counter => ref Projectile.ai[0];
		public ref float Cooldown => ref Projectile.ai[1];
		private readonly int cooldownMax = 90;

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 46;
			Projectile.timeLeft = Projectile.SentryLifeTime;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
            Projectile.tileCollide = false;
			Projectile.sentry = true;
			Projectile.ignoreWater = true;
			Projectile.sentry = true;
		}

		public override void AI()
		{
			if (Cooldown > 0)
			{
				if (--Cooldown <= 20)
					Projectile.alpha = Math.Max(Projectile.alpha - (255 / 20), 0);
				return;
			}

			if (Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position + new Vector2(0, Projectile.gfxOffY), Projectile.width, Projectile.height,
					DustID.GreenTorch, 0, 0, 150, default, .1f);
				dust.noGravity = true;
				dust.noLightEmittence = true;
				dust.fadeIn = Main.rand.NextFloat(.5f, 1.5f);
				dust.velocity = Vector2.UnitY * -Main.rand.NextFloat(.5f, 1.5f);
			}

			if ((Counter = ++Counter % 60) == 0)
			{
				const int range = 18 * 16;
				var target = Projectile.OwnerMinionAttackTargetNPC ?? Main.npc.Where(x => x.active && x.CanBeChasedBy(Projectile) && Collision.CanHit(Projectile, x) && Projectile.Distance(x.Center) < range).OrderBy(x => x.Distance(Projectile.Center)).FirstOrDefault();
				
				if (target != default)
				{
					Vector2 pos = new Vector2(Projectile.Center.X, Projectile.Center.Y - 13);

					if (Projectile.owner == Main.myPlayer)
					{
						Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, pos.DirectionTo(target.Center) * 6, ModContent.ProjectileType<BismiteShot>(), Projectile.damage, 0, Projectile.owner);
						proj.DamageType = DamageClass.Summon;
						proj.netUpdate = true;
					}
					DoDusts();
					SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.Center);
				}
			}
		}

		public void SpecialAttack()
        {
			Cooldown = cooldownMax;
			Projectile.damage *= 2;
			ProjectileExtras.Explode(Projectile.whoAmI, 150, 150, delegate
			{
				for (int i = 0; i < 5; i++)
				{
					Vector2 unit = Main.rand.NextVector2Unit();
					ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center + (unit * Main.rand.NextFloat(10f)), unit, Color.LimeGreen * Main.rand.NextFloat(.2f, .4f), Main.rand.NextFloat() + 1, Main.rand.Next(10, 30)));
				}
				for (int i = 0; i < 5; i++)
					ParticleHandler.SpawnParticle(new GlowParticle(Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f), Color.GreenYellow * .75f, Main.rand.NextFloat(.5f, 1f), Main.rand.Next(10, 15)));
				for (int i = 0; i < 20; i++)
					Dust.NewDustPerfect(Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(80)), 
						Main.rand.NextFromList(DustID.Clentaminator_Green, DustID.GemEmerald), Scale: Main.rand.NextFloat()).noGravity = true;
			});
			if (Projectile.owner == Main.myPlayer)
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.UnitY * -.5f, ModContent.ProjectileType<NoxiousIndicator>(), 0, 0, Projectile.owner);

			for (int i = 0; i < 10; i++)
			{
				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 6f), 
					ModContent.ProjectileType<BismiteShard>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
				
				proj.DamageType = DamageClass.Summon;
				proj.timeLeft = 120;
				proj.netUpdate = true;
			}

			SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.Center);
			DoDusts();
			Projectile.damage = Projectile.originalDamage;
			Projectile.alpha = 200;
		}

		public override bool? CanDamage() => Cooldown == cooldownMax;

		public override bool? CanCutTiles() => false;

		private void DoDusts()
		{
			for (int i = 0; i < 10; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Green, Scale: 1.5f);
				dust.noGravity = true;
				dust.velocity = Projectile.DirectionTo(dust.position) * Main.rand.NextFloat(.5f, 2f);
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.DD2_WitherBeastHurt, Projectile.Center);
			DoDusts();
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Texture2D glow = Mod.Assets.Request<Texture2D>("Effects/Masks/CircleGradient").Value;

			float pulse = (float)Math.Sin(Main.timeForVisualEffects / 30f);
			float offY = (float)Math.Sin(Main.timeForVisualEffects / 50f) * 5f;
			Color color = Projectile.GetAlpha(Color.Lerp(lightColor, Color.LightGreen with { A = 0 }, .5f + (pulse * .25f)));

			Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
				null, Projectile.GetAlpha(Color.Yellow with { A = 0 }) * .25f * pulse, Projectile.rotation, glow.Size() / 2, Projectile.scale * .3f, SpriteEffects.None);
			for (int i = 0; i < 2; i++)
			{
				if (i == 1)
					color *= .2f * pulse;

				Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY + offY),
					null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale + (pulse * .5f * i), SpriteEffects.None);
			}
			return false;
		}
	}
}