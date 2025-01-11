using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Consumable.Potion;
using SpiritMod.NPCs.Boss.MoonWizard.Projectiles;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using SpiritMod.Biomes.Events;

namespace SpiritMod.NPCs.MoonjellyEvent
{
	public class MoonjellyGiant : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 8;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
			NPCHelper.ImmuneTo(this, BuffID.Poisoned, BuffID.Venom);

			var drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Position = new Vector2(0f, 20f),
				PortraitPositionYOverride = 10f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 36;
			NPC.height = 70;
			NPC.damage = 16;
			NPC.defense = 10;
			NPC.lifeMax = 65;
			NPC.HitSound = SoundID.NPCHit25;
			NPC.DeathSound = SoundID.NPCDeath28;
			NPC.value = 250f;
			NPC.knockBackResist = 0f;
			NPC.alpha = 100;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.aiStyle = -1;

			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.GiantJellyBanner>();
			SpawnModBiomes = [ModContent.GetInstance<JellyDelugeBiome>().Type];
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.08f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override bool PreAI()
		{
			NPC.TargetClosest(true);
			Lighting.AddLight(NPC.Center, 0.075f * 2, 0.231f * 2, 0.255f * 2);

			Player target = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

			Vector2 vector2_1 = target.Center - NPC.Center + new Vector2(0, -100f);
			float distance = vector2_1.Length();

			Vector2 desiredVelocity = NPC.velocity;
			if (distance < 20)
				desiredVelocity.Normalize();

			if (distance < 40.0)
				desiredVelocity = vector2_1 * (5f * 0.025f);
			else if (distance < 80.0)
				desiredVelocity = vector2_1 * (5f * 0.075f);
			else
				desiredVelocity = vector2_1 * 5f;

			NPC.SimpleFlyMovement(desiredVelocity, 0.05f);
			NPC.rotation = NPC.velocity.X * 0.1f;

			if (NPC.ai[0] == 0f)
			{
				for (int i = 0; i < 5; i++)
				{
					Vector2 vel = Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * new Vector2(Main.rand.Next(3, 8), Main.rand.Next(3, 8));
					Vector2 pos = new(NPC.Center.X + Main.rand.Next(-20, 20), NPC.Center.Y + Main.rand.Next(-20, 20));
					int type = ModContent.ProjectileType<ElectricJellyfishOrbiter>();
					int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), pos.X, pos.Y, vel.X, vel.Y, type, NPCUtils.ToActualDamage(30, 1.5f, 2f), 0.0f, Main.myPlayer, 0.0f, NPC.whoAmI);
					Main.projectile[p].scale = Main.rand.NextFloat(.6f, .95f);
					Main.projectile[p].ai[0] = NPC.whoAmI;

					NPC.ai[0] = 1f;
					NPC.netUpdate = true;
				}
			}

			return false;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			var pos = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY);
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/MoonjellyEvent/MoonjellyGiant_Glow").Value, screenPos);

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 15; k++)
				Dust.NewDustPerfect(NPC.Center, 226, Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(5), 0, default, 0.65f).noGravity = true;

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
				for (int k = 0; k < 30; k++)
					Dust.NewDustPerfect(NPC.Center, 226, Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(7), 0, default, 0.95f).noGravity = true;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon(ItemID.Gel, 1, 2, 4);
			npcLoot.AddCommon<MoonJelly>(2);
			npcLoot.AddCommon<Items.Accessory.MoonlightSack.Moonlight_Sack>(12);
		}
	}
}