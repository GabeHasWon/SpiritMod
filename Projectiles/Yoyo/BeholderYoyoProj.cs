using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Yoyo
{
	public class BeholderYoyoProj : ModProjectile
	{
		private int ManaCounter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.BeholderYoyo.DisplayName");

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Valor);
			AIType = ProjectileID.Valor;
			Projectile.timeLeft = 1200;
			Projectile.penetrate = -1;
		}

		public override void AI()
		{
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.PurificationPowder, Scale: 0.6f);
				dust.noGravity = true;

				Vector2 vector = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.0f, 4.0f);
				dust.velocity = vector;
				dust.position = Projectile.Center - (vector * 24f);
			}

			if (Main.myPlayer == Projectile.owner)
			{
				Player player = Main.player[Projectile.owner];
				if (player.channel && player.statMana > 0)
				{
					if (++ManaCounter >= 6)
					{
						ManaCounter = 0;
						player.statMana--;
					}

					if (player.controlUseTile && Projectile.frameCounter <= 0)
					{
						for (int n = 0; n < Main.maxNPCs; n++)
						{
							NPC npc = Main.npc[n];
							if (Projectile.Distance(npc.Center) < 640 && npc.CanBeChasedBy(Projectile) && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
							{
								CastMagic(Projectile.DirectionTo(npc.Center) * 10);

								Projectile.velocity = Projectile.DirectionFrom(npc.Center) * 8;
								Projectile.frameCounter = 20;

								Projectile.netUpdate = true;
								break;
							}
						}
					}
					if (Projectile.frameCounter > 0)
						Projectile.frameCounter--;
				}
				if (player.statMana <= 0)
					player.channel = false;
			}
		}

		private void CastMagic(Vector2 velocity)
		{
			Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ProjectileID.Fireball, Projectile.damage, Projectile.knockBack / 2f, Projectile.owner, 0f, 0f);
			proj.friendly = true;
			proj.hostile = false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}
			return false;
		}

		public override void PostDraw(Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
		}
	}
}