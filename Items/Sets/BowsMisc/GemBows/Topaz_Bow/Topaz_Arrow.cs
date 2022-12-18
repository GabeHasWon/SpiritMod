using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Sets.BowsMisc.GemBows.Topaz_Bow
{
	public class Topaz_Arrow : GemArrow
	{
		public Topaz_Arrow() : base(Color.Yellow, DustID.GemTopaz) { }
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Topaz Arrow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; 
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		protected override void SafeSetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.extraUpdates = 2;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57F;
			Projectile.velocity.Y += .02f;
		}
	}
}
