using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.Tiles.BaseTile;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture
{
	public class SepulchreChestTile1 : BaseChest
	{
		public override string DefaultName => "Sepulchre Chest";

		public override void StaticDefaults(ModTranslation name)
		{
			AddMapEntry(new Color(120, 82, 49), name);
			ChestDrop = ModContent.ItemType<SepulchreChest>();
			DustType = DustID.Asphalt;
		}

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            SoundEngine.PlaySound(SoundID.NPCHit4, new Vector2(i * 16, j * 16));
			base.KillMultiTile(i, j, frameX, frameY);
        }
    }
}