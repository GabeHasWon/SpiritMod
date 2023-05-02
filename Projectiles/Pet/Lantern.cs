using SpiritMod.GlobalClasses.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Pet
{
	public class Lantern : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lantern");
			Main.projFrames[Projectile.type] = 1;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.ZephyrFish);
			AIType = ProjectileID.ZephyrFish;
			Projectile.width = 46;
			Projectile.height = 46;
			Projectile.scale = 0.9f;
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

			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Green, 0, -1f, 0, default, 1f);
			dust.scale *= 0.5f;
			dust.noGravity = true;
			Lighting.AddLight((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f), 0.75f / 2, 1.5f / 2, 0.75f / 2);
		}
	}
}