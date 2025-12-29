using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Client.Administration.Managers;
using Content.Shared.Administration;
using HarmonyLib;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;


[CompilerGenerated]
public class MainController
{
	public static MainController Instance
	{
		get
		{
			bool flag = MainController._instance == null;
			if (flag)
			{
				MainController._instance = new MainController();
			}
			return MainController._instance;
		}
	}
	public void PanicUnload()
	{
		bool isAlreadyUnloaded = this._isUnloaded;
		if (!isAlreadyUnloaded)
		{
			this.UpdateAdminStatus(false);
			this.DisableAimBot();
			this.DisableVisuals();
			this.DisableMisc();
			this.DisableSettings();
			this._isUnloaded = true;
		}
	}
	public void Unload()
	{
		this.HideDebugConsole();
		this.SaveState();
		this.DisableAimBot();
		this.DisableVisuals();
		this.DisableMisc();
		this.DisableSettings();
	}
	public void Restore()
	{
		this.RestoreState();
	}
	private void SaveState()
	{
		MainController.SystemState systemState = new MainController.SystemState();
		systemState.GunAimBotEnabled = CerberusConfig.GunAimBot.Enabled;
		systemState.GunHelperEnabled = CerberusConfig.GunHelper.Enabled;
		systemState.MeleeAimBotEnabled = CerberusConfig.MeleeAimBot.Enabled;
		systemState.MeleeHelperEnabled = CerberusConfig.MeleeHelper.Enabled;
		systemState.EspEnabled = CerberusConfig.Esp.Enabled;
		systemState.FunEnabled = CerberusConfig.Fun.Enabled;
		systemState.TextureEnabled = CerberusConfig.Texture.Enabled;
		systemState.EyeZoom = CerberusConfig.Eye.Zoom;
		systemState.EyeFovEnabled = CerberusConfig.Eye.FovEnabled;
		systemState.EyeFullBrightEnabled = CerberusConfig.Eye.FullBrightEnabled;
		systemState.ClydePatch = CerberusConfig.Settings.ClydePatch;
		systemState.SmokePatch = CerberusConfig.Settings.SmokePatch;
		systemState.OverlaysPatch = CerberusConfig.Settings.OverlaysPatch;
		systemState.ShowHealth = CerberusConfig.Hud.ShowHealth;
		systemState.ShowStamina = CerberusConfig.Hud.ShowStamina;
		systemState.ShowAntag = CerberusConfig.Hud.ShowAntag;
		systemState.ShowJobIcons = CerberusConfig.Hud.ShowJobIcons;
		systemState.ShowMindShieldIcons = CerberusConfig.Hud.ShowMindShieldIcons;
		systemState.ShowCriminalRecordIcons = CerberusConfig.Hud.ShowCriminalRecordIcons;
		systemState.ShowSyndicateIcons = CerberusConfig.Hud.ShowSyndicateIcons;
		systemState.AntiSoapEnabled = CerberusConfig.Misc.AntiSoapEnabled;
		systemState.AntiAfkEnabled = CerberusConfig.Misc.AntiAfkEnabled;
		systemState.ShowExplosive = CerberusConfig.Misc.ShowExplosive;
		systemState.ShowTrajectory = CerberusConfig.Misc.ShowTrajectory;
		systemState.AntiAimEnabled = CerberusConfig.Misc.AntiAimEnabled;
		systemState.TrashTalkEnabled = CerberusConfig.Misc.TrashTalkEnabled;
		systemState.ItemSearcherEnabled = CerberusConfig.Misc.ItemSearcherEnabled;
		systemState.TranslateMePatch = CerberusConfig.Settings.TranslateMePatch;
		systemState.TranslateChatPatch = CerberusConfig.Settings.TranslateChatPatch;
		systemState.ChatEnabled = CerberusConfig.Spammer.ChatEnabled;
		systemState.AHelpEnabled = CerberusConfig.Spammer.AHelpEnabled;
		systemState.AdminPatch = CerberusConfig.Settings.AdminPatch;
		systemState.NoDmgFriendPatch = CerberusConfig.Settings.NoDmgFriendPatch;
		systemState.DamageForcePatch = CerberusConfig.Settings.DamageForcePatch;
		systemState.AntiScreenGrubPatch = CerberusConfig.Settings.AntiScreenGrubPatch;
		this._savedState = systemState;
	}
	private void RestoreState()
	{
		bool flag = this._savedState == null;
		if (!flag)
		{
			CerberusConfig.GunAimBot.Enabled = this._savedState.GunAimBotEnabled;
			CerberusConfig.GunHelper.Enabled = this._savedState.GunHelperEnabled;
			CerberusConfig.MeleeAimBot.Enabled = this._savedState.MeleeAimBotEnabled;
			CerberusConfig.MeleeHelper.Enabled = this._savedState.MeleeHelperEnabled;
			CerberusConfig.Esp.Enabled = this._savedState.EspEnabled;
			CerberusConfig.Fun.Enabled = this._savedState.FunEnabled;
			CerberusConfig.Texture.Enabled = this._savedState.TextureEnabled;
			CerberusConfig.Eye.Zoom = this._savedState.EyeZoom;
			CerberusConfig.Eye.FovEnabled = this._savedState.EyeFovEnabled;
			CerberusConfig.Eye.FullBrightEnabled = this._savedState.EyeFullBrightEnabled;
			CerberusConfig.Settings.ClydePatch = this._savedState.ClydePatch;
			CerberusConfig.Settings.SmokePatch = this._savedState.SmokePatch;
			CerberusConfig.Settings.OverlaysPatch = this._savedState.OverlaysPatch;
			CerberusConfig.Misc.AntiSoapEnabled = this._savedState.AntiSoapEnabled;
			CerberusConfig.Misc.AntiAfkEnabled = this._savedState.AntiAfkEnabled;
			CerberusConfig.Misc.AntiAimEnabled = this._savedState.AntiAimEnabled;
			CerberusConfig.Misc.TrashTalkEnabled = this._savedState.TrashTalkEnabled;
			CerberusConfig.Misc.ItemSearcherEnabled = this._savedState.ItemSearcherEnabled;
			CerberusConfig.Misc.ShowExplosive = this._savedState.ShowExplosive;
			CerberusConfig.Misc.ShowTrajectory = this._savedState.ShowTrajectory;
			CerberusConfig.Settings.TranslateMePatch = this._savedState.TranslateMePatch;
			CerberusConfig.Settings.TranslateChatPatch = this._savedState.TranslateChatPatch;
			CerberusConfig.Spammer.ChatEnabled = this._savedState.ChatEnabled;
			CerberusConfig.Spammer.AHelpEnabled = this._savedState.AHelpEnabled;
			CerberusConfig.Settings.AdminPatch = this._savedState.AdminPatch;
			CerberusConfig.Settings.NoDmgFriendPatch = this._savedState.NoDmgFriendPatch;
			CerberusConfig.Settings.DamageForcePatch = this._savedState.DamageForcePatch;
			CerberusConfig.Settings.AntiScreenGrubPatch = this._savedState.AntiScreenGrubPatch;
			CerberusConfig.Hud.ShowHealth = this._savedState.ShowHealth;
			CerberusConfig.Hud.ShowStamina = this._savedState.ShowStamina;
			bool showAntag = this._savedState.ShowAntag;
			if (showAntag)
			{
				this._componentManager.AddComponent("ShowAntagIcons", null);
			}
			bool showJobIcons = this._savedState.ShowJobIcons;
			if (showJobIcons)
			{
				this._componentManager.AddComponent("ShowJobIcons", null);
			}
			bool showMindShieldIcons = this._savedState.ShowMindShieldIcons;
			if (showMindShieldIcons)
			{
				this._componentManager.AddComponent("ShowMindShieldIcons", null);
			}
			bool showCriminalRecordIcons = this._savedState.ShowCriminalRecordIcons;
			if (showCriminalRecordIcons)
			{
				this._componentManager.AddComponent("ShowCriminalRecordIcons", null);
			}
			bool showSyndicateIcons = this._savedState.ShowSyndicateIcons;
			if (showSyndicateIcons)
			{
				this._componentManager.AddComponent("ShowSyndicateIcons", null);
			}
		}
	}
	private void DisableAimBot()
	{
		CerberusConfig.GunAimBot.Enabled = false;
		CerberusConfig.GunHelper.Enabled = false;
		CerberusConfig.MeleeAimBot.Enabled = false;
		CerberusConfig.MeleeHelper.Enabled = false;
	}
	private void DisableVisuals()
	{
		CerberusConfig.Esp.Enabled = false;
		CerberusConfig.Fun.Enabled = false;
		CerberusConfig.Texture.Enabled = false;
		CerberusConfig.Eye.Zoom = 1f;
		CerberusConfig.Eye.FovEnabled = false;
		CerberusConfig.Eye.FullBrightEnabled = false;
		CerberusConfig.Settings.ClydePatch = false;
		CerberusConfig.Settings.SmokePatch = false;
		CerberusConfig.Settings.OverlaysPatch = false;
		CerberusConfig.Hud.ShowHealth = false;
		CerberusConfig.Hud.ShowStamina = false;
		this._componentManager.RemoveComponent("ShowAntagIcons", null);
		this._componentManager.RemoveComponent("ShowJobIcons", null);
		this._componentManager.RemoveComponent("ShowMindShieldIcons", null);
		this._componentManager.RemoveComponent("ShowCriminalRecordIcons", null);
		this._componentManager.RemoveComponent("ShowSyndicateIcons", null);
	}
	private void DisableMisc()
	{
		CerberusConfig.Misc.AntiSoapEnabled = false;
		CerberusConfig.Misc.AntiAfkEnabled = false;
		CerberusConfig.Misc.AntiAimEnabled = false;
		CerberusConfig.Misc.TrashTalkEnabled = false;
		CerberusConfig.Misc.ItemSearcherEnabled = false;
		CerberusConfig.Settings.TranslateMePatch = false;
		CerberusConfig.Settings.TranslateChatPatch = false;
		CerberusConfig.Spammer.ChatEnabled = false;
		CerberusConfig.Spammer.AHelpEnabled = false;
	}
	private void DisableSettings()
	{
		CerberusConfig.Settings.AdminPatch = false;
		CerberusConfig.Settings.NoDmgFriendPatch = false;
		CerberusConfig.Settings.DamageForcePatch = false;
		CerberusConfig.Settings.AntiScreenGrubPatch = false;
	}
	private void UpdateAdminStatus(bool status)
	{
		Type typeFromHandle = typeof(ClientAdminManager);
		MethodInfo methodInfo = AccessTools.Method(typeFromHandle, "UpdateMessageRx", new Type[] { typeof(MsgUpdateAdminStatus) }, null);
		bool flag = AccessTools.TypeByName("IClientAdminManager") == null;
		if (!flag)
		{
			ClientAdminManager clientAdminManager = IoCManager.Resolve<IClientAdminManager>() as ClientAdminManager;
			MsgUpdateAdminStatus msgUpdateAdminStatus = new MsgUpdateAdminStatus
			{
				Admin = new AdminData
				{
					Active = status,
					Flags = (AdminFlags)(status ? (-1) : 0)
				},
				AvailableCommands = Array.Empty<string>()
			};
			methodInfo.Invoke(clientAdminManager, new object[] { msgUpdateAdminStatus });
		}
	}
	private void HideDebugConsole()
	{
		IUserInterfaceManager userInterfaceManager = IoCManager.Resolve<IUserInterfaceManager>();
		PropertyInfo propertyInfo = AccessTools.Property(userInterfaceManager.GetType(), "DebugConsole");
		bool flag = propertyInfo == null;
		if (!flag)
		{
			object value = propertyInfo.GetValue(userInterfaceManager);
			bool flag2 = value == null;
			if (!flag2)
			{
				Type type = value.GetType();
				PropertyInfo propertyInfo2 = AccessTools.Property(type, "MainControl");
				bool flag3 = propertyInfo2 == null;
				if (!flag3)
				{
					Control control = propertyInfo2.GetValue(value) as Control;
					bool flag4 = control == null;
					if (!flag4)
					{
						bool visible = control.Visible;
						if (visible)
						{
							MethodInfo methodInfo = AccessTools.Method(type, "Toggle", null, null);
							bool flag5 = methodInfo == null;
							if (!flag5)
							{
								methodInfo.Invoke(value, null);
							}
						}
					}
				}
			}
		}
	}
	
	private static MainController _instance;
	private readonly ComponentManager _componentManager = new ComponentManager();
	private bool _isUnloaded;
	
	private MainController.SystemState _savedState;
	
	private class SystemState
	{
		public bool GunAimBotEnabled { get; set; }
		public bool GunHelperEnabled { get; set; }
		public bool MeleeAimBotEnabled { get; set; }
		public bool MeleeHelperEnabled { get; set; }
		public bool EspEnabled { get; set; }
		public bool FunEnabled { get; set; }
		public bool TextureEnabled { get; set; }
		public float EyeZoom { get; set; }
		public bool EyeFovEnabled { get; set; }
		public bool EyeFullBrightEnabled { get; set; }
		public bool ClydePatch { get; set; }
		public bool SmokePatch { get; set; }
		public bool OverlaysPatch { get; set; }
		public bool ShowHealth { get; set; }
		public bool ShowStamina { get; set; }
		public bool ShowAntag { get; set; }
		public bool ShowJobIcons { get; set; }
		public bool ShowMindShieldIcons { get; set; }
		public bool ShowCriminalRecordIcons { get; set; }
		public bool ShowSyndicateIcons { get; set; }
		public bool AntiSoapEnabled { get; set; }
		public bool AntiAfkEnabled { get; set; }
		public bool ShowExplosive { get; set; }
		public bool ShowTrajectory { get; set; }
		public bool AntiAimEnabled { get; set; }
		public bool TrashTalkEnabled { get; set; }
		public bool ItemSearcherEnabled { get; set; }
		public bool TranslateMePatch { get; set; }
		public bool TranslateChatPatch { get; set; }
		public bool ChatEnabled { get; set; }
		public bool AHelpEnabled { get; set; }
		public bool AdminPatch { get; set; }
		public bool NoDmgFriendPatch { get; set; }
		public bool DamageForcePatch { get; set; }
		public bool AntiScreenGrubPatch { get; set; }
	}
}
