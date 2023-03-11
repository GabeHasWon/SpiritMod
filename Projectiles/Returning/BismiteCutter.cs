using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Buffs.DoT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.GameContent;

namespace SpiritMod.Projectiles.Returning
{
	public class BismiteCutter : ModProjectile
	{
		private float hitCounter = 0;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Bismite Cutter");

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.aiStyle = 3;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 3;
			Projectile.timeLeft = 600;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			Projectile.rotation += 0.08f;

			for (int i = 0; i < 2; i++)
			{
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Plantera_Green, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].scale = 0.9f;
			}

			if (hitCounter > 0)
				hitCounter -= 0.01f;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.NextBool(5))
			{
				target.AddBuff(ModContent.BuffType<FesteringWounds>(), 180);
				hitCounter = 1f;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			if (hitCounter > 0)
			{
				Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Pulse").Value;
				Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White * hitCounter, Projectile.rotation, glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}
			return false;
		}
		public override void SendExtraAI(BinaryWriter writer) => writer.Write(hitCounter);

		public override void ReceiveExtraAI(BinaryReader reader) => hitCounter = reader.Read();
	}
}
