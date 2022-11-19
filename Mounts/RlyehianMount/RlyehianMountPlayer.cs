using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace SpiritMod.Mounts.RlyehianMount
{
	internal class RlyehianMountPlayer : ModPlayer
	{
		public bool usingMount = false;
		public override void PostUpdate()
		{
			if (!usingMount)
				return;
			Player.legFrame = new Rectangle(0, 9000, 40, 56);
			Player.bodyFrame = new Rectangle(0, 0, 40, 56);
			usingMount = false;
		}
	}
}