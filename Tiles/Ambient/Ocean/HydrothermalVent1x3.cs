using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Ocean
{
	[Sacrifice(1)]
	public class LargeVentItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = 0;
			Item.rare = ItemRarityID.Blue;
			Item.createTile = ModContent.TileType<Breakable1x3Vent>();
			Item.maxStack = Item.CommonMaxStack;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
		}

		public override bool? UseItem(Player player)
		{
			Item.placeStyle = Main.rand.Next(2);
			return null;
		}

		public override void AddRecipes()
		{
			var recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.AshBlock, 20);
			recipe.AddIngredient(ItemID.Obsidian, 1);
			recipe.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe.Register();
		}
	}

	[TileTag(TileTags.Indestructible)]
	public class HydrothermalVent1x3 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileSpelunker[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Origin = new Point16(0, 2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.RandomStyleRange = 1;
			TileObjectData.addTile(Type);

			TileID.Sets.DisableSmartCursor[Type] = true;
			DustType = DustID.Stone;

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Hydrothermal Vent");
			AddMapEntry(new Color(64, 54, 66), name);
		}

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			if (player.ZoneBeach && player.GetSpiritPlayer().Submerged(45))
				HitWire(i, j);
			return true;
		}

		public sealed override void HitWire(int i, int j)
		{
			if (Wiring.CheckMech(i, j, 7200))
			{
				for (int k = 0; k <= 20; k++)
					Dust.NewDustPerfect(new Vector2(i * 16 + 12, j * 16 - 36), ModContent.DustType<Dusts.BoneDust>(), new Vector2(0, 6).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1));

				for (int k = 0; k <= 20; k++)
					Dust.NewDustPerfect(new Vector2(i * 16 + 12, j * 16 - 36), ModContent.DustType<Dusts.FireClubDust>(), new Vector2(0, 6).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1));

				Projectile.NewProjectile(new EntitySource_Wiring(i, j), i * 16 + 12, j * 16 - 36, 0, -7, ModContent.ProjectileType<Projectiles.HydrothermalVentPlume>(), 5, 0f);
			}
		}

		public sealed override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<LargeVentItem>();
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) => spriteEffects = (i % 2 == 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

		public override void NearbyEffects(int i, int j, bool closer)
		{
			var config = ModContent.GetInstance<Utilities.SpiritClientConfig>();

			Tile t = Framing.GetTileSafely(i, j);

			if (t.TileFrameY == 0 && t.TileFrameX == 0)
				HydrothermalVent1x2.SpawnSmoke(new Vector2(i - 0.75f, j) * 16);

			if (config.VentCritters)
			{
				if (t.LiquidAmount > 155)
				{
					HydrothermalVent1x2.SpawnCritter<NPCs.Critters.TinyCrab>(i, j, 500);
					HydrothermalVent1x2.SpawnCritter<NPCs.Critters.Ocean.Crinoid>(i, j, 110);
					HydrothermalVent1x2.SpawnCritter<NPCs.Critters.TubeWorm>(i, j, 425);
				}
			}
		}
	}

	public class Breakable1x3Vent : HydrothermalVent1x3
	{
		public override string Texture => base.Texture.Replace(nameof(Breakable1x3Vent), nameof(HydrothermalVent1x3));

		public override bool CanExplode(int i, int j) => true;
		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => true;
	}
}
