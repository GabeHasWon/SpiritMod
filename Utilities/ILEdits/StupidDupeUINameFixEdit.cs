using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Items;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace SpiritMod.Utilities.ILEdits;

class StupidDupeUINameFixEdit : ILEdit
{
	public override void Load(Mod mod) => IL_UIDynamicItemCollection.DrawSelf += StupidFix;

	private void StupidFix(ILContext il)
	{
		ILCursor c = new(il);
		var setDefaults = typeof(Item).GetMethod(nameof(Item.SetDefaults), BindingFlags.Public | BindingFlags.Instance, [typeof(int)]);
		
		c.GotoNext(x => x.MatchCallvirt(setDefaults));

		c.Emit(OpCodes.Ldarg_0);
		c.Emit<UIDynamicItemCollection>(OpCodes.Ldfld, "_item");
		c.EmitDelegate((Item item) => item.ClearNameOverride());
	}
}
