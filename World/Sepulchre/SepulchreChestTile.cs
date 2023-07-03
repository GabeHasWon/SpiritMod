using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.NPCs.Enchanted_Armor;
using SpiritMod.Projectiles;
using SpiritMod.Tiles.BaseTile;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.World.Sepulchre
{
	public class SepulchreChestTile : BaseChest
	{
		public override string DefaultName => "Sepulchre Chest";

		public override void StaticDefaults(ModTranslation name)
		{
			Main.tileShine[Type] = 1200;
			AddMapEntry(new Color(120, 82, 49), name);
			ChestDrop = ModContent.ItemType<SepulchreChest>();
			DustType = DustID.Asphalt;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath6, new Vector2(i * 16, j * 16));
			base.KillMultiTile(i, j, frameX, frameY);
		}

		public override bool IsLockedChest(int i, int j) => true;

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];

			bool anyCursedArmor = false;
			for (int indexX = -70; indexX <= 70; indexX++)
			{
				for (int indexY = -90; indexY <= 90; indexY++)
				{
					if (Framing.GetTileSafely(indexX + i, indexY + j).TileType == ModContent.TileType<CursedArmor>())
					{
						WorldGen.KillTile(indexX + i, indexY + j);

						if (Main.netMode != NetmodeID.SinglePlayer)
							NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, indexX + i, indexY + j);
						anyCursedArmor = true;
					}
				}
			}

			if (anyCursedArmor)
			{
				CombatText.NewText(new Rectangle(i * 16, j * 16, 20, 10), Color.GreenYellow, "Cursed!");
				return true;
			}
			else if (NPC.AnyNPCs(ModContent.NPCType<Enchanted_Armor>()))
			{
				SoundEngine.PlaySound(SoundID.NPCDeath6 with { Volume = 0.5f, PitchVariance = 0.2f }, new Vector2(i * 16, j * 16));
				foreach (NPC npc in Main.npc.Where(x => x.active && x.type == ModContent.NPCType<Enchanted_Armor>()))
					npc.ai[1] = 30;

				return true;
			}

			Main.mouseRightRelease = false;

			int left = i;
			int top = j;

			if (tile.TileFrameX % 36 != 0)
				left--;

			if (tile.TileFrameY != 0)
				top--;

			if (player.sign >= 0)
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
			}

			if (Main.editChest)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = "";
			}

			if (player.editedChestName)
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
				player.editedChestName = false;
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (left == player.chestX && top == player.chestY && player.chest >= 0)
				{
					player.chest = -1;
					Recipe.FindRecipes();
					SoundEngine.PlaySound(SoundID.MenuClose);
				}
				else
				{
					NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top, 0f, 0f, 0, 0, 0);
					Main.stackSplit = 600;
				}
			}
			else
			{
				int chest = Chest.FindChest(left, top);
				if (chest >= 0)
				{
					Main.stackSplit = 600;

					if (chest == player.chest)
					{
						player.chest = -1;
						SoundEngine.PlaySound(SoundID.MenuClose);
					}
					else
					{
						player.chest = chest;
						Main.playerInventory = true;
						Main.recBigList = false;
						player.chestX = left;
						player.chestY = top;
						SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
					}

					Recipe.FindRecipes();
				}
			}
			return true;
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
			if (tile.TileFrameX % 36 != 0)
				left--;
			if (tile.TileFrameY != 0)
				top--;
			int chest = Chest.FindChest(left, top);
			player.cursorItemIconID = -1;
			if (chest < 0)
			{
				player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
			}
			else
			{
				player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : "Sepulchre Chest";
				if (player.cursorItemIconText == "Sepulchre Chest")
				{
					player.cursorItemIconID = ModContent.ItemType<SepulchreChest>();
					player.cursorItemIconText = "";
				}
			}
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == "")
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}
	}
}