using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Mangrove_Defender
{
	public class Earth_Slam_Falling : ModProjectile
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Projectiles.Earth_Slam_Projectile.DisplayName");

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.aiStyle = 1;
			AIType = ProjectileID.Bullet;
			Projectile.hide = true;
			Projectile.scale = 1f;
		}

		public override void OnKill(int timeLeft) => Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y - 40, 0f, 0f, ModContent.ProjectileType<Earth_Slam_Projectile>(), 23, 10f, Projectile.owner);
	}
}