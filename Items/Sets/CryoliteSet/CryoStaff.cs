using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.CryoliteSet
{
	public class CryoStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cryo Staff");
			Tooltip.SetDefault("Casts a spread of icy magic bolts, which inflict 'Cryo Crush'\nCryo Crush deals increased damage to weakened enemies");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 32;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 9;
			Item.width = 46;
			Item.height = 46;
			Item.useTime = 27;
			Item.useAnimation = 27;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 4.5f;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 0, 70, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<CryoliteMage>();
			Item.shootSpeed = 8f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (Main.rand.NextBool(3))
			{
				Vector2 origVect = velocity;
				for (int X = 0; X < 3; X++)
				{
					Vector2 newVect = origVect.RotatedBy(-System.Math.PI / (Main.rand.Next(300, 500) / 10));
					if (Main.rand.NextBool(2))
						newVect = origVect.RotatedBy(System.Math.PI / (Main.rand.Next(300, 500) / 10));

					Projectile proj = Main.projectile[Projectile.NewProjectile(source, position.X, position.Y, newVect.X, newVect.Y, type, damage, knockback, player.whoAmI)];
					proj.friendly = true;
					proj.hostile = false;
					proj.netUpdate = true;
				}
			}
			return true;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.06f, .16f, .22f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CryoliteBar>(), 15);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}