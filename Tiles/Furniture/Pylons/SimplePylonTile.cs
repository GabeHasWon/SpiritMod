using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Pylons
{
	internal abstract class SimplePylonTile<T> : ModPylon where T : ModItem
	{
		public const int CrystalHorizontalFrameCount = 2;
		public const int CrystalVerticalFrameCount = 8;
		public const int CrystalFrameHeight = 64;

		public Asset<Texture2D> crystalTexture;
		public Asset<Texture2D> highlightTexture;
		public Asset<Texture2D> mapIcon;

		internal abstract string MapKeyName { get; }
		internal abstract Condition CanBeSold { get; }

		public override void Load()
		{
			crystalTexture = ModContent.Request<Texture2D>(Texture + "_Crystal");
			highlightTexture = ModContent.Request<Texture2D>("SpiritMod/Tiles/Furniture/Pylons/PylonHighlight");
			mapIcon = ModContent.Request<Texture2D>(Texture + "_MapIcon");
		}

		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.StyleHorizontal = true;

			TEModdedPylon moddedPylon = ModContent.GetInstance<SimplePylonEntity>();
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(moddedPylon.PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(moddedPylon.Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);

			TileID.Sets.InteractibleByNPCs[Type] = true;
			TileID.Sets.PreventsSandfall[Type] = true;

			DustType = -1;
			AddToArray(ref TileID.Sets.CountsAsPylon);
			StaticDefaults(Language.GetText(MapKeyName));
		}

		/// <param name="name"> The default name associated with the pylon. </param>
		public virtual void StaticDefaults(LocalizedText name) => AddMapEntry(Color.White, name);

		public virtual bool IsSold(int npcType, Player player, bool npcHappyEnough) => true;
		public override NPCShop.Entry GetNPCShopEntry() => new(ModContent.ItemType<T>(), Condition.AnotherTownNPCNearby, Condition.NotInEvilBiome, Condition.HappyEnoughToSellPylons, CanBeSold);

		public override void MouseOver(int i, int j)
		{
			Main.LocalPlayer.cursorItemIconEnabled = true;
			Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<T>();
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) => ModContent.GetInstance<SimplePylonEntity>().Kill(i, j); //Kill pylon tile entity
		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) => DefaultDrawPylonCrystal(spriteBatch, i, j, crystalTexture, highlightTexture, new Vector2(0f, -12f), Color.White * 0.1f, Color.White, 4, CrystalVerticalFrameCount);

		public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale)
		{
			// Just like in SpecialDraw, we want things to be handled the EXACT same way vanilla would handle it, which ModPylon also has built in methods for:
			bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f), drawColor, deselectedScale, selectedScale);
			DefaultMapClickHandle(mouseOver, pylonInfo, MapKeyName, ref mouseOverText);
		}
	}
}