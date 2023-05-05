using SpiritMod.GlobalClasses.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.DonatorItems
{
	public class DemonicBlob : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wishbone");
			Main.projFrames[Projectile.type] = 27;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.netImportant = true;
			Projectile.width = 45;
			Projectile.height = 62;
			Projectile.scale = 0.85f;
			Projectile.aiStyle = 144;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.manualDirectionChange = true;
			AIType = ProjectileID.DD2PetGato;
		}

		private int animationCounter;
		private int frame;
		public override void AI()
		{
			Main.player[Projectile.owner].GetModPlayer<PetPlayer>().PetFlag(Projectile);

			if (++animationCounter >= 6)
			{
				animationCounter = 0;

				if (++frame >= Main.projFrames[Projectile.type])
					frame = Main.rand.NextBool(2) ? 9 : 0;
			}
			Projectile.frameCounter = 2;
			Projectile.frame = frame;
		}
	}
}
