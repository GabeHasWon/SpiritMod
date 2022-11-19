using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.Players
{
	/// <summary>
	/// Controls legacy pet bools and gives the PetFlag tool.<br/>
	/// DO NOT make a new bool - simply use <see cref="PetFlag(Projectile)"/> in the pet's AI instead! Reduces field spam and is more readable.<br/>
	/// Simply put this line in the pet's AI: <code>Owner.GetModPlayer&lt;GlobalClasses.Players.PetPlayer>().PetFlag(Projectile);</code>
	/// </summary>
	public class PetPlayer : ModPlayer
	{
		public bool starPet = false;
		public bool saucerPet = false;
		public bool bookPet = false;
		public bool swordPet = false;
		public bool shadowPet = false;
		public bool starachnidPet = false;
		public bool thrallPet = false;
		public bool jellyfishPet = false;
		public bool phantomPet = false;
		public bool lanternPet = false;
		public bool maskPet = false;
		public bool harpyPet = false;
		public bool cultFishPet = false;
		public bool briarSlimePet = false;
		public bool scarabPet = false;
		public bool vinewrathPet = false;
		public bool mjwPet = false;
		public bool starplatePet = false;
		public bool infernonPet = false;

		public Dictionary<int, bool> pets = new();

		public override void ResetEffects()
		{
			starPet = false;
			saucerPet = false;
			bookPet = false;
			swordPet = false;
			shadowPet = false;
			starachnidPet = false;
			thrallPet = false;
			jellyfishPet = false;
			phantomPet = false;
			lanternPet = false;
			maskPet = false;
			harpyPet = false;
			cultFishPet = false;
			briarSlimePet = false;
			scarabPet = false;
			vinewrathPet = false;
			mjwPet = false;
			starplatePet = false;
			infernonPet = false;

			foreach (int item in pets.Keys)
				pets[item] = false;
		}

		/// <summary>Automatically sets pet flag for any given projectile using <see cref="pets"/>.</summary>
		public void PetFlag(Projectile projectile)
		{
			var modPlayer = Main.player[projectile.owner].GetModPlayer<PetPlayer>();

			if (!modPlayer.pets.ContainsKey(projectile.type))
				modPlayer.pets.Add(projectile.type, true);

			if (Player.dead)
				modPlayer.pets[projectile.type] = false;

			if (modPlayer.pets[projectile.type])
				projectile.timeLeft = 2;
		}
	}
}
