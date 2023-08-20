using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.Leather
{
	[AutoloadEquip(EquipType.Shoes)]
	public class ExplorerTreads : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Explorer's Treads");
			/* Tooltip.SetDefault("50% chance to dodge traps and hazards\n" +
				"Grants a short speed boost after touching traps or hazards\n" +
				"'Makes exploring temples like a walk in the park'"); */
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 20;
			Item.value = Item.buyPrice(0, 6, 75, 0);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<MyPlayer>().explorerTreads = true;

		public static bool DoDodgeEffect(Player player, IEntitySource damageSource)
		{
			player.AddBuff(ModContent.BuffType<ExplorerSpeed>(), 180);

			if (Main.rand.NextBool(2)) //50% chance to dodge
			{
				for (int i = 0; i < 3; i++)
				{
					int goreType = i switch
					{
						1 => GoreID.Smoke2,
						2 => GoreID.Smoke3,
						_ => GoreID.Smoke1
					};
					Gore gore = Gore.NewGoreDirect(damageSource, player.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 30) - new Vector2(20), Vector2.Zero, goreType);
					gore.velocity = -Vector2.UnitY;
					gore.alpha = 180;
				}
				player.SetImmuneTimeForAllTypes(30);
				SoundEngine.PlaySound(SoundID.NPCDeath6 with { Volume = .6f }, player.Center);
				return true;
			}
			return false;
		}
	}
}
