using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.DonatorItems
{
	class DuskfeatherBlade : ModProjectile
	{

		private const float Range = 25 * 16;
		private const float Max_Dist = 100 * 16;
		private const int Total_Updates = 3;
		private const int Total_Lifetime = 600 * Total_Updates;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Duskfeather Blade");
			Main.projFrames[Projectile.type] = 13;
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
					Retract(projectile, true);
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
				if (projectile.type == ModContent.ProjectileType<DuskfeatherBlade>() &&
					projectile.owner == player.whoAmI &&
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
			projectile.ai[0] = (int)DuskfeatherState.Return;

			if (fromRightClick)
			{
				projectile.damage = (int)(projectile.damage * 1.5f);
				projectile.scale *= 1.5f;
			}

			projectile.netUpdate = true;
		}

		public override void Kill(int timeLeft)
		{
			if (Projectile.alpha == 255)
				return;
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
			get { return new Vector2(Projectile.localAI[0], Projectile.localAI[1]); }
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
			int minFrame = 7;
			int maxFrame = 12;

			switch (State)
			{
				case DuskfeatherState.Moving:
					AIMove();
					break;
				case DuskfeatherState.StuckInBlock:
					maxFrame = 7;
					AIStopped();
					break;
				case DuskfeatherState.Stopped:
					minFrame = 0;
					maxFrame = 6;
					AIStopped();
					break;
				case DuskfeatherState.Return:
					AIReturn();
					break;
				case DuskfeatherState.FadeOut:
					minFrame = 0;
					maxFrame = 6;
					AIFade();
					break;
				case DuskfeatherState.FadeOutStuck:
					maxFrame = 7;
					AIFade();
					break;
			}

			if (Projectile.numUpdates == 0)
			{
				if (State == DuskfeatherState.Moving || State == DuskfeatherState.Return)
					++Projectile.frameCounter;
				if (++Projectile.frameCounter >= 5)
				{
					Projectile.frameCounter = 0;
					++Projectile.frame;
				}
				if (Projectile.frame < minFrame || Projectile.frame > maxFrame)
					Projectile.frame = minFrame;
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
			Projectile.rotation = (float)System.Math.Atan2(velocity.X, -velocity.Y) + (float)System.Math.PI;

			Projectile.timeLeft++;
		}

		private void AIFade()
		{
			if (Projectile.numUpdates == 0)
			{
				Projectile.alpha += 5;
				if (Projectile.alpha >= 255)
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

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (State == DuskfeatherState.Moving)
				Stop();
		}

		public override bool? CanHitNPC(NPC target) => (State == DuskfeatherState.Moving || State == DuskfeatherState.Return) ? null : false;

		public override bool? CanCutTiles() =>  State == DuskfeatherState.Moving ? null : false;

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