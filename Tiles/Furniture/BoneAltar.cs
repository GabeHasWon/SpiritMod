using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.NPCs.Reach;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture
{
	public class BoneAltar : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileLighted[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Bone Altar");
			AddMapEntry(Colors.RarityAmber, name);

			DustType = DustID.Bone;
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) { r = .46f; g = .32f; b = .1f; }

		public override bool IsTileDangerous(int i, int j, Player player) => true;

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 4 : 8;

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
				zero = Vector2.Zero;

			int height = tile.TileFrameY == 36 ? 18 : 16;
			spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Furniture/BoneAltar_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (closer && Main.rand.NextBool(20))
				Dust.NewDust(new Vector2(i * 16, j * 16 - 10), 0, 16, DustID.Torch, 0.0f, -1, 0, new Color(), 0.5f);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			int npcType = ModContent.NPCType<ForestWraith>();
			if (NPC.AnyNPCs(npcType))
				return;

			for (int a = 0; a < 30; a++)
			{
				Vector2 offset = (Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * new Vector2(40)) + new Vector2(16);
				Dust dust = Dust.NewDustDirect(new Vector2(i * 16, j * 16) + offset, 0, 0, DustID.Torch, 0.0f, 0.0f, 0, new Color(), 1f);
				dust.velocity = Vector2.Zero;
				dust.noGravity = true;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int npcID = NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16, j * 16 - 300, npcType, ai0: 2, ai1: 1);

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcID);
					ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(Language.GetTextValue("Mods.SpiritMod.NPCs.ForestWraith.BoneAltar")), new Color(0, 170, 60));
				}
				else
				{
					Main.NewText(Language.GetTextValue("Mods.SpiritMod.NPCs.ForestWraith.BoneAltar"), 0, 170, 60);
				}
			}
		}
	}
}