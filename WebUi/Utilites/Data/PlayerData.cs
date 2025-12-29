using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;


[CompilerGenerated]
public class PlayerData
{
	public PlayerData(ICommonSession session)
	{
		this.Session = session;
		this.Status = "Online";
		this.EntityName = "Unknown";
		this.LastKnownPosition = Vector2.Zero;
		this.Job = "Unknown";
	}
	public ICommonSession Session { get; set; } 
	public string Status { get; set; }
	public string EntityName { get; set; }
	public EntityUid? AttachedEntity { get; set; }
	public Vector2 LastKnownPosition { get; set; }
	public string Job { get; set; }
	public bool IsVisible { get; set; }
}
