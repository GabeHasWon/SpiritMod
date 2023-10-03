using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.NPCs.Boss.MoonWizard.Projectiles;

namespace SpiritMod.Projectiles.Magic
{
	public class JellynautOrbiter : ModProjectile
	{
		private float alphaCounter;

		public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Arcane Jellyfish");
            Main.projFrames[Projectile.type] = 5;
        }

		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
            Projectile.hide = false;
			Projectile.timeLeft = 9999;
		}

		public override void AI()
        {
			if (Projectile.timeLeft > 30)
			{
				if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
					Projectile.alpha = Math.Max(128, Projectile.alpha);
				else
					Projectile.alpha = Math.Max(0, Projectile.alpha - 25);

				if (!Main.player[Projectile.owner].GetModPlayer<MyPlayer>().jellynautHelm)
					Projectile.timeLeft = 30;
			}

			if (Projectile.Distance(Main.player[Projectile.owner].Center) > 40)
			{
				Vector2 unitY = Projectile.DirectionTo(Main.player[Projectile.owner].Center);
				if (unitY.HasNaNs())
					unitY = Vector2.UnitY;

				Projectile.velocity = (Projectile.velocity * 9f + unitY * 7.5f) / 10f;
			}

			if (++Projectile.frameCounter > 4)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
			}

			alphaCounter += .04f;
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			Lighting.AddLight(new Vector2(Projectile.Center.X, Projectile.Center.Y), 0.075f * 2, 0.231f * 2, 0.255f * 2);
		}

		public override bool? CanDamage() => false;

		public override bool PreDraw(ref Color lightColor)
        {
            float sineAdd = (float)Math.Sin(alphaCounter) + 3;
            SpriteEffects spriteEffects = (Projectile.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D ripple = Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_49").Value;
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Color drawColor = Projectile.GetAlpha(new Color((int)(7.5f * sineAdd), (int)(16.5f * sineAdd), (int)(18f * sineAdd), 0));

			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, spriteEffects, 0);
			Main.EntitySpriteDraw(ripple, Projectile.Center - Main.screenPosition, null, drawColor, 0, ripple.Size() / 2f, Projectile.scale * .5f, spriteEffects, 0);
            
			return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item69, Projectile.Center);

			if (Projectile.owner == Main.myPlayer)
			{
				Vector2 velocity = Projectile.DirectionTo(Main.MouseWorld) * 12f;

				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, velocity, ModContent.ProjectileType<JellyfishOrbiter_Projectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
				proj.friendly = true;
				proj.hostile = false;
				proj.DamageType = DamageClass.Magic;
				proj.netUpdate = true;
			}
		}
	}
}
