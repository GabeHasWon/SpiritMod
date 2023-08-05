using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SpiritMod.Items.Accessory.Bauble
{
	public class IceReflector : ModProjectile
	{
		private readonly int maxAlpha = 150;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Ice Reflector");

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(44, 60);
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = Bauble.shieldTime;
		}

		public override void AI()
		{
			Projectile.alpha = (int)MathHelper.Max(maxAlpha, Projectile.alpha - 10);

			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0f, 0f);
			dust.scale = 0.5f;
			dust.noGravity = true;
			dust.noLight = true;

			Projectile.Center = Main.player[Projectile.owner].Center;
			Projectile.gfxOffY = Main.player[Projectile.owner].gfxOffY;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
			=> overPlayers.Add(index);

		public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

		public override bool? CanDamage() => false;

		public override bool? CanCutTiles() => false;
	}
}
