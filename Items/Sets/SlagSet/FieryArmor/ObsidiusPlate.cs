using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SlagSet.FieryArmor
{
	[AutoloadEquip(EquipType.Body)]
	public class ObsidiusPlate : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slag Tyrant's Platemail");
			Tooltip.SetDefault("4% increased minion damage\nIncreases your max number of minions");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 20;
			Item.value = Item.sellPrice(0, 0, 35, 0);
			Item.rare = ItemRarityID.Orange;
			Item.defense = 8;
		}

		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) => glowMaskColor = new Color(100, 100, 100, 100);

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.4f, .12f, .028f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_ItemGlow").Value, rotation, scale);
		}

		public override void UpdateEquip(Player player)
		{
			player.maxMinions++;
			player.GetDamage(DamageClass.Summon) += .04f;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<CarvedRock>(), 17);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
