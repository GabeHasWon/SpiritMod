using Microsoft.Xna.Framework;
using SpiritMod.Tiles.BaseTile;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture.Acid
{
	public class AcidChestTile : BaseChest
	{
		public override string DefaultName => "Corrosive Chest";

		public override void StaticDefaults(ModTranslation name)
		{
			AddMapEntry(new Color(100, 122, 111), name, MapChestName);
			ChestDrop = ModContent.ItemType<Items.Placeable.Furniture.Acid.AcidChest>();
			DustType = DustID.Iron;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			SoundEngine.PlaySound(SoundID.NPCHit4, new Vector2(i * 16, j * 16));
			base.KillMultiTile(i, j, frameX, frameY);
		}
	}
}