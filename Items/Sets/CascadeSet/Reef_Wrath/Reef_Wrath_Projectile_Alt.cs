using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.CascadeSet.Reef_Wrath
{
	public class Reef_Wrath_Projectile_Alt : ModProjectile
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Projectiles.Reef_Wrath_Projectile_1.DisplayName");

		public override void SetDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.aiStyle = 1;
			AIType = ProjectileID.WoodenArrowFriendly;
			Projectile.hide = true;
			Projectile.scale = 1f;
			Projectile.timeLeft = 2;
		}

		public override bool ShouldUpdatePosition() => false;

		public override bool? CanDamage() => false;

		public override void OnKill(int timeLeft)
		{
			for (int i = 1; i <= 3; i++)
			{
				Vector2 position = Projectile.position;
				position += new Vector2(0, - (18 * (i - 1))).RotatedBy(Projectile.velocity.ToRotation());
				Projectile.NewProjectile(Projectile.GetSource_Death(), position, Projectile.velocity, Mod.Find<ModProjectile>("Reef_Wrath_Projectile_" + i).Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
			}

			SoundEngine.PlaySound(SoundID.LiquidsWaterLava);
		}
	}
}
