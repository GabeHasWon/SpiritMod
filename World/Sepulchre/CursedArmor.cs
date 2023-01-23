using Microsoft.Xna.Framework;
using SpiritMod.NPCs.Enchanted_Armor;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.World.Sepulchre
{
	public class CursedArmor : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(0, 3);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.addTile(Type);

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Cursed Armor");
			AddMapEntry(Color.DarkSlateGray, name);

			MineResist = 0.2f;
		}

		public override bool IsTileDangerous(int i, int j, Player player) => true;

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath6 with { PitchVariance = 0.2f }, new Vector2(i * 16, j * 16));
			NPC npc = Main.npc[NPC.NewNPC(new EntitySource_TileBreak(i, j), (i + 1) * 16, (j + 4) * 16, ModContent.NPCType<Enchanted_Armor>())];
			npc.velocity = Vector2.Zero;
			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
		}
	}

	public class CursedArmorItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Draugr Statue");
			Tooltip.SetDefault("'Take caution...'");
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 42;
			Item.maxStack = 999;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<CursedArmor>();
		}

		public override void AddRecipes()
		{
			var recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<Items.Placeable.Tiles.SepulchreBrickTwoItem>(), 20);
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.Register();
		}
	}
}
