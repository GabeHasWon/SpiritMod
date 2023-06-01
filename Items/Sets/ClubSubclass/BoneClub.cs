using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ClubSubclass
{
	public class BoneClub : ClubItem
	{
		internal override int ChargeTime => 50;
		internal override float Acceleration => 17f;
		internal override int MinDamage => 66;
		internal override int MaxDamage => 180;
		internal override float MinKnockback => 5f;
		internal override float MaxKnockback => 9f;

		public override void SetStaticDefaults()
		{ 
			DisplayName.SetDefault("Bone Club");
			Tooltip.SetDefault("Charged strikes launch bones forward");
		}

		public override void Defaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.crit = 6;
            Item.value = Item.sellPrice(0, 1, 40, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<Projectiles.Clubs.BoneClubProj>();
        }
	}
}