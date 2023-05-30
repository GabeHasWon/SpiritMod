using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using SpiritMod.Items.Consumable.Food;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using SpiritMod.Buffs.Tiles;

namespace SpiritMod.NPCs.Pagoda.Yuurei
{
	public class PagodaGhostHostile : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Disturbed Yuurei");
			Main.npcFrameCount[NPC.type] = 4;
			NPCID.Sets.TrailCacheLength[NPC.type] = 3;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
			NPCHelper.BuffImmune(Type);
		}

		public override void SetDefaults()
		{
			NPC.width = 30;
			NPC.height = 40;
			NPC.damage = 23;
			NPC.noGravity = true;
			NPC.defense = 4;
			NPC.lifeMax = 50;
			NPC.HitSound = SoundID.NPCHit3;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 120f;
			NPC.knockBackResist = .1f;
			NPC.noTileCollide = true;
			NPC.aiStyle = 44;
			AIType = NPCID.FlyingAntlion;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.YureiBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
				new FlavorTextBestiaryInfoElement("Guardians of an abandoned skyward shrine. Despite being long forgotten, they fight with surprising agility."),
			});
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.15f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 99);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 99);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 99);

				for (int i = 0; i < 40; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.RainbowTorch, 0f, -2f, 0, new Color(0, 255, 142), .6f);
					dust.noGravity = true;

					dust.position.X += (Main.rand.Next(-50, 51) / 20) - 1.5f;
					dust.position.Y += (Main.rand.Next(-50, 51) / 20) - 1.5f;
					if (dust.position != NPC.Center)
						dust.velocity = NPC.DirectionTo(dust.position) * 6f;
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Ramen>(), 16));

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.HasBuff(ModContent.BuffType<PagodaCurse>()) ? 0.5f : 0f;

		public override void AI()
		{
			Player player = Main.player[NPC.target];
			NPC.alpha += 1;
			if (NPC.alpha >= 180)
			{
				int angle = Main.rand.Next(360);
				int distX = (int)(Math.Sin(angle * (Math.PI / 180)) * 90);
				int distY = (int)(Math.Cos(angle * (Math.PI / 180)) * 300);

				if (Main.netMode != NetmodeID.Server)
				{
					for (int i = 0; i < 3; ++i)
						Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, 99);

					SoundEngine.PlaySound(SoundID.NPCDeath6, NPC.Center);
				}
				NPC.position.X = player.position.X + distX;
				NPC.position.Y = player.position.Y + distY;
				NPC.alpha = 0;
			}
			NPC.spriteDirection = NPC.direction;
		}

		public override Color? GetAlpha(Color lightColor) => new Color(100, 100, 100, 100) * NPC.Opacity;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width * 0.5f, (NPC.height * 0.5f));
			for (int k = 0; k < NPC.oldPos.Length; k++) {
				var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
				Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY);
				Color color = NPC.GetAlpha(drawColor) * (float)(((float)(NPC.oldPos.Length - k) / (float)NPC.oldPos.Length) / 2);
				spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
			}
			return true;
		}
	}
}
