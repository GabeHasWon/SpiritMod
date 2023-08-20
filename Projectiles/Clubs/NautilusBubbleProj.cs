using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using System;

namespace SpiritMod.Projectiles.Clubs
{
	public class NautilusBubbleProj : ModProjectile, IDrawAdditive
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Bubble");

		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.width = Projectile.height = 42;
			Projectile.friendly = true;
			Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
			Projectile.tileCollide = false;
			Projectile.alpha = 110;
			Projectile.ArmorPenetration = 10;
		}

		public override void AI() => Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(0, -0.25f), 0.03f);

		private Vector2 WobbleScale()
		{
			float magnitude = .05f;
			Vector2 lerp = ((float)Main.timeForVisualEffects / 30).ToRotationVector2() * magnitude;
			return new Vector2(1 - lerp.X, 1 - lerp.Y) * Projectile.scale;
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

			for (int i = 0; i < 2; i++)
			{
				float lerp = Math.Max(0.1f, (float)Main.timeForVisualEffects / 30).ToRotationVector2().Y;
				spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White * .5f * (1f - (float)(i * lerp)), Projectile.rotation, glow.Size() / 2, WobbleScale(), SpriteEffects.None, 0);
			}
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2f, WobbleScale(), SpriteEffects.None, 0);
            
			return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.boss && !target.friendly && target.knockBackResist != 0f && !target.dontTakeDamage)
                target.velocity.Y -= 5.6f;
        }

        public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
			for (int i = 0; i < 20; i++)
			{
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.CryoDust>(), 0f, -2f, 0, default, 2.2f);
				Main.dust[num].noGravity = true;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].scale *= .2825f;
				if (Main.dust[num].position != Projectile.Center)
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 1f;
			}
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
    }
}
