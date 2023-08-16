using Microsoft.Xna.Framework;
using SpiritMod.GlobalClasses.Items;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace SpiritMod.Items.Glyphs
{
	public class ChaosGlyph : GlyphBase
	{
		public override GlyphType Glyph => GlyphType.Chaos;
		public override Color Color => Main.DiscoColor;
		public override string Effect => "Unstable";
		public override string Addendum => "Your weapon periodically switches between glyph effects";

		public override void SetDefaults()
		{
			Item.height = Item.width = 28;
			Item.rare = ItemRarityID.LightPurple;
			Item.maxStack = 999;
		}

		public override void OnApply(Item item, Player player)
		{
			item.GetGlobalItem<GlyphGlobalItem>().SetGlyph(item, Randomize());
			item.GetGlobalItem<GlyphGlobalItem>().randomGlyph = true;
			SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/GlyphAttach"), player.Center);
		}

		public static GlyphType Randomize(GlyphType current = GlyphType.None)
		{
			while (true)
			{
				GlyphType type = (GlyphType)Main.rand.Next((int)GlyphType.Count);
				GlyphType[] blacklist = new GlyphType[] { GlyphType.None, GlyphType.Chaos, current };

				if (!blacklist.Contains(type))
					return type;
			}
		}
	}
}