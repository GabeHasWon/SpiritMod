using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SummonsMisc.RodofDunes
{
	public class RodofDunes : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 28;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Blue;
			Item.mana = 10;
			Item.damage = 6;
			Item.knockBack = 2;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 15;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<SandWarriorMinion>();
			Item.UseSound = SoundID.Item44;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			position += position.DirectionTo(Main.MouseWorld) * MathHelper.Min(position.Distance(Main.MouseWorld), 100);
			velocity = Main.rand.NextVector2Circular(3, 3);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.SandBlock, 50);
			recipe.AddRecipeGroup("SpiritMod:GoldBars", 5);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}