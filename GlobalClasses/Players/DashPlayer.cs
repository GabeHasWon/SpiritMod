using Microsoft.Xna.Framework;
using SpiritMod.Dusts;
using SpiritMod.Items.BossLoot.ScarabeusDrops.ChitinArmor;
using SpiritMod.Items.Equipment.AuroraSaddle;
using SpiritMod.Mounts;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.Players;

internal class DashPlayer : ModPlayer
{
	public DashType ActiveDash { get; private set; }

	public int chitinDashTicks;
	public bool chitinSet;

	private readonly int[] _horiTimers = new int[2];
	private int _dashTimer = 0;
	private int _dashCooldown = 0;

	public override void ResetEffects()
	{
		chitinDashTicks = Math.Max(chitinDashTicks - 1, 0);
		chitinSet = false;
	}

	internal void DashMovement()
	{
		DashType dash = FindDashes();
		_dashTimer--;

		if (_dashTimer == 0)
		{
			if (ActiveDash != DashType.None)
			{
				//DashEnd();
				ActiveDash = DashType.None;
			}
		}
		else if (_dashTimer > 0)
			ModifyActiveDash(dash);
		else if (dash != DashType.None && Player.whoAmI == Main.myPlayer && _dashTimer < 0)
		{
			if (_dashCooldown-- > 0)
				return;

			_horiTimers[0]--;
			_horiTimers[1]--;

			if (Player.controlRight && Player.releaseRight)
			{
				if (_horiTimers[0] > 0)
				{
					PerformDash(dash, 1);
					Player.dashTime = 0;
				}
				else
					_horiTimers[0] = 15;
			}
			else if (Player.controlLeft && Player.releaseLeft)
			{
				if (_horiTimers[1] > 0)
				{
					PerformDash(dash, -1);
					Player.dashTime = 0;
				}
				else
					_horiTimers[1] = 15;
			}
		}
	}

	/// <summary>Controls an active dash, i.e. visuals, speed, etc while dashing.</summary>
	/// <param name="dash">Dash type to use.</param>
	private void ModifyActiveDash(DashType dash)
	{
		// Powered phase
		// Manage dash abilities here
		float speedCap = 20f;
		float decayCapped = 0.992f;
		float speedMax = Math.Max(Player.accRunSpeed, Player.maxRunSpeed);
		float decayMax = 0.96f;
		int delay = 20;

		switch (dash)
		{
			case DashType.Chitin:
				ChitinHelmet.ChitinDashVisuals(Player, out speedCap, out decayCapped, out speedMax, out decayMax, out delay);
				break;
			case DashType.AuroraStag:
				AuroraDashVisuals(out speedCap, out decayCapped, speedMax, out decayMax, out delay);
				break;
		}

		if (dash != DashType.None)
		{
			if (speedCap < speedMax)
				speedCap = speedMax;

			Player.vortexStealthActive = false;
			if (Player.velocity.X > speedCap || Player.velocity.X < -speedCap)
				Player.velocity.X = Player.velocity.X * decayCapped;
			else if (Player.velocity.X > speedMax || Player.velocity.X < -speedMax)
				Player.velocity.X = Player.velocity.X * decayMax;
			else
			{
				Player.dashDelay = delay;

				if (Player.velocity.X < 0f)
					Player.velocity.X = -speedMax;
				else if (Player.velocity.X > 0f)
					Player.velocity.X = speedMax;
			}
		}
	}

	private void AuroraDashVisuals(out float speedCap, out float decayCapped, float speedMax, out float decayMax, out int delay)
	{
		Player.noKnockback = true;

		for (int i = 0; i < 2; i++)
		{
			AuroraStagMount.MakeStar(Main.rand.NextFloat(0.1f, 0.2f), Player.Center);
			Dust dust = Dust.NewDustDirect(Player.position - new Vector2(40, 0), Player.width + 80, Player.height, DustID.RainbowTorch, Player.velocity.X * Main.rand.NextFloat(), 0, 200, AuroraStagMount.AuroraColor * 0.8f, Main.rand.NextFloat(0.8f, 1.1f));
			dust.fadeIn = 0.4f;
			dust.noGravity = true;
			if (Player.miscDyes[3] != null && Player.miscDyes[3].active)
				dust.shader = GameShaders.Armor.GetShaderFromItemId(Player.miscDyes[3].type);
		}

		speedCap = speedMax;
		decayCapped = 0.92f;
		decayMax = decayCapped;
		delay = 40;
	}

	//private void DashEnd() { }

	internal void PerformDash(DashType dash, sbyte dir, bool local = true)
	{
		float horizontalSpeed = dir;

		switch (dash)
		{
			case DashType.Chitin:
				horizontalSpeed *= 20;

				if (Main.netMode != NetmodeID.Server)
				{
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/BossSFX/Scarab_Roar2") with { PitchVariance = 0.2f, Volume = 0.5f }, Player.Center);

					for (int i = 0; i < 16; i++)
						Dust.NewDust(Player.position, Player.width, Player.height, Mod.Find<ModDust>("SandDust").Type, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), Scale: Main.rand.NextFloat(1, 2));
				}

				_dashTimer = 15;
				_dashCooldown = 20;
				break;
			case DashType.Drakomire:
				horizontalSpeed *= 23f;

				_dashTimer = 15;
				_dashCooldown = 600;
				break;
			case DashType.AuroraStag:
				horizontalSpeed *= 40;
				for (int i = 0; i < 25; i++)
					AuroraStagMount.MakeStar(Main.rand.NextFloat(0.4f, 0.6f), Player.Center);

				_dashTimer = 20;
				break;
		}

		Player.velocity.X = horizontalSpeed;

		Point feet = (Player.Center + new Vector2(dir * (Player.width >> 1) + 2, Player.gravDir * -Player.height * .5f + Player.gravDir * 2f)).ToTileCoordinates();
		Point legs = (Player.Center + new Vector2(dir * (Player.width >> 1) + 2, 0f)).ToTileCoordinates();

		if (WorldGen.SolidOrSlopedTile(feet.X, feet.Y) || WorldGen.SolidOrSlopedTile(legs.X, legs.Y))
			Player.velocity.X = Player.velocity.X / 2f;

		Player.dashDelay = -1;
		ActiveDash = dash;

		if (!local || Main.netMode == NetmodeID.SinglePlayer)
			return;

		ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.Dash, 3);
		packet.Write((byte)Player.whoAmI);
		packet.Write((byte)dash);
		packet.Write(dir);
		packet.Send();
	}

	public DashType FindDashes()
	{
		if (Player.mount.Active)
		{
			if (Player.mount.Type == ModContent.MountType<AuroraStagMount>())
				return DashType.AuroraStag;
			else if (Player.mount.Type == ModContent.MountType<Drakomire>())
				return DashType.Drakomire;
			return DashType.None;
		}
		if (chitinSet)
			return DashType.Chitin;

		return DashType.None;
	}
}
