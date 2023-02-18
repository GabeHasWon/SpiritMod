using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BloodcourtSet
{
	public class FangTome : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tome of the Thousand Fangs");
			Tooltip.SetDefault("Summon a set of gnashing teeth\nInflicts 'Surging Anguish'");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 30;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 12;
			Item.width = 28;
			Item.height = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.noMelee = true;
			Item.knockBack = 0;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<BloodFangs>();
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.shootSpeed = 0f;
			Item.crit = 6;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, .42f, .02f, .13f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) 
			=> position = Main.MouseWorld;

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<DreamstrideEssence>(), 8);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}