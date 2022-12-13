using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BowsMisc.GemBows.Diamond_Bow
{
	public class Diamond_Arrow : GemArrow
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Diamond Arrow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; 
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.arrow = true;
			Projectile.aiStyle = 1;
			Projectile.penetrate = 2;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.arrow = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		protected override void SafeSetDefaults()
		{
			Projectile.penetrate = 2;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;

			dustType = DustID.GemDiamond;
			glowColor = Color.White;
		}
	}
}
