using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Buffs.Glyph;
using SpiritMod.Projectiles;
using System.Linq;

namespace SpiritMod.Items.Glyphs
{
	public class SanguineGlyph : GlyphBase
	{
		public override GlyphType Glyph => GlyphType.Sanguine;
		public override Color Color => new(255, 79, 56);
		public override string Effect => "Leeching";
		public override string Addendum => "Dealing damage boosts life regeneration briefly";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sanguine Glyph");
			Tooltip.SetDefault($"{Addendum}\n-10% damage");
		}

		public override void SetDefaults()
		{
			Item.width = Item.height = 28;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.maxStack = 999;
		}

		public static void DrainEffect(Player owner, Entity target)
		{
			owner.AddBuff(ModContent.BuffType<SanguineRegen>(), (int)MathHelper.Clamp(owner.HeldItem.useTime * 2, 30, 120));

			int cystType = ModContent.ProjectileType<BloodyCyst>();
			if (target is NPC npc)
			{
				int numOnTarget = (owner.ownedProjectileCounts[cystType] > 0) ? Main.projectile.Where(x => x.active && (x.type == cystType) && (x.owner == owner.whoAmI) && (x.ModProjectile is BloodyCyst bloodyCyst) && (bloodyCyst.TargetWhoAmI == target.whoAmI)).Count() : 0;
				if (numOnTarget < 5)
				{
					Vector2 position = npc.position + new Vector2(npc.width * Main.rand.NextFloat(), npc.height * Main.rand.NextFloat());
					Projectile.NewProjectile(owner.GetSource_OnHit(target), position, Vector2.UnitX.RotatedByRandom(5f), ModContent.ProjectileType<BloodyCyst>(), 0, 0, Main.myPlayer, npc.whoAmI);
				}
			}
		}
	}
}