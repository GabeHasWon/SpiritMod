using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Block
{
	public class SepulchreBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlendAll[Type] = true;
			HitSound = SoundID.Tink;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(87, 85, 81));
			DustType = DustID.Wraith;
		}
	}
}

