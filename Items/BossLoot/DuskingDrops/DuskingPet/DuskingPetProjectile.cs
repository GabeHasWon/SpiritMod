using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.DuskingDrops.DuskingPet
{
	public class DuskingPetProjectile : ModProjectile
	{
		private Player Owner => Main.player[Projectile.owner];

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Minor Shadowflame");
			Main.projFrames[Projectile.type] = 5;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Truffle);
			Projectile.aiStyle = 0;
			Projectile.width = 24;
			Projectile.height = 56;
			Projectile.light = 0;
			Projectile.tileCollide = false;

			AIType = 0;
		}

		public override void AI()
		{
			Owner.GetModPlayer<GlobalClasses.Players.PetPlayer>().PetFlag(Projectile);

			FollowPlayer();
			Projectile.spriteDirection = Owner.direction;

			if (Main.rand.NextBool(13))
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame);
		}

		private void FollowPlayer()
		{
			Projectile.velocity = Vector2.Zero;

			Projectile.frameCounter++;
			Projectile.frame = Projectile.frameCounter / 5 % 5;

			Vector2 restingPos = Owner.Center - new Vector2(30 * Owner.direction, 20);
			if (!((int)Projectile.Center.X == (int)restingPos.X && (int)Projectile.Center.Y == (int)restingPos.Y))
				Projectile.Center = Vector2.Lerp(Projectile.Center, restingPos, 0.1f);

			Lighting.AddLight(Projectile.Center, 2f, 0.2f, 1.9f);

			if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
				Projectile.alpha = (int)MathHelper.Lerp(Projectile.alpha, 155, 0.1f);
			else
				Projectile.alpha = (int)MathHelper.Lerp(Projectile.alpha, 0, 0.1f);
		}
		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override void PostDraw(Color lightColor)
		{
			Rectangle frame = new Rectangle(0, TextureAssets.Projectile[Projectile.type].Height() / Main.projFrames[Projectile.type] * Projectile.frame, 
				TextureAssets.Projectile[Projectile.type].Width(), TextureAssets.Projectile[Projectile.type].Height() / Main.projFrames[Projectile.type]);
			//Draw a glowmask
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.position - Main.screenPosition, frame, 
				Projectile.GetAlpha(Color.White), Projectile.rotation, Vector2.Zero, Projectile.scale, (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
		}
	}
}
