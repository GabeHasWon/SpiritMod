using SpiritMod.Items.Consumable;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace SpiritMod.Tiles.Furniture.Critters
{
	public static class CageData
	{
		public const AnchorType Anchor = AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Platform | AnchorType.Table;

		public static void SetData(ModTile tile)
		{
			Main.tileLighted[tile.Type] = true;
			Main.tileFrameImportant[tile.Type] = true;
			Main.tileNoAttach[tile.Type] = true;
			Main.tileLavaDeath[tile.Type] = true;
			Main.tileSolidTop[tile.Type] = true;
			Main.tileTable[tile.Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(TileObjectData.newTile.AnchorBottom.type, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(tile.Type);
		}
	}

	[Sacrifice(1)]
	public class BlossomCage : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 0, 30, 0);
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Blossom_Tile>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BlossmoonItem>(), 1);
			recipe.AddIngredient(ItemID.Terrarium, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

	public class Blossom_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			CageData.SetData(this);

			AnimationFrameHeight = 36;
			DustType = DustID.Glass;
			
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (.33f, .025f, 1.15f);

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter >= 10) //replace 10 with duration of frame in ticks
			{
				frameCounter = 0;
				frame++;
				frame %= 5;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}

	[Sacrifice(1)]
	public class BriarmothCage : ModItem
    {
		public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 0, 30, 0);
            Item.maxStack = Item.CommonMaxStack;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Briarmoth_Tile>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BriarmothItem>(), 1);
            recipe.AddIngredient(ItemID.Bottle, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }

    public class Briarmoth_Tile : ModTile
    {
        public override void SetStaticDefaults()
        {
			CageData.SetData(this);

			AnimationFrameHeight = 36;
			DustType = DustID.Glass;
            
			LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(200, 200, 200), name);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 10) //replace 10 with duration of frame in ticks
            {
                frameCounter = 0;
                frame++;
                frame %= 16;
            }
        }

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}

	[Sacrifice(1)]
	public class CleftCage : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 0, 30, 0);
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Cleft_Tile>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CleftItem>(), 1);
			recipe.AddIngredient(ItemID.Terrarium, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

	public class Cleft_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			CageData.SetData(this);

			DustType = DustID.Glass;
			AnimationFrameHeight = 36;

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter >= 13) //replace 10 with duration of frame in ticks
			{
				frameCounter = 0;
				frame++;
				frame %= 7;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}

	[Sacrifice(1)]
	public class PaleCrinoidCage : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 0, 30, 0);
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<PaleCrinoid_Tile>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CrinoidItem>(), 1);
			recipe.AddIngredient(ItemID.Terrarium, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

	public class PaleCrinoid_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileTable[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.Origin = new Point16(3, 2);
			TileObjectData.addTile(Type);

			DustType = DustID.Glass;
			AnimationFrameHeight = 54;
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter >= 8) //replace 10 with duration of frame in ticks
			{
				frameCounter = 0;
				frame++;
				frame %= 6;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}

	[Sacrifice(1)]
	public class RedCrinoidCage : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 0, 30, 0);
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<RedCrinoid_Tile>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CrinoidItem>(), 1);
			recipe.AddIngredient(ItemID.Terrarium, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

	public class RedCrinoid_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileTable[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(CageData.Anchor, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(3, 2);
			TileObjectData.addTile(Type);
			
			DustType = DustID.Glass;
			AnimationFrameHeight = 54;
			
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter >= 8) //replace 10 with duration of frame in ticks
			{
				frameCounter = 0;
				frame++;
				frame %= 6;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}

	[Sacrifice(1)]
	public class BrownCrinoidCage : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 0, 30, 0);
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<BrownCrinoid_Tile>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CrinoidItem>(), 1);
			recipe.AddIngredient(ItemID.Terrarium, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

	public class BrownCrinoid_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileTable[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(CageData.Anchor, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(3, 2);
			TileObjectData.addTile(Type);

			DustType = DustID.Glass;
			AnimationFrameHeight = 54;
			
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter >= 8) //replace 10 with duration of frame in ticks
			{
				frameCounter = 0;
				frame++;
				frame %= 6;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}

	[Sacrifice(1)]
	public class LuvdiscBowl : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 32;
			Item.value = Item.buyPrice(0, 0, 30, 0); ;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Luvdisc_Tile>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<LuvdiscItem>(), 1);
			recipe.AddIngredient(ItemID.BottledWater, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

	public class Luvdisc_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.addTile(Type);
			
			DustType = DustID.Glass;
			AnimationFrameHeight = 36;
			
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter >= 10) //replace 10 with duration of frame in ticks
			{
				frameCounter = 0;
				frame++;
				frame %= 6;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}

	[Sacrifice(1)]
	public class GulperBowl : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 32;
			Item.value = Item.buyPrice(0, 0, 30, 0);
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = Mod.Find<ModTile>("Gulper_Tile").Type;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<GulperItem>(), 1);
			recipe.AddIngredient(ItemID.BottledWater, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

	public class Gulper_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.addTile(Type);
			
			DustType = DustID.Glass;
			AnimationFrameHeight = 36;
			
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter >= 10) //replace 10 with duration of frame in ticks
			{
				frameCounter = 0;
				frame++;
				frame %= 6;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}

	[Sacrifice(1)]
	public class TubewormBowl : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 32;
			Item.value = Item.buyPrice(0, 0, 30, 0); ;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = Mod.Find<ModTile>("Tubeworm_Tile").Type;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<TubewormItem>(), 1);
			recipe.AddIngredient(ItemID.BottledWater, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

	public class Tubeworm_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.addTile(Type);

			DustType = DustID.Glass;
			AnimationFrameHeight = 36;
			
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter >= 14) //replace 10 with duration of frame in ticks
			{
				frameCounter = 0;
				frame++;
				frame %= 6;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}

	[Sacrifice(1)]
	public class VibeshroomJarItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 0, 30, 0);
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<VibeshroomJar_Tile>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<VibeshroomItem>(), 1);
			recipe.AddIngredient(ItemID.Terrarium, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

	public class VibeshroomJar_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.addTile(Type);
			
			DustType = DustID.Glass;
			AnimationFrameHeight = 36;
			
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter >= 15) //replace 10 with duration of frame in ticks
			{
				frameCounter = 0;
				frame++;
				frame %= 14;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}