using SpiritMod.Projectiles.Yoyo;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.InfernonDrops
{
	public class EyeOfTheInferno : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodYoyo);
			Item.damage = 42;
			Item.value = Terraria.Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.knockBack = 2.9f;
			Item.channel = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 26;
			Item.useTime = 26;
			Item.shoot = ModContent.ProjectileType<EyeOfTheInfernoProj>();
		}
	}
}