using Microsoft.Xna.Framework;
using SpiritMod.Tiles.BaseTile;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture.Acid
{
	public class AcidChestTile : BaseChest
	{
		public override int ChestDrop => ModContent.ItemType<Items.Placeable.Furniture.Acid.AcidChest>();

		public override void StaticDefaults(LocalizedText name)
		{
			AddMapEntry(new Color(100, 122, 111), name, MapChestName);
			DustType = DustID.Iron;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			SoundEngine.PlaySound(SoundID.NPCHit4, new Vector2(i * 16, j * 16));
			base.KillMultiTile(i, j, frameX, frameY);
		}
	}
}