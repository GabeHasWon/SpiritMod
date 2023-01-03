using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AvianDrops.ApostleArmor
{
	[AutoloadEquip(EquipType.Body)]
	public class TalonGarb : ModItem
	{
		public override void Load()
		{
			if (Main.netMode == NetmodeID.Server)
				return;
			EquipLoader.AddEquipTexture(Mod, "SpiritMod/Items/BossLoot/AvianDrops/ApostleArmor/TalonGarb_Legs", EquipType.Legs, null, "TalonGarb_Legs");
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Apostle's Garb");
			Tooltip.SetDefault("Increases magic and ranged damage by 7%\nIncreases movement speed by 10%");
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 22;
			Item.value = 10000;
			Item.rare = ItemRarityID.Orange;
			Item.defense = 5;
		}

		public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
		{
			robes = true;
			equipSlot = EquipLoader.GetEquipSlot(Mod, "TalonGarb_Legs", EquipType.Legs);
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(DamageClass.Magic) += .07f;
			player.GetDamage(DamageClass.Ranged) += .07f;
			player.moveSpeed += 0.10f;
		}
	}
}
