using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.SpecialSellItem;
using SpiritMod.Projectiles.Glyph;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Glyphs
{
	public class VoidGlyph : GlyphBase, ISpecialSellItem
	{
		public const int CollapseDuration = 300;

		public override GlyphType Glyph => GlyphType.Void;
		public override Color Color => new(233, 89, 255);

		public override void SetDefaults()
		{
			Item.width = Item.height = 28;
			Item.rare = ItemRarityID.LightPurple;
			Item.maxStack = Item.CommonMaxStack;
		}

		public static void VoidCollapse(Player owner, NPC target, Projectile proj, int damage)
		{
			int riftType = ModContent.ProjectileType<VoidRift>();
			if (!(proj is Projectile shot && shot.type == riftType))
			{
				if (owner.ownedProjectileCounts[riftType] > 0)
				{
					var onTarget = Main.projectile.Where(x => x.active && (x.type == riftType) && (x.owner == owner.whoAmI) && (x.ModProjectile is VoidRift voidRift) && (voidRift.TargetWhoAmI == target.whoAmI)).FirstOrDefault();
					if (onTarget != default)
					{
						onTarget.damage += damage * (++(onTarget.ModProjectile as VoidRift).Stacks + 1);
						onTarget.netUpdate = true;

						return;
					} //Update the damage of active rifts
				}
				Projectile.NewProjectile(owner.GetSource_OnHit(target), target.Center, Vector2.Zero, riftType, damage, 0, owner.whoAmI, target.whoAmI);
			}
		}

		public int SellAmount() => 2;
	}
}