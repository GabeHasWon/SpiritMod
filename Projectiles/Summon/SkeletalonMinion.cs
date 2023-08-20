using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Summon
{
	public class SkeletalonMinion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Skeletal Harpy");
			Main.projFrames[Type] = 5;
			ProjectileID.Sets.MinionSacrificable[Type] = true;
			ProjectileID.Sets.CultistIsResistantTo[Type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.netImportant = true;
			Projectile.width = 42;
			Projectile.height = 44;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            Projectile.minionSlots = 0;
			Projectile.penetrate = 3;
			Projectile.timeLeft = 180;
		}

		public override void AI()
		{
			for (int num526 = 0; num526 < 1000; num526++)
			{
				if (num526 != Projectile.whoAmI && Main.projectile[num526].active && Main.projectile[num526].owner == Projectile.owner && Main.projectile[num526].type == Projectile.type && Math.Abs(Projectile.position.X - Main.projectile[num526].position.X) + Math.Abs(Projectile.position.Y - Main.projectile[num526].position.Y) < (float)Projectile.width)
				{
					if (Projectile.position.X < Main.projectile[num526].position.X)
						Projectile.velocity.X = Projectile.velocity.X - 0.05f;
					else
						Projectile.velocity.X = Projectile.velocity.X + 0.05f;

					if (Projectile.position.Y < Main.projectile[num526].position.Y)
						Projectile.velocity.Y = Projectile.velocity.Y - 0.05f;
					else
						Projectile.velocity.Y = Projectile.velocity.Y + 0.05f;
				}
			}

			Vector2 position = Projectile.position;
			float num529 = 900f;
			bool canTarget = false;

			if (Projectile.ai[0] == 0f)
			{
				for (int n = 0; n < Main.maxNPCs; n++)
				{
					NPC npc = Main.npc[n];
					if (npc.CanBeChasedBy(Projectile, false))
					{
						float num532 = npc.position.X + (float)(npc.width / 2);
						float num533 = npc.position.Y + (float)(npc.height / 2);
						float num534 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num532) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num533);
						
						if (num534 < num529 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
						{
							num529 = num534;
							position.X = num532;
							position.Y = num533;
							canTarget = true;
						}
					}
				}
			}
			else
			{
				Projectile.tileCollide = false;
			}

			if (!canTarget)
			{
				Projectile.friendly = true;
				float num535 = 8f;
				if (Projectile.ai[0] == 1f)
					num535 = 12f;

				Vector2 vector38 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
				float num536 = Main.player[Projectile.owner].Center.X - vector38.X;
				float num537 = Main.player[Projectile.owner].Center.Y - vector38.Y - 60f;
				float num538 = (float)Math.Sqrt((double)(num536 * num536 + num537 * num537));

				if (num538 < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
					Projectile.ai[0] = 0f;

				if (num538 > 2000f)
				{
					Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
					Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.width / 2);
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
			}
			else
			{
				if (Projectile.ai[1] == -1f)
					Projectile.ai[1] = 17f;
				if (Projectile.ai[1] > 0f)
					Projectile.ai[1] -= 1f;

				if (Projectile.ai[1] == 0f)
				{
					Projectile.friendly = true;
					float num539 = 8f;
					Vector2 vector39 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
					float num540 = position.X - vector39.X;
					float num541 = position.Y - vector39.Y;
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

			Projectile.rotation = Projectile.velocity.X * 0.05f;
			if (Math.Abs(Projectile.velocity.X) > 0.2)
				Projectile.spriteDirection = -Projectile.direction;

			if (++Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
			}

			int fadeoutTime = 12;
			if (Projectile.timeLeft <= fadeoutTime)
				Projectile.alpha += 255 / fadeoutTime;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Buffs.SummonTag.SummonTag5>(), 180, true);

		public override bool MinionContactDamage() => true;
	}
}