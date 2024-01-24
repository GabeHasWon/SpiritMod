using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Yoyo
{
	public class BeholderYoyoProj : ModProjectile
	{
		public int ManaCounter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private int manaCounter;

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.BeholderYoyo.DisplayName");

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 4;
			ProjectileID.Sets.TrailingMode[Type] = 0;
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

			Player owner = Main.player[Projectile.owner];
			if (owner.channel && owner.statMana > 0)
			{
				manaCounter = ++manaCounter % 60;
				if (manaCounter % (60 / 10) == 0)
					owner.statMana--;

				if (owner.whoAmI == Main.myPlayer && owner.controlUseTile && Projectile.frameCounter <= 0)
				{
					var target = Main.npc.Where(x => x.Distance(Projectile.Center) < 640 && x.CanBeChasedBy(Projectile) 
						&& Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, x.position, x.width, x.height)).OrderBy(x => x.Distance(Projectile.Center)).FirstOrDefault();
					if (target != default)
					{
						CastMagic(Projectile.DirectionTo(target.Center) * 10);

						Projectile.velocity = Projectile.DirectionFrom(target.Center) * 8;
						Projectile.frameCounter = 20;
						Projectile.netUpdate = true;
					}
				}
				if (Projectile.frameCounter > 0)
					Projectile.frameCounter--;
			}
			if (owner.statMana <= 0)
				owner.channel = false;
		}

		private void CastMagic(Vector2 velocity)
		{
			Player owner = Main.player[Projectile.owner];
			owner.statMana = System.Math.Max(owner.statMana - 10, 0);

			Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ProjectileID.Fireball, Projectile.damage, Projectile.knockBack / 2f, Projectile.owner, 0f, 0f);
			proj.friendly = true;
			proj.hostile = false;
			proj.netUpdate = true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + (Projectile.Size / 2) + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
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