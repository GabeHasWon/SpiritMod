using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace SpiritMod.Items.Armor.StarjinxSet
{
	[AutoloadEquip(EquipType.Head)]
    public class StarlightHat : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => false;

		public override void SetStaticDefaults()
		{
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");

			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
		}

		public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.value = Item.sellPrice(gold : 8);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 7;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(DamageClass.Magic) += 0.12f;
			player.GetCritChance(DamageClass.Magic) += 6;
		}

		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) => glowMaskColor = Color.White * 0.75f;
		
		public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == Mod.Find<ModItem>("StarlightMantle").Type && legs.type == Mod.Find<ModItem>("StarlightSandals").Type;

		public override void UpdateArmorSet(Player player)
        {
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Starjinx");
			player.manaCost *= 1.5f;
			MyPlayer modplayer = player.GetModPlayer<MyPlayer>();
			modplayer.StarjinxSet = true;

			if (Main.rand.NextBool(30) && Main.netMode != NetmodeID.Server)
		    {
				Gore.NewGore(player.GetSource_Accessory(Item, "Armor"), player.position + new Vector2(Main.rand.Next(player.width), Main.rand.Next(player.height)), 
					player.velocity / 2 + Main.rand.NextVector2Circular(1, 1), 
					Mod.Find<ModGore>("StarjinxGore").Type, 
					Main.rand.NextFloat(0.25f, 0.75f));
			}
		}

		public override void ArmorSetShadows(Player player) => player.armorEffectDrawOutlinesForbidden = true;

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(Mod, "Starjinx", 8);
			recipe.AddIngredient(ItemID.Silk, 4);
			recipe.AddIngredient(ItemID.FallenStar, 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
    }
}
