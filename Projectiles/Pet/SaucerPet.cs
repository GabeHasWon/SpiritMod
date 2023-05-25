using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.GlobalClasses.Players;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Pet
{
	public class SaucerPet : ModProjectile
	{
		private float frameCounter;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Support Saucer");
			Main.projFrames[Projectile.type] = 4;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.ZephyrFish);
			AIType = ProjectileID.ZephyrFish;
			Projectile.width = 40;
			Projectile.height = 30;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			player.GetModPlayer<PetPlayer>().PetFlag(Projectile);
			player.zephyrfish = false; //Relic from AIType

			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustPerfect(Projectile.position + new Vector2(Projectile.width / 2, Projectile.height) + (Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 4), DustID.Electric, Vector2.Zero, 0, default, .5f);
				dust.noGravity = true;
				dust.velocity = Vector2.UnitY;
			}

			Projectile.frame = (int)(frameCounter += .2f) % Main.projFrames[Type];
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}
	}
}