using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.DonatorItems
{
	class DuskfeatherBlade : ModProjectile, ITrailProjectile
	{
		private const float Range = 25 * 16;
		private const float Max_Dist = 100 * 16;
		private const int Total_Updates = 3;
		private const int Total_Lifetime = 600 * Total_Updates;

		public void DoTrailCreation(TrailManager trailManager)
			=> trailManager.CreateTrail(Projectile, new StandardColorTrail(Color.HotPink with { A = 0 }), new RoundCap(), new DefaultTrailPosition(), 40, 160, new ImageShader(Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_3", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, 0.05f, 1f, 1f));

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Duskfeather Blade");
			Main.projFrames[Projectile.type] = 8;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Throwing;
			Projectile.height = 14;
			Projectile.width = 14;
			Projectile.alpha = 255;
			Projectile.penetrate = 2;
			Projectile.aiStyle = -1;
			Projectile.extraUpdates = Total_Updates - 1;
			Projectile.timeLeft = Total_Lifetime;
		}

		internal static void AttractBlades(Player player)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				var projectile = Main.projectile[i];

				if (!projectile.active)
					continue;

				int state = (int)projectile.ai[0];

				if (projectile.type == ModContent.ProjectileType<DuskfeatherBlade>() && projectile.owner == player.whoAmI && state != (int)DuskfeatherState.FadeOut && state != (int)DuskfeatherState.FadeOutStuck)
				{
					Retract(projectile, true);
					projectile.netUpdate = true;
				}
			}
		}

		internal static void AttractOldestBlade(Player player)
		{
			Projectile oldest = null;
			int timeLeft = int.MaxValue;

			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				var projectile = Main.projectile[i];
				if (!projectile.active)
					continue;

				int state = (int)projectile.ai[0];

				if (projectile.type == ModContent.ProjectileType<DuskfeatherBlade>() && projectile.owner == player.whoAmI &&
					state != (int)DuskfeatherState.Return &&
					state != (int)DuskfeatherState.FadeOut &&
					state != (int)DuskfeatherState.FadeOutStuck &&
					projectile.timeLeft < timeLeft)
				{
					timeLeft = projectile.timeLeft;
					oldest = projectile;
				}
			}

			if (oldest != null)
				Retract(oldest);
		}

		internal static void Retract(Projectile projectile, bool fromRightClick = false)
		{
			if (fromRightClick && projectile.ai[0] != (int)DuskfeatherState.Return)
			{
				projectile.damage = (int)(projectile.damage * 1.5f);
				projectile.scale *= 1.5f;
			}

			projectile.ai[0] = (int)DuskfeatherState.Return;
		}

		private DuskfeatherState State
		{
			get => (DuskfeatherState)(int)Projectile.ai[0];
			set => Projectile.ai[0] = (int)value;
		}

		private float FiringVelocity
		{
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		private Vector2 Origin
		{
			get => new(Projectile.localAI[0], Projectile.localAI[1]);
			set
			{
				Projectile.localAI[0] = value.X;
				Projectile.localAI[1] = value.Y;
			}
		}

		private float Poof
		{
			get => Projectile.localAI[0];
			set => Projectile.localAI[0] = value;
		}

		public override void AI()
		{
			if (State < DuskfeatherState.Return)
			{
				if (Projectile.alpha > 25)
					Projectile.alpha -= 25;
				else
					Projectile.alpha = 0;
			}

			switch (State)
			{
				case DuskfeatherState.Moving:
					AIMove();
					break;
				case DuskfeatherState.StuckInBlock:
					AIStopped();
					break;
				case DuskfeatherState.Stopped:
					AIStopped();
					break;
				case DuskfeatherState.Return:
					AIReturn();
					break;
				case DuskfeatherState.FadeOut:
					AIFade();
					break;
				case DuskfeatherState.FadeOutStuck:
					AIFade();
					break;
			}

			if (Projectile.numUpdates == 0)
			{
				if (++Projectile.frameCounter >= 4)
				{
					Projectile.frameCounter = 0;
					Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
				}
			}
		}

		private void AIMove()
		{
			if (Origin == Vector2.Zero)
			{
				Projectile.rotation = (float)System.Math.Atan2(Projectile.velocity.X, -Projectile.velocity.Y);
				Origin = Projectile.position;
				Projectile.velocity *= 1f / Total_Updates;
				FiringVelocity = Projectile.velocity.Length();
			}

			float distanceFromStart = Vector2.DistanceSquared(Projectile.position, Origin);
			if (Range * Range < distanceFromStart)
			{
				Stop();
			}
		}

		private void AIStopped()
		{
			float distanceFromOwner = Vector2.DistanceSquared(Projectile.position, Main.player[Projectile.owner].position);

			if (Max_Dist * Max_Dist < distanceFromOwner)
				State = State == DuskfeatherState.Stopped ? DuskfeatherState.FadeOut : DuskfeatherState.FadeOutStuck;

			if (Projectile.timeLeft < 9)
				Retract(Projectile);
		}

		private void AIReturn()
		{
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;

			if (Poof == 0)
				Poof = 1;

			Vector2 velocity = Main.player[Projectile.owner].MountedCenter - Projectile.position;
			float distance = velocity.Length();

			if (distance < FiringVelocity)
			{
				Projectile.Kill();
				return;
			}

			float startFade = 10 * Total_Updates * FiringVelocity;

			if (distance < startFade)
				Projectile.alpha = 255 - (int)(distance / startFade * 255);

			velocity /= distance;
			velocity *= FiringVelocity *
				(distance < Range ?
				1.5f :
				1.5f + (distance - Range) / Range);

			Projectile.velocity = velocity;
			Projectile.rotation = (float)Math.Atan2(velocity.X, -velocity.Y) + (float)Math.PI;

			Projectile.timeLeft++;

			Dust.NewDustPerfect(Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 18f), Main.rand.NextBool(2) ? DustID.PinkCrystalShard : DustID.Shadowflame, Projectile.velocity * .5f).noGravity = true;
			if (Main.rand.NextBool(5) && !Main.dedServ)
			{
				float scale = Main.rand.NextFloat();
				ParticleHandler.SpawnParticle(new StarParticle(Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 3), Projectile.velocity * .8f, Color.LightPink, .2f * scale, (int)(18 * scale)));
			}
		}

		private void AIFade()
		{
			if (Projectile.numUpdates == 0)
			{
				if ((Projectile.alpha += 5) >= 255)
					Projectile.Kill();
			}
		}

		private void Stop()
		{
			Projectile.velocity = Vector2.Zero;
			State = DuskfeatherState.Stopped;
			Poof = 0;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			width = 0;
			height = 0;
			return true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (State != 0)
				return false;

			Projectile.position += Projectile.velocity *= Total_Updates;
			Stop();
			State = DuskfeatherState.StuckInBlock;
			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (State == DuskfeatherState.Moving)
				Stop();
		}

		public override bool? CanHitNPC(NPC target) => (State == DuskfeatherState.Moving || State == DuskfeatherState.Return) ? null : false;

		public override bool? CanCutTiles() =>  State == DuskfeatherState.Moving ? null : false;

		public override bool PreDraw(ref Color lightColor)
		{
			if ((State == DuskfeatherState.Stopped) || (State == DuskfeatherState.StuckInBlock)) //Draw an arrow directed at the projectile owner
			{
				float magnitude = .15f;
				Vector2 lerp = ((float)Main.timeForVisualEffects / 50).ToRotationVector2() * magnitude;
				Vector2 scale = new Vector2(1 - lerp.X, 1 - lerp.Y) * Projectile.scale;

				Texture2D arrowTexture = ModContent.Request<Texture2D>(Texture + "_Arrow").Value;

				float angleToOwner = Projectile.AngleTo(Main.player[Projectile.owner].Center);
				Vector2 position = Projectile.Center + (Vector2.UnitX * 20).RotatedBy(angleToOwner);
				Color color = Color.White * Math.Min(.5f, Projectile.Distance(Main.player[Projectile.owner].Center) * 0.001f);

				//Draw the arrow texture
				Main.EntitySpriteDraw(arrowTexture, position - Main.screenPosition, null, color, angleToOwner, arrowTexture.Size() / 2, scale, SpriteEffects.None, 0);
			}

			Texture2D texture = TextureAssets.Projectile[Type].Value;
			int frameX = (State == DuskfeatherState.Return) ? 1 : 0;
			Rectangle drawFrame = texture.Frame(2, Main.projFrames[Type], frameX, Projectile.frame, -2, -2);
			lightColor = Color.White;

			//Draw the projectile normally
			Main.EntitySpriteDraw(texture, Projectile.Center, drawFrame, Projectile.GetAlpha(lightColor), Projectile.rotation, drawFrame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + (Projectile.Size / 2) + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

				Main.EntitySpriteDraw(texture, drawPos, drawFrame, color * .6f, Projectile.rotation, drawFrame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}

			return false;
		}

		public enum DuskfeatherState
		{
			Moving = 0,
			StuckInBlock,
			Stopped,
			Return,
			FadeOut,
			FadeOutStuck
		}
	}
}