using SpiritMod.GlobalClasses.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Pet
{
	public class ShadowPet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shadow Pup");
			Main.projFrames[Projectile.type] = 12;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Truffle);
			AIType = ProjectileID.Truffle;
			Projectile.width = 30;
			Projectile.height = 36;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			player.GetModPlayer<PetPlayer>().PetFlag(Projectile);
			player.truffle = false; //Relic from AIType
		}
	}
}