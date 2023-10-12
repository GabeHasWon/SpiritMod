using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.SeraphSet;
using SpiritMod.Items.Sets.MagicMisc.AstralClock;
using SpiritMod.Items.Weapon.Summon;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Buffs;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using SpiritMod.Biomes.Events;

namespace SpiritMod.NPCs.BlueMoon.Bloomshroom
{
	public class Bloomshroom : ModNPC
	{
		bool attack = false;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 12;
			NPCHelper.ImmuneTo<StarFlame>(this);

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Position = new Vector2(2f, 0f),
				Velocity = 1f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 50;
			NPC.height = 54;
			NPC.damage = 29;
			NPC.defense = 16;
			NPC.lifeMax = 600;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath16;
			NPC.value = 600f;
			NPC.knockBackResist = .35f;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BloomshroomBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<MysticMoonBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 30; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Obsidian, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, 2.5f * hit.HitDirection, -2.5f, 0, default, .34f);
			}

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Glumshroom1").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Gloomshroom2").Type, 1f);
			}
		}


		int frame = 0;
		int timer = 0;

		public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
			Player target = Main.player[NPC.target];
			int distance = (int)Math.Sqrt((NPC.Center.X - target.Center.X) * (NPC.Center.X - target.Center.X) + (NPC.Center.Y - target.Center.Y) * (NPC.Center.Y - target.Center.Y));

			if (distance < 360)
				attack = true;

			if (distance > 380)
				attack = false;

			if (attack)
			{
				NPC.velocity.X = .008f * NPC.direction;

				if (frame == 9 && timer == 0)
				{
					SoundEngine.PlaySound(SoundID.Item95, NPC.Center);
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y - 10, 0, -4, ModContent.ProjectileType<BloomshroomHostile>(), 31, 0);
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y - 10, 6f, -4, ModContent.ProjectileType<BloomshroomHostile>(), 31, 0);

						if (Main.rand.NextBool(3))
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y - 10, -6f, -4, ModContent.ProjectileType<BloomshroomHostile>(), 25, 0);
					}
				}

				if (target.position.X > NPC.position.X)
					NPC.direction = 1;
				else
					NPC.direction = -1;
			}
			else
			{
				NPC.aiStyle = 26;
				AIType = NPCID.Skeleton;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/BlueMoon/Bloomshroom/Bloomshroom_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, screenPos);

		public override void FindFrame(int frameHeight)
		{
			timer++;
			if (attack || NPC.IsABestiaryIconDummy)
			{
				if (timer >= 12)
				{
					frame++;
					timer = 0;
				}

				if (frame > 11)
					frame = 7;

				if (frame < 7)
					frame = 7;
			}
			else
			{
				if (timer >= 4)
				{
					frame++;
					timer = 0;
				}

				if (frame > 6)
					frame = 0;
			}
			NPC.frame.Y = frameHeight * frame;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoonStone>(), 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StopWatch>(), 100));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GloomgusStaff>(), 100));
		}
	}
}