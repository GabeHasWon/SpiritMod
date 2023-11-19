using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Held
{
	public class GlowStingSpear : ModProjectile
	{
		private readonly float startDistance = 38f;
		private float Distance
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.width = Projectile.height = 12;
			Projectile.alpha = 255;
			Projectile.scale = 1f;
			Projectile.ownerHitCheck = true;
			Projectile.penetrate = -1;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			player.ChangeDir(Projectile.direction);
			Projectile.timeLeft = 2;

			if (player.ItemAnimationEndingOrEnded)
			{
				if (player.controlUseTile && !player.dead && player.active && !player.frozen) //Keep the projectile alive if the player is holding right-click
				{
					if (Main.LocalPlayer == player)
					{
						Projectile.velocity = (player.HeldItem.shootSpeed * Vector2.UnitX).RotatedBy(player.AngleTo(Main.MouseWorld)).RotatedByRandom(0.12f);
						Projectile.netUpdate = true;
					}

					Projectile.ResetLocalNPCHitImmunity();
					Distance = 0;
					Projectile.alpha = 255;
					SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);

					player.itemAnimation = player.itemAnimationMax;
					player.itemTime = player.itemTimeMax;
				}
				else Projectile.active = false;
			}
			else if (!player.ItemAnimationJustStarted && Projectile.alpha > 0)
				Projectile.alpha -= 255 / 5;

			float moveDist = Math.Min(8, player.itemAnimation);
			Projectile.Center = player.Center + (Vector2.Normalize(Projectile.velocity) * (startDistance + (Distance += moveDist))) - Projectile.velocity;
			Projectile.rotation = Projectile.velocity.ToRotation() + ((Projectile.direction == -1) ? 0.785f : 2.355f);

			player.itemRotation = MathHelper.WrapAngle(player.AngleTo(Projectile.Center) - ((player.direction < 0) ? MathHelper.Pi : 0));
			player.heldProj = Projectile.whoAmI;

			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Flare_Blue, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.5f);
			int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.5f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust2].noGravity = true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			SpriteEffects effects = (Projectile.direction == -1) ? SpriteEffects.FlipHorizontally : 0;
			Vector2 origin = (effects == SpriteEffects.FlipHorizontally) ? new Vector2(texture.Width, 0) : Vector2.Zero;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects, 0);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, effects, 0);

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(3))
				target.AddBuff(ModContent.BuffType<StarFlame>(), 180);
		}
	}
}