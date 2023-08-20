using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Projectiles.Clubs;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class GoldClub : ClubItem
    {
		internal override int ChargeTime => 60;
		internal override float Acceleration => 18f;
		internal override int MinDamage => 30;
		internal override int MaxDamage => 105;
		internal override float MinKnockback => 6f;
		internal override float MaxKnockback => 10f;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Golden Greathammer");

		public override void Defaults()
        {
            Item.width = 58;
            Item.height = 58;
            Item.crit = 6;
			Item.value = Item.sellPrice(0, 0, 22, 0);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<GoldClubProj>();
        }

		public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.GoldBar, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}