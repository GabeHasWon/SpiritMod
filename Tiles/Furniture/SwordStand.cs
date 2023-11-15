using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.Items.Sets.DashSwordSubclass.AnimeSword;
using SpiritMod.Items.Sets.DashSwordSubclass.BladeOfTheDragon;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture;

public class SwordStand : ModTile
{
	private static bool TileEntityCheck(int i, int j, out SwordStandTileEntity entity)
	{
		if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity value) && value is SwordStandTileEntity)
		{
			entity = value as SwordStandTileEntity;
			return true;
		}
		entity = null;
		return false;
	}

	public override void SetStaticDefaults()
	{
		Main.tileTable[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
		ModTileEntity tileEntity = ModContent.GetInstance<SwordStandTileEntity>();
		TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(tileEntity.Hook_AfterPlacement, -1, 0, false);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.AnchorWall = true;
		TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
		TileObjectData.addAlternate(1);
		TileObjectData.addTile(Type);

		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(140, 140, 140), name);
		DustType = -1;
	}

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

	public override bool RightClick(int i, int j)
	{
		static bool CanPlace(Item item) => (item.IsMelee() && !item.noMelee && item.useTime > 0 && item.pick == 0 && item.axe == 0) ||
			new int[] { ModContent.ItemType<AnimeSword>(), ModContent.ItemType<BladeOfTheDragon>() }.Contains(item.type);
		static Item Held(Player player) => (Main.mouseItem == null || Main.mouseItem.IsAir) ? player.HeldItem : Main.mouseItem;

		//Select the top leftmost of the tile, because that's where our entity is
		Tile tile = Framing.GetTileSafely(i, j);
		(i, j) = (i - (tile.TileFrameX % 54 / 18), j - (tile.TileFrameY / 18));

		if (TileEntityCheck(i, j, out SwordStandTileEntity entity))
		{
			if (entity.item != null)
			{
				Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_TileInteraction(i, j), entity.item);
				entity.item = null;
			}
			else if (CanPlace(Held(Main.LocalPlayer)))
			{
				entity.item = Held(Main.LocalPlayer).Clone();
				Held(Main.LocalPlayer).TurnToAir();
				return true;
			}
		}
		return false;
	}

	public override void MouseOver(int i, int j)
	{
		//Select the top leftmost of the tile, because that's where our entity is
		Tile tile = Framing.GetTileSafely(i, j);
		(i, j) = (i - (tile.TileFrameX % 54 / 18), j - (tile.TileFrameY / 18));

		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = (TileEntityCheck(i, j, out SwordStandTileEntity entity) && entity.item != null) 
			? entity.item.type
			: ModContent.ItemType<SwordStandItem>();
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY) => ModContent.GetInstance<SwordStandTileEntity>().Kill(i, j);

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		var drops = new List<Item>() { new Item(ModContent.ItemType<SwordStandItem>()) };
		if (TileEntityCheck(i, j, out SwordStandTileEntity entity) && entity.item != null)
			drops.Add(entity.item);

		foreach (Item item in drops)
			yield return item;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (TileEntityCheck(i, j, out SwordStandTileEntity entity) && entity.item != null)
		{
			if ((new int[] { ModContent.ItemType<AnimeSword>(), ModContent.ItemType<BladeOfTheDragon>() }).Contains(entity.item.type))
			{
				Texture2D texture = (entity.item.type == ModContent.ItemType<AnimeSword>())
					? Mod.Assets.Request<Texture2D>("Items/Sets/DashSwordSubclass/AnimeSword/AnimeSword_Held").Value 
					: Mod.Assets.Request<Texture2D>("Items/Sets/DashSwordSubclass/BladeOfTheDragon/BladeOfTheDragon_Held").Value;

				Vector2 lightOff = Lighting.LegacyEngine.Mode > 1 ? Vector2.Zero : Vector2.One * 12;
				Vector2 offset = new Vector2(36, (entity.item.type == ModContent.ItemType<AnimeSword>()) ? 12 : 24);

				Vector2 drawPos = ((new Vector2(i, j) + lightOff) * 16) + offset - Main.screenPosition;
				Rectangle frame = texture.Frame(1, 2, sizeOffsetY: -2);

				spriteBatch.Draw(texture, drawPos, frame, Lighting.GetColor(i, j), 0, frame.Size() / 2, 1, SpriteEffects.None, 0);
			} //If the item is a katana, draw specially
			else
			{
				Texture2D texture = TextureAssets.Item[entity.item.type].Value;
				Vector2 offset = Lighting.LegacyEngine.Mode > 1 ? Vector2.Zero : Vector2.One * 12;
				Vector2 drawPos = ((new Vector2(i, j) + offset) * 16) + new Vector2(24, 12) - Main.screenPosition;

				spriteBatch.Draw(texture, drawPos, null, Lighting.GetColor(i, j), MathHelper.PiOver4, texture.Size() / 2, 1, SpriteEffects.None, 0);
			}
		}
		return true;
	}
}

public class SwordStandTileEntity : ModTileEntity
{
	public Item item = null;

	public override bool IsTileValidForEntity(int x, int y)
	{
		Tile tile = Framing.GetTileSafely(x, y);
		return tile.HasTile && tile.TileType == ModContent.TileType<SwordStand>();
	}

	public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
	{
		Point pos = new Point(i - 1, j - 1);
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			NetMessage.SendTileSquare(Main.myPlayer, pos.X, pos.Y, 3);
			NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, pos.X, pos.Y, Type, 0f, 0, 0, 0);
			return -1;
		}
		return Place(pos.X, pos.Y);
	}

	public override void OnNetPlace() => NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);

	public override void NetSend(BinaryWriter writer) => ItemIO.Send(item, writer);

	public override void NetReceive(BinaryReader reader) => item = ItemIO.Receive(reader);

	public override void SaveData(TagCompound tag)
	{
		if (item != null)
			tag[nameof(item)] = item;
	}

	public override void LoadData(TagCompound tag) => item = tag.Get<Item>(nameof(item));
}