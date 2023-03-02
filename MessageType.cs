namespace SpiritMod
{
	public enum MessageType : byte
	{
		None = 0,
		ProjectileData,
		Dodge,
		Dash,
		PlayerGlyph,
		AuroraData,
		BossSpawnFromClient,
		StartTide,
		TideData,
		TameAuroraStag,
		SpawnTrail,
		PlaceMapPin,
		PlaceSuperSunFlower,
		DestroySuperSunFlower,
		SpawnExplosiveBarrel,
		FathomlessData,
		StarjinxData,
		BoonData,
		RequestQuestManager,
		RecieveQuestManager,
		Quest,
	}

	public enum QuestMessageType : byte
	{
		Deactivate = 0,
		Activate,
		ProgressOrComplete,
		SyncOnNPCLoot,
		SyncOnEditSpawnPool,
		ObtainQuestBook,
		Unlock,
		SyncNPCQueue
	}
}
