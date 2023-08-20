using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.Trails;
using System.IO;

namespace SpiritMod.Projectiles.Arrow
{
	public class PositiveArrow : ModProjectile, ITrailProjectile
	{
		private bool StuckToTile { get => (int)Projectile.ai[0] == 1; set => Projectile.ai[0] = value ? 1 : 0; }
		
		private bool StuckToNPC => targetWhoAmI != -1;

		private int targetWhoAmI = -1;
		private Vector2? stuckOffset;

		private const int stuckTimeLeft = 220;

		public void DoTrailCreation(TrailManager tM) => tM.CreateTrail(Projectile, new StandardColorTrail(Color.Cyan with { A = 0 }), new RoundCap(), new ZigZagTrailPosition(3f), 5f, 250f);

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Positive Arrow");

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(16);
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			if (StuckToNPC)
			{
				NPC target = Main.npc[targetWhoAmI];

				if (!stuckOffset.HasValue)
					stuckOffset = Projectile.Center - target.Center;

				if (target.active && !target.dontTakeDamage)
				{
					Projectile.Center = target.Center + stuckOffset.Value;
					Projectile.gfxOffY = target.gfxOffY;

					if (Projectile.timeLeft % 30f == 0f)
						target.HitEffect(0, 1.0);
				}
				else Projectile.Kill();
			}
			else if (!StuckToTile)
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override bool? CanDamage() => !(StuckToNPC || StuckToTile);

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (!StuckToNPC)
			{
				Projectile.timeLeft = stuckTimeLeft;
				targetWhoAmI = target.whoAmI;
				Projectile.netUpdate = true;
				OnStick();
			}
			Projectile.velocity = Vector2.Zero;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (!StuckToTile)
			{
				StuckToTile = true;
				Projectile.timeLeft = stuckTimeLeft;
				Projectile.position += Projectile.velocity * 2;
				OnStick();
			}
			Projectile.velocity = Vector2.Zero;

			return false;
		}

		private void OnStick()
		{
			//Remove old stuck arrows if the total player-owned amount exceeds 3
			int maxStuck = 3;
			Point[] array = new Point[maxStuck];
			int count = 0;

			for (int n = 0; n < Main.maxProjectiles; n++)
			{
				if (n != Projectile.whoAmI && Main.projectile[n].active && Main.projectile[n].owner == Main.myPlayer && Main.projectile[n].ModProjectile is PositiveArrow posArrow && (posArrow.StuckToNPC || posArrow.StuckToTile))
				{
					array[count++] = new Point(n, Main.projectile[n].timeLeft);
					if (count >= array.Length)
						break;
				}
			}
			if (count >= array.Length)
			{
				int num33 = 0;
				for (int num34 = 1; num34 < array.Length; num34++)
				{
					if (array[num34].Y < array[num33].Y)
						num33 = num34;
				}
				Main.projectile[array[num33].X].Kill();
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 8; i++)
				Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, Projectile.velocity.X, Projectile.velocity.Y).noGravity = true;
		}

		public override void SendExtraAI(BinaryWriter writer) => writer.Write(targetWhoAmI);

		public override void ReceiveExtraAI(BinaryReader reader) => targetWhoAmI = reader.ReadInt32();
	}
}