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
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Support Saucer");
			Main.projFrames[Projectile.type] = 5;
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

			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0, 0, 0, default, .5f);
			dust.noGravity = true;
			dust.velocity = -Vector2.UnitY;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, texture.Height / Main.projFrames[Type]);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}
	}
}