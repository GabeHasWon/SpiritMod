using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.Tiles;
using SpiritMod.Items.Placeable.Furniture.Bamboo;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Bamboo
{
	public class BambooPikeTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileNoFail[Type] = true;
			Main.tileFrameImportant[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.AlternateTile, 1, 0);
			TileObjectData.newTile.AnchorAlternateTiles = new int[] { Type };
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			RegisterItemDrop(ModContent.ItemType<BambooPikeItem>());
			AddMapEntry(new Color(80, 140, 35));
			DustType = DustID.JunglePlants;
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			bool hasTileAbove = Framing.GetTileSafely(i, j - 1).TileType == Type;
			bool hasTileBelow = Framing.GetTileSafely(i, j + 1).TileType == Type;

			if (!Framing.GetTileSafely(i, j + 1).HasTile) //Has any tile below
			{
				WorldGen.KillTile(i, j, false, false, false);
				return false;
			}

			if (hasTileAbove) //Pick the appropriate frame depending on tile stack
				tile.TileFrameY = (short)(18 * (hasTileBelow ? 1 : 2));
			else
				tile.TileFrameY = 0;

			if (hasTileBelow) //Inherit the same horizontal tile frame as the rest of the stack
				tile.TileFrameX = Framing.GetTileSafely(i, j + 1).TileFrameX;

			return false;
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			//Hurt players
			Player player = Main.LocalPlayer;
			if (player.velocity.Y < 1.25f)
				return;

			bool PlayerColliding()
			{
				Vector2 pos = new Vector2(player.Center.X, player.position.Y);
				for (int i = 0; i < 4; i++)
				{
					Tile tile = Framing.GetTileSafely((int)(pos.X / 16), (int)(pos.Y / 16) + i);
					if (tile.HasTile && tile.TileType == ModContent.TileType<BambooPikeTile>() && tile.TileFrameY == 0) //The tip of the pike
						return true;
				}
				return false;
			}

			if (PlayerColliding())
			{
				float damage = MathHelper.Clamp(player.velocity.Y, 2, 10) * 10f;
				player.Hurt(PlayerDeathReason.ByOther(3), (int)damage, 0);

				int buffType = ModContent.BuffType<Impaled>();
				bool hadBuff = player.HasBuff(buffType);

				player.velocity = new Vector2(0, 1);
				player.AddBuff(buffType, 500);

				SoundEngine.PlaySound(SoundID.NPCDeath12, player.Center);
				for (int d = 0; d < 20; d++)
					Dust.NewDustPerfect(player.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 10), DustID.Blood);

				if (!hadBuff && Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
			}
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			Texture2D texture = TextureAssets.Tile[Type].Value;
			Rectangle source = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);

			Vector2 offset = (Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1) ? Vector2.Zero : Vector2.One * 12;
			Vector2 drawPos = ((new Vector2(i, j) + offset) * 16) - Main.screenPosition + new Vector2(tile.TileFrameX / 18 * 2, 2);

			spriteBatch.Draw(texture, drawPos, source, Lighting.GetColor(i, j), 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
			return false;
		}
	}

	internal class BambooPikeNPC : GlobalNPC
	{
		public override void AI(NPC npc)
		{
			if (npc.velocity.Y < 1.25f || npc.HasBuff(ModContent.BuffType<Impaled>()))
				return;

			float damage = MathHelper.Min(npc.velocity.Y, 10) * 10f;
			Tile tile = Framing.GetTileSafely((int)(npc.Center.X / 16), (int)((npc.Center.Y + ((npc.height / 2) - 8)) / 16));

			if (tile.HasTile && tile.TileType == ModContent.TileType<BambooPikeTile>() && tile.TileFrameY == 0) //The tip of the pike
			{
				npc.SimpleStrikeNPC((int)damage, 1, false, 0);
				npc.AddBuff(ModContent.BuffType<Impaled>(), 500);

				SoundEngine.PlaySound(SoundID.NPCDeath12, npc.Center);
			}
		}
	}
}