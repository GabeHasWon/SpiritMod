﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using SpiritMod.Tiles;
using System;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using SpiritMod.Mechanics.QuestSystem;
using SpiritMod.Mechanics.BackgroundSystem;
using System.Collections.Generic;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Effects.SurfaceWaterModifications;
using SpiritMod.Items.Sets.FloatingItems.MessageBottle;
using System.Reflection;
using SpiritMod.NPCs.Town.Oracle;
using SpiritMod.Mechanics.CollideableNPC;
using SpiritMod.GlobalClasses.Players;
using System.Linq;
using SpiritMod.Utilities.ILEdits;
using MonoMod.RuntimeDetour;

namespace SpiritMod.Utilities;

public static class SpiritDetours
{
	private static Hook NPCAIHook;

	public static void Initialize()
	{
		On_Main.DrawProjectiles += Main_DrawProjectiles;
		On_Main.DrawCachedProjs += Main_DrawCachedProjs;
		On_Main.DrawNPCs += Main_DrawNPCs;
		On_Main.DrawPlayers_AfterProjectiles += Main_DrawPlayers_AfterProjectiles;
		On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += TrailCheck2;
		On_Player.KeyDoubleTap += Player_KeyDoubleTap;
		On_Main.DrawDust += AdditiveCalls;
		On_Player.ToggleInv += Player_ToggleInv;
		On_Player.Update_NPCCollision += CollideableNPCDetours.SolidTopCollision;
		On_Projectile.VanillaAI += CollideableNPCDetours.Grappling;
		On_Main.DrawInterface += DrawParticles;
		Terraria.Localization.On_LanguageManager.GetTextValue_string += LanguageManager_GetTextValue_string1;
		On_Main.DrawNPCChatButtons += Main_DrawNPCChatButtons;
		On_WorldGen.SpreadGrass += On_WorldGen_SpreadGrass;
		On_Main.DrawBackgroundBlackFill += Main_DrawBackgroundBlackFill; //BackgroundItemManager.Draw()
		On_Main.Update += Main_Update; //BackgroundItemManager.Update()
		On_Main.CheckMonoliths += DrawPrimRTs;

		Main.OnPreDraw += Main_OnPreDraw;

		NPCAIHook = new Hook(NPCAIMethod, NPCAIMod);
		NPCAIHook.Apply();

		SurfaceWaterModifications.Load();

		foreach (var item in typeof(SpiritDetours).Assembly.GetTypes().Where(x => typeof(ILEdit).IsAssignableFrom(x) && !x.IsAbstract))
		{
			var inst = (ILEdit)Activator.CreateInstance(item);
			inst.Load(SpiritMod.Instance);
		}
	}

	private static void On_WorldGen_SpreadGrass(On_WorldGen.orig_SpreadGrass orig, int i, int j, int dirt, int grass, bool repeat, TileColorCache color)
	{
		if (grass == TileID.CorruptGrass || grass == TileID.CrimsonGrass || grass == TileID.HallowedGrass)
			foreach (Point16 point in MyWorld.superSunFlowerPositions)
				if (Math.Abs(point.X - i) < SuperSunFlower.Range * 2)
					return;

		orig(i, j, dirt, grass, repeat, color);
	}

	private static int TrailCheck2(On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig, IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1, float ai2)
	{
		int index = orig(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1, ai2);
		Projectile projectile = Main.projectile[index];

		if (projectile.ModProjectile is ITrailProjectile)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				(projectile.ModProjectile as ITrailProjectile).DoTrailCreation(SpiritMod.TrailManager);

			else
				SpiritMod.WriteToPacket(SpiritMod.Instance.GetPacket(), (byte)MessageType.SpawnTrail, index).Send();
		}

		//if (Main.netMode != NetmodeID.Server)
		//	SpiritMod.TrailManager.DoTrailCreation(projectile);

		return index;
	}

	private static void DrawPrimRTs(On_Main.orig_CheckMonoliths orig)
	{
		if (Main.spriteBatch != null && SpiritMod.primitives != null)
		{
			Main.screenPosition += Main.LocalPlayer.velocity;

			SpiritMod.primitives.DrawTrailsProj(Main.spriteBatch, Main.graphics.GraphicsDevice);
			SpiritMod.primitives.DrawTrailsNPC(Main.spriteBatch, Main.graphics.GraphicsDevice);

			Main.screenPosition -= Main.LocalPlayer.velocity;
		}
	}

	private static void Main_DrawPlayers_AfterProjectiles(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
	{
		orig(self);

		ExtraDrawOnPlayer.DrawPlayers();
		//SpiritMod.Metaballs.DrawFriendlyLayer(Main.spriteBatch);
	}

	public static void Unload()
	{
		Main.OnPreDraw -= Main_OnPreDraw;

		SurfaceWaterModifications.Unload();
		NPCAIHook.Undo();
	}

	private static readonly MethodInfo NPCAIMethod = typeof(NPCLoader).GetMethod(nameof(NPCLoader.NPCAI));
	public delegate void orig_NPCAI(NPC npc);
	public delegate void hook_NPCAI(orig_NPCAI orig, NPC npc);

	/// <summary>Detour for NPCLoader.NPCAI; allows the Message in a Bottle mount to still have waterborne enemies target it when not "in" the water.</summary>
	private static void NPCAIMod(orig_NPCAI orig, NPC npc)
	{
		bool removeWet = false;
		Player target = null;

		if (npc.target >= 0 && npc.target < Main.maxPlayers && !npc.townNPC && !npc.friendly)
		{
			target = Main.player[npc.target];
			removeWet = target.HasBuff(ModContent.BuffType<BottleMountBuff>()) && (target.velocity.Y == -target.gravity || target.velocity.Y == 0);
		}

		if (removeWet)
			target.wet = true;

		orig(npc);

		if (removeWet)
			target.wet = false;
	}

	private static void Main_Update(On_Main.orig_Update orig, Main self, GameTime gameTime)
	{
		if (Main.PlayerLoaded && BackgroundItemManager.Loaded && !Main.gamePaused) //Update all background items
			BackgroundItemManager.Update();

		SpiritMod.deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
		SpiritMod.Instance?.CheckScreenSize();

		orig(self, gameTime);
	}

	private static void Main_DrawBackgroundBlackFill(On_Main.orig_DrawBackgroundBlackFill orig, Main self)
	{
		orig(self);

		if (Main.PlayerLoaded && BackgroundItemManager.Loaded && !Main.gameMenu)
			BackgroundItemManager.Draw(); //Draw all background items
	}

	private static void AdditiveCalls(On_Main.orig_DrawDust orig, Main self)
	{
		AdditiveCallManager.DrawAdditiveCalls(Main.spriteBatch);
		orig(self);
	}

	private const float ProfileNameScale = 1f; //Profile name scale - 1f because the higher is poorly resized
	public static bool HoveringQuestButton = false;

	private static void Main_DrawNPCChatButtons(On_Main.orig_DrawNPCChatButtons orig, int superColor, Color chatColor, int numLines, string focusText, string focusText3) //Portrait drawing - Gabe
	{
		if (Main.LocalPlayer.talkNPC != -1)
		{
			NPC talkNPC = Main.npc[Main.LocalPlayer.talkNPC];
			//bool hasPortrait = PortraitManager.HasPortrait(talkNPC.type) || PortraitManager.HasCallPortrait(talkNPC.type);

			//if (ModContent.GetInstance<SpiritClientConfig>().ShowNPCPortraits && hasPortrait)
			//{
			//	Point size = new Point(108, 108);
			//	if (PortraitManager.HasPortrait(talkNPC.type)) //In-house portrait
			//	{
			//		BasePortrait portrait = PortraitManager.GetPortrait(talkNPC.type); //Gets portrait
			//		size = portrait.BaseSize;

			//		var pos = new Vector2(Main.screenWidth / 2 - (252 + portrait.BaseSize.X), 100); //Portrait position
			//		Main.spriteBatch.Draw(portrait.Texture, pos, portrait.GetFrame(Main.npcChatText, talkNPC), Color.White, 0f, default, 1f, SpriteEffects.None, 0f); //Portrait
			//	}
			//	else if (PortraitManager.HasCallPortrait(talkNPC.type)) //Mod.Call portrait
			//	{
			//		ModCallPortrait portrait = PortraitManager.GetCallPortrait(talkNPC.type); //Gets portrait
			//		size = portrait.BaseSize;

			//		var pos = new Vector2(Main.screenWidth / 2 - (252 + portrait.BaseSize.X), 100); //Portrait position
			//		Main.spriteBatch.Draw(portrait.Texture, pos, portrait.GetFrame(Main.npcChatText, talkNPC), Color.White, 0f, default, 1f, SpriteEffects.None, 0f); //Portrait
			//	}

			//	DrawPortraitName(talkNPC, size); //Draws the name
			//}

			//if (talkNPC.type == NPCID.Angler)
			//	focusText = ""; // empty string, we'll add our own angler quest button

			var queue = ModContent.GetInstance<QuestWorld>().NPCQuestQueue;

			if (QuestManager.QuestBookUnlocked && queue.ContainsKey(talkNPC.type) && queue[talkNPC.type].Count > 0) //If this NPC has a quest
			{
				// TODO: localization
				string questText = "Quest";

				DynamicSpriteFont font = FontAssets.MouseText.Value;
				Vector2 scale = new Vector2(0.9f);
				Vector2 stringSize = ChatManager.GetStringSize(font, questText, scale);
				Vector2 position = new Vector2((180 + Main.screenWidth / 2) + stringSize.X - 50f, 130 + numLines * 30);
				Color baseColor = new Color(superColor, (int)(superColor / 1.1), superColor / 2, superColor);
				Vector2 mousePos = new Vector2(Main.mouseX, Main.mouseY);

				if (mousePos.Between(position, position + stringSize * scale) && !PlayerInput.IgnoreMouseInterface) //Mouse hovers over button
				{
					Main.LocalPlayer.mouseInterface = true;
					Main.LocalPlayer.releaseUseItem = true;
					scale *= 1.1f;

					if (!HoveringQuestButton)
						SoundEngine.PlaySound(SoundID.MenuTick);

					HoveringQuestButton = true;

					if (Main.mouseLeft && Main.mouseLeftRelease) //If clicked on, unlock a quest.
					{
						UnlockQuestFromNPC(talkNPC);
					}
				}
				else
				{
					if (HoveringQuestButton)
						SoundEngine.PlaySound(SoundID.MenuTick);

					HoveringQuestButton = false;
				}

				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, questText, position + new Vector2(16f, 14f), baseColor, 0f,
					stringSize * 0.5f, scale * new Vector2(1f));
			}

			if (talkNPC.type == ModContent.NPCType<Oracle>())
				Oracle.DrawBuffButton(superColor, numLines);
		}
		orig(superColor, chatColor, numLines, focusText, focusText3);
	}
	
	public static void UnlockQuestFromNPC(NPC talkNPC) {
		var queue = ModContent.GetInstance<QuestWorld>().NPCQuestQueue;
		Quest q = queue[talkNPC.type].Dequeue();
		QuestManager.UnlockQuest(q, true);

		Main.npcChatText = q.QuestDescription;

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.Quest, 4);
			packet.Write((byte)QuestMessageType.SyncNPCQueue);
			packet.Write(true);
			packet.Write((ushort)talkNPC.type);
			packet.Write((byte)Main.myPlayer);
			packet.Send();
		}
	}

	private static void DrawPortraitName(NPC talkNPC, Point size)
	{
		string name = talkNPC.GivenName;
		Vector2 centring = ChatManager.GetStringSize(FontAssets.ItemStack.Value, name, new Vector2(ProfileNameScale)) / 2; //Position centring
		Vector2 textPos = new Vector2(Main.screenWidth / 2 - (198 + size.X), 114 + size.Y) - centring; //Real position
		ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.ItemStack.Value, name, textPos, new Color(240, 240, 240), 0f, new Vector2(), new Vector2(ProfileNameScale), -1, 2f); //Name
	}

	private static void Main_OnPreDraw(GameTime obj)
	{

	}

	private static void Main_DrawCachedProjs(On_Main.orig_DrawCachedProjs orig, Main self, List<int> projCache, bool startSpriteBatch)
	{
		if (!Main.dedServ && projCache == Main.instance.DrawCacheProjsBehindNPCs)
			SpiritMod.TrailManager.DrawTrails(Main.spriteBatch, TrailLayer.UnderCachedProjsBehindNPC);

		orig(self, projCache, startSpriteBatch);
	}

	private static void Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
	{
		if (!Main.dedServ)
		{
			SpiritMod.TrailManager.DrawTrails(Main.spriteBatch, TrailLayer.UnderProjectile);
			SpiritMod.primitives.DrawTargetProj(Main.spriteBatch);
		}

		orig(self);

		if (!Main.dedServ)
			SpiritMod.TrailManager.DrawTrails(Main.spriteBatch, TrailLayer.AboveProjectile);
	}

	private static void Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
	{
		if (!Main.dedServ)
			SpiritMod.primitives.DrawTargetNPC(Main.spriteBatch);

		if (SpiritMod.Metaballs is null || !SpiritMod.Metaballs.loaded) //LTTS
		{
			orig(self, behindTiles);
			return;
		}
		
		SpiritMod.Metaballs.DrawEnemyLayer(Main.spriteBatch);
		SpiritMod.Metaballs.DrawNebulaLayer(Main.spriteBatch);

		//PathfinderGNPC.DrawBuffedOutlines(Main.spriteBatch);
		orig(self, behindTiles);
	}

	private static void Player_KeyDoubleTap(On_Player.orig_KeyDoubleTap orig, Player self, int keyDir)
	{
		orig(self, keyDir);

		self.GetSpiritPlayer().DoubleTapEffects(keyDir);

		if (keyDir == 0)
			self.GetModPlayer<DoubleTapPlayer>().DoubleTapDown();
	}

	private static void Player_ToggleInv(On_Player.orig_ToggleInv orig, Player self)
	{
		SpiritMod spirit = ModContent.GetInstance<SpiritMod>();

		if (spirit.BookUserInterface.CurrentState != null) 
		{
			spirit.BookUserInterface.SetState(null);
			SoundEngine.PlaySound(SoundID.MenuClose);
			return;
		}

		orig(self);
	}

        private static string LanguageManager_GetTextValue_string1(Terraria.Localization.On_LanguageManager.orig_GetTextValue_string orig, Terraria.Localization.LanguageManager self, string key)
        {
            if (key == "GameUI.LightRain" || key == "GameUI.Rain" || key == "GameUI.HeavyRain" || key == "GameUI.Clear" || key == "GameUI.PartlyCloudy" || key == "GameUI.MostlyCloudy" || key == "GameUI.Overcast"|| key == "GameUI.Cloudy")
                return SpiritMod.GetWeatherRadioText(key);
            return orig(self, key);
        }

	private static void DrawParticles(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
	{
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
		ParticleHandler.DrawAllParticles(Main.spriteBatch);
		Main.spriteBatch.End();

		orig(self, gameTime);
	}
}