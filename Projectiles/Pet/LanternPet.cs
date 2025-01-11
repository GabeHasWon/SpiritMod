using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.GlobalClasses.Players;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Pet
{
	public class LanternPet : ModProjectile
	{
		private float frameCounter;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Lantern");
			Main.projFrames[Type] = 6;
			Main.projPet[Type] = true;
			ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Type], 6)
				.WithSpriteDirection(-1)
				.WithCode(DelegateMethods.CharacterPreview.Float);
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.ZephyrFish);
			AIType = ProjectileID.ZephyrFish;
			Projectile.width = 46;
			Projectile.height = 46;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			player.GetModPlayer<PetPlayer>().PetFlag(Projectile);

			player.zephyrfish = false; //Relic from AIType

			if (player == Main.LocalPlayer && player.controlDown && player.releaseDown && player.doubleTapCardinalTimer[0] > 0 && player.doubleTapCardinalTimer[0] != 15)
			{
				Projectile.velocity += 5f * Projectile.DirectionTo(Main.MouseWorld);
				Projectile.netUpdate = true;
			}

			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Green, 0, 0, 0, default, .75f);
				dust.velocity = Vector2.Zero;
				dust.noGravity = true;
			}

			Lighting.AddLight((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f), 0.75f / 1.5f, 1.5f / 1.5f, 0.75f / 1.5f);
			Projectile.frame = (int)(frameCounter += .2f) % Main.projFrames[Type];
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle drawFrame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, drawFrame, Projectile.GetAlpha(lightColor), Projectile.rotation, drawFrame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}