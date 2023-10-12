using SpiritMod.Projectiles.Returning;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class MadHat : ModItem
	{
		public override void SetStaticDefaults() => ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;

		public override void SetDefaults()
		{
			Item.damage = 49;
			Item.DamageType = DamageClass.Magic;
			Item.width = 18;
			Item.mana = 7;
			Item.height = 28;
			Item.useTime = 19;
			Item.useAnimation = 19;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true; //so the item's animation doesn't do damage
			Item.knockBack = 5;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<MadHatProj>();
			Item.shootSpeed = 10f;
			Item.noUseGraphic = true;
			Item.defense = 6;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetCritChance(DamageClass.Magic) += 8;
			player.GetDamage(DamageClass.Magic) += 0.1f;
		}
	}
}
