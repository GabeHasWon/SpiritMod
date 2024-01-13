using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.DataStructures;
using SpiritMod.Projectiles.Clubs;
using Terraria.Localization;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public abstract class ClubItem : ModItem
    {
		internal abstract int ChargeTime { get; }
		internal abstract float Acceleration { get; }
		internal abstract int MinDamage { get; }
		internal abstract int MaxDamage { get; }
		internal abstract float MinKnockback { get; }
		internal abstract float MaxKnockback { get; }

		public virtual void Defaults() { }

		public sealed override void SetDefaults()
        {
			Item.damage = MinDamage;
			Item.knockBack = MinKnockback;
            Item.channel = true;
            Item.useTime = 320;
            Item.useAnimation = 320;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useTurn = true;
            Item.autoReuse = false;
            Item.shootSpeed = 1f;

			Defaults();
		}

		public override bool? CanAutoReuseItem(Player player) => false;
		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			StatModifier meleeDMG = player.GetTotalDamage(DamageClass.Melee);
			StatModifier meleeKB = player.GetTotalKnockback(DamageClass.Melee);

			Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);

			if (proj.ModProjectile is ClubProj clubProj)
			{
				float speedMult = player.GetTotalAttackSpeed(DamageClass.Melee);

				clubProj.SetStats(
					(int)(ChargeTime * MathHelper.Max(.15f, 2f - (float)speedMult)),
					Acceleration,
					(int)meleeDMG.ApplyTo(MinDamage), 
					(int)meleeDMG.ApplyTo(MaxDamage), 
					(int)meleeKB.ApplyTo(MinKnockback), 
					(int)meleeKB.ApplyTo(MaxKnockback));
			}

			return false;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			StatModifier meleeStat = Main.LocalPlayer.GetTotalDamage(DamageClass.Melee);

			foreach (TooltipLine line in tooltips)
			{
				if (line.Mod == "Terraria" && line.Name == "Damage") //Replace the vanilla text with our own
					line.Text = $"{(int)meleeStat.ApplyTo(MinDamage)}-{(int)meleeStat.ApplyTo(MaxDamage)}" + Language.GetText("LegacyTooltip.2");
			}
		}
	}
}