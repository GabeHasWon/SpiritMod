using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Summon.Zones;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Buffs.Zones;
using Terraria.DataStructures;

namespace SpiritMod.Items.Sets.ArcaneZoneSubclass
{
	public class SlowCodex : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Arcane Codex: Slow Zone");
			// Tooltip.SetDefault("Summons a slow zone at the cursor position\nSlow zones reduce enemy movement speed\nZones count as sentries");
            SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Sets/ArcaneZoneSubclass/SlowCodex_Glow");
        }

        public override void SetDefaults()
		{
			Item.damage = 0;
			Item.DamageType = DamageClass.Summon;
			Item.mana = 10;
			Item.width = 54;
			Item.height = 50;
			Item.useTime = 31;
			Item.useAnimation = 31;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.DD2_EtherianPortalSpawnEnemy;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<SlowZone>();
			Item.shootSpeed = 0f;
			Item.buffType = ModContent.BuffType<CryoZoneTimer>();
			Item.buffTime = Projectile.SentryLifeTime;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			position = Main.MouseWorld;
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            player.UpdateMaxTurrets();
			return false;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.1f, .2f, .22f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}
		public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<EmptyCodex>(), 1);
            recipe.AddIngredient(ModContent.ItemType<Items.Sets.CryoliteSet.CryoliteBar>(), 8);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}