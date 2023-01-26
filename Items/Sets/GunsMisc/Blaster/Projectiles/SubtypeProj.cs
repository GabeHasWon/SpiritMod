using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public abstract class SubtypeProj : ModProjectile
	{
		public int Subtype { get; set; }
		protected int maxSubtypes = 4;

		public override void SendExtraAI(BinaryWriter writer) => writer.Write(Subtype);
		public override void ReceiveExtraAI(BinaryReader reader) => Subtype = reader.Read();

		protected Rectangle GetDrawFrame(Texture2D texture) => new(texture.Width / maxSubtypes * Subtype, texture.Height / Main.projFrames[Projectile.type] * Projectile.frame, (texture.Width / maxSubtypes) - 2, (texture.Height / Main.projFrames[Projectile.type]) - ((Main.projFrames[Projectile.type] > 1) ? 2 : 0));
	}
}
