using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Magic
{
	public class GoblinSorcererStaff : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sorcerer's Wand");
			Tooltip.SetDefault("Launches a shadowflame orb into the sky");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Weapon/Magic/GoblinSorcererStaff_Glow");
			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 48;
			Item.height = 50;
			Item.value = Item.buyPrice(0, 0, 60, 0);
			Item.rare = ItemRarityID.Green;
			Item.damage = 13;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.mana = 10;
			Item.knockBack = 3;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.UseSound = SoundID.Item21;
			Item.shoot = ModContent.ProjectileType<Projectiles.Magic.GobSorcererOrb>();
			Item.shootSpeed = 8f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
        {
            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, player.Center);
            Projectile.NewProjectile(source, position, Vector2.UnitY * -4f, type, damage, knockback, player.whoAmI, velocity.X, velocity.Y);
            
			return false;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) =>
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}
}
