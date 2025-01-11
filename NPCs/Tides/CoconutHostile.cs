using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Tides
{
	public class CoconutHostile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
			Projectile.width = 14;
			Projectile.penetrate = 1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.height = 14;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Main.netMode != NetmodeID.Server)
			{
				Vector2 GoreVel = Projectile.velocity;
				GoreVel.X = 2f;
				GoreVel.Y *= -0.2f;
				Gore.NewGore(Projectile.GetSource_Misc("TileHit"), Projectile.position, GoreVel, Mod.Find<ModGore>("CoconutSpurtGore").Type, 1f);
				GoreVel.X = -2f;
				Gore.NewGore(Projectile.GetSource_Misc("TileHit"), Projectile.position, GoreVel, Mod.Find<ModGore>("CoconutSpurtGore").Type, 1f);
			}

			return true;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCHit18, Projectile.Center);
		}
	}
}
