﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using SpiritMod.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.GameContent.Bestiary;
using Humanizer;
using System.Diagnostics;

namespace SpiritMod.NPCs.Town.Oracle;

[AutoloadHead]
public class Oracle : ModNPC
{
	public override string Name => "Oracle";

	public const int AuraRadius = 263;

	public static bool HoveringBuffButton = false;

	private float RealAuraRadius => AuraRadius * RealAuraScale;
	private float RealAuraScale => Math.Min(AttackTimer / 150f, 1f);

	private float timer = 0;
	private float movementDir = 0;
	private float movementTimer = 0;

	public ref float Teleport => ref NPC.ai[0];
	public ref float AttackTimer => ref NPC.ai[1];
	public ref float TeleportX => ref NPC.ai[2];
	public ref float TeleportY => ref NPC.ai[3];

	private ref Player NearestPlayer => ref Main.player[NPC.target];

	private Rectangle[] runeSources = null;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 8;
		NPCID.Sets.ActsLikeTownNPC[NPC.type] = true;
	}

	public override void SetDefaults()
	{
		NPC.CloneDefaults(NPCID.Guide);
		NPC.townNPC = true;
		NPC.friendly = true;
		NPC.aiStyle = -1;
		NPC.damage = 30;
		NPC.defense = 30;
		NPC.lifeMax = 300;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.knockBackResist = 0f;
		NPC.noGravity = true;
		NPC.dontTakeDamage = true;
		NPC.immortal = true;
		TownNPCStayingHomeless = true;
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Marble");

	public override void AI()
	{
		timer++;
		NPC.velocity.Y = (float)Math.Sin(timer * 0.06f) * 0.4f;

		NPC.TargetClosest(true);
		Movement();
		OffenseAbilities();
	}

	private void Movement()
	{
		const float MoveSpeed = 1f;

		if (IsBeingTalkedTo() || AttackTimer > 5)
		{
			NPC.velocity.X *= 0.96f;
			movementTimer = 50;
			movementDir = 0f;
			return;
		}

		if (Teleport <= 0)
		{
			if (NPC.position.X < (Main.offLimitBorderTiles + 5) * 16)
			{
				TryGetInWorld(true);
				return;
			}
			else if (NPC.position.X > (Main.maxTilesX - (Main.offLimitBorderTiles + 5)) * 16)
			{
				TryGetInWorld(false);
				return;
			}
		}

		if (Teleport-- > 0)
		{
			if (Teleport > 50)
			{
				NPC.alpha += 2;
				AttackTimer--;
			}

			movementDir = 0f;
			movementTimer = 100;

			if (Teleport == 195)
			{
				string key = !Collision.DrownCollision(NPC.position, NPC.width, NPC.height) ? "Dry" : "Wet";
				string message = Language.GetTextValue("Mods.SpiritMod.NPCs.Oracle.Teleport." + key);
				CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Center.Y - 40, NPC.width, 20), Color.LightGoldenrodYellow, message);
			}

			if (Teleport == 50)
			{
				const float SwirlSize = 1.664f;
				const float Degrees = 2.5f;

				NPC.Center = new Vector2(TeleportX, TeleportY);
				NPC.alpha = 0;

				float Closeness = 50f;

				for (float swirlDegrees = Degrees; swirlDegrees < 160 + Degrees; swirlDegrees += 7f)
				{
					Closeness -= SwirlSize; //It closes in
					double radians = MathHelper.ToRadians(swirlDegrees);

					for (int i = 0; i < 4; ++i) //Spawn dust
					{
						Vector2 offset = new Vector2(Closeness).RotatedBy(radians + (MathHelper.PiOver2 * i));
						int d = Dust.NewDust(NPC.Center + offset, 2, 2, DustID.GoldCoin, 0, 0);
						Main.dust[d].noGravity = true;
					}
				}

				SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, NPC.Center);
			}
		}

		int tileDist = GetTileAt(0, out bool liquid);

		HandleFloatHeight(tileDist);

		if (!liquid)
		{
			GetTileAt(-1, out bool left);
			GetTileAt(1, out bool right);

			movementTimer--;
			if (movementTimer < 0)
			{
				var options = new List<float> { 0f };

				if (!left)
					options.Add(-MoveSpeed);

				if (!right)
					options.Add(MoveSpeed);

				if (movementDir == 0)
					movementDir = Main.rand.Next(options);
				else
					movementDir = 0f;

				movementTimer = movementDir == 0f ? Main.rand.Next(200, 300) : Main.rand.Next(300, 400);

				NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
			}

			if (movementDir < 0f && left)
				movementDir = 0f;
			else if (movementDir > 0f && right)
				movementDir = 0f;
		}
		else
			ScanForLand();

		NPC.velocity.X = movementDir == 0f ? NPC.velocity.X * 0.98f : movementDir;

		if ((movementDir == 0f && Math.Abs(NPC.velocity.X) < 0.15f) || IsBeingTalkedTo())
		{
			if (NPC.DistanceSQ(NearestPlayer.Center) < 400 * 400)
				NPC.direction = NPC.spriteDirection = NearestPlayer.Center.X < NPC.Center.X ? -1 : 1;
		}
		else
			NPC.direction = NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;
	}

	/// <summary>
	/// Tries to adjust the Oracle's position to be in-world without overlapping tiles.
	/// </summary>
	/// <param name="goRight"></param>
	private void TryGetInWorld(bool goRight)
	{
		Vector2 pos = new Vector2(goRight ? (Main.offLimitBorderTiles + 5) * 16 
			: (Main.maxTilesX - (Main.offLimitBorderTiles + 5)) * 16, NPC.Center.Y) + new Vector2(goRight ? 500 : -500, -500);

		while (Collision.SolidCollision(pos, NPC.width, NPC.height))
		{
			pos += new Vector2(goRight ? 500 : -500, -500);

			for (int i = 0; i < 10; ++i)
			{
				pos.Y += 100;

				if (!Collision.SolidCollision(pos, NPC.width, NPC.height))
					break;
			}
		}

		Teleport = 200;
		TeleportX = pos.X;
		TeleportY = pos.Y;
	}

	private void HandleFloatHeight(int tileDist)
	{
		int[] ceilingHeights = new int[5];
		for (int i = -2; i < 3; ++i)
			ceilingHeights[i + 2] = GetTileAt(-1, out _, true);

		int avgCeilingHeight = 0;

		for (int i = 0; i < ceilingHeights.Length; ++i)
		{
			if (ceilingHeights[i] == -1)
				avgCeilingHeight += 10;
			else
				avgCeilingHeight += (int)(NPC.Center.Y / 16f) - ceilingHeights[i];
		}

		avgCeilingHeight /= 5;
		int adjustLevHeight = 5;

		if (avgCeilingHeight <= 10)
			adjustLevHeight = (int)(avgCeilingHeight * 0.25f);

		adjustLevHeight -= 5;

		if ((NPC.Center.Y / 16f) + 6 + adjustLevHeight < tileDist)
			NPC.velocity.Y += 0.36f; //Grounds the NPC
		if ((NPC.Center.Y / 16f) > tileDist - (5 + adjustLevHeight))
			NPC.velocity.Y -= 0.36f; //Raises the NPC

		if (Collision.DrownCollision(NPC.position, NPC.width, NPC.height))
		{
			if (NPC.breath <= 0 && Teleport < 0)
			{
				Vector2 relativePos = NPC.Center - new Vector2(Main.rand.Next(-400, 400), Main.rand.Next(-400, 400));

				if (!Collision.SolidCollision(relativePos, NPC.width, NPC.height))
				{
					Teleport = 200;
					TeleportX = relativePos.X;
					TeleportY = relativePos.Y;
				}
			}
		}
	}

	private void ScanForLand()
	{
		const int SearchDist = 20;

		int nearestTileDir = 1000;

		for (int i = -SearchDist; i < SearchDist + 1; ++i)
		{
			GetTileAt(i, out bool liq);
			int thisDist = (int)(NPC.Center.X / 16f) + i;
			if (!liq && Math.Abs((int)(NPC.Center.X / 16f) - nearestTileDir) > Math.Abs((int)(NPC.Center.X / 16f) - thisDist))
				nearestTileDir = thisDist;
		}

		if (nearestTileDir != 1000)
		{
			int dir = (NPC.Center.X / 16f) > nearestTileDir ? -1 : 1;
			NPC.velocity.X = dir * 1.15f;
		}
	}

	public bool IsBeingTalkedTo()
	{
		for (int i = 0; i < Main.maxPlayers; ++i)
		{
			Player p = Main.player[i];
			if (p.active && !p.dead && p.talkNPC == NPC.whoAmI)
				return true;
		}

		return false;
	}

	private int GetTileAt(int xOffset, out bool liquid, bool up = false)
	{
		int tileDist = (int)(NPC.Center.Y / 16f);
		liquid = true;

		while (true)
		{
			tileDist += !up ? 1 : -1;

			if (tileDist < 20)
				return -1;

			Tile t = Framing.GetTileSafely((int)(NPC.Center.X / 16f) + xOffset, tileDist);
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

	private void OffenseAbilities()
	{
		bool enemyNearby = false;

		for (int i = 0; i < Main.maxNPCs; ++i)
		{
			NPC cur = Main.npc[i];
			if (cur.active && cur.CanBeChasedBy() && cur.DistanceSQ(NPC.Center) < AuraRadius * AuraRadius) //Scan for NPCs
			{
				if (cur.DistanceSQ(NPC.Center) < RealAuraRadius * RealAuraRadius) //Actually inflict damage to NPCs
					cur.AddBuff(ModContent.BuffType<GreekFire>(), 2);

				enemyNearby = true;
			}
		}

		if (float.IsNaN(AttackTimer))
			AttackTimer = 0;

		if (enemyNearby)
			AttackTimer = (float)Math.Min(Math.Pow(AttackTimer + 1, 1.005f), 150);
		else
		{
			AttackTimer = (float)Math.Max(Math.Pow(AttackTimer, 0.991f), 0f);
			if (AttackTimer < 10)
				AttackTimer--;
		}
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(timer);
		writer.Write(movementDir);
		writer.Write(movementTimer);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		timer = reader.ReadSingle();
		movementDir = reader.ReadSingle();
		movementTimer = reader.ReadSingle();
	}

	public override void FindFrame(int frameHeight)
	{
		if (AttackTimer > 5 && NPC.frame.Y < 200)
			NPC.frame.Y = 200;
		else if (AttackTimer <= 5 && NPC.frame.Y >= 200)
			NPC.frame.Y = 0;

		NPC.frameCounter += 6;
		if (AttackTimer > 2)
			NPC.frameCounter += 2;

		if (NPC.frameCounter > 42)
		{
			NPC.frame.Y += frameHeight;

			int max = AttackTimer > 5 ? 8 : 4;
			if (NPC.frame.Y >= frameHeight * max)
				NPC.frame.Y = frameHeight * (max - 4);

			NPC.frameCounter = 0;
		}
	}

	public override string GetChat()
	{
		List<string> options = new();
		for (int i = 1; i < 21; i++)
			options.Add(Language.GetTextValue("Mods.SpiritMod.NPCs.Oracle.Dialogue.Basic" + i).FormatWith(Main.LocalPlayer.name));

		return Main.rand.Next(options);
	}

	public override List<string> SetNPCNameList() => new() { "Pythia", "Cassandra", "Chrysame", "Eritha", "Theoclea", "Hypatia", "Themistoclea", "Phemonoe" };

	public override void OnChatButtonClicked(bool firstButton, ref string shopName)
	{
		if (firstButton)
			shopName = "Shop";
	}

	public override void AddShops()
	{
		NPCShop shop = new NPCShop(Type);

		void AddOlympiumItem<T>(int price) where T : ModItem
		{
			shop.Add(new Item(ModContent.ItemType<T>())
			{
				shopCustomPrice = price,
				shopSpecialCurrency = SpiritMod.OlympiumCurrencyID
			});
		}

		void VanillaOlympiumItem(int id, int price)
		{
			shop.Add(new Item(id)
			{
				shopCustomPrice = price,
				shopSpecialCurrency = SpiritMod.OlympiumCurrencyID
			});
		}

		AddOlympiumItem<Items.Sets.OlympiumSet.ArtemisHunt.ArtemisHunt>(25);
		AddOlympiumItem<Items.Sets.OlympiumSet.MarkOfZeus.MarkOfZeus>(25);
		AddOlympiumItem<Items.Sets.OlympiumSet.BetrayersChains.BetrayersChains>(25);
		AddOlympiumItem<Items.Sets.OlympiumSet.Eleutherios.Eleutherios>(20);
		AddOlympiumItem<Items.Consumable.Potion.MirrorCoat>(2);
		AddOlympiumItem<Items.Consumable.OliveBranch>(2);
		AddOlympiumItem<OracleScripture>(1);
		VanillaOlympiumItem(ItemID.PocketMirror, 10);

		shop.Register();
	}

	public override void SetChatButtons(ref string button, ref string button2) => button = Language.GetTextValue("LegacyInterface.28");

	public override float SpawnChance(NPCSpawnInfo spawnInfo) => Main.hardMode && !NPC.AnyNPCs(ModContent.NPCType<Oracle>()) && spawnInfo.Marble && !spawnInfo.Player.ZoneDungeon ? 0.1f : 0f;

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (AttackTimer > 10 || Teleport > 50)
		{
			float wave = (float)Math.Cos(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * MathHelper.TwoPi) + 0.5f;

			SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Color baseCol = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightGoldenrodYellow);

			for (int i = 0; i < 4; i++)
			{
				Color col = NPC.GetAlpha(baseCol) * (1f - wave);
				Vector2 drawPos = NPC.Center + (i / 4f * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * (4f * wave + 4f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY) - NPC.velocity * i;
				Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, col, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
			}
		}

		return true;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D aura = Mod.Assets.Request<Texture2D>("NPCs/Town/Oracle/OracleAura").Value;

		if (runeSources == null) //Initialize runeSources
		{
			static Rectangle IndividualRuneSource() => new(0, 32 * Main.rand.Next(8), 32, 32);

			runeSources = new Rectangle[8];

			for (int i = 0; i < runeSources.Length; ++i)
				runeSources[i] = IndividualRuneSource();
		}

		float wave = (float)Math.Cos(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * MathHelper.TwoPi) + 0.5f;

		SpriteEffects direction = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		Color baseCol = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightGoldenrodYellow);

		for (int i = 0; i < 4; i++)
		{
			Color col = NPC.GetAlpha(baseCol) * (1f - wave);
			Vector2 drawPos = NPC.Center + (i / 4f * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * (4f * wave + 4f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY) - NPC.velocity * i;
			spriteBatch.Draw(aura, drawPos, null, col, timer * 0.02f, aura.Size() / 2f, RealAuraScale, direction, 0f);
			DrawRuneCircle(spriteBatch, i, col, wave);
			DrawLetter(spriteBatch, i, col, wave);
		}

		Vector2 drawPosition = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);
		spriteBatch.Draw(aura, drawPosition, null, baseCol, timer * 0.02f, aura.Size() / 2f, RealAuraScale, SpriteEffects.None, 0f);
		DrawRuneCircle(spriteBatch, -1, baseCol);
		DrawLetter(spriteBatch, -1, baseCol);
	}

	private void DrawRuneCircle(SpriteBatch spriteBatch, int i, Color col, float wave = 0f)
	{
		Texture2D runes = Mod.Assets.Request<Texture2D>("NPCs/Town/Oracle/OracleRunes").Value;

		Vector2 drawPos = NPC.Center + (i / 4f * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * (4f * wave + 4f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY) - NPC.velocity * i;
		if (i == -1)
			drawPos = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);

		void DrawIndividualRune(int offset)
		{
			Vector2 circleOffset = new Vector2(0, 88 * RealAuraScale).RotatedBy((-timer * 0.02f) + (MathHelper.PiOver4 * offset));
			spriteBatch.Draw(runes, drawPos + circleOffset, runeSources[offset], col, 0f, new Vector2(16), RealAuraScale * 0.9f, SpriteEffects.None, 0f);
		}

		for (int j = 0; j < 8; ++j)
			DrawIndividualRune(j);
	}

	private void DrawLetter(SpriteBatch spriteBatch, int i, Color col, float wave = 0f)
	{
		Texture2D letter = Mod.Assets.Request<Texture2D>("NPCs/Town/Oracle/OracleAuraLetter").Value;

		Vector2 drawPos = NPC.Center + (i / 4f * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * (4f * wave + 4f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY) - NPC.velocity * i;
		if (i == -1)
			drawPos = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);

		Vector2 circleOffset = new Vector2(0, 228 * RealAuraScale).RotatedBy(timer * 0.02f);
		spriteBatch.Draw(letter, drawPos + circleOffset, null, col, 0f, letter.Size() / 2f, RealAuraScale, SpriteEffects.None, 0f);
	}

	public static void DrawBuffButton(int superColor, int numLines)
	{
		string text = Language.GetTextValue("Mods.SpiritMod.NPCs.Oracle.Bless.Button");

		DynamicSpriteFont font = FontAssets.MouseText.Value;
		Vector2 scale = new Vector2(0.9f);
		Vector2 stringSize = ChatManager.GetStringSize(font, text, scale);
		Vector2 position = new Vector2(180 + Main.screenWidth / 2 + stringSize.X - 20f, 130 + numLines * 30);
		Color baseColor = new Color(superColor, (int)(superColor / 1.1), superColor / 2, superColor);
		Vector2 mousePos = new Vector2(Main.mouseX, Main.mouseY);

		if (mousePos.Between(position, position + stringSize * scale) && !PlayerInput.IgnoreMouseInterface) //Mouse hovers over button
		{
			Main.LocalPlayer.mouseInterface = true;
			Main.LocalPlayer.releaseUseItem = true;
			scale *= 1.1f;

			if (!HoveringBuffButton)
				SoundEngine.PlaySound(SoundID.MenuTick);

			HoveringBuffButton = true;

			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				Bless();
			}
		}
		else
		{
			if (HoveringBuffButton)
				SoundEngine.PlaySound(SoundID.MenuTick);

			HoveringBuffButton = false;
		}

		ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, position + new Vector2(16f, 14f), baseColor, 0f, stringSize * 0.5f, scale);
	}

	public static void Bless()
	{
		List<string> options = new();

		for (int i = 1; i < 6; i++)
			options.Add(Language.GetTextValue("Mods.SpiritMod.NPCs.Oracle.Bless.Dialogue" + i));

		for (int i = 0; i < 30; i++)
		{
			int num = Dust.NewDust(new Vector2(Main.LocalPlayer.Center.X + Main.rand.Next(-100, 100), Main.LocalPlayer.Center.Y + Main.rand.Next(-100, 100)), Main.LocalPlayer.width, Main.LocalPlayer.height, ModContent.DustType<Dusts.BlessingDust>(), 0f, -2f, 0, default, 2f);
			Main.dust[num].noGravity = true;
			Main.dust[num].scale = Main.rand.Next(70, 105) * 0.01f;
			Main.dust[num].fadeIn = 1;
		}

		int glyphnum = Main.rand.Next(10);
		DustHelper.DrawDustImage(new Vector2(Main.LocalPlayer.Center.X, Main.LocalPlayer.Center.Y - 25), ModContent.DustType<Dusts.MarbleDust>(), 0.05f, "SpiritMod/Effects/Glyphs/Glyph" + glyphnum, 1f);
		Main.npcChatText = Main.rand.Next(options);
		SoundEngine.PlaySound(SoundID.Item29 with { PitchVariance = 0.4f, Volume = 0.6f }, Main.LocalPlayer.Center);
		SoundEngine.PlaySound(SoundID.NPCDeath6 with { PitchVariance = 0.4f, Volume = 0.2f }, Main.LocalPlayer.Center);

		Main.LocalPlayer.AddBuff(ModContent.BuffType<OracleBoonBuff>(), 3600 * 5);
	}
}