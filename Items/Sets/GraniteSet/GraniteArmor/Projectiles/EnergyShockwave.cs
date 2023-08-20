using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GraniteSet.GraniteArmor.Projectiles
{
	public class EnergyShockwave : ModProjectile
	{
		private const int timeLeftMax = 50;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Energy Stomp");
			Main.projFrames[Type] = 3;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = timeLeftMax;
			Projectile.height = 20;
			Projectile.width = 20;
			Projectile.tileCollide = false;
			Projectile.hide = true;
			Projectile.penetrate = -1;
			AIType = ProjectileID.Bullet;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void OnSpawn(IEntitySource source) => Projectile.frame = Main.rand.Next(Main.projFrames[Type]);

		public override void AI()
		{
			Projectile.velocity *= 0.95f;

			Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X < 0) ? -1 : 1;
			Projectile.scale -= 1 / (timeLeftMax * 1.5f);
			Projectile.rotation += 0.1f * Projectile.direction;

			Vector2 position = Projectile.position + new Vector2(Projectile.width * Main.rand.NextFloat(1.0f), Projectile.height * Main.rand.NextFloat(1.0f));
			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustPerfect(position, DustID.Electric, Vector2.Zero, 0, default, 0.8f);
				dust.noGravity = true;
			}
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
			=> behindNPCsAndTiles.Add(index);

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, texture.Height / Main.projFrames[Type]);

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] + (Projectile.Size / 2) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.White) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * .5f;
				float scale = Projectile.scale;

				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, frame, color, Projectile.rotation, frame.Size() / 2, scale, (Projectile.direction < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
			}
			return false;
		}
	}
}
