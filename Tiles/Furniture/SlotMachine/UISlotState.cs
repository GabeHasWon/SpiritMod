using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SpiritMod.Tiles.Furniture.SlotMachine
{
	class UISlotState : UIState
	{
		private UIDragableElement mainPanel;

		internal static float offsetX = -1;
		internal static float offsetY = -1;

		readonly Player _player;
		readonly int X;
		readonly int Y;

		public UISlotState(int x, int y, Player player)
		{
			X = x;
			Y = y;
			_player = player;
		}

		public override void OnInitialize()
		{
			mainPanel = new UIDragableElement
			{
				HAlign = 0.5f,
				VAlign = 0.5f,
				Width = StyleDimension.FromPixels(300f),
				Height = StyleDimension.FromPixels(188f),
			};

			Append(mainPanel);

			if (offsetX != -1)
			{
				mainPanel.Left.Set(offsetX, 0f);
				mainPanel.Top.Set(offsetY, 0f);
			}

			var panelBackground = new UIImage(ModContent.Request<Texture2D>("SpiritMod/Tiles/Furniture/SlotMachine/SlotMachineBaseFramer"))
			{
				Width = StyleDimension.FromPercent(1),
				Height = StyleDimension.FromPercent(1)
			};

			panelBackground.SetPadding(12);
			mainPanel.Append(panelBackground);
			mainPanel.AddDragTarget(panelBackground);

			UIImageButton closeButton = new UIImageButton(ModContent.Request<Texture2D>("SpiritMod/Tiles/Furniture/SlotMachine/Close"))
			{
				Width = StyleDimension.FromPixels(15),
				Height = StyleDimension.FromPixels(15),
				Left = StyleDimension.FromPixelsAndPercent(-50, 1),
				Top = StyleDimension.FromPixels(5)
			};
			closeButton.OnClick += CloseButton_OnClick;
			panelBackground.Append(closeButton);

			UIImageButton pullButton = new UIImageButton(ModContent.Request<Texture2D>("SpiritMod/Tiles/Furniture/SlotMachine/SlotMachineBaseFramer"))
			{
				Left = StyleDimension.FromPixelsAndPercent(-20, 1),
				Top = StyleDimension.FromPixels(-10),
				Width = StyleDimension.FromPixels(40),
				Height = StyleDimension.FromPixels(40),
			};
			pullButton.OnClick += PullButton_OnClick;
			panelBackground.Append(pullButton);
		}

		private int drawCounter;
		private float pulleyCounter = -1;

		private float symbolCounterOne = 0;
		private float symbolCounterTwo = 0;
		private float symbolCounterThree = 0;

		private bool[] hasRigged = new[] { false, false };

		public override void Update(GameTime gameTime)
		{
			if (pulleyCounter < 20) //Unset rig flags
				hasRigged[0] = hasRigged[1] = false;

			if (pulleyCounter < 30)
				symbolCounterOne += Main.rand.NextFloat(0.25f, 0.45f) * (float)gameTime.ElapsedGameTime.TotalSeconds * 60;

			if (pulleyCounter < 40)
				symbolCounterTwo += Main.rand.NextFloat(0.25f, 0.45f) * (float)gameTime.ElapsedGameTime.TotalSeconds * 60;

			if (pulleyCounter < 50)
				symbolCounterThree += Main.rand.NextFloat(0.25f, 0.45f) * (float)gameTime.ElapsedGameTime.TotalSeconds * 60;

			if (pulleyCounter >= 0)
				pulleyCounter += 0.4f * (float)gameTime.ElapsedGameTime.TotalSeconds * 60;

			TryRigResult(ref symbolCounterTwo, ref hasRigged[0], 40); //Try rigging (1/10 chance to rig positively)
			TryRigResult(ref symbolCounterThree, ref hasRigged[1], 50);

			base.Update(gameTime);

			Vector2 dist = _player.Center - (new Vector2(X, Y) * 16);
			if (dist.LengthSquared() > 100 * 100)
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				ModContent.GetInstance<SpiritMod>().SlotUserInterface.SetState(null);
			}

			if (pulleyCounter >= 55)
				CheckRewards();
		}

		private void TryRigResult(ref float timer, ref bool flag, int max)
		{
			if (pulleyCounter >= max && !flag)
			{
				flag = true;

				if (Main.LocalPlayer.RollLuck(32) == 0)
					timer = symbolCounterOne;
			}
		}

		private void CheckRewards()
		{
			if (((int)symbolCounterThree % numberOfSymbols) == ((int)symbolCounterTwo % numberOfSymbols) && ((int)symbolCounterThree % numberOfSymbols) == ((int)symbolCounterOne % numberOfSymbols))
			{
				var loc = new Rectangle(X * 16, Y * 16, _player.width, _player.height);
				var src = Main.LocalPlayer.GetSource_GiftOrReward(null);

				switch ((int)symbolCounterThree % numberOfSymbols)
				{
					case 0: //bell
						CombatText.NewText(loc, Color.Gold, "You win a bell!");
						Main.LocalPlayer.QuickSpawnItem(src, ModContent.ItemType<Items.Consumable.SurrenderBell>(), 1);
						break;
					case 1: //mjw
						CombatText.NewText(loc, Color.Gray, "You win a...jelly?");
						Main.LocalPlayer.QuickSpawnItem(src, ModContent.ItemType<Items.Consumable.DistressJellyItem>(), 1);
						break;
					case 2: //lemon
						CombatText.NewText(loc, Color.Gray, "You win lemons!");
						Main.LocalPlayer.QuickSpawnItem(src, ItemID.Lemon, 5);
						break;
					case 3: //diamond
						CombatText.NewText(loc, Color.Green, "You win diamonds!");
						Main.LocalPlayer.QuickSpawnItem(src, ItemID.Diamond, 15);
						break;
					case 4: //bomb
						CombatText.NewText(loc, Color.Red, "You win a...bomb!");
						Main.LocalPlayer.QuickSpawnItem(src, ItemID.Bomb, 1);
						break;
					case 5: //cherry
						CombatText.NewText(loc, Color.Red, "You win a ch- apple!");
						Main.LocalPlayer.QuickSpawnItem(src, ItemID.Apple, 1);
						break;
					case 6: //bar
						CombatText.NewText(loc, Color.Green, "You win a brick of gold!");
						Main.LocalPlayer.QuickSpawnItem(src, ItemID.GoldBar, 1);
						break;
					case 7: //seven
						CombatText.NewText(loc, Color.Gold, "You win the JACKPOT!");
						Main.LocalPlayer.QuickSpawnItem(src, ItemID.PlatinumCoin, 3);
						Main.LocalPlayer.QuickSpawnItem(src, ItemID.EnchantedSword, 1);
						Main.LocalPlayer.QuickSpawnItem(src, ItemID.DiscountCard, 1);
						break;
				}

				ModContent.GetInstance<SpiritMod>().SlotUserInterface.SetState(null);
			}
		}

		const int numberOfSymbols = 8;

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (mainPanel is null)
				return;

			if (mainPanel.ContainsPoint(Main.MouseScreen))
				Main.LocalPlayer.mouseInterface = true;

			if (mainPanel.Left.Pixels != 0)
			{
				offsetX = mainPanel.Left.Pixels;
				offsetY = mainPanel.Top.Pixels;
			}

			drawCounter++;

			var rectbase = new Rectangle(drawCounter % 16 > 7 ? 0 : 246, 0, 246, 188);
			int width = spriteBatch.GraphicsDevice.Viewport.Width;
			int height = spriteBatch.GraphicsDevice.Viewport.Height;
			Vector2 position = new Vector2(width, height) * new Vector2(mainPanel.HAlign, mainPanel.VAlign) / Main.UIScale;
			Texture2D texturebase = ModContent.Request<Texture2D>("SpiritMod/Tiles/Furniture/SlotMachine/SlotMachineBase", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			spriteBatch.Draw(texturebase, new Vector2(offsetX, offsetY) + position, rectbase, Color.White, 0, new Vector2(texturebase.Width / 4, texturebase.Height / 2), 1, SpriteEffects.None, 0f);

			Texture2D texturepull = ModContent.Request<Texture2D>("SpiritMod/Tiles/Furniture/SlotMachine/SlotMachinePulley", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			var rectpull = new Rectangle(26 * Clamp((int)pulleyCounter, 0, 6), 0, 26, 80);
			spriteBatch.Draw(texturepull, new Vector2(offsetX, offsetY) + position + new Vector2(texturebase.Width / 4, texturebase.Height / -2), rectpull, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);

			Texture2D texturesymbols = ModContent.Request<Texture2D>("SpiritMod/Tiles/Furniture/SlotMachine/SlotMachineSymbols", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

			Vector2 symbolOffsetOne = new Vector2(29, 70);
			Vector2 symbolOffsetTwo = symbolOffsetOne + new Vector2(68, 0);
			Vector2 symbolOffsetThree = symbolOffsetTwo + new Vector2(68, 0);

			var rectSymbolOne = new Rectangle(52 * ((int)symbolCounterOne % numberOfSymbols), 0, 52, 70);
			var rectSymbolTwo = new Rectangle(52 * ((int)symbolCounterTwo % numberOfSymbols), 0, 52, 70);
			var rectSymbolThree = new Rectangle(52 * ((int)symbolCounterThree % numberOfSymbols), 0, 52, 70);

			Vector2 offset = new Vector2(texturebase.Width / -4, texturebase.Height / -2);
			spriteBatch.Draw(texturesymbols, symbolOffsetOne + position + new Vector2(offsetX, offsetY) + offset, rectSymbolOne, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
			spriteBatch.Draw(texturesymbols, symbolOffsetTwo + position + new Vector2(offsetX, offsetY) + offset, rectSymbolTwo, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
			spriteBatch.Draw(texturesymbols, symbolOffsetThree + position + new Vector2(offsetX, offsetY) + offset, rectSymbolThree, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);

			base.DrawSelf(spriteBatch);
		}

		private void CloseButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			ModContent.GetInstance<SpiritMod>().SlotUserInterface.SetState(null);
		}

		private void PullButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (pulleyCounter == -1 || pulleyCounter > 60)
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				pulleyCounter = 0;
			}
		}

		private static int Clamp(int x, int min, int max) //for some reason Math.Clamp wouldn't work
		{
			if (min <= x && x <= max)
				return x;
			if (min > x)
				return min;
			if (max < x)
				return max;
			return x;
		}
	}
}