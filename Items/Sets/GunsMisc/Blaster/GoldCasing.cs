using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class GoldCasing : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			gore.numFrames = 7;
			gore.behindTiles = true;
			gore.timeLeft = Gore.goreTime * 3;
			ChildSafety.SafeGore[gore.type] = true;
		}

		public override bool Update(Gore gore)
		{
			gore.velocity.X *= 0.99f;
			gore.velocity.Y += 0.18f;

			gore.rotation = gore.velocity.ToRotation() + MathHelper.PiOver2;
			gore.position += gore.velocity;

			if (Collision.SolidCollision(gore.position, 2, 2)) 
			{
				gore.active = false;
				return false;
			}

			if (++gore.frameCounter > 4) 
			{
				gore.frameCounter = 0;
				gore.frame = (byte)(++gore.frame % gore.numFrames);
			}
			return false;
		}
	}
}
