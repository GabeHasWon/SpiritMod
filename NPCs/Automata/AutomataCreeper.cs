using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.IO;
using SpiritMod.Mechanics.BoonSystem;
using SpiritMod.Buffs;
using Terraria.Audio;
using SpiritMod.Buffs.DoT;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.Automata
{
	public class AutomataCreeper : ModNPC, IBoonable
	{
		private Vector2 dirUnit;

		private bool attacking = false;
		private bool fired = false;

		private int AiTimer
		{
			get => (int)NPC.ai[2];
			set => NPC.ai[2] = value;
		}

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Arachmaton");
			Main.npcFrameCount[NPC.type] = 5;
			NPCHelper.ImmuneTo(this, BuffID.Poisoned, BuffID.Venom, ModContent.BuffType<FesteringWounds>(), ModContent.BuffType<BloodCorrupt>(), ModContent.BuffType<BloodInfusion>());
		}

		public override void SetDefaults()
		{
			NPC.width = 36;
			NPC.height = 36;
			NPC.damage = 70;
			NPC.defense = 30;
			NPC.lifeMax = 350;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath14;
			NPC.value = 180f;
			NPC.knockBackResist = 0;
			NPC.noGravity = true;
			NPC.noTileCollide = true;

			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.ArachmatonBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Marble,
				new FlavorTextBestiaryInfoElement("Forged from brass, a sturdy metal. Makes a lasting frame. A core from marble, containing untapped magic. This gives it mind. And at last, the spark of life. Roam free, new friend."),
			});
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			if (Main.rand.NextBool(5))
				target.AddBuff(BuffID.BrokenArmor, 1800);
		}

		public override void SendExtraAI(BinaryWriter writer) => writer.WriteVector2(dirUnit);

		public override void ReceiveExtraAI(BinaryReader reader) => dirUnit = reader.ReadVector2();

		public override void AI()
		{
			const int attackDuration = 50;
			const int timerMax = 300;

			if (dirUnit == Vector2.Zero)
			{
				NPC.direction = Math.Sign(NPC.Center.X - Main.player[NPC.target].Center.X);
				dirUnit = Vector2.UnitX * NPC.direction;
			}
			else NPC.direction = NPC.oldDirection;

			if (AiTimer == (timerMax - attackDuration))
			{
				attacking = true;
				NPC.frameCounter = 0;
			}
			if (AiTimer == 0)
			{
				NPC.velocity = dirUnit * 3;
				attacking = false;
			}

			AiTimer = ++AiTimer % timerMax;

			if (!attacking)
				Crawl();
			else
				Attack();
		}

		protected virtual void Attack()
		{
			NPC.velocity = Vector2.Zero;

			if ((int)NPC.frameCounter == 2)
			{
				if (!fired)
				{
					fired = true;

					int glyphnum = Main.rand.Next(10);
					DustHelper.DrawDustImage(NPC.Center, 6, 0.15f, "SpiritMod/Effects/Glyphs/Glyph" + glyphnum, 1.5f);

					if (Main.netMode != NetmodeID.MultiplayerClient)
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dirUnit, ModContent.ProjectileType<AutomataCreeperProj>(), Main.expertMode ? 40 : 60, 4, Main.myPlayer, NPC.ai[0], NPC.ai[1]);

					SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
					SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
				}
			}
			else fired = false;
		}

		protected Vector2 Collide() => Collision.noSlopeCollision(NPC.position, NPC.velocity, NPC.width, NPC.height, true, true);

		protected void Crawl()
		{
			Vector2 velFactor = Collide();
			NPC.collideX = Math.Abs(velFactor.X) < 0.5f;
			NPC.collideY = Math.Abs(velFactor.Y) < 0.5f;

			NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), .15f);

			if (NPC.ai[0] == 0f)
			{
				NPC.TargetClosest();
				dirUnit.Y = 1;
				NPC.ai[0] = 1f;
			}

			if (NPC.ai[1] == 0f)
			{
				if (NPC.collideY)
					NPC.ai[0] = 2f;

				if (!NPC.collideY && NPC.ai[0] == 2f)
				{
					dirUnit.X = -dirUnit.X;
					NPC.ai[1] = 1f;
					NPC.ai[0] = 1f;
				}
				if (NPC.collideX)
				{
					dirUnit.Y = -dirUnit.Y;
					NPC.ai[1] = 1f;
				}
			}
			else
			{
				if (NPC.collideX)
					NPC.ai[0] = 2f;

				if (!NPC.collideX && NPC.ai[0] == 2f)
				{
					dirUnit.Y = -dirUnit.Y;
					NPC.ai[1] = 0f;
					NPC.ai[0] = 1f;
				}
				if (NPC.collideY)
				{
					dirUnit.X = -dirUnit.X;
					NPC.ai[1] = 0f;
				}
			}

			NPC.velocity = 3 * dirUnit;
			NPC.velocity = Collide();
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 10; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Sunflower, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.47f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith, 2.5f * hit.HitDirection, -2.5f, 0, default, Main.rand.NextFloat(.45f, .55f));
			}

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				SoundEngine.PlaySound(SoundID.NPCDeath6 with { PitchVariance = 0.2f }, NPC.Center);

				for (int i = 0; i < 4; ++i)
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(NPC.velocity.X * .5f, NPC.velocity.Y * .5f), 99);
				for (int i = 1; i < 6; ++i)
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AutomataCreeper" + i).Type, 1f);
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.ArmorPolish, 100));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Accessory.GoldenApple>(), 85));
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
			var effect = (NPC.direction > 0) ? SpriteEffects.FlipVertically : SpriteEffects.None;
			Main.EntitySpriteDraw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effect, 0);

			Lighting.AddLight((int)(NPC.Center.X / 16f), (int)((NPC.Center.Y + (NPC.height / 2f)) / 16f), 0.1f * 2, 0.1f * 2, .1f * 2);

			return false;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => (spawnInfo.SpawnTileType == TileID.Marble) && spawnInfo.SpawnTileY > Main.rockLayer && Main.hardMode ? 1f : 0f;

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter = (NPC.frameCounter + .2f) % Main.npcFrameCount[NPC.type];
			NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
			
			NPC.frame.Width = 56;
			NPC.frame.X = attacking ? 0 : NPC.frame.Width;
		}
	}

	public class AutomataCreeperProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			// DisplayName.SetDefault("Cog");
		}

		public Vector2 dirUnit;
		public float speed = 1f;

		bool collideX = false;
		bool collideY = false;

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(36);
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.timeLeft = 150;
			Projectile.ignoreWater = true;
			Projectile.scale = 1;
		}

		public override void AI()
		{
			if (dirUnit == Vector2.Zero) //On spawn
			{
				dirUnit = Projectile.velocity;
				Projectile.scale = 0;
			}
			Projectile.scale = Math.Min(1, Projectile.scale + .1f);

			int fadeTime = 10;
			if (Projectile.timeLeft < fadeTime)
				Projectile.alpha += 255 / fadeTime;

			int speedMax = 12;
			for (int i = 0; i < speed / speedMax * 3; i++)
			{
				int dustType = Main.rand.NextBool(2) ? DustID.Torch : DustID.SpelunkerGlowstickSparkle;
				Dust.NewDustPerfect(new Vector2(Projectile.position.X + Main.rand.Next(Projectile.width), Projectile.Bottom.Y - Main.rand.Next(7)), dustType, new Vector2(-Projectile.velocity.X, -Projectile.velocity.Y * .5f), 100, default, Main.rand.NextFloat(0.8f, 1.1f)).noGravity = true;
			}
			if (Projectile.timeLeft % 15 == 0)
				SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFurySwing with { Pitch = .2f }, Projectile.Center);

			speed = Math.Min(speedMax, speed * 1.03f);

			Vector2 velFactor = Collide();
			collideX = Math.Abs(velFactor.X) < 0.5f;
			collideY = Math.Abs(velFactor.Y) < 0.5f;

			if (Projectile.ai[1] == 0f)
			{
				Projectile.rotation += dirUnit.X * dirUnit.Y * 0.43f;

				if (collideY)
					Projectile.ai[0] = 2f;

				if (!collideY && Projectile.ai[0] == 2f)
				{
					dirUnit.X = -dirUnit.X;
					Projectile.ai[1] = 1f;
					Projectile.ai[0] = 1f;
				}
				if (collideX)
				{
					dirUnit.Y = -dirUnit.Y;
					Projectile.ai[1] = 1f;
				}
			}
			else
			{
				Projectile.rotation -= dirUnit.X * dirUnit.Y * 0.13f;

				if (collideX)
					Projectile.ai[0] = 2f;

				if (!collideX && Projectile.ai[0] == 2f)
				{
					dirUnit.Y = -dirUnit.Y;
					Projectile.ai[1] = 0f;
					Projectile.ai[0] = 1f;
				}
				if (collideY)
				{
					dirUnit.X = -dirUnit.X;
					Projectile.ai[1] = 0f;
				}
			}
			Projectile.velocity = speed * dirUnit;
			Projectile.velocity = Collide();
		}

		protected virtual Vector2 Collide() => Collision.noSlopeCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, true, true);

		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			Lighting.AddLight((int)(Projectile.position.X / 16f), (int)(Projectile.position.Y / 16f), 0.301f, .047f, .016f);
			return false;
		}
	}
}
