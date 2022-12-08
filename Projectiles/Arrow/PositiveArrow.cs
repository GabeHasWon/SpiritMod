using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Particles;
using SpiritMod.Items.BossLoot.StarplateDrops;

namespace SpiritMod.Projectiles.Arrow
{
	public class PositiveArrow : ModProjectile, ITrailProjectile
	{
		private int State
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private const int STRUCK_NPC = 1;
		private const int STRUCK_TILE = 2;

		private bool doDamageEffect;

		private int TargetIndex
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}
		private Projectile targetProj;
		public int oppositearrow;

		public void DoTrailCreation(TrailManager tM) => tM.CreateTrail(Projectile, new StandardColorTrail(new Color(122, 233, 255)), new RoundCap(), new ZigZagTrailPosition(3f), 8f, 250f);
		public override void SetStaticDefaults() => DisplayName.SetDefault("Positive Arrow");

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			oppositearrow = ModContent.ProjectileType<NegativeArrow>();
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
			Projectile.penetrate = -1;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20;
		}

		//public override void SendExtraAI(BinaryWriter writer) => writer.Write(stuck);
		//public override void ReceiveExtraAI(BinaryReader reader) => stuck = reader.ReadBoolean();

		public override void Kill(int timeLeft)
		{
			if (timeLeft <= 0)
				SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			for (int i = 0; i < 3; i++)
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (State != STRUCK_TILE) 
			{
				Projectile.rotation = oldVelocity.ToRotation() + 1.57f;
				Projectile.position += Projectile.velocity * 2;
				State = STRUCK_TILE;
				Projectile.timeLeft = 370;
				Projectile.velocity = Vector2.Zero;
				Projectile.aiStyle = -1;
			}
			return false;
		}

		public override bool? CanDamage() => doDamageEffect;
		public override bool PreAI()
		{
			doDamageEffect = State == 0;
			if (State == STRUCK_TILE) 
			{
				CheckColliding(ref Projectile.damage);
				Projectile.timeLeft = Math.Min(Projectile.timeLeft, 360);
				Projectile.velocity = Vector2.Zero;
				return false;
			}
			else if (State != 0) 
			{
				Projectile.timeLeft = Math.Min(Projectile.timeLeft, 360);
				Projectile.ignoreWater = true;
				Projectile.tileCollide = false;

				if (Main.npc[TargetIndex].active && !Main.npc[TargetIndex].dontTakeDamage) 
				{
					Projectile.Center = Main.npc[TargetIndex].Center - Projectile.velocity * 2f;
					Projectile.gfxOffY = Main.npc[TargetIndex].gfxOffY;
					if (Projectile.timeLeft % 30f == 0f)
						Main.npc[TargetIndex].HitEffect(0, 1.0);
				}
				else 
					Projectile.Kill();
				return false;
			}
			return base.PreAI();
		}

		private void CheckColliding(ref int Damage)
		{
			const int range = 40;
			var list = Main.projectile.Where(x => x.active && x.type == oppositearrow && ((x.Distance(Projectile.Center) <= range && x.ai[0] == STRUCK_TILE) || (x.ai[1] == TargetIndex && x.ai[0] == STRUCK_NPC && Projectile.ai[0] == STRUCK_NPC)));
			if (list.Any()) 
			{
				foreach (var proj in list) 
				{
					for (int num621 = 0; num621 < 20; num621++)
					{
						int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.OrangeTorch, 0f, 0f, 100, default, 2f);
						Main.dust[num622].velocity *= 3f;
						if (Main.rand.NextBool(2))
							Main.dust[num622].noGravity = true;
						Main.dust[num622].scale = 0.5f;
					}
					if (!Main.dedServ)
						ParticleHandler.SpawnParticle(new ElectricalBurst(proj.Center, 1, 0));
					proj.Kill();
				}
				doDamageEffect = true;

				Projectile.timeLeft = 8;
				Damage = (int)(Damage * 1.5);
				ProjectileExtras.Explode(Projectile.whoAmI, 120, 120, delegate 
				{
					SoundEngine.PlaySound(SoundID.Item93, Projectile.position);
					SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
					for (int num621 = 0; num621 < 40; num621++)
					{
						int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, default, 2f);
						Main.dust[num622].velocity *= 3f;
						if (Main.rand.NextBool(2))
							Main.dust[num622].noGravity = true;
						Main.dust[num622].scale = 0.5f;
					}
					DustHelper.DrawStar(new Vector2(Projectile.Center.X, Projectile.Center.Y), 226, pointAmount: 5, mainSize: 2.5f, dustDensity: 2, pointDepthMult: 0.3f, noGravity: true);
				});
				if (!Main.dedServ)
					ParticleHandler.SpawnParticle(new ElectricalBurst(Projectile.Center, 1, 0));
				Projectile.Kill();
			}
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			State = STRUCK_NPC;
			TargetIndex = target.whoAmI;
			Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
			Projectile.netUpdate = true;
			CheckColliding(ref damage);
			int num31 = 1;
			Point[] array2 = new Point[num31];
			int num32 = 0;

			for (int n = 0; n < 1000; n++) 
			{
				if (n != Projectile.whoAmI && Main.projectile[n].active && Main.projectile[n].owner == Main.myPlayer && Main.projectile[n].type == Projectile.type && Main.projectile[n].ai[0] == 1f && Main.projectile[n].ai[1] == target.whoAmI) {
					array2[num32++] = new Point(n, Main.projectile[n].timeLeft);
					if (num32 >= array2.Length)
						break;
				}
			}

			if (num32 >= array2.Length) 
			{
				int num33 = 0;
				for (int num34 = 1; num34 < array2.Length; num34++) {
					if (array2[num34].Y < array2[num33].Y)
						num33 = num34;
				}
				Main.projectile[array2[num33].X].Kill();
			}
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.velocity.Y += 0.25f;

			const float homingSpeed = 14f;
			const int maxDist = 240;
			if (targetProj == null)
			{
				//Find a valid homing target
				foreach (Projectile proj in Main.projectile)
				{
					if (proj.active && proj.type == oppositearrow && proj.ai[0] > 0 && Projectile.Distance(proj.Center) <= maxDist)
					{
						targetProj = proj;
						break;
					}
				}
			}
			else if (!targetProj.active)
				targetProj = null;
			else if (Projectile.Distance(targetProj.Center) <= maxDist && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, targetProj.position, targetProj.width, targetProj.height))
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(targetProj.Center) * homingSpeed, 0.075f);
		}
	}
}