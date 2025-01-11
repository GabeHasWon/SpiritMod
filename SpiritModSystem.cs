using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.GlobalClasses.Projectiles;
using SpiritMod.GlobalClasses.Tiles;
using SpiritMod.Items.Consumable;
using SpiritMod.Items.Sets.BismiteSet;
using SpiritMod.Items.Sets.BloodcourtSet;
using SpiritMod.Items.Sets.BowsMisc.GemBows.Emerald_Bow;
using SpiritMod.Items.Sets.BowsMisc.GemBows.Ruby_Bow;
using SpiritMod.Items.Sets.BowsMisc.GemBows.Sapphire_Bow;
using SpiritMod.Items.Sets.BowsMisc.GemBows.Topaz_Bow;
using SpiritMod.Items.Sets.BriarDrops;
using SpiritMod.Items.Sets.CoilSet;
using SpiritMod.Items.Sets.FrigidSet;
using SpiritMod.Items.Sets.HuskstalkSet;
using SpiritMod.Items.Sets.SlagSet;
using SpiritMod.Mechanics.AutoSell;
using SpiritMod.Mechanics.QuestSystem;
using SpiritMod.NPCs.AuroraStag;
using SpiritMod.NPCs.StarjinxEvent;
using SpiritMod.NPCs.Tides.Tide;
using SpiritMod.Particles;
using SpiritMod.Utilities;
using SpiritMod.Utilities.Journey;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace SpiritMod
{
	internal class SpiritModSystem : ModSystem
	{
		public override void PostAddRecipes()
		{
			ExplosivesCache.Initialize(Mod);
			ModContent.GetInstance<TagGlobalTile>().Load(Mod);
			SacrificeAutoloader.Load(Mod);
		}

		public override void AddRecipeGroups()
		{
			RecipeGroup woodGrp = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Wood"]];
			woodGrp.ValidItems.Add(ModContent.ItemType<AncientBark>());
			woodGrp.ValidItems.Add(ModContent.ItemType<Items.Placeable.Tiles.SpiritWoodItem>());
			woodGrp.ValidItems.Add(ModContent.ItemType<Items.Sets.FloatingItems.Driftwood.DriftwoodTileItem>());

			RecipeGroup butterflyGrp = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Butterflies"]];
			butterflyGrp.ValidItems.Add(ModContent.ItemType<BriarmothItem>());

			RecipeGroup BaseGroup(object GroupName, int[] Items)
			{
				string Name = "";
				Name += GroupName switch
				{
					//modcontent items
					int i => Lang.GetItemNameValue((int)GroupName),
					//vanilla item ids
					short s => Lang.GetItemNameValue((short)GroupName),
					//custom group names
					_ => GroupName.ToString(),
				};
				return new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Name, Items);
			}

			RecipeGroup.RegisterGroup("SpiritMod:CopperBars", BaseGroup(ItemID.CopperBar, new int[]
				{ ItemID.CopperBar, ItemID.TinBar }));

			RecipeGroup.RegisterGroup("SpiritMod:SilverBars", BaseGroup(ItemID.SilverBar, new int[]
				{ ItemID.SilverBar, ItemID.TungstenBar }));

			RecipeGroup.RegisterGroup("SpiritMod:GoldBars", BaseGroup(ItemID.GoldBar, new int[]
				{ ItemID.GoldBar, ItemID.PlatinumBar }));

			RecipeGroup.RegisterGroup("SpiritMod:Tier3HMBar", BaseGroup(ItemID.AdamantiteBar, new int[]
				{ ItemID.AdamantiteBar, ItemID.TitaniumBar }));

			RecipeGroup.RegisterGroup("SpiritMod:PHMEvilMaterial", BaseGroup(ItemID.ShadowScale, new int[]
				{ ItemID.ShadowScale, ItemID.TissueSample }));

			RecipeGroup.RegisterGroup("SpiritMod:EvilMaterial", BaseGroup(ItemID.CursedFlame, new int[]
				{ ItemID.CursedFlame, ItemID.Ichor }));

			RecipeGroup.RegisterGroup("SpiritMod:EmeraldBows", BaseGroup("Emerald or Ruby Bow", new int[]
				{ ModContent.ItemType<Emerald_Bow>(), ModContent.ItemType<Ruby_Bow>() }));

			RecipeGroup.RegisterGroup("SpiritMod:TopazBows", BaseGroup("Sapphire or Topaz Bow", new int[]
				{ ModContent.ItemType<Sapphire_Bow>(), ModContent.ItemType<Topaz_Bow>() }));

			RecipeGroup.RegisterGroup("SpiritMod:AmethystStaffs", BaseGroup("Amethyst or Topaz Staff", new int[]
				{ ItemID.AmethystStaff, ItemID.TopazStaff }));

			RecipeGroup.RegisterGroup("SpiritMod:SapphireStaffs", BaseGroup("Sapphire or Emerald Staff", new int[]
				{ ItemID.SapphireStaff, ItemID.EmeraldStaff }));

			RecipeGroup.RegisterGroup("SpiritMod:RubyStaffs", BaseGroup("Ruby or Diamond Staff", new int[]
				{ ItemID.RubyStaff, ItemID.DiamondStaff }));
		}

		public override void AddRecipes()
		{
			Recipe.Create(ItemID.IceTorch, 3)
				.AddIngredient(ModContent.ItemType<FrigidFragment>())
				.AddIngredient(ItemID.Torch, 3);

			Recipe.Create(ItemID.PoisonDart, 25)
				.AddIngredient(ModContent.ItemType<BismiteCrystal>());

			Recipe.Create(ItemID.StinkPotion)
				.AddTile(TileID.Bottles)
				.AddIngredient(ModContent.ItemType<BismiteCrystal>())
				.AddIngredient(ItemID.BottledWater)
				.AddIngredient(ItemID.Waterleaf);

			Recipe.Create(ItemID.WrathPotion)
				.AddTile(TileID.Bottles).AddIngredient(ModContent.ItemType<DreamstrideEssence>(), 2)
				.AddIngredient(ItemID.BottledWater)
				.AddIngredient(ItemID.Deathweed);
			
			Recipe.Create(ItemID.RagePotion)
				.AddTile(TileID.Bottles)
				.AddIngredient(ModContent.ItemType<DreamstrideEssence>(), 2)
				.AddIngredient(ItemID.BottledWater)
				.AddIngredient(ItemID.Deathweed);

			Recipe.Create(ItemID.ThornsPotion)
				.AddTile(TileID.Bottles).AddIngredient(ModContent.ItemType<EnchantedLeaf>())
				.AddIngredient(ItemID.BottledWater)
				.AddIngredient(ItemID.Deathweed)
				.AddIngredient(ItemID.Waterleaf)
				.AddIngredient(ItemID.Stinger)
				.AddIngredient(ItemID.Cactus);

			Recipe.Create(ItemID.Timer1Second)
				.AddTile(TileID.Anvils)
				.AddIngredient(ModContent.ItemType<TechDrive>())
				.AddIngredient(ItemID.Wire);

			Recipe.Create(ItemID.Timer3Second)
				.AddTile(TileID.Anvils)
				.AddIngredient(ModContent.ItemType<TechDrive>(), 3)
				.AddIngredient(ItemID.Wire);

			Recipe.Create(ItemID.Timer5Second)
				.AddTile(TileID.Anvils)
				.AddIngredient(ModContent.ItemType<TechDrive>(), 5)
				.AddIngredient(ItemID.Wire);

			Recipe.Create(ItemID.InfernoPotion)
				.AddTile(TileID.Bottles)
				.AddIngredient(ModContent.ItemType<CarvedRock>())
				.AddIngredient(ItemID.BottledWater)
				.AddIngredient(ItemID.Hellstone)
				.AddIngredient(ItemID.SoulofNight);

			Recipe.Create(ItemID.PoisonedKnife, 50)
				.AddIngredient(ModContent.ItemType<BismiteCrystal>())
				.AddIngredient(ItemID.ThrowingKnife, 50);
		}

		public override void ModifyLightingBrightness(ref float scale)
		{
			if (Main.LocalPlayer.ZoneBriar() && !Main.dayTime)
				scale *= .96f;
		}

		public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
		{
			if (ModContent.GetInstance<Biomes.BiomeTileCounts>().spiritCount > 0)
			{
				float strength = ModContent.GetInstance<Biomes.BiomeTileCounts>().spiritCount / 160f;
				if (strength > MyWorld.spiritLight)
					MyWorld.spiritLight += 0.01f;
				if (strength < MyWorld.spiritLight)
					MyWorld.spiritLight -= 0.01f;
			}
			else
				MyWorld.spiritLight -= 0.02f;

			if (MyWorld.spiritLight < 0f)
				MyWorld.spiritLight = 0f;
			else if (MyWorld.spiritLight > .9f)
				MyWorld.spiritLight = .9f;

			static int ColorAdjustment(int col, float light)
			{
				float val = 250f / 1.14f * light * (col / 255f);
				if (val < 0)
					val = 0;
				return (int)val;
			}

			if (MyWorld.spiritLight > 0f)
			{
				int r = backgroundColor.R - ColorAdjustment(backgroundColor.R, MyWorld.spiritLight);
				int g = backgroundColor.G - ColorAdjustment(backgroundColor.G, MyWorld.spiritLight);
				int b = backgroundColor.B - ColorAdjustment(backgroundColor.B, MyWorld.spiritLight);

				backgroundColor.R = (byte)r;
				backgroundColor.G = (byte)g;
				backgroundColor.B = (byte)b;
			}

			if (ModContent.GetInstance<Biomes.BiomeTileCounts>().asteroidCount > 0)
			{
				float strength = ModContent.GetInstance<Biomes.BiomeTileCounts>().asteroidCount / 160f;
				if (strength > MyWorld.asteroidLight)
					MyWorld.asteroidLight += 0.01f;
				if (strength < MyWorld.asteroidLight)
					MyWorld.asteroidLight -= 0.01f;
			}
			else
				MyWorld.asteroidLight -= 0.02f;

			if (MyWorld.asteroidLight < 0f)
				MyWorld.asteroidLight = 0f;
			else if (MyWorld.asteroidLight > 1f)
				MyWorld.asteroidLight = 1f;

			if (MyWorld.asteroidLight > 0f)
			{
				int r = backgroundColor.R - ColorAdjustment(backgroundColor.R, MyWorld.asteroidLight);
				if (backgroundColor.R > r)
					backgroundColor.R = (byte)r;

				int g = backgroundColor.G - ColorAdjustment(backgroundColor.G, MyWorld.asteroidLight);
				if (backgroundColor.G > g)
					backgroundColor.G = (byte)g;

				int b = backgroundColor.B - ColorAdjustment(backgroundColor.B, MyWorld.asteroidLight);

				if (backgroundColor.B > b)
					backgroundColor.B = (byte)b;
			}
		}

		public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
		{
			SpiritMod mod = Mod as SpiritMod;

			if (!Main.gameMenu)
			{
				mod.screenshakeTimer++;

				if (SpiritMod.tremorTime > 0 && mod.screenshakeTimer >= 20) // so it doesnt immediately decrease
					SpiritMod.tremorTime -= 0.5f;
				if (SpiritMod.tremorTime < 0)
					SpiritMod.tremorTime = 0;

				Main.screenPosition += new Vector2(SpiritMod.tremorTime * Main.rand.NextFloat(), SpiritMod.tremorTime * Main.rand.NextFloat());
			}
			else // dont shake on the menu
			{
				SpiritMod.tremorTime = 0;
				mod.screenshakeTimer = 0;
			}

			SpiritMod.InvokeModifyTransform(Transform);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			SpiritMod mod = Mod as SpiritMod;

			mod.BookUserInterface?.Update(gameTime);
			mod.SlotUserInterface?.Update(gameTime);
		}

		public override void PostUpdateEverything()
		{
			if (!Main.dedServ)
			{
				ParticleHandler.RunRandomSpawnAttempts();
				ParticleHandler.UpdateAllParticles();
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (inventoryIndex != -1)
			{
				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
					"SpiritMod: BookUI",
					delegate
					{
						SpiritMod.QuestHUD.Draw(Main.spriteBatch);

						if (Main.playerInventory && QuestManager.QuestBookUnlocked)
						{
							Texture2D bookTexture = SpiritMod.Instance.Assets.Request<Texture2D>("UI/QuestUI/Textures/QuestBookInventoryButton", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
							Vector2 bookSize = new Vector2(50, 52);
							QuestUtils.QuestInvLocation loc = ModContent.GetInstance<SpiritClientConfig>().QuestBookLocation;
							Vector2 position = Vector2.Zero;
							switch (loc)
							{
								case QuestUtils.QuestInvLocation.Minimap:
									position = new Vector2(Main.screenWidth - Main.miniMapWidth - bookSize.X * 2.3f, Main.miniMapY + 4);

									if (Main.screenWidth < 900)
										position.Y -= 60;
									break;
								case QuestUtils.QuestInvLocation.Trashcan:
									position = new Vector2(SpiritMod.AutoTrashEnabled ? 334 : 388, 258);
									break;
								case QuestUtils.QuestInvLocation.FarLeft:
									position = new Vector2(20, Main.GameModeInfo.IsJourneyMode ? 310 : 258);
									break;
							}

							Rectangle frame = new Rectangle(0, 0, 49, 52);
							bool hover = false;

							if (Main.MouseScreen.Between(position, position + bookSize))
							{
								hover = true;
								frame.X = 50;
								Main.LocalPlayer.mouseInterface = true;
								if (Main.mouseLeft && Main.mouseLeftRelease)
								{
									Main.mouseLeftRelease = false;
									QuestManager.SetBookState(SpiritMod.Instance._questBookToggle = !SpiritMod.Instance._questBookToggle);
								}
							}

							if (hover != SpiritMod.Instance._questBookHover)
							{
								SpiritMod.Instance._questBookHover = hover;
								SoundEngine.PlaySound(SoundID.MenuTick);
							}

							Main.spriteBatch.Draw(bookTexture, position, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
						}

						SpiritMod.Instance.BookUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);

				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
					"SpiritMod: SlotUI",
					delegate
					{
						SpiritMod.Instance.SlotUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);

				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
					"SpiritMod: SellUI",
					delegate
					{
						if (ModContent.GetInstance<SpiritClientConfig>().QuickSell)
							SpiritMod.DrawUpdateToggles();
						if (AutoSellUI.visible)
						{
							SpiritMod.Instance.AutoSellUI_INTERFACE.Update(Main._drawInterfaceGameTime);
							SpiritMod.Instance.AutoSellUI_SHORTCUT.Draw(Main.spriteBatch);
						}
						if (Mechanics.AutoSell.Sell_NoValue.Sell_NoValue.visible)
						{
							SpiritMod.Instance.SellNoValue_INTERFACE.Update(Main._drawInterfaceGameTime);
							SpiritMod.Instance.SellNoValue_SHORTCUT.Draw(Main.spriteBatch);
						}
						if (Mechanics.AutoSell.Sell_Lock.Sell_Lock.visible)
						{
							SpiritMod.Instance.SellLock_INTERFACE.Update(Main._drawInterfaceGameTime);
							SpiritMod.Instance.SellLock_SHORTCUT.Draw(Main.spriteBatch);
						}
						if (Mechanics.AutoSell.Sell_Weapons.Sell_Weapons.visible)
						{
							SpiritMod.Instance.SellWeapons_INTERFACE.Update(Main._drawInterfaceGameTime);
							SpiritMod.Instance.SellWeapons_SHORTCUT.Draw(Main.spriteBatch);
						}
						return true;
					},
					InterfaceScaleType.UI)
				);

				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer("SpiritMod: Starjinx UI", delegate
				{
					StarjinxUI.DrawStarjinxEventUI(Main.spriteBatch);

					return true;
				}, InterfaceScaleType.UI));
			}

			if (TideWorld.TheTide)
			{
				int index = layers.FindIndex(layer => layer is not null && layer.Name.Equals("Vanilla: Inventory"));
				LegacyGameInterfaceLayer NewLayer = new LegacyGameInterfaceLayer("SpiritMod: Tide UI",
					delegate
					{
						SpiritMod.Instance.DrawEventUI(Main.spriteBatch);
						return true;
					},
					InterfaceScaleType.UI);
				layers.Insert(index, NewLayer);
			}

			int mouseIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Item / NPC Head"));
			if (mouseIndex != -1)
			{
				layers.Insert(mouseIndex, new LegacyGameInterfaceLayer(
					"Spirit: Stag Hover",
					delegate
					{
						Item item = Main.mouseItem.IsAir ? Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem] : Main.mouseItem;
						AuroraStag auroraStag = Main.LocalPlayer.GetModPlayer<MyPlayer>().hoveredStag;

						if (item.type == ModContent.ItemType<Items.Consumable.Food.IceBerries>() && auroraStag != null && !auroraStag.NPC.immortal && auroraStag.TameAnimationTimer == 0)
						{
							Texture2D itemTexture = TextureAssets.Item[item.type].Value;
							Vector2 itemPos = Main.MouseScreen + Vector2.UnitX * -(itemTexture.Width / 2 + 4);
							Vector2 origin = new Vector2(itemTexture.Width / 2, 0);
							Main.spriteBatch.Draw(itemTexture, itemPos, new Rectangle(0, 0, itemTexture.Width, itemTexture.Height / 3), Color.White, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1.5f) * 0.2f, origin, 1f, SpriteEffects.None, 0f);
						}

						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		public override void PreUpdateItems()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				SpiritMod.TrailManager.UpdateTrails();
				SpiritMod.primitives.UpdateTrails();
			}
		}
	}
}
