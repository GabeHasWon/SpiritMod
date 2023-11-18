using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Particles;
using SpiritMod.Prim;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Utilities;

namespace SpiritMod.Projectiles.Glyph
{
	public class CursedPhantom : ModProjectile, ITrailProjectile
	{
		private ref float Counter => ref Projectile.ai[0];
		private ref float IdleTime => ref Projectile.ai[1];

		private Vector2? originPos = null;
		private bool foundNPC; 

		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 8;
			ProjectileID.Sets.TrailCacheLength[Type] = 6;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(20);
			Projectile.penetrate = 1;
			Projectile.friendly = true;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 280;
			Projectile.alpha = 150;
		}

		public void DoTrailCreation(TrailManager tManager)
		{
			tManager.CreateTrail(Projectile, new StandardColorTrail(Color.LimeGreen), new RoundCap(), new DefaultTrailPosition(), 10f, 50f);
			tManager.CreateTrail(Projectile, new StandardColorTrail(Color.White), new RoundCap(), new DefaultTrailPosition(), 4f, 40f);
			SpiritMod.primitives.CreateTrail(new SkullPrimTrail(Projectile, Color.LimeGreen, 12, 5));
		}

		public override void AI()
		{
			int maxRange = 400;
			bool foundNPC = false;

			Player owner = Main.player[Projectile.owner];
			if (originPos == null)
				originPos = Projectile.Center;

			foreach (NPC npc in Main.npc) //Home in on nearby NPCs
			{
				if (npc.active && !npc.friendly && npc.CanBeChasedBy(Projectile) && Projectile.Distance(npc.Center) <= maxRange)
				{
					foundNPC = true;
					break;
				}
			}

			if (++Counter >= IdleTime)
			{
				if (Counter == IdleTime)
					Projectile.netUpdate = true;
				else if (foundNPC)
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(owner.Center) * 8f, .02f);
				else
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(owner.Center) * 8f, .02f);

				Projectile.alpha = Math.Max(Projectile.alpha - 5, 0);
			}
			else
			{
				if ((originPos is Vector2 origin) && (Projectile.Distance(origin) > (16 * 12)))
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(origin) * 6f, .04f);
				else
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Vector2.Normalize(Projectile.velocity) * 4f).RotatedBy(Main.rand.NextFloat(-1f, 1f)), .1f);
			}
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Counter < IdleTime)
			{
				Projectile.velocity = -oldVelocity;
				return false;
			}
			return true;
		}
		public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
		{
			if (Main.masterMode)
				modifiers.FinalDamage /= 6;
			else if (Main.expertMode)
				modifiers.FinalDamage /= 4;
			else
				modifiers.FinalDamage /= 2;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info) => Projectile.penetrate--;

		public override void OnKill(int timeLeft)
		{
			if (timeLeft <= 0)
			{
				for (int i = 0; i < 15; i++)
				{
					Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 3f, 0, default, Main.rand.NextFloat(.8f, 2f));
					dust.noGravity = true;
					dust.fadeIn = 1.2f;
				}
			}
			else
			{
				ProjectileExtras.Explode(Projectile.whoAmI, 80, 80, delegate
				{
					for (int i = 0; i < 30; i++)
					{
						Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 5f, 0, default, Main.rand.NextFloat(.8f, 2f));
						dust.noGravity = true;
						dust.fadeIn = 1.2f;

						if (i < 2)
							ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center + (Main.rand.NextVector2Unit() * 14f), Vector2.Normalize(Projectile.velocity).RotatedByRandom(2f), Color.LightGreen, Main.rand.NextFloat(.4f, .7f), 30));
					}
					SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);
					ParticleHandler.SpawnParticle(new CursedPhantomCloud(Projectile.Center, Main.rand.NextFloat(.8f, 1.1f), Main.rand.NextFloat(-.2f, .2f)));
				});
			}
		}

		public override bool? CanCutTiles() => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.QuickDrawTrail(baseOpacity: 1f, drawColor: Color.LimeGreen with { A = 0 });

			Texture2D tail = ModContent.Request<Texture2D>(Texture + "_Tail").Value;
			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
			{
				if (i % 2 == 0)
					continue;
				float scale = Projectile.scale * (1f - ((float)i / ProjectileID.Sets.TrailCacheLength[Type] * .3f));
				Vector2 drawPosition = Projectile.oldPos[i] + (Projectile.Size / 2) - Main.screenPosition;

				Main.EntitySpriteDraw(tail, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.oldRot[i], tail.Size() / 2, scale,
					(Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
			}

			Projectile.QuickDraw();
			Projectile.QuickDrawGlow();
			return false;
		}
	}

	internal class CursedPhantomCloud : Particle
	{
		private const int _numFrames = 6;
		private int _frame;
		private const int _displayTime = 20;

		public CursedPhantomCloud(Vector2 position, float scale, float rotation)
		{
			Position = position;
			Scale = scale;
			Rotation = rotation;
		}

		public override void Update()
		{
			_frame = (int)(_numFrames * TimeActive / _displayTime);
			Lighting.AddLight(Position, Color.LimeGreen.ToVector3() / 3);

			if (TimeActive > _displayTime)
				Kill();
		}

		public override bool UseCustomDraw => true;

		public override void CustomDraw(SpriteBatch spriteBatch)
		{
			Texture2D tex = ParticleHandler.GetTexture(Type);
			var DrawFrame = new Rectangle(0, _frame * tex.Height / _numFrames, tex.Width, tex.Height / _numFrames);
			spriteBatch.Draw(tex, Position - Main.screenPosition, DrawFrame, Color.White, Rotation, DrawFrame.Size() / 2, Scale, SpriteEffects.None, 0);
		}
	}
}