using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Linq;

namespace SpiritMod.Items.Sets.BismiteSet
{
	public class BismiteSummonStaff : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.QueenSpiderStaff);
			Item.damage = 12;
			Item.mana = 10;
			Item.width = 50;
			Item.height = 50;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Blue;
			Item.knockBack = 2.5f;
			Item.UseSound = SoundID.Item20;
			Item.shoot = ModContent.ProjectileType<BismiteSentrySummon>();
			Item.shootSpeed = 0f;
		}

		public override bool AltFunctionUse(Player player)
		{
			var sentries = Main.projectile.Where(x => x.active && x.type == Item.shoot && x.owner == player.whoAmI && (x.ModProjectile as BismiteSentrySummon).Cooldown <= 0);
			return sentries.Any();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
            if (player.altFunctionUse != 2)
            {
                if (Vector2.Distance(Main.MouseWorld, position) < 600f)
                {
                    Projectile.NewProjectile(source, Main.MouseWorld, velocity, type, damage, knockback, player.whoAmI);
                    player.UpdateMaxTurrets();
                }
            }
            else
            {
				var sentries = Main.projectile.Where(x => x.active && x.type == Item.shoot && x.owner == player.whoAmI);
				foreach (Projectile sentry in sentries)
					(sentry.ModProjectile as BismiteSentrySummon).SpecialAttack();
            }
            return false;
		}

        public override void AddRecipes()  
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BismiteCrystal>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
