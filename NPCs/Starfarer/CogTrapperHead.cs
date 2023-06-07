using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Armor.ProtectorateSet;
using SpiritMod.Items.Accessory;
using SpiritMod.Items.Weapon.Yoyo;
using SpiritMod.Items.Material;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using SpiritMod.Biomes;

namespace SpiritMod.NPCs.Starfarer
{
	public class CogTrapperHead : ModNPC
	{
		internal const float Speed = 6.5f;
		internal const float TurnSpeed = 0.125f;

		public bool tail = false;
		public int minLength = 12;
		public int maxLength = 18;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stardancer");
			Main.npcFrameCount[NPC.type] = 1;
			NPCHelper.BuffImmune(Type, true);

			var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				CustomTexturePath = $"{Texture}_Bestiary",
				Position = new Vector2(40f, 26f),
				PortraitPositionXOverride = 0f,
				PortraitPositionYOverride = 12f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
		}

		public override void SetDefaults()
		{
			NPC.damage = 32;
			NPC.npcSlots = 14f;
			NPC.width = 26;
			NPC.height = 26;
			NPC.defense = 0;
			NPC.lifeMax = 225;
			NPC.aiStyle = -1;
			NPC.knockBackResist = 0f;
			NPC.value = 540;
			NPC.alpha = 255;
			NPC.behindTiles = true;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath14;
			NPC.netAlways = true;

			AnimationType = 10;
			AIType = -1;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.StardancerBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<AsteroidBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				new FlavorTextBestiaryInfoElement("Strange mechanical constructs assemble around a beacon, calling attention to precious astral metals. It seems you aren't the only one who wants it."),
			});
		}

		public override void AI()
		{
			Player player = Main.player[NPC.target];
			Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), 0f, 0.0375f * 2, 0.125f * 2);

			if (NPC.ai[3] > 0f)
				NPC.realLife = (int)NPC.ai[3];

			if (NPC.target < 0 || NPC.target == 255 || player.dead)
				NPC.TargetClosest(true);

			if (NPC.alpha != 0)
			{
				for (int num934 = 0; num934 < 2; num934++)
				{
					int num935 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, 1f);
					Main.dust[num935].noGravity = true;
					Main.dust[num935].noLight = true;
				}
			}

			NPC.alpha = Math.Max(NPC.alpha - 12, 0);

			if (Main.netMode != NetmodeID.MultiplayerClient)
				SpawnBody();

			NPC.localAI[1] = 0f;

			if (player.dead) //Despawn stuff
			{
				NPC.TargetClosest(false);
				NPC.velocity.Y -= 2f;

				if (NPC.position.Y > Main.worldSurface * 16)
					NPC.velocity.Y += 2f;

				if (NPC.position.Y > Main.rockLayer * 16)
				{
					for (int i = 0; i < 200; i++)
					{
						if (Main.npc[i].type == Type)
							Main.npc[i].active = false;
					}
				}
			}

			NPC.ai[1]++;

			if (NPC.ai[1] < 220)
			{
				NPC.velocity += NPC.DirectionTo(player.Center) * 0.2f;

				if (NPC.velocity.LengthSquared() > 8 * 8)
					NPC.velocity = Vector2.Normalize(NPC.velocity) * 8;
			}
			else
			{
				if (NPC.ai[2] == 0)
					NPC.ai[2] = 1;

				if (NPC.ai[1] > 340)
				{
					NPC.ai[1] = 0;
					NPC.ai[2] = Main.rand.NextBool() ? -1 : 1;
				}

				if (NPC.velocity.LengthSquared() < 8 * 8)
					NPC.velocity *= 1.2f;
				else
					NPC.velocity = NPC.velocity.RotatedBy(0.08f * NPC.ai[2]);
			}

			NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
		}

		private void SpawnBody()
		{
			if (!tail && NPC.ai[0] == 0f)
			{
				int current = NPC.whoAmI;
				int length = Main.rand.Next(minLength, maxLength);

				for (int i = 0; i < length; i++)
				{
					int type = i != length - 1 ? ModContent.NPCType<CogTrapperBody>() : ModContent.NPCType<CogTrapperTail>();
					int trailing = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI);

					Main.npc[trailing].realLife = NPC.whoAmI;
					Main.npc[trailing].ai[1] = current;
					Main.npc[trailing].ai[2] = NPC.whoAmI;

					Main.npc[current].ai[0] = trailing;
					NPC.netUpdate = true;
					current = trailing;
				}
				tail = true;
			}

			if (!NPC.active && Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon<StarEnergy>(1);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, hitDirection, -1f, 0, default, 1f);

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Stardancer1").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Stardancer2").Type, 1f);

				NPC.position += (NPC.Size / 2);
				NPC.width = 20;
				NPC.height = 20;
				NPC.position -= (NPC.Size / 2);

				for (int num621 = 0; num621 < 5; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, .5f);
					Main.dust[num622].velocity *= 2f;
				}

				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, 1f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 4f;
					num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.DungeonSpirit, 0f, 0f, 100, default, .5f);
					Main.dust[num624].velocity *= 1f;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			Color col = NPC.IsABestiaryIconDummy ? Color.White : drawColor;
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, col, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width * 0.5f, NPC.height * 0.5f);
			drawOrigin.Y += 30f;
			drawOrigin.Y += 8f;
			--drawOrigin.X;
			Vector2 position1 = NPC.Bottom - Main.screenPosition;
			Texture2D texture2D2 = TextureAssets.GlowMask[239].Value;
			float num11 = (float)((double)Main.GlobalTimeWrappedHourly % 1.0 / 1.0);
			float num12 = num11;
			if ((double)num12 > 0.5)
				num12 = 1f - num11;
			if ((double)num12 < 0.0)
				num12 = 0.0f;
			float num13 = (float)(((double)num11 + 0.5) % 1.0);
			float num14 = num13;
			if ((double)num14 > 0.5)
				num14 = 1f - num13;
			if ((double)num14 < 0.0)
				num14 = 0.0f;
			Rectangle r2 = texture2D2.Frame(1, 1, 0, 0);
			drawOrigin = r2.Size() / 2f;
			Vector2 position3 = position1 + new Vector2(0.0f, -20f);
			Color color3 = new Color(84, 207, 255) * 1.6f;
			Main.spriteBatch.Draw(texture2D2, position3, r2, color3, NPC.rotation, drawOrigin, NPC.scale * 0.35f, SpriteEffects.FlipHorizontally, 0.0f);
			float num15 = 1f + num11 * 0.75f;
			Main.spriteBatch.Draw(texture2D2, position3, r2, color3 * num12, NPC.rotation, drawOrigin, NPC.scale * 0.5f * num15, SpriteEffects.FlipHorizontally, 0.0f);
			float num16 = 1f + num13 * 0.75f;
			Main.spriteBatch.Draw(texture2D2, position3, r2, color3 * num14, NPC.rotation, drawOrigin, NPC.scale * 0.5f * num16, SpriteEffects.FlipHorizontally, 0.0f);
			GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/Starfarer/CogTrapperHead_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, screenPos);
		}
	}
}