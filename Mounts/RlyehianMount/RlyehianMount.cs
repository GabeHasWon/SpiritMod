using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.Mount;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mounts.RlyehianMount
{
	public class RlyehianMount : ModMount
	{
		private int flightTime = 0;
		private int attackCooldown = 0;

		private readonly int mountStatesMax = 5;
		private int mountState = FLOAT;
		private const int NORMAL = 0;
		private const int FLOAT = 2;
		private const int ATTACKING = 4;

		public override void SetStaticDefaults()
		{
			MountData.spawnDust = DustID.Shadowflame;
			MountData.buff = ModContent.BuffType<RlyehianMountBuff>();
			MountData.heightBoost = 10;
			MountData.fallDamage = 0f;
			MountData.runSpeed = 5f;
			MountData.dashSpeed = 8f;
			MountData.flightTimeMax = 300;
			MountData.fatigueMax = 320;
			MountData.jumpHeight = 10;
			MountData.acceleration = 0.4f;
			MountData.jumpSpeed = 10f;
			MountData.blockExtraJumps = true;
			MountData.totalFrames = 25; //25 frames total, 5 frames per column
			MountData.usesHover = false;

			int[] array = new int[MountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
				array[l] = 0;

			MountData.playerYOffsets = array;

			MountData.xOffset = 0;
			MountData.yOffset = -4;
			MountData.bodyFrame = 0;
			MountData.playerHeadOffset = 0;

			MountData.frontTexture = ModContent.Request<Texture2D>("SpiritMod/Mounts/RlyehianMount/RlyehianMount");
			if (Main.netMode != NetmodeID.Server)
			{
				MountData.textureWidth = MountData.frontTexture.Width();
				MountData.textureHeight = MountData.frontTexture.Height();
			}
		}

		public override void UpdateEffects(Player player)
		{
			player.gravity = 0;
			player.fallStart = (int)(player.position.Y / 16.0);
			int floatHeight = 18;
			if (player.controlJump && flightTime > 0)
			{
				floatHeight = 7 * 16;
				flightTime--;
				mountState = FLOAT;
			}
			else mountState = NORMAL;

			ControlFloatHeight(player, floatHeight);

			if (mountState == FLOAT)
			{
				NPC target = GetTarget(player);
				if (target != null)
				{
					mountState = ATTACKING;
					int cooldownTime = 14;
					attackCooldown = ++attackCooldown % cooldownTime;
					if (attackCooldown == 0)
					{
						Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<RlyehianMount_Proj>(), 
							10, 2, player.whoAmI, (int)(target.Center - player.Center).Length());
						proj.rotation = player.AngleTo(target.Center);
					}
				}
			}
		}

		private void ControlFloatHeight(Player player, int height)
		{
			float baseHeight = height;
			float moveSpeed = 0.2f;
			float maxSpeed = 2f;

			Vector2 orig = player.Center;
			int tileY = GetTileAt(orig, 0, out bool _, false) * 16;
			float gotoY = tileY - baseHeight;

			Vector2 goPos = new Vector2(player.MountedCenter.X, gotoY - 16);

			if (player.DistanceSQ(goPos) > (maxSpeed + 1) * (maxSpeed + 1))
				player.velocity.Y += MathHelper.Clamp(player.DirectionTo(goPos).Y * moveSpeed, -maxSpeed, maxSpeed);
			else
			{
				player.velocity.Y *= 0.8f;
				if (mountState != FLOAT)
					flightTime = MountData.flightTimeMax;
			}
		}

		private static NPC GetTarget(Player player)
		{
			int detectWidth = player.width * 3;
			Rectangle detectRange = new Rectangle((int)player.getRect().Center.X - (detectWidth / 2), (int)player.getRect().Bottom, detectWidth, 8 * 16);
			foreach (NPC npc in Main.npc)
			{
				if (npc.active && !npc.friendly && npc.getRect().Bottom >= player.getRect().Bottom && 
					detectRange.Intersects(npc.getRect()) && Collision.CanHitLine(npc.Center, 0, 0, player.Center, 0, 0))
					return npc;
			}
			return null;
		}

		private static int GetTileAt(Vector2 searchPos, int xOffset, out bool liquid, bool up = false)
		{
			int tileDist = (int)(searchPos.Y / 16f);
			liquid = true;

			while (true)
			{
				tileDist += !up ? 1 : -1;

				if (tileDist < 20)
					return -1;

				Tile t = Framing.GetTileSafely((int)(searchPos.X / 16f) + xOffset, tileDist);
				if (t.HasTile && Main.tileSolid[t.TileType])
				{
					liquid = false;
					break;
				}
				else if (t.LiquidAmount > 155)
					break;
			}
			return tileDist;
		}

		public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
		{
			int framesPerColumn = MountData.totalFrames / mountStatesMax;
			int maxFrame = (framesPerColumn * mountState) + framesPerColumn;

			mountedPlayer.mount._frameCounter += 0.2f;
			bool inTransition = (int)mountedPlayer.mount._frameCounter < (maxFrame - framesPerColumn);
			mountedPlayer.mount._frameCounter %= maxFrame;
			int lowerBound = inTransition ? (maxFrame - (framesPerColumn * 2)) : (maxFrame - framesPerColumn);
			if (lowerBound < 0)
				lowerBound = 0;

			//Apply the lower bound
			if ((int)mountedPlayer.mount._frameCounter < lowerBound)
			{
				mountedPlayer.mount._frameCounter = lowerBound;
			}
			mountedPlayer.mount._frame = (int)mountedPlayer.mount._frameCounter;
			return false;
		}

		public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
		{
			drawPlayer.GetModPlayer<RlyehianMountPlayer>().usingMount = true;

			texture = ModContent.Request<Texture2D>("SpiritMod/Mounts/RlyehianMount/RlyehianMount").Value;

			int verticalFrame = drawPlayer.mount._frame % mountStatesMax;
			int horizontalFrame = (int)(drawPlayer.mount._frame / mountStatesMax);
			Rectangle sourceRect = new Rectangle(56 * horizontalFrame, 46 * verticalFrame, 54, 44);

			SpriteEffects effect = drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			int frameOffX = (drawPlayer.direction == 1) ? -30 : -24;
			DrawData data = new DrawData(texture, drawPosition + new Vector2(frameOffX, 0), sourceRect, drawColor, 0f, drawOrigin, 1f, effect, 0);
			playerDrawData.Add(data);
			return false;
		}

		public override void SetMount(Player player, ref bool skipDust)
		{
			skipDust = true;
			DustHelper.DrawDiamond(player.Center, 173, 10);
		}
	}
}
