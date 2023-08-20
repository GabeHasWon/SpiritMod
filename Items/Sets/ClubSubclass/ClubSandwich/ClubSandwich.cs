using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Consumable.Food;
using SpiritMod.Projectiles.Clubs;
using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Sets.ClubSubclass.ClubSandwich
{
    public class ClubSandwich : ClubItem
    {
		internal override int ChargeTime => 52;
		internal override float Acceleration => 17f;
		internal override int MinDamage => 43;
		internal override int MaxDamage => 95;
		internal override float MinKnockback => 5f;
		internal override float MaxKnockback => 9f;

		public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Club Sandwich");
            // Tooltip.SetDefault("Fully charged slams release sandwich bits\nCollect bits to boost stats");
        }

        public override void Defaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.crit = 4;
            Item.value = Item.sellPrice(0, 1, 42, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<ClubSandwichProj>();
        }

		public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Baguette>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CaesarSalad>(), 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}