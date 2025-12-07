using SpiritMod.Items;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.Projectiles;

public class ExplosivesGlobalProjectile : GlobalProjectile
{
	public override bool InstancePerEntity => true;

	public override void OnSpawn(Projectile projectile, IEntitySource source)
	{
		if (source is EntitySource_ItemUse_WithAmmo parentSource)
			if (parentSource.Entity is Player player && player.GetSpiritPlayer().longFuse && projectile.friendly && ProjectileID.Sets.Explosive[projectile.type])
				projectile.timeLeft = (int)(projectile.timeLeft * 1.5f); //Makes it last 150% longer
	}
}
