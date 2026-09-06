using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Candy;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpiritMod.Items.Halloween
{
	public class Candy : CandyBase
	{
		protected override bool CloneNewInstances => true;

		public int Variant { get; internal set; }

		public const int VariantCount = 31;

		internal override Point Size => new(34, 16);

		public override void Defaults()
		{
			Item.width = Size.X;
			Item.height = Size.Y;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = 1;
			Item.buffType = ModContent.BuffType<CandyBuff>();
			Item.buffTime = 14400;

			Variant = Main.rand.Next(VariantCount);
		}

		internal static string GetCandyName(int variant) => Language.GetTextValue($"Mods.SpiritMod.Items.Candy.Names.{variant}");

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			base.ModifyTooltips(tooltips);

			int index = tooltips.FindIndex(tooltip => tooltip.Name.Equals("ItemName"));
			if (index >= 0) {

				TooltipLine line = new TooltipLine(Mod, "ItemNameSub", GetCandyName(Variant));
				tooltips.Insert(index + 1, line);
			}
		}

		public override void SaveData(TagCompound tag) => tag.Add("Variant", Variant);

		public override void LoadData(TagCompound tag) => Variant = tag.GetInt("Variant");

		public override void NetSend(BinaryWriter writer) => writer.Write((byte)Variant);

		public override void NetReceive(BinaryReader reader) => Variant = reader.ReadByte();
	}
}
