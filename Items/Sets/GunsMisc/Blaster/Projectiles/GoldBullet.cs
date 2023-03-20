using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Effects;
using SpiritMod.Mechanics.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public class GoldBullet : ModProjectile, ITrailProjectile
	{
		private bool StruckNPC { get => Projectile.ai[0] != 0; set => Projectile.ai[0] = value ? 1 : 0; }

		public void DoTrailCreation(TrailManager tManager)
		{
			tManager.CreateTrail(Projectile, new GradientTrail(Color.Yellow, Color.Orange), new TriangleCap(), new DefaultTrailPosition(), 5f, 704f, new DefaultShader());
			tManager.CreateTrail(Projectile, new GradientTrail(Color.Orange, Color.Red * .5f), new TriangleCap(), new DefaultTrailPosition(), 5f, 704f, new DefaultShader());
		}

		public override void SetStaticDefaults() => DisplayName.SetDefault("Gold Bullet");
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 80;
			Projectile.height = 6;
			Projectile.width = 6;
			AIType = ProjectileID.Bullet;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			Vector2 position = Projectile.Center + new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * 10f, Main.rand.NextFloat(-1.0f, 1.0f) * 10f);
			for (int i = 0; i < 2; i++)
			{
				int num = Dust.NewDust(position, 0, 0, DustID.IchorTorch, 0, 0, 100, default, 0.8f);
				Main.dust[num].velocity = Projectile.velocity * .25f;
				Main.dust[num].noGravity = true;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (target.TryGetGlobalNPC(out GoldBlasterGNPC GNPC))
			{
				if (GNPC.numMarks < GNPC.maxNumMarks)
					GNPC.numMarks++;
				else
				{
					Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, (-Projectile.velocity).RotatedBy(1f), ModContent.GoreType<GoldCasing>());
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Ricochet") with { Volume = 0.75f, PitchVariance = 0.5f }, Projectile.Center);
				}
			}
			StruckNPC = true;
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);

			if (!StruckNPC) //If the projectile dies without striking an NPC, clear all marks
			{
				foreach (NPC npc in Main.npc)
					if (npc.active && npc.TryGetGlobalNPC(out GoldBlasterGNPC GNPC))
						GNPC.TryVoidMarks(npc);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), 
				Projectile.rotation, TextureAssets.Projectile[Projectile.type].Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
