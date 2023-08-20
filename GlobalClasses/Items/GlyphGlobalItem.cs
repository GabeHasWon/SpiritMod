using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Glyphs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpiritMod.GlobalClasses.Items
{
	public class GlyphGlobalItem : GlobalItem
	{
		public override bool InstancePerEntity => true;

		protected override bool CloneNewInstances => true;

		public bool randomGlyph = false;
		public GlyphType Glyph { get; private set; }

		public void SetGlyph(Item item, GlyphType glyph)
		{
			if (Glyph == glyph)
				return;
			if (glyph == GlyphType.None)
				randomGlyph = false;

			item.prefix = 0;
			item.rare = item.OriginalRarity;

			Glyph = glyph;
			AdjustStats(item);
		}

		private static bool CanBeAppliedTo(Player player, Item item)
			=> item.maxStack == 1 && player.HeldItem.ModItem is GlyphBase glyph && glyph.CanApply(item) && item.GetGlobalItem<GlyphGlobalItem>().Glyph != glyph.Glyph;

		public override bool CanRightClick(Item item) => CanBeAppliedTo(Main.LocalPlayer, item) || base.CanRightClick(item);

		public override void RightClick(Item item, Player player)
		{
			if (CanBeAppliedTo(player, item))
			{
				(player.HeldItem.ModItem as GlyphBase).OnApply(item, player);

				Main.mouseItem.stack--;
				item.stack++; //Prevent the right-clicked item from turning to air
			}
		}

		public override bool CanReforge(Item item) => Glyph == GlyphType.None;

		private void AdjustStats(Item item)
		{
			//This may not respect modded item stat data, but prefix bonuses can't be accurately removed otherwise
			Item norm = new Item();
			norm.netDefaults(item.netID);

			float damage = 0;
			int crit = 0;
			float mana = 0;
			float knockBack = 0;
			float shootSpeed = 0;
			float useTime = 0;
			float size = 0;
			int tileBoost = 0;

			switch (Glyph)
			{
				case GlyphType.Sanguine:
					damage -= .10f;
					break;
				case GlyphType.Blaze:
					crit += 10;
					damage += .25f;
					break;
			}

			item.damage = norm.damage + (int)Math.Round(norm.damage * damage);
			item.useAnimation = norm.useAnimation + (int)Math.Round(norm.useAnimation * useTime);
			item.useTime = norm.useTime + (int)Math.Round(norm.useTime * useTime);
			item.reuseDelay = norm.reuseDelay + (int)Math.Round(norm.reuseDelay * useTime);
			item.mana = norm.mana + (int)Math.Round(norm.mana * mana);
			item.knockBack = norm.knockBack + knockBack;
			item.scale = norm.scale + size;

			if (item.shoot >= ProjectileID.None && !item.IsMelee()) //Don't change velocity for spears
				item.shootSpeed = norm.shootSpeed + shootSpeed;

			item.crit = norm.crit + crit;
			item.tileBoost = norm.tileBoost + tileBoost;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			bool insertStats = false;

			if (Glyph != GlyphType.None)
			{
				insertStats = true;

				var lookup = GlyphBase.FromType(Glyph);
				if (lookup.Effect != null && lookup.Addendum != null)
				{
					var nameLine = tooltips.Where(x => x.Name == "ItemName").FirstOrDefault();
					if (nameLine is TooltipLine nameTip)
						nameTip.Text = nameTip.Text.Insert(0, $"{lookup.Effect} "); //Insert the glyph's effect as a prefix

					List<TooltipLine> tips = new()
					{
						new TooltipLine(Mod, "GlyphAddendum", lookup.Effect)
						{
							OverrideColor = randomGlyph ? GlyphBase.FromType(GlyphType.Chaos).Color : lookup.Color
						},
						new TooltipLine(Mod, "GlyphAddendum", lookup.Addendum)
					};
					tooltips.AddRange(tips);
				}
			}

			if (insertStats)
				InsertStatInfo(item, tooltips);
		}

		private void InsertStatInfo(Item item, List<TooltipLine> tooltips)
		{
			int index = 0;
			for (int i = tooltips.Count - 1; i >= 0; i--)
			{
				TooltipLine curr = tooltips[i];
				if (curr.Mod != "Terraria")
					continue;
				if (curr.Name == "Price" ||
					curr.Name == "Expert" ||
					curr.Name == "SetBonus")
					continue;

				index = i + 1;
				break;
			}

			Item compare = new Item();
			compare.netDefaults(item.netID);
			string line;
			TooltipLine tip;

			if (compare.damage != item.damage)
			{
				double damage = (item.damage - compare.damage);
				damage = damage / ((float)compare.damage) * 100.0;
				damage = Math.Round(damage);
				if (damage > 0.0)
					line = "+" + damage + Language.GetTextValue("LegacyTooltip.39");
				else
					line = damage.ToString() + Language.GetTextValue("LegacyTooltip.39");

				tip = new TooltipLine(Mod, "PrefixDamage", line);
				if (damage < 0.0)
					tip.IsModifierBad = true;

				tip.IsModifier = true;
				tooltips.Insert(index++, tip);
			}

			if (compare.useAnimation != item.useAnimation)
			{
				double speed = (item.useAnimation - compare.useAnimation);
				speed = speed / ((float)compare.useAnimation) * 100.0;
				speed = Math.Round(speed);
				speed *= -1.0;
				if (speed > 0.0)
					line = "+" + speed + Language.GetTextValue("LegacyTooltip.40");
				else
					line = speed.ToString() + Language.GetTextValue("LegacyTooltip.40");

				tip = new TooltipLine(Mod, "PrefixSpeed", line);
				if (speed < 0.0)
					tip.IsModifierBad = true;

				tip.IsModifier = true;
				tooltips.Insert(index++, tip);
			}

			if (compare.crit != item.crit)
			{
				double crit = (item.crit - compare.crit);
				if (crit > 0.0)
					line = "+" + crit + Language.GetTextValue("LegacyTooltip.41");
				else
					line = crit.ToString() + Language.GetTextValue("LegacyTooltip.41");

				tip = new TooltipLine(Mod, "PrefixCritChance", line);
				if (crit < 0.0)
					tip.IsModifierBad = true;

				tip.IsModifier = true;
				tooltips.Insert(index++, tip);
			}

			if (compare.mana != item.mana)
			{
				double mana = (item.mana - compare.mana);
				mana = mana / ((float)compare.mana) * 100.0;
				mana = Math.Round(mana);
				if (mana > 0.0)
					line = "+" + mana + Language.GetTextValue("LegacyTooltip.42");
				else
					line = mana.ToString() + Language.GetTextValue("LegacyTooltip.42");

				tip = new TooltipLine(Mod, "PrefixUseMana", line);
				if (mana > 0.0)
					tip.IsModifierBad = true;

				tip.IsModifier = true;
				tooltips.Insert(index++, tip);
			}

			if (compare.scale != item.scale)
			{
				double scale = item.scale - compare.scale;
				scale = scale / compare.scale * 100.0;
				scale = Math.Round(scale);
				if (scale > 0.0)
					line = "+" + scale + Language.GetTextValue("LegacyTooltip.43");
				else
					line = scale.ToString() + Language.GetTextValue("LegacyTooltip.43");

				tip = new TooltipLine(Mod, "PrefixSize", line);
				if (scale < 0.0)
					tip.IsModifierBad = true;

				tip.IsModifier = true;
				tooltips.Insert(index++, tip);
			}

			if (compare.shootSpeed != item.shootSpeed)
			{
				double velocity = item.shootSpeed - compare.shootSpeed;
				velocity = velocity / compare.shootSpeed * 100.0;
				velocity = Math.Round(velocity);
				if (velocity > 0.0)
					line = "+" + velocity + Language.GetTextValue("LegacyTooltip.44");
				else
					line = velocity.ToString() + Language.GetTextValue("LegacyTooltip.44");

				tip = new TooltipLine(Mod, "PrefixShootSpeed", line);
				if (velocity < 0.0)
					tip.IsModifierBad = true;

				tip.IsModifier = true;
				tooltips.Insert(index++, tip);
			}

			if (compare.knockBack != item.knockBack)
			{
				double knockback = (item.knockBack - compare.knockBack);
				knockback = knockback / compare.knockBack * 100.0;
				knockback = Math.Round(knockback);
				if (knockback > 0.0)
					line = "+" + knockback + Language.GetTextValue("LegacyTooltip.45");
				else
					line = knockback.ToString() + Language.GetTextValue("LegacyTooltip.45");

				tip = new TooltipLine(Mod, "PrefixKnockback", line);
				if (knockback < 0.0)
					tip.IsModifierBad = true;

				tip.IsModifier = true;
				tooltips.Insert(index++, tip);
			}
		}

		private static readonly Vector2 SlotDimensions = new(52, 52);
		public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if (Glyph == GlyphType.None)
				return;

			static float Rescale(Vector2 frame)
			{
				float scalar = 1f;
				if (frame.X > 32 || frame.Y > 32)
					scalar = 32f / Math.Max(frame.X, frame.Y);

				return scalar;
			}
			float slotScale = Rescale(frame.Size()) * Main.inventoryScale;
			Vector2 slotOrigin = position + frame.Size() * (.5f * slotScale) - (SlotDimensions * (.5f * Main.inventoryScale));

			Texture2D texture = GlyphBase.FromType(Glyph).Overlay;
			if (texture != null)
			{
				Vector2 offset = (SlotDimensions - texture.Size() - new Vector2(4f)) * Main.inventoryScale;
				spriteBatch.Draw(texture, slotOrigin + offset, null, drawColor, 0f, Vector2.Zero, Main.inventoryScale, SpriteEffects.None, 0f);
			}
		}

		public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			if (Glyph != GlyphType.None && GlyphBase.FromType(Glyph).Overlay is Texture2D texture)
			{
				Vector2 position = item.position - Main.screenPosition;
				position.X += item.width >> 1;
				position.Y += item.height - (TextureAssets.Item[item.type].Value.Height >> 1);

				(alphaColor.R, alphaColor.G, alphaColor.B) = ((byte)Math.Min(alphaColor.R + 25, 255), (byte)Math.Min(alphaColor.G + 25, 255), (byte)Math.Min(alphaColor.B + 25, 255));
				Vector2 origin = new Vector2(texture.Width >> 1, texture.Height >> 1);
				spriteBatch.Draw(texture, position, null, alphaColor, rotation, origin, scale, SpriteEffects.None, 0f);
			}
		}

		public override void SaveData(Item item, TagCompound tag)
		{
			tag.Add("randomGlyph", randomGlyph);
			tag.Add("glyph", (int)Glyph);
		}

		public override void LoadData(Item item, TagCompound data)
		{
			randomGlyph = data.GetBool("randomGlyph");

			GlyphType glyph = (GlyphType)data.GetInt("glyph");
			if (glyph > GlyphType.None && glyph < GlyphType.Count)
				Glyph = glyph;
			else
				Glyph = GlyphType.None;

			AdjustStats(item);
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			writer.Write(randomGlyph);
			writer.Write((byte)Glyph);
		}

		public override void NetReceive(Item item, BinaryReader reader)
		{
			randomGlyph = reader.ReadBoolean();

			GlyphType glyph = (GlyphType)reader.ReadByte();
			if (glyph > GlyphType.None && glyph < GlyphType.Count)
				Glyph = glyph;
			else
				Glyph = GlyphType.None;

			AdjustStats(item);
		}
	}
}