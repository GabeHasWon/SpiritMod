using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class PlatinumClub : ClubItem
    {
		internal override int ChargeTime => 55;
		internal override Vector2 Size => new(84, 84);
		internal override float Acceleration => 18f;
		internal override int MinDamage => 31;
		internal override int MaxDamage => 110;
		internal override float MinKnockback => 6f;
		internal override float MaxKnockback => 9f;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Platinum Scepter");

		public override void Defaults()
        {
            Item.width = 58;
            Item.height = 58;
            Item.crit = 6;
            Item.value = Item.sellPrice(0, 0, 22, 0);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<Projectiles.Clubs.PlatinumClubProj>();
        }

		public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.PlatinumBar, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}