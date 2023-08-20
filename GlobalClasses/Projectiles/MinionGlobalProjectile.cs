using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.Projectiles
{
	public class MinionGlobalProjectile : GlobalProjectile
	{
		//Bandaid fix for our minion projectiles critting
		public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (ProjectileID.Sets.MinionShot[projectile.type] && projectile.ModProjectile != null && projectile.ModProjectile.Mod == Mod)
				modifiers.SetCrit();
		}
	}
}