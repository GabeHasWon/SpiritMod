using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Boss.Infernon
{
	public class InfernalBlastHostile : ModProjectile
	{
		private int Target
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private bool FoundTarget
		{
			get => Projectile.ai[1] == 1;
			set => Projectile.ai[1] = value ? 1 : 0;
		}

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Infernal Blast");

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 12;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 120;
		}

		public override void AI()
		{
			for (int i = 0; i < 16; i++)
			{
				float x = Projectile.Center.X - Projectile.velocity.X / 10f * (float)i;
				float y = Projectile.Center.Y - Projectile.velocity.Y / 10f * (float)i;
				int num = Dust.NewDust(new Vector2(x, y), 26, 26, DustID.Torch, 0f, 0f, 0, default, 1f);
				Main.dust[num].alpha = Projectile.alpha;
				Main.dust[num].position.X = x;
				Main.dust[num].position.Y = y;
				Main.dust[num].velocity *= 0f;
				Main.dust[num].noGravity = true;
			}

			if (!FoundTarget)
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
					return;

				int distance = 2000;
				foreach (Player player in Main.player)
				{
					int sampleDist = (int)Vector2.Distance(player.Center, Projectile.Center);

					if (player.active && !player.dead && sampleDist < distance)
					{
						distance = sampleDist;

						Target = player.whoAmI;
						FoundTarget = true;
					}
				}

				Projectile.netUpdate = FoundTarget;
			}
			else if (Target >= 0 && Target < Main.maxPlayers)
			{
				Player targetPlayer = Main.player[Target];

				if (!targetPlayer.active || targetPlayer.dead)
				{
					FoundTarget = false;
					Projectile.netUpdate = true;
				}
				else
				{
					float currentRot = Projectile.velocity.ToRotation();
					Vector2 direction = targetPlayer.Center - Projectile.Center;
					float targetAngle = direction.ToRotation();

					if (direction == Vector2.Zero)
						targetAngle = currentRot;

					float desiredRot = currentRot.AngleLerp(targetAngle, 0.1f);
					Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(desiredRot, default);
				}
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 4; i < 31; i++)
			{
				float x = Projectile.oldVelocity.X * (30f / i);
				float y = Projectile.oldVelocity.Y * (30f / i);
				int newDust = Dust.NewDust(new Vector2(Projectile.oldPosition.X - x, Projectile.oldPosition.Y - y), 8, 8, DustID.Torch, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.8f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].velocity *= 0.5f;
			}
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			if (Main.rand.NextBool(4))
				target.AddBuff(BuffID.OnFire, 180, true);
		}
	}
}