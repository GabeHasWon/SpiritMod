using System;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.Vulture_Matriarch.Matriarch_Wings
{
	public class Matriarch_Wings_Dust : ModDust
	{
		public override void OnSpawn(Dust dust) { }

		public override bool Update(Dust dust)
		{
			dust.noGravity = true;
			dust.position += dust.velocity;
			dust.velocity.Y = Math.Min(dust.velocity.Y + .1f, 1.8f);

			if ((dust.scale *= 0.98f) < 0.2f)
				dust.active = false;

			return false;
		}
	}
}
