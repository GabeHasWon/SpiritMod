using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.SeaSnailVenom
{
	public class Sea_Snail_Poison_Projectile : ModProjectile
	{
		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sea Snail's Poison");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; 
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(8);
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.timeLeft = 180;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y + player.height - 34), player.width, 6, DustID.Venom, 0, 0, 220, new Color(), .4f);
				dust.fadeIn = 1f;
				dust.noGravity = true;
				dust.velocity *= 0.2f;
			}
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			width = height = 8;
			return true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override bool? CanCutTiles() => false;

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(70, 60 * 4);
	}
}
