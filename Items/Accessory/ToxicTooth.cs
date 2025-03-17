using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory
{
    public class ToxicTooth : SpiritAccessory
    {
        public override string SetDisplayName => "Toxic Tooth";
        public override string SetTooltip => "Increases armor penetration by 3 and melee speed by 5%\nMelee attacks occasionally strike enemies twice\nMelee hits on foes may cause them to emit a cloud of poisonous gas";
        public override int ArmorPenetration => 3;
        public override List<SpiritPlayerEffect> AccessoryEffects => new List<SpiritPlayerEffect>() {
            new CleftHornEffect()
        };

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
            Item.defense = 1;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetSpiritPlayer().wheezeScale = true;
			player.GetAttackSpeed(DamageClass.Melee) += .05f;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CleftHorn>(), 1);
			recipe.AddIngredient(ModContent.ItemType<WheezerScale>(), 1);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
	}
}
