using SpiritMod.Buffs.Pet;
using SpiritMod.Projectiles.DonatorItems;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace SpiritMod.Items.Pets
{
	public class CagedMoonlight : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Caged Moonlight");
			Tooltip.SetDefault("Summons a faerie to protect you\n'Here resides a being comprised of pure starfire.'\n'Thine enemies shall be harassed by luminous lances.'");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Pets/CagedMoonlight_Glow");
        }

		public override void SetDefaults()
		{
			Item.damage = 0;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shoot = ModContent.ProjectileType<HarpyPet>();
			Item.width = 16;
			Item.height = 30;
			Item.useAnimation = 20;
			Item.useTime = 20;
			Item.rare = ItemRarityID.Orange;
			Item.noMelee = true;
			Item.value = Item.sellPrice(0, 3, 50, 0);
			Item.buffType = ModContent.BuffType<HarpyPetBuff>();
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
				player.AddBuff(Item.buffType, 3600, true);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.08f, .38f, .28f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override bool CanUseItem(Player player) => player.miscEquips[1].IsAir;

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe(1);
			modRecipe.AddIngredient(ModContent.ItemType<Consumable.Potion.MoonJelly>());
			modRecipe.AddIngredient(ItemID.CrystalShard, 10);
			modRecipe.AddIngredient(ItemID.SoulofLight, 5);
			modRecipe.AddTile(TileID.MythrilAnvil);
			modRecipe.Register();
		}
	}
}