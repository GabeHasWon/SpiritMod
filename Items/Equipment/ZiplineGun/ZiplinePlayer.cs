using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Equipment.ZiplineGun
{
	public class ZiplinePlayer : ModPlayer
	{
		public Vector2 directionUnit = Vector2.Zero;
		private int cooldown;

		public bool OnZipline => directionUnit != Vector2.Zero;

		public override void PreUpdateMovement()
		{
			if (OnZipline && cooldown <= 0) //The player is on a "zipline"
			{
				bool cancel = Player.controlJump || Player.controlDown || (Player.grappling[0] > -1);

				if (cancel)
				{
					cooldown = 30;

					if (Player.controlJump)
					{
						Player.velocity.Y -= Player.jumpSpeed * Player.gravDir;
						Player.jump = Player.jumpHeight;
					}
				}
				else
				{
					Vector2 adjustment = Vector2.UnitY * -0.1f; //This keeps the player in position
					float magnitude = Math.Sign(Player.velocity.X) * (7f * Player.moveSpeed);
					Player.velocity = (directionUnit * magnitude) + adjustment;
					Player.fallStart = (int)Player.position.Y / 16;

					if (Player.velocity.Length() > .5f)
						Dust.NewDustPerfect(Player.Center + (Vector2.UnitY * (Player.height / 2) * Player.gravDir), DustID.Torch, new Vector2(-Player.velocity.X * Main.rand.NextFloat(), -Main.rand.NextFloat()), 0, default, .75f);
				}
			}

			cooldown = Math.Max(0, --cooldown);
			directionUnit = Vector2.Zero;
		}

		public override void PreUpdate()
		{
			if (OnZipline) Player.justJumped = true;
		}
	}
}