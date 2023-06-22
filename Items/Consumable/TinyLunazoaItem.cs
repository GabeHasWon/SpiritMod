using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.NPCs.MoonjellyEvent;
using SpiritMod.Projectiles.Bullet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	[Sacrifice(5)]
	public class TinyLunazoaItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tiny Lunazoa");
			Tooltip.SetDefault("Increases in bait power at night, and even further during the Jelly Deluge");
		}

		public override void SetDefaults()
		{
			Item.DamageType = DamageClass.Ranged;
			Item.width = Item.height = 32;
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = 99;
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(0, 0, 1, 0);
			Item.useTime = Item.useAnimation = 20;
			Item.bait = 20;
			Item.ammo = Item.type;
			Item.shoot = ModContent.ProjectileType<LunazoaProj>();
			Item.noMelee = true;
			Item.consumable = true;
			Item.autoReuse = true;
			Item.makeNPC = ModContent.NPCType<TinyLunazoa>();
		}

		public override bool CanShoot(Player player) => false;

		public override void UpdateInventory(Player player) => Item.bait = 30 + (!Main.dayTime ? 15 : 0) + (MyWorld.jellySky ? 20 : 0);

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}
}
