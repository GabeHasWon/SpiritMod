//using System;
//using Terraria.ID;
//using Terraria;
//using SpiritMod.Mechanics.Fathomless_Chest;

//namespace SpiritMod.Mechanics.Syncing;

//public class UseMysticalDiceModule(byte fromWho, byte invSlot)
//{
//	private readonly byte _from = fromWho;
//	private readonly byte _invSlot = invSlot;

//	private bool _successFromServer = false;

//	protected override void Receive()
//	{
//		if (Main.netMode == NetmodeID.Server)
//		{
//			_successFromServer = Mystical_Dice.FindFathomlessChest(Main.player[_from], Main.player[_from].inventory[_invSlot]);
//			Send(-1, -1);

//			if (_successFromServer)
//				NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, _from);
//		}
//		else if (Main.myPlayer == _from)
//		{
//			if (_successFromServer)
//				Mystical_Dice.FailFind(Main.LocalPlayer, Main.LocalPlayer.inventory[_invSlot]);
//		}
//	}
//}