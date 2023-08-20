using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using SpiritMod.Projectiles.Returning;
using Terraria.Audio;

namespace SpiritMod.Items.Sets.SlingHammerSubclass
{
	public abstract class SlingHammerProj : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 90;
			Projectile.height = 90;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.ownerHitCheck = true;
		}

		protected virtual int Height => 60;
		protected virtual int Width => 60;
		protected virtual int ChargeTime => 50;
		protected virtual float ChargeRate => 0.7f;
		protected virtual int ThrownProj => ModContent.ProjectileType<SlagHammerProjReturning>();
		protected virtual float DamageMult => 1.5f;
		protected virtual int ThrowSpeed => 16;


		protected double radians = 0;
		protected int flickerTime = 0;
		protected float alphaCounter = 0;
		protected bool released = false;
		public override void AI()
		{
			alphaCounter += 0.08f;
            Player player = Main.player[Projectile.owner];
			if (!released)
			{
                Projectile.scale = MathHelper.Clamp(Projectile.ai[0] / 10, 0, 1);

				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)radians + 1.57f);
				player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (float)radians + 1.57f);
			}
            if (player.direction == 1)
            {
                radians += (double)((Projectile.ai[0] + 10) / 200);
            }
            else
            {
                radians -= (double)((Projectile.ai[0] + 10) / 200);
            }
            if (radians > 6.28)
            {
                radians -= 6.28;
            }
            if (radians < -6.28)
            {
                radians += 6.28;
            }
			Projectile.velocity = Vector2.Zero;
			if (Projectile.ai[0] % 20 == 0)
				SoundEngine.PlaySound(SoundID.Item19 with { PitchVariance = 0.1f, Volume = 0.5f }, Projectile.Center);

			if (Projectile.ai[0] < ChargeTime)
            {
                Projectile.ai[0]+= ChargeRate;
                if (Projectile.ai[0] >= ChargeTime)
                    SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);
            }
            Vector2 direction = Main.MouseWorld - player.position;
            direction.Normalize();

			Projectile.position = player.Center - (Vector2.UnitX.RotatedBy(radians) * 40) - (Projectile.Size / 2);
			player.itemTime = 4;
			player.itemAnimation = 4;
			player.itemRotation = MathHelper.WrapAngle((float)radians);

			if (player.whoAmI == Main.myPlayer)
				player.ChangeDir(Math.Sign(direction.X));

			if (!player.channel || released)
            {
                if (Projectile.ai[0] < ChargeTime || released)
                {
                    released = true;
                    Projectile.scale -= 0.15f;
                    if (Projectile.scale < 0.15f)
                    {
                        Projectile.active = false;
                    }
                }
                else
                {
                    Projectile.active = false;
                    direction *= ThrowSpeed;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, direction, ThrownProj, (int)(Projectile.damage * DamageMult), Projectile.knockBack, Projectile.owner);
				}
            }
		}
		public override bool PreDraw(ref Color lightColor)
        {
                Color color = lightColor;
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, new Rectangle(0, 0, Width, Height), color, (float)radians + 3.9f, new Vector2(0, Height), Projectile.scale, SpriteEffects.None, 0);
                if (Projectile.ai[0] >= ChargeTime)
                {
                    Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, new Rectangle(0, Height * 2, Width, Height), Color.White * 0.9f, (float)radians + 3.9f, new Vector2(0, Height), Projectile.scale, SpriteEffects.None, 1);

                    if (flickerTime < 16)
                    {
                        flickerTime++;
                        color = Color.White;
                        float flickerTime2 = (float)(flickerTime / 20f);
                        float alpha = 1.5f - ((flickerTime2 * flickerTime2 / 2) + (2f * flickerTime2));
                        if (alpha < 0)
                        {
                            alpha = 0;
                        }
                        Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, new Rectangle(0, Height, Width, Height), color * alpha, (float)radians + 3.9f, new Vector2(0, Height), Projectile.scale, SpriteEffects.None, 1);
                    }
                }
                return false;
        }

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			SafeModifyHitNPC(target, ref modifiers);
			Player player = Main.player[Projectile.owner];

			if (target.Center.X > player.Center.X)
				modifiers.HitDirectionOverride = 1;
			else
				modifiers.HitDirectionOverride = -1;
		}

		public virtual void SafeModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) { }
	}
}