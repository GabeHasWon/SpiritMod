using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.Mount;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mounts.RlyehianMount
{
	public class RlyehianMount : ModMount
	{
		public override void Load() => Terraria.DataStructures.On_PlayerDrawLayers.DrawPlayer_32_FrontAcc_FrontPart += DrawOverPlayer;
		public override void Unload() => Terraria.DataStructures.On_PlayerDrawLayers.DrawPlayer_32_FrontAcc_FrontPart -= DrawOverPlayer;

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
			if (player.controlJump)
			{
				floatHeight = 10 * 16;
				mountState = FLOAT;
			}
			else mountState = NORMAL;

			ControlFloatHeight(player, floatHeight);

			if (mountState == FLOAT && GetTarget(player, floatHeight) is NPC target)
			{
				mountState = ATTACKING;
				int cooldownTime = 14;
				attackCooldown = ++attackCooldown % cooldownTime;

				if (attackCooldown == 0)
				{
					int damage = (int)player.GetDamage(DamageClass.Summon).ApplyTo(20);
					Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromThis("Mount"), player.Center, Vector2.Zero, ModContent.ProjectileType<RlyehianMount_Proj>(),
						damage, 2, player.whoAmI, (int)(target.Center - player.Center).Length());
					proj.rotation = player.AngleTo(target.Center);
					proj.netUpdate = true;
				}
			}
		}

		private static void ControlFloatHeight(Player player, int height)
		{
			float baseHeight = height;
			float moveSpeed = 0.2f;
			float maxSpeed = 2f;

			int tileY = GetTileAt(player.Center, 12, 0, out bool _, false) * 16;
			float gotoY = tileY - baseHeight;

			Vector2 goPos = new Vector2(player.MountedCenter.X, gotoY - 16);

			if (player.DistanceSQ(goPos) > (maxSpeed + 1) * (maxSpeed + 1))
				player.velocity.Y += MathHelper.Clamp(player.DirectionTo(goPos).Y * moveSpeed, -maxSpeed, maxSpeed);
			else
				player.velocity.Y *= 0.5f;
		}

		private static NPC GetTarget(Player player, int detectHeight)
		{
			int detectWidth = player.width * 3;
			Rectangle detectRange = new Rectangle((int)player.getRect().Center.X - (detectWidth / 2), (int)player.getRect().Bottom, detectWidth, detectHeight);
			
			var npc = Main.npc.Where(x => x.active && !x.CanDamage() && x.getRect().Bottom >= player.getRect().Bottom &&
				detectRange.Intersects(x.getRect()) && Collision.CanHitLine(x.Center, 0, 0, player.Center, 0, 0)).FirstOrDefault();

			return (npc != default) ? npc : null;
		}

		private static int GetTileAt(Vector2 searchPos, int tileLength, int xOffset, out bool liquid, bool up = false)
		{
			int tileDist = (int)(searchPos.Y / 16f);
			liquid = true;

			for (int i = 0; i < tileLength; i++)
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
			if ((int)mountedPlayer.mount._frameCounter < lowerBound)
				mountedPlayer.mount._frameCounter = lowerBound;

			mountedPlayer.mount._frame = (int)mountedPlayer.mount._frameCounter;
			return false;
		}

		public override void SetMount(Player player, ref bool skipDust)
		{
			skipDust = true;
			DustHelper.DrawDiamond(player.Center, 173, 10);
		}

		public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
		{
			drawPlayer.GetModPlayer<RlyehianMountPlayer>().usingMount = true;
			return false;
		}

		private void DrawOverPlayer(Terraria.DataStructures.On_PlayerDrawLayers.orig_DrawPlayer_32_FrontAcc_FrontPart orig, ref PlayerDrawSet drawinfo)
		{
			orig(ref drawinfo);

			Player drawPlayer = drawinfo.drawPlayer;
			if (!(drawPlayer.mount.Active && drawPlayer.mount.Type == ModContent.MountType<RlyehianMount>()))
				return;

			Texture2D texture = ModContent.Request<Texture2D>("SpiritMod/Mounts/RlyehianMount/RlyehianMount").Value;

			int verticalFrame = drawPlayer.mount._frame % mountStatesMax;
			int horizontalFrame = (int)(drawPlayer.mount._frame / mountStatesMax);
			Rectangle sourceRect = new Rectangle(56 * horizontalFrame, 46 * verticalFrame, 54, 44);
			SpriteEffects effect = drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			int frameOffX = (drawPlayer.direction == 1) ? -30 : -24;

			Vector2 position = new Vector2((int)(drawPlayer.position.X - Main.screenPosition.X + (float)(drawPlayer.width / 2) + (float)MountData.xOffset),
				(int)(drawPlayer.position.Y - Main.screenPosition.Y + (float)(drawPlayer.height / 2) + (float)MountData.yOffset));

			DrawData data = new DrawData(texture, position + new Vector2(frameOffX, 0), sourceRect, Lighting.GetColor(drawPlayer.position.ToTileCoordinates()), 0f, Vector2.Zero, 1f, effect, 0)
			{
				shader = drawPlayer.cMount
			};
			drawinfo.DrawDataCache.Add(data);
		}
	}
}
