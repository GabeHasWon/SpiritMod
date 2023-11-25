using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class WoodenClub : ClubItem
    {
		internal override int ChargeTime => 50;
		internal override float Acceleration => 17f;
		internal override int MinDamage => 10;
		internal override int MaxDamage => 35;
		internal override float MinKnockback => 5f;
		internal override float MaxKnockback => 8f;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Wooden Club");

		public override void Defaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.crit = 4;
            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.White;
            Item.shoot = ModContent.ProjectileType<Projectiles.Clubs.WoodClubProj>();
        }

		public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 30);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}