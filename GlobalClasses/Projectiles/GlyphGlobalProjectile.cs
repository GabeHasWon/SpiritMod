using SpiritMod.GlobalClasses.Items;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.Projectiles
{
	public class GlyphGlobalProjectile : GlobalProjectile
	{
		private static bool hasNext = false;
		private static int nextType;
		private static GlyphType nextGlyph;

        public GlyphType Glyph { get; private set; }

        public override bool InstancePerEntity => true;

		public override void SetDefaults(Projectile projectile)
		{
			bool send = true;

			if (MyPlayer.swingingCheck && MyPlayer.swingingItem != null)
				Glyph = MyPlayer.swingingItem.GetGlobalItem<GlyphGlobalItem>().Glyph;
			else if (Main.ProjectileUpdateLoopIndex >= 0)
			{
				Projectile source = Main.projectile[Main.ProjectileUpdateLoopIndex];
				if (source.active && source.owner == Main.myPlayer)
					Glyph = source.GetGlobalProjectile<GlyphGlobalProjectile>().Glyph;
			}
			else if (hasNext)
			{
				send = false;
				hasNext = false;
				if (projectile.type == nextType)
					Glyph = nextGlyph;
			}
			else
				Glyph = 0;

			if (send && Glyph != 0 && Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.ProjectileData, 3);
				packet.Write((short)projectile.type);
				packet.Write((byte)Glyph);
				packet.Send();
			}
		}

		internal static void ReceiveProjectileData(BinaryReader reader, int sender)
		{
			if (Main.netMode != NetmodeID.Server)
				return;

			hasNext = true;
			nextType = reader.ReadInt16();
			nextGlyph = (GlyphType)reader.ReadByte();

			ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.ProjectileData, 3);
			packet.Write((short)nextType);
			packet.Write((byte)nextGlyph);
			packet.Send(-1, sender);
		}
	}
}
