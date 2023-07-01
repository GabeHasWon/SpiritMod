using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Pins
{
	// Want to add another map pin?
	// Follow the pattern here, and add the corresponding textures.
	// Careful with removing them, though - placed pins remain in the save data

	public class PinRed : AMapPin
	{
		public override string PinName => "Red";
		public override Color TextColor => Color.IndianRed;
	}

	public class PinGreen : AMapPin
	{
		public override string PinName => "Green";
		public override Color TextColor => Color.Lime;
	}

	public class PinBlue : AMapPin
	{
		public override string PinName => "Blue";
		public override Color TextColor => Color.DeepSkyBlue;
	}

	public class PinYellow : AMapPin
	{
		public override string PinName => "Yellow";
		public override Color TextColor => Color.Gold;
	}

	public class PinTree : AMapPin
	{
		public override string PinName => "Tree";
		public override Color TextColor => Color.ForestGreen;
	}

	public class PinStar : AMapPin
	{
		public override string PinName => "Star";
		public override Color TextColor => Color.LightGoldenrodYellow;
	}

	public class PinCopperCoin : AMapPin
	{
		public override string PinName => "Copper";
		public override Color TextColor => Color.SaddleBrown;
	}

	public class PinSilverCoin : AMapPin
	{
		public override string PinName => "Silver";
		public override Color TextColor => Color.Silver;
	}

	public class PinGoldCoin : AMapPin
	{
		public override string PinName => "Gold";
		public override Color TextColor => Color.Gold;
	}

	public class PinScarab : AMapPin
	{
		public override string PinName => "Scarab";
		public override Color TextColor => Color.RoyalBlue;
	}

	public class PinMoonjelly : AMapPin
	{
		public override string PinName => "Moonjelly";
		public override Color TextColor => Color.RoyalBlue;
	}
}