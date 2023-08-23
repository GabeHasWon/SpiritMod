using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.GlobalClasses.Players;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Pet
{
	public class PhantomPet : ModProjectile
	{
		private float frameCounter;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Phantom");
			Main.projFrames[Type] = 4;
			Main.projPet[Type] = true;
			ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Type], 6)
				.WithSpriteDirection(-1)
				.WithCode(DelegateMethods.CharacterPreview.Float);
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.ZephyrFish);
			AIType = ProjectileID.ZephyrFish;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			player.GetModPlayer<PetPlayer>().PetFlag(Projectile);

			player.zephyrfish = false; //Relic from AIType

			Projectile.frame = (int)(frameCounter += .2f) % Main.projFrames[Type];
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects, 0);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects, 0);

			return false;
		}
	}
}