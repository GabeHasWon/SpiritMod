using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Gores
{
	public class BlueShardGore : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			gore.numFrames = 4;
			gore.frame = (byte)Main.rand.Next(gore.numFrames);
			gore.timeLeft = 255;
			ChildSafety.SafeGore[gore.type] = true;
		}

		public override Color? GetAlpha(Gore gore, Color lightColor) => Color.White * (float)(1f - (float)(gore.alpha / 255f));
		public override bool Update(Gore gore)
		{
			gore.light = 0.5f;
			gore.position += gore.velocity;
			gore.velocity *= 0.98f;
			gore.rotation += gore.velocity.Length() / 40;
			gore.alpha += 1;

			if (gore.alpha >= 255)
				gore.active = false;
			return false;
		}
	}
}