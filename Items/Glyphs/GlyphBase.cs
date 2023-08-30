using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.GlobalClasses.Items;
using SpiritMod.Mechanics.SpecialSellItem;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Glyphs
{
	[Sacrifice(5)]
	public abstract class GlyphBase : ModItem, ISpecialSellItem
	{
		private static GlyphBase[] _lookup;
		public static GlyphBase FromType(GlyphType type) => _lookup[(byte)type];

		public abstract GlyphType Glyph { get; }
		public virtual Texture2D Overlay => ModContent.Request<Texture2D>(Texture + "_Icon").Value;
		public virtual Color Color => Color.White;
		public virtual string ItemType => "weapon";
		public virtual string Effect => Language.GetTextValue($"Mods.SpiritMod.Items.{Name}.Effect");
		public virtual string Addendum => Language.GetTextValue($"Mods.SpiritMod.Items.{Name}.Addendum");

		public virtual bool CanApply(Item item) => item.IsWeapon() && (item.GetGlobalItem<GlyphGlobalItem>().Glyph != Glyph || item.GetGlobalItem<GlyphGlobalItem>().randomGlyph);

		public virtual void OnApply(Item item, Player player)
		{
			item.GetGlobalItem<GlyphGlobalItem>().SetGlyph(item, Glyph);
			SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/GlyphAttach"), player.Center);
		}

		public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ChaosGlyph>();

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			int index = tooltips.FindIndex(x => x.Name == "Tooltip0");
			if (index < 0)
				return;

			Player player = Main.LocalPlayer;
			TooltipLine line;

			Color color = Color;
			color *= Main.mouseTextColor / 255f;
			line = new TooltipLine(Mod, "GlyphTooltip",
				"The enchanted " + ItemType + " will gain: [c/" +
				string.Format("{0:X2}{1:X2}{2:X2}:", color.R, color.G, color.B) + Effect + "]")
			{
				OverrideColor = new Color(120, 190, 120)
			};
			tooltips.Insert(index, line);

			if (Item.shopCustomPrice.HasValue)
				line = new TooltipLine(Mod, "GlyphHint", "Can only be applied to " + ItemType + "s");
			else
				line = new TooltipLine(Mod, "GlyphHint", "Hold this glyph and right-click the " + ItemType + " you want to enchant");

			line.OverrideColor = new Color(120, 190, 120);
			tooltips.Insert(index, line);
		}

		internal static void InitializeGlyphLookup()
		{
			_lookup = new GlyphBase[(byte)GlyphType.Count];

			Type glyphBase = typeof(GlyphBase);
			foreach (Type type in SpiritMod.Instance.Code.GetTypes())
			{
				if (type.IsAbstract)
					continue;
				if (!type.IsSubclassOf(glyphBase))
					continue;

				Item item = new Item();
				item.SetDefaults(SpiritMod.Instance.Find<ModItem>(type.Name).Type, true);
				GlyphBase glyph = (GlyphBase)item.ModItem;
				_lookup[(byte)glyph.Glyph] = glyph;
			}

			GlyphBase zero = _lookup[0];
			for (int i = 1; i < _lookup.Length; i++) {
				if (_lookup[i] == null)
					_lookup[i] = zero;
			}
		}

		internal static void UninitGlyphLookup() => _lookup = null;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
			=> GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public int CurrencyID() => SpiritMod.GlyphCurrencyID;

		public int SellType() => ModContent.ItemType<Glyph>();

		public string SellName() => "glyph";
	}
}