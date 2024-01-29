using SpiritMod.NPCs.Boss.ReachBoss;
using SpiritMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	[Sacrifice(3)]
	public class ReachBossSummon : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
            Item.rare = ItemRarityID.Green;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 20;
            Item.noMelee = true;
            Item.consumable = true;
            Item.autoReuse = false;
            Item.UseSound = SoundID.Item43;
        }

		public override bool CanUseItem(Player player) => !NPC.AnyNPCs(ModContent.NPCType<ReachBoss>()) && player.ZoneBriar() && !player.ZoneOverworldHeight;

		public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ReachBoss>());
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(null, "BismiteCrystal", 2);
            recipe.AddIngredient(null, "EnchantedLeaf", 2);
            recipe.AddRecipeGroup("SpiritMod:PHMEvilMaterial", 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}