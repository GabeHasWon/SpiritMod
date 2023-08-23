using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.GlobalClasses.Players;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.DonatorItems
{
	public class AbominationPet : ModProjectile
	{
		private float frameCounter;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Wishbone");
			Main.projFrames[Type] = 7;
			Main.projPet[Type] = true;
			ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Type], 6)
				.WithOffset(-8, 0)
				.WithCode(DelegateMethods.CharacterPreview.Float);
		}

		public override void SetDefaults()
		{
			Projectile.netImportant = true;
			Projectile.Size = new Vector2(40);
			Projectile.aiStyle = 144;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.manualDirectionChange = true;
			AIType = ProjectileID.DD2PetGato;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			player.GetModPlayer<PetPlayer>().PetFlag(Projectile);

			player.petFlagDD2Gato = false;

			Projectile.frame = (int)(frameCounter += .2f) % Main.projFrames[Type];
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle drawFrame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, drawFrame, Projectile.GetAlpha(lightColor), Projectile.rotation, drawFrame.Size() / 2, Projectile.scale, effects, 0);
			return false;
		}
	}
}
