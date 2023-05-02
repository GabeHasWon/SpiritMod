using SpiritMod.GlobalClasses.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Pet
{
	public class PhantomPet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantom");
			Main.projFrames[Projectile.type] = 6;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.ZephyrFish);
			AIType = ProjectileID.ZephyrFish;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			player.GetModPlayer<PetPlayer>().PetFlag(Projectile);
			player.zephyrfish = false; //Relic from AIType
		}
	}
}