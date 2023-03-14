using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.DoT;
using SpiritMod.Particles;
using SpiritMod.Utilities;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.CryoliteSet.CryoSword
{
	public class CryoSwordProj : ModProjectile
	{
		private double UseCounter
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}

		private double Radians
		{
			get => Projectile.ai[1];
			set => Projectile.ai[1] = (float)value;
		}
		private readonly Vector2 Size = new(46, 50);

		private bool ReverseSwing => Projectile.velocity.X < 0;
		private bool StrongAttack => Math.Abs(Projectile.velocity.X) > 1;

		public override string Texture => "SpiritMod/Items/Sets/CryoliteSet/CryoSword/CryoSword";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rimehowl");
			ProjectileID.Sets.TrailCacheLength[Type] = 10;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.width = Projectile.height = 48;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 1;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			player.heldProj = Projectile.whoAmI;

			int startBias = StrongAttack ? 38 : 24;

			int degrees = (int)((double)(UseCounter / player.itemAnimationMax * -180) + 55) * player.direction * (int)player.gravDir;
			if (ReverseSwing)
				degrees = (int)((float)Math.PI - (int)((double)(UseCounter / player.itemAnimationMax * -180) + 55) * player.direction * (int)player.gravDir);
			
			degrees -= startBias * player.direction;
			
			if (player.direction == 1)
				degrees += 180;

			Radians = degrees * (Math.PI / 180);

			float amount = 0.1f * (player.HeldItem.useTime / player.itemTimeMax);
			if (StrongAttack)
			{
				amount = (float)(player.itemAnimationMax - (float)player.itemAnimation) / player.itemAnimationMax;
				amount = EaseFunction.EaseCircularInOut.Ease(amount);
			}

			UseCounter = MathHelper.Lerp((float)UseCounter, player.itemAnimationMax, amount);

			Projectile.position.X = player.Center.X - (int)(Math.Cos(Radians * 0.96) * Size.X) - (Projectile.width / 2);
			Projectile.position.Y = player.Center.Y - (int)(Math.Sin(Radians * 0.96) * Size.Y) - (Projectile.height / 2);

			Projectile.rotation = (float)Radians + 3.9f + ((GetEffects(player) == SpriteEffects.FlipHorizontally) ? MathHelper.PiOver2 : 0);

			if (player.itemTime < 2)
				Projectile.active = false;

			player.itemRotation = MathHelper.WrapAngle((float)Radians + ((player.direction < 0) ? 0 : MathHelper.Pi));

			if (UseCounter < (player.itemTimeMax - 4)) //Do fancy dusts
			{
				Vector2 vel = player.DirectionTo(Projectile.Center).RotatedBy(1.57f * ((GetEffects(player) == SpriteEffects.FlipHorizontally) ? -1 : 1));
				Vector2 dustPos = player.Center + (player.DirectionTo(Projectile.Center) * (Size.Length() * Main.rand.NextFloat(0.5f, 1.1f))) + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, 4.0f));
				
				Dust dust = Dust.NewDustPerfect(dustPos, DustID.DungeonSpirit, Vector2.Zero, 0, Color.White, Main.rand.NextFloat(0.5f, 1.0f));
				dust.velocity = vel * Main.rand.NextFloat(0.0f, 3.0f);
				dust.noGravity = true;

				if (Main.rand.NextBool(7) && !Main.dedServ)
					ParticleHandler.SpawnParticle(new CryoParticle(player.Center + (player.DirectionTo(Projectile.Center) * Size.Length()), vel * 3, Color.LightBlue with { A = 80 }, Main.rand.NextFloat(1.0f, 3.0f), 30));
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Main.player[Projectile.owner].Center, Projectile.Center) ? true : base.Colliding(projHitbox, targetHitbox);

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
			=> hitDirection = Math.Sign(target.Center.X - Main.player[Projectile.owner].Center.X);

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Vector2 hitPos = target.getRect().ClosestPointInRect(Projectile.Center);
			Player player = Main.player[Projectile.owner];

			for (int i = 0; i < 18; i++)
			{
				Vector2 vel = player.DirectionTo(Projectile.Center).RotatedBy(1.57f * ((GetEffects(player) == SpriteEffects.FlipHorizontally) ? -1 : 1));

				Dust dust = Dust.NewDustPerfect(hitPos, Main.rand.NextBool(2) ? DustID.GemDiamond : DustID.DungeonSpirit, (vel * Main.rand.NextFloat(2.0f, 7.0f)).RotatedByRandom(0.1f), 0, Color.White, Main.rand.NextFloat(0.5f, 1.5f));
				dust.noGravity = true;

				if (Main.rand.NextBool(5) && !Main.dedServ)
					ParticleHandler.SpawnParticle(new CryoParticle(player.Center + (player.DirectionTo(Projectile.Center) * Size.Length()), (vel * 3).RotatedByRandom(0.3f), Color.LightBlue, Main.rand.NextFloat(1.0f, 3.0f), 30));
			}

			if (Main.rand.NextBool(4))
				target.AddBuff(ModContent.BuffType<CryoCrush>(), 300);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			float quoteant = (float)((float)UseCounter / player.itemAnimationMax);

			SpriteEffects effects = GetEffects(player);

			Vector2 origin = new Vector2(0, Size.Y);
			if (effects == SpriteEffects.FlipHorizontally)
				origin = Size;

			Texture2D smear = ModContent.Request<Texture2D>(Texture + "_Smear").Value;
			Rectangle sFrame = new Rectangle(0, smear.Height / 6 * (int)((float)quoteant * 7f), smear.Width, (smear.Height / 6) - 2);

			//Draw a smear
			Main.EntitySpriteDraw(smear, player.Center + (Vector2.UnitX * Size.Length() * player.direction) - Main.screenPosition, sFrame, Color.Lerp(Color.White, Color.Blue, quoteant) with { A = 100 },
				MathHelper.Pi * MathHelper.Clamp(-player.direction, 0, 1), new Vector2(sFrame.Width, sFrame.Height / 2), Projectile.scale, SpriteEffects.None, 0);

			//Draw the projectile
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, player.Center - Main.screenPosition, new Rectangle(0, 0, (int)Size.X, (int)Size.Y), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects, 0);
			//Draw the projectile glowmasks
			for (int i = 0; i < 2; i++)
			{
				var glowTex = (i == 0) ? ModContent.Request<Texture2D>(Texture + "_Glow").Value : ModContent.Request<Texture2D>(Texture + "_Pulse").Value;
				Color color = Color.White;

				if (i > 0)
				{
					color *= 1f - quoteant;

					for (int k = 0; k < (StrongAttack ? 7 : Projectile.oldPos.Length); k++)
					{
						Vector2 drawPos = player.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
						Color trailCol = Projectile.GetAlpha(color.MultiplyRGBA(Color.LightBlue)) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * (quoteant + 0.5f);
						Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, new Rectangle(0, 0, (int)Size.X, (int)Size.Y), trailCol, Projectile.oldRot[k], origin, Projectile.scale, effects, 0);
					}
				}

				Main.EntitySpriteDraw(glowTex, player.Center - Main.screenPosition, new Rectangle(0, 0, (int)Size.X, (int)Size.Y), Projectile.GetAlpha(color), Projectile.rotation, origin, Projectile.scale, effects, 0);
			}

			return false;
		}

		private SpriteEffects GetEffects(Player player) => ((player.direction * (int)player.gravDir * Projectile.velocity.X) < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
	}
}