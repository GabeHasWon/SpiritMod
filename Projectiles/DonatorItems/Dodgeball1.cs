using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.DonatorItems
{
	public class Dodgeball1 : ModProjectile
	{
		public override string Texture => "SpiritMod/Items/DonatorItems/DodgeBall1";

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 4;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.WoodenBoomerang);
			Projectile.extraUpdates = 1;
			Projectile.width = 30;
			Projectile.height = 30;
			AIType = ProjectileID.WoodenBoomerang;
			Projectile.penetrate = -1;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center, (Color.LightYellow with { A = 0 }) * .1f, 80, 5, PulseCircle.MovementType.OutwardsSquareRooted) { RingColor = Color.White * .1f });
			for (int i = 0; i < 8; i++)
			{
				Vector2 linePos = Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(25f, 80f));
				ParticleHandler.SpawnParticle(new ImpactLine(linePos, linePos.DirectionTo(Projectile.Center) * 2.5f, Color.White * .4f, new Vector2(.5f, 1f), 8));
			}
			SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Item/Dodgeball") with { PitchVariance = .5f }, Projectile.Center);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + (Projectile.Size / 2) + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
			}
			return false;
		}
	}
}
