using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.Mount;
using SpiritMod.NPCs.Boss.FrostTroll;
using SpiritMod.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mounts.SnowMongerMount
{
	public class SnowmongerMount : ModMount
	{
		public override void Load()
		{
			Terraria.On_Player.KeyDoubleTap += Player_KeyDoubleTap;
			Terraria.DataStructures.On_PlayerDrawLayers.DrawPlayer_32_FrontAcc_FrontPart += DrawOverPlayer;
		}

		public override void Unload()
		{
			Terraria.On_Player.KeyDoubleTap -= Player_KeyDoubleTap;
			Terraria.DataStructures.On_PlayerDrawLayers.DrawPlayer_32_FrontAcc_FrontPart -= DrawOverPlayer;
		}

		const int DashTimeMax = 24;
		const int DashCooldownTime = -DashTimeMax * 3;
		int dashTime = 0;

		bool Dashing => dashTime > 0;

		private void Player_KeyDoubleTap(Terraria.On_Player.orig_KeyDoubleTap orig, Player self, int keyDir)
		{
			if ((keyDir == 2 || keyDir == 3) && self.mount.Active && self.mount.Type == ModContent.MountType<SnowmongerMount>() && dashTime <= DashCooldownTime)
				Dash(self, keyDir);
			else
				orig(self, keyDir);
		}

		public override void SetStaticDefaults()
		{
			MountData.spawnDust = 160;
			MountData.buff = ModContent.BuffType<SnowmongerMountBuff>();
			MountData.heightBoost = 10;
			MountData.fallDamage = 0f;
			MountData.runSpeed = 4f;
			MountData.dashSpeed = 6f;
			MountData.flightTimeMax = 200;
			MountData.fatigueMax = 320;
			MountData.jumpHeight = 10;
			MountData.acceleration = 0.3f;
			MountData.jumpSpeed = 10f;
			MountData.blockExtraJumps = true;
			MountData.totalFrames = 14;
			MountData.usesHover = false;

			int[] array = new int[MountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
				array[l] = 16;

			MountData.playerYOffsets = array;

			MountData.xOffset = 0;
			MountData.yOffset = 4;
			MountData.bodyFrame = 0;
			MountData.playerHeadOffset = 22;

			if (Main.netMode != NetmodeID.Server)
			{
				MountData.textureWidth = MountData.frontTexture.Width();
				MountData.textureHeight = MountData.frontTexture.Height();
			}
		}

		public override void UpdateEffects(Player player)
		{
			Lighting.AddLight(player.position, 0f, 0.18f, 0.4f);
			player.gravity = 0;
			player.fallStart = (int)(player.position.Y / 16.0);
			if (dashTime > DashCooldownTime)
				dashTime--;

			int maxHeight = 12 * 16;
			if (player.controlJump)
				maxHeight = 38 * 16;
			else if (player.controlDown)
				maxHeight = 3 * 16;

			if (!Dashing)
				ControlFloatHeight(player, maxHeight);
			else
				UpdateDash(player);
		}

		private void UpdateDash(Player player)
		{
			if (dashTime < DashTimeMax / 4)
				player.velocity.X *= 0.8f;
			player.velocity.Y = 0f;
			if (dashTime % 4 == 0)
			{
				Vector2 vel = new Vector2(0, 10).RotatedByRandom(0.08f);
				int damage = (int)player.GetDamage(DamageClass.Summon).ApplyTo(40);
				Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromThis("Mount"), player.MountedCenter + new Vector2(0, 40), vel, ModContent.ProjectileType<SnowMongerBeam>(), damage, 0.5f, player.whoAmI);
				proj.hostile = false;
				proj.friendly = true;
				proj.width = 40;
				proj.height = 40;

				SoundEngine.PlaySound(SoundID.Item91 with { Volume = 0.5f }, player.Center);
			}
			for (int i = 0; i < 2; i++)
			{
				int type = Main.rand.NextBool(2) ? DustID.FrostHydra : DustID.GemSapphire;
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, type, 0f, 0f);
				dust.noGravity = true;
				dust.velocity.X = player.velocity.X / 3;
			}
		}

		private void Dash(Player player, int dir)
		{
			dashTime = DashTimeMax;
			player.direction = dir == 2 ? 1 : -1;
			player.velocity = new Vector2(player.direction * 28, 0);
			for (int i = 0; i < 2; i++)
				ParticleHandler.SpawnParticle(new PulseCircle(player.Center + (player.velocity * i * 2), Color.LightBlue, 100 - (i * 20), 15) 
				{ Angle = player.velocity.ToRotation(), ZRotation = 0.5f });
			SoundEngine.PlaySound(SoundID.Item67 with { Volume = 0.7f }, player.Center);
		}

		private static void ControlFloatHeight(Player player, float floatHeight)
		{
			const float MaxSpeed = 5.8f;
			const float MoveSpeed = 0.23f;

			float baseHeight = floatHeight;

			Vector2 orig = player.Center;
			int tileY = GetTileAt(orig, 0, out bool _, false) * 16;
			float gotoY = tileY - baseHeight;

			Vector2 goPos = new Vector2(player.MountedCenter.X, gotoY - 16);

			if (player.DistanceSQ(goPos) > 32 * 32)
				player.velocity.Y += MathHelper.Clamp(player.DirectionTo(goPos).Y * MoveSpeed, -MaxSpeed, MaxSpeed);
			else
				player.velocity.Y *= 0.9f;
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
			mountedPlayer.mount._frameCounter += 0.2f;
			if (!Dashing)
			{
				mountedPlayer.mount._frame = (int)(mountedPlayer.mount._frameCounter %= 8);

				if (mountedPlayer.mount._frame > 4)
					mountedPlayer.mount._frameCounter = 0;
			}
			else
			{
				mountedPlayer.mount._frame = (int)(mountedPlayer.mount._frameCounter %= 8) + 5;

				if (mountedPlayer.mount._frame > 8)
				{
					mountedPlayer.mount._frame = 8;
					mountedPlayer.mount._frameCounter -= 0.2f;
				}

				if (dashTime > 5 && dashTime < 10)
					mountedPlayer.mount._frame = 9;
				else if (dashTime <= 5)
					mountedPlayer.mount._frame = 10;
			}
			return false;
		}

		public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow) => false;

		private void DrawOverPlayer(Terraria.DataStructures.On_PlayerDrawLayers.orig_DrawPlayer_32_FrontAcc_FrontPart orig, ref PlayerDrawSet drawinfo)
		{
			orig(ref drawinfo);

			Player drawPlayer = drawinfo.drawPlayer;
			if (!(drawPlayer.mount.Active && drawPlayer.mount.Type == ModContent.MountType<SnowmongerMount>()))
				return;

			int curFrame = drawPlayer.mount._frame;
			Texture2D texture = ModContent.Request<Texture2D>("SpiritMod/Mounts/SnowMongerMount/SnowmongerMount").Value;
			Rectangle sourceRect = new Rectangle(0, curFrame * 56, 78, 56);
			SpriteEffects effect = drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			int frameOffX = drawPlayer.direction == 1 ? 6 : -6;

			if (curFrame > 5)
				sourceRect = new Rectangle(78, (curFrame - 5) * 56, 78, 56);
			Vector2 position;
			position.X = (int)(drawPlayer.position.X - Main.screenPosition.X + (float)(drawPlayer.width / 2) + (float)MountData.xOffset);
			position.Y = (int)(drawPlayer.position.Y - Main.screenPosition.Y + (float)(drawPlayer.height / 2) + (float)MountData.yOffset);

			var mount = new DrawData(texture, position - new Vector2(40 + frameOffX, 20), sourceRect, Lighting.GetColor(drawPlayer.position.ToTileCoordinates()), 0f, Vector2.One, 1f, effect, 0);
			mount.shader = drawPlayer.cMount;
			drawinfo.DrawDataCache.Add(mount);

			var tex = ModContent.Request<Texture2D>("SpiritMod/Mounts/SnowMongerMount/SnowmongerMount_Glow").Value;
			var glow = new DrawData(tex, position - new Vector2(40 + frameOffX, 20), sourceRect, Color.White, 0f, Vector2.One, 1f, effect, 0);
			glow.shader = drawPlayer.cMount;
			drawinfo.DrawDataCache.Add(glow);
		}
	}
}
