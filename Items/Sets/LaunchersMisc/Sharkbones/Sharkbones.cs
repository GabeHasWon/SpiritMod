using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.LaunchersMisc.Liberty;
using SpiritMod.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.LaunchersMisc.Sharkbones
{
	public class Sharkbones : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sharkbones");
			Tooltip.SetDefault("Fires homing rockets");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 120;
			Item.DamageType = DamageClass.Ranged;
			Item.Size = new Vector2(94, 30);
			Item.useTime = Item.useAnimation = 40;
			Item.useTurn = false;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.knockBack = 4;
			Item.channel = true;
			Item.value = Item.buyPrice(0, 10, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SharkbonesRocket>();
			Item.shootSpeed = 1f;
			Item.useAmmo = AmmoID.Rocket;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, player.MountedCenter, velocity, ModContent.ProjectileType<SharkbonesProjHeld>(), damage, knockback, player.whoAmI);
			return false;
		}

		public override bool CanConsumeAmmo(Item item, Player player) => player.ownedProjectileCounts[Item.shoot] > 0;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
			=> GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<LibertyItem>());
			recipe.AddIngredient(ItemID.SharkFin, 5);
			recipe.AddIngredient(ItemID.IllegalGunParts);
			recipe.AddIngredient(ItemID.SoulofMight, 15);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}