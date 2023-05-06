using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Summon;
using SpiritMod.Projectiles.BaseProj;
using System;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Projectiles.Summon
{
	[AutoloadMinionBuff("Flying Snake", "'Quite venomous...'")]
	public class SnakeMinion : BaseMinion
	{
		public SnakeMinion() : base(1000, 1800, new Vector2(24, 24)) { }

		public override void AbstractSetStaticDefaults()
		{
			DisplayName.SetDefault("Flying Snake");
			Main.projFrames[Projectile.type] = 4;
		}

		private ref float Timer => ref Projectile.ai[0];

		public override bool PreAI()
		{
			foreach (Projectile proj in Main.projectile)
			{
				if (proj.whoAmI != Projectile.whoAmI && proj.active && proj.owner == Projectile.owner && proj.type == Projectile.type && Math.Abs(Projectile.position.X - proj.position.X) + Math.Abs(Projectile.position.Y - proj.position.Y) < (float)Projectile.width)
				{
					if (Projectile.position.X < proj.position.X)
						Projectile.velocity.X = Projectile.velocity.X - 0.05f;
					else
						Projectile.velocity.X = Projectile.velocity.X + 0.05f;

					if (Projectile.position.Y < proj.position.Y)
						Projectile.velocity.Y = Projectile.velocity.Y - 0.05f;
					else
						Projectile.velocity.Y = Projectile.velocity.Y + 0.05f;
				}
			}
			return true;
		}

		public override void IdleMovement(Player player)
		{
			Projectile.friendly = true;
			float num535 = 8f;
			if (Projectile.ai[0] == 1f)
				num535 = 12f;

			Vector2 vector38 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
			float num536 = Main.player[Projectile.owner].Center.X - vector38.X;
			float num537 = Main.player[Projectile.owner].Center.Y - vector38.Y - 60f;
			float num538 = (float)Math.Sqrt((num536 * num536 + num537 * num537));
			if (num538 < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
			{
				Projectile.ai[0] = 0f;
			}
			if (num538 > 2000f)
			{
				Projectile.position.X = Main.player[Projectile.owner].Center.X - (Projectile.width / 2f);
				Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (Projectile.width / 2f);
			}
			if (num538 > 70f)
			{
				num538 = num535 / num538;
				num536 *= num538;
				num537 *= num538;
				Projectile.velocity.X = (Projectile.velocity.X * 20f + num536) / 21f;
				Projectile.velocity.Y = (Projectile.velocity.Y * 20f + num537) / 21f;
			}
			else
			{
				if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
				{
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
				Projectile.velocity *= 1.01f;
			}
			Projectile.friendly = false;
			Projectile.rotation = Projectile.velocity.X * 0.05f;

			if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
			{
				Projectile.spriteDirection = -Projectile.direction;
				return;
			}
		}

		public override void TargettingBehavior(Player player, NPC target)
		{
			int reposInt = 17;
			int shootInt = 20;

			Timer = ++Timer % Math.Max(reposInt, shootInt);

			if (Timer % shootInt == 0)
			{
				Vector2 shootVec = Projectile.DirectionTo(target.Center) * 12f;

				int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, shootVec, ProjectileID.VenomFang, Projectile.damage, Projectile.knockBack / 2f, Projectile.owner, 0f, 0f);
				Main.projectile[p].friendly = true;
				Main.projectile[p].hostile = false;
			}
			if (Projectile.Distance(target.Center) < 100)
			{
				if (Timer % reposInt == 0)
				{
					Projectile.friendly = true;
					float num539 = 8f;
					Vector2 vector39 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
					float num540 = Projectile.position.X - vector39.X;
					float num541 = Projectile.position.Y - vector39.Y;
					float num542 = (float)Math.Sqrt((double)(num540 * num540 + num541 * num541));

					if (num542 < 100f)
						num539 = 10f;

					num542 = num539 / num542;
					num540 *= num542;
					num541 *= num542;
					Projectile.velocity.X = (Projectile.velocity.X * 14f + num540) / 15f;
					Projectile.velocity.Y = (Projectile.velocity.Y * 14f + num541) / 15f;
				}
				else
				{
					Projectile.friendly = false;

					if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 10f)
						Projectile.velocity *= 1.05f;
				}
			}
			else
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * 5f, 0.1f);
			}

			Projectile.rotation = Projectile.velocity.X * 0.05f;

			if (Math.Abs(Projectile.velocity.X) > 0.2)
			{
				Projectile.spriteDirection = -Projectile.direction;
				return;
			}
		}

		public override bool DoAutoFrameUpdate(ref int framespersecond, ref int startframe, ref int endframe)
		{
			framespersecond = 10;
			return true;
		}
	}
}
