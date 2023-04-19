using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class FloranBludgeon : ClubItem
    {
		internal override int MinDamage => 32;
		internal override int MaxDamage => 80;
		internal override float MinKnockback => 4f;
		internal override float MaxKnockback => 8f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Floran Bludgeon");
            Tooltip.SetDefault("Releases a shockwave along the ground");
        }

        public override void Defaults()
        {
            Item.width = 58;
            Item.height = 58;
            Item.crit = 6;
			Item.value = Item.sellPrice(0, 0, 22, 0);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<Projectiles.Clubs.FloranBludgeonProj>();
        }

		public override void AddRecipes()
        {
			var recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<FloranSet.FloranBar>(), 15);
            recipe.AddIngredient(ModContent.ItemType<BriarDrops.EnchantedLeaf>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}