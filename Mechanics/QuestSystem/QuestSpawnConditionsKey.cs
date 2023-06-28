namespace SpiritMod.Mechanics.QuestSystem;

/// <summary>
/// Marks a method as a SpawnCondition method, and assigns it a key.<br/>
/// The key is named as follows:<code>"ContainerClass.MethodName"</code>
/// This code only runs for methods in classes that inherit <see cref="Quest"/>.<br/>
/// These methods should ALWAYS be static, deterministic and private if possible.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
sealed class QuestSpawnConditionsKeyAttribute : System.Attribute
{
	public QuestSpawnConditionsKeyAttribute()
	{
	}
}