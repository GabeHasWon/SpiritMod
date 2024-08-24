using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace SpiritMod.Projectiles.Thrown.Charge;

public class FrigidJavelinProj : JavelinProj
{
	internal override int ChargeTime => 125;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
	}

	public override void AI()
	{
		if (Released && Main.rand.NextBool(7))
			Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.BlueCrystalShard, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f).noGravity = true;
	}

	public override void HitNPC(NPC target, NPC.HitInfo hit, int damage) => target.AddBuff(BuffID.Frostburn, 180, true);

	public override void OnKill(int timeLeft)
	{
		if (!Released || Main.netMode == NetmodeID.Server)
			return;

		if (!Embeded)
			SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

		for (int i = 0; i < 20; i++)
			Dust.NewDustPerfect(Projectile.Center, DustID.BlueCrystalShard, -(Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(1.0f, 3.0f)).RotatedByRandom(1.5f), 0, default, 1f).noGravity = true;
	}
}
