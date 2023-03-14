using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs;
using SpiritMod.Dusts;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BloodcourtSet.Headsplitter
{
	public class HeadsplitterProj : ModProjectile
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
		private const int Size = 50;
		private float distance = Size;

		private bool ReverseSwing => Projectile.velocity.X < 0;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Headsplitter");

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.width = Projectile.height = 48;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Main.player[Projectile.owner].Center, Projectile.Center) ? true : base.Colliding(projHitbox, targetHitbox);

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
			=> hitDirection = Math.Sign(target.Center.X - Main.player[Projectile.owner].Center.X);

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Vector2 hitPos = target.getRect().ClosestPointInRect(Projectile.Center);

			for (int i = 0; i < 8; i++)
			{
				Dust dust = Dust.NewDustPerfect(hitPos, DustID.LavaMoss, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, 5.0f), 0, Color.White, Main.rand.NextFloat(0.5f, 1.0f));
				dust.noGravity = true;
				dust.shader = GameShaders.Armor.GetSecondaryShader(93, Main.LocalPlayer);
			}
			
			if (Main.rand.NextBool(3))
			{
				if (!Main.dedServ)
					ParticleHandler.SpawnParticle(new SplitterImpact(hitPos, 1f, (ReverseSwing ? MathHelper.Pi : 0f) - (hitPos.DirectionTo(Main.player[Projectile.owner].Center).X / 5)));

				target.AddBuff(ModContent.BuffType<SurgingAnguish>(), 180);
			}
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			player.heldProj = Projectile.whoAmI;

			int degrees = (int)((double)(UseCounter / player.itemAnimationMax * -180) + 55) * player.direction * (int)player.gravDir;
			if (ReverseSwing)
				degrees = (int)((float)Math.PI - (int)((double)(UseCounter / player.itemAnimationMax * -180) + 55) * player.direction * (int)player.gravDir);
			if (player.direction == 1)
				degrees += 180;

			Radians = degrees * (Math.PI / 180);
			float amount = 0.2f * (player.HeldItem.useTime / player.itemTimeMax);
			UseCounter = MathHelper.Lerp((float)UseCounter, player.itemAnimationMax, amount);

			Projectile.position.X = player.Center.X - (int)(Math.Cos(Radians * 0.96) * Size) - (Projectile.width / 2);
			Projectile.position.Y = player.Center.Y - (int)(Math.Sin(Radians * 0.96) * Size) - (Projectile.height / 2);

			if (player.itemTime < 2)
				Projectile.active = false;
			player.itemRotation = MathHelper.WrapAngle((float)Radians + ((player.direction < 0) ? 0 : MathHelper.Pi));

			if (UseCounter < (player.itemTimeMax - 8)) //Do fancy dusts
			{
				Vector2 dustPos = player.Center + (player.DirectionTo(Projectile.Center) * (distance + 8)) + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, 4.0f));
				Dust dust = Dust.NewDustPerfect(dustPos, DustID.LavaMoss, Vector2.Zero, 0, Color.White, Main.rand.NextFloat(0.5f, 1.0f));
				dust.velocity = Vector2.Zero;
				dust.noGravity = true;
				dust.shader = GameShaders.Armor.GetSecondaryShader(93, Main.LocalPlayer);
			}

			if (distance < Size)
				distance++;

			int maxDist = Size - 4;
			bool collided = false;
			//Allow the projectile to be pushed back by solid tiles
			for (int i = 0; i < Size; i++)
			{
				Vector2 position = player.Center + (player.DirectionTo(Projectile.Center) * (distance - 8));
				if (WorldGen.SolidOrSlopedTile(Main.tile[(position / 16).ToPoint()]) && distance > maxDist)
				{
					collided = true;
					distance--;
				}
				else
				{
					if (collided)
						for (int d = 0; d < 3; d++)
							Dust.NewDustPerfect(position, ModContent.DustType<NightmareDust>(), Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.5f, 2.0f), 0, default, Main.rand.NextFloat(1.0f, 2.0f));
					break;
				}
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

			SpriteEffects effects = ((player.direction * (int)player.gravDir * Projectile.velocity.X) < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			float rotation = (float)Radians + 3.9f + ((effects == SpriteEffects.FlipHorizontally) ? MathHelper.PiOver2 : 0);

			float quoteant = (float)(distance / Size);

			Vector2 origin = new Vector2(Size * (float)(1f - quoteant), Size * quoteant);
			if (effects == SpriteEffects.FlipHorizontally)
				origin = new Vector2(Size, Size) * quoteant;

			Texture2D trail = ModContent.Request<Texture2D>("SpiritMod/Items/Sets/BloodcourtSet/Headsplitter/Trail").Value;

			quoteant = (float)((float)UseCounter / player.itemAnimationMax);
			Color color = Color.Lerp(Color.Magenta, Color.Red, quoteant) with { A = 0 };
			Vector2 scale = new Vector2((1f - quoteant) * .5f, .27f) * Projectile.scale;
			Vector2 trailPos = player.Center + (player.DirectionTo(Projectile.Center) * (distance - 14));

			//Draw the trail
			Main.EntitySpriteDraw(trail, trailPos - Main.screenPosition, null, color, (float)Radians - 4.71f, new Vector2(trail.Width * ((effects == SpriteEffects.FlipHorizontally) ? 0 : 1), trail.Height / 2), scale, effects, 0);
			//Draw the projectile
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, player.Center - Main.screenPosition, new Rectangle(0, 0, Size, Size), Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, effects, 0);
			//Draw the projectile glowmask
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, player.Center - Main.screenPosition, new Rectangle(0, 0, Size, Size), Projectile.GetAlpha(Color.White), rotation, origin, Projectile.scale, effects, 0);

			return false;
		}
	}
}