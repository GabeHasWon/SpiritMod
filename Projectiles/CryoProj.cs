using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;

namespace SpiritMod.Projectiles;

class CryoProj : ModProjectile
{
	public override string Texture => SpiritMod.EMPTY_TEXTURE;

	public override void SetDefaults()
	{
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = 4;
		Projectile.timeLeft = 500;
		Projectile.height = 30;
		Projectile.width = 30;
		Projectile.extraUpdates = 1;
		Projectile.ignoreWater = true;
	}

	public override bool? CanCutTiles() => false;

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.width = Projectile.height = 100 + ((int)player.GetSpiritPlayer().cryoTimer + 1) / 4;
		Projectile.Center = new Vector2(player.Center.X + (player.direction > 0 ? 0 : 0), player.position.Y + 30);

		var list = Main.npc.Take(Main.maxNPCs).Where(x => x.Hitbox.Intersects(Projectile.Hitbox));
		foreach (var npc in list)
			if (!npc.friendly)
				npc.AddBuff(ModContent.BuffType<MageFreeze>(), 20);

		for (int k = 0; k < 4; k++)
		{
			Vector2 center = Projectile.Center;
			Vector2 vector2 = Vector2.UnitY.RotatedByRandom(MathF.Tau) * new Vector2(Projectile.height, Projectile.height) * Projectile.scale * 1.45f / 2f;
			float num8 = player.miscCounter / 60f;
			float num7 = 2.09439516f;
			for (int i = 0; i < 3; i++)
			{
				int num6 = Dust.NewDust(center + vector2, 0, 0, DustID.Snow, 0f, 0f, 100, default, 0.7f);
				Main.dust[num6].noGravity = true;
				Main.dust[num6].velocity = Vector2.Zero;
				Main.dust[num6].noLight = true;
				Main.dust[num6].position = center + vector2 + (num8 * 6.28318548f + num7 * (float)i).ToRotationVector2() * 12f;
			}
		}
	}
}