using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class SoulPotionWard : ModProjectile
	{
		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Soul Guard");

		public override void SetDefaults()
		{
			Projectile.width = 300;
			Projectile.height = 300;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 120;
			Projectile.light = 0.75f;
			Projectile.extraUpdates = 1;
			Projectile.alpha = 255;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
		}

		public override void AI()
		{
			Rectangle rect = new Rectangle((int)Projectile.Center.X, (int)Projectile.position.Y, 300, 300);
			for (int index1 = 0; index1 < 200; index1++) {
				if (rect.Contains(Main.npc[index1].Center.ToPoint()))
					Main.npc[index1].AddBuff(ModContent.BuffType<SoulBurn>(), 240);
			}
		}
	}
}
