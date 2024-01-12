using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using SpiritMod.Projectiles;

namespace SpiritMod.NPCs.ArterialGrasper
{
	public class TendonEffect : ModProjectile
	{
		public int Parent { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

		int TrapperID => ModContent.NPCType<CrimsonTrapper>();

		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.tileCollide = true;
			Projectile.alpha = 0;
			Projectile.extraUpdates = 1;
		}

		bool stuck = false;

		public override void AI()
		{
			if (!Main.npc[Parent].active || Main.npc[Parent].type != TrapperID)
			{
				Projectile.Kill();
				return;
			}
			Projectile.timeLeft++;

			if (!stuck)
				Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			else
				Projectile.velocity = Vector2.Zero;
		}
		public override bool PreDraw(ref Color lightColor)
		{
			NPC parent = Main.npc[Parent];
			if (parent.active && parent.type == TrapperID)
				ProjectileExtras.DrawChain(Projectile.whoAmI, parent.Center, "SpiritMod/NPCs/ArterialGrasper/" + Name + "_Chain");
			return false;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCsAndTiles.Add(index);

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			stuck = true;
			if (oldVelocity.X != Projectile.velocity.X) //if its an X axis collision
			{
				if (Projectile.velocity.X > 0)
					Projectile.rotation = 1.57f;
				else
					Projectile.rotation = 4.71f;
			}
			if (oldVelocity.Y != Projectile.velocity.Y) //if its a Y axis collision
			{
				if (Projectile.velocity.Y > 0)
					Projectile.rotation = 3.14f;
				else
					Projectile.rotation = 0f;
			}
			return false;
		}
	}
}