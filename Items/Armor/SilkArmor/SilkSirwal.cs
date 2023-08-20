using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.SilkArmor
{
	[AutoloadEquip(EquipType.Legs)]
	public class SilkSirwal : ModItem
	{
		public override void Load()
		{
			if (Main.netMode == NetmodeID.Server)
				return;
			EquipLoader.AddEquipTexture(Mod, "SpiritMod/Items/Armor/SilkArmor/SilkSirwalFemale_Legs", EquipType.Legs, null, "SilkSirwalFemale_Legs");
		}

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sirwal");
			Main.RegisterItemAnimation(Type, new DrawAnimationVertical(2, 2) { NotActuallyAnimating = true });
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 18;
			Item.value = 10000;
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			frame.Y = (frame.Height + 2) * (Main.LocalPlayer.Male ? 0 : 1);
			spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, Item.GetAlpha(drawColor), 0f, origin, scale, SpriteEffects.None, 0f);
			return false;
		}

		public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
		{
			if (!male)
				equipSlot = EquipLoader.GetEquipSlot(Mod, "SilkSirwalFemale_Legs", EquipType.Legs);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Silk, 10);
			recipe.AddRecipeGroup("SpiritMod:GoldBars", 2);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}