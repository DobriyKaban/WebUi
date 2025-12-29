using System;
using System.Collections.Generic;
using System.Numerics;
using CerberusWareV3.Localization;
using Hexa.NET.ImGui;

namespace CerberusWareV3.Configuration
{
	public class ConfigManagerData
	{
		public ConfigManagerData.GunAimBotDataClass GunAimBotData { get; set; } = new ConfigManagerData.GunAimBotDataClass();
		public ConfigManagerData.MeleeAimBotDataClass MeleeAimBotData { get; set; } = new ConfigManagerData.MeleeAimBotDataClass();
		public ConfigManagerData.GunHelperDataClass GunHelperData { get; set; } = new ConfigManagerData.GunHelperDataClass();
		public ConfigManagerData.MeleeHelperDataClass MeleeHelperData { get; set; } = new ConfigManagerData.MeleeHelperDataClass();
		public ConfigManagerData.EspDataClass EspData { get; set; } = new ConfigManagerData.EspDataClass();
		public ConfigManagerData.EyeDataClass EyeData { get; set; } = new ConfigManagerData.EyeDataClass();
		public ConfigManagerData.HudDataClass HudData { get; set; } = new ConfigManagerData.HudDataClass();
		public ConfigManagerData.StorageViewerDataClass StorageViewerData { get; set; } = new ConfigManagerData.StorageViewerDataClass();
		public ConfigManagerData.SpammerDataClass SpammerData { get; set; } = new ConfigManagerData.SpammerDataClass();
		public ConfigManagerData.MiscDataClass MiscData { get; set; } = new ConfigManagerData.MiscDataClass();
		public ConfigManagerData.FunDataClass FunData { get; set; } = new ConfigManagerData.FunDataClass();
		public ConfigManagerData.TextureDataClass TextureData { get; set; } = new ConfigManagerData.TextureDataClass();
		public ConfigManagerData.SettingsDataClass SettingsData { get; set; } = new ConfigManagerData.SettingsDataClass();
		public ConfigManagerData.NotificationsDataClass NotificationsData { get; set; } = new ConfigManagerData.NotificationsDataClass();
		public ConfigManagerData.NoSavedConfigDataClass NoSavedConfigData { get; set; } = new ConfigManagerData.NoSavedConfigDataClass();
		public class GunAimBotDataClass
		{
			public bool Enabled { get; set; }
			public ImGuiKey HotKey { get; set; }
			public bool TargetCritical { get; set; }
			public bool MinSpread { get; set; }
			public bool HitScan { get; set; }
			public bool AutoPredict { get; set; }
			public bool PredictEnabled { get; set; }
			public float PredictCorrection { get; set; }
			public bool ShowCircle { get; set; }
			public bool ShowLine { get; set; }
			public float CircleRadius { get; set; }
			public Vector4 Color { get; set; }
			public int TargetPriority { get; set; }
			public bool OnlyPriority { get; set; }
		}
		public class MeleeAimBotDataClass
		{
			public bool Enabled { get; set; }
			public ImGuiKey LightHotKey { get; set; }
			public ImGuiKey HeavyHotKey { get; set; }
			public bool TargetCritical { get; set; }
			public bool ShowCircle { get; set; }
			public bool ShowLine { get; set; }
			public float CircleRadius { get; set; }
			public Vector4 Color { get; set; }
			public int TargetPriority { get; set; }
			public bool OnlyPriority { get; set; }
			public bool FixNetworkDelay { get; set; }
			public float FixDelay { get; set; }
		}
		public class GunHelperDataClass
		{
			public bool Enabled { get; set; }
			public bool ShowAmmo { get; set; }
			public bool AutoBolt { get; set; }
			public bool AutoReload { get; set; }
			public float AutoReloadDelay { get; set; }
		}
		public class MeleeHelperDataClass
		{
			public bool Enabled { get; set; }
			public bool Attack360 { get; set; }
			public bool AutoAttack { get; set; }
			public bool RotateToTarget { get; set; }
		}
		public class EspDataClass
		{
			public bool Enabled { get; set; }
			public bool ShowName { get; set; }
			public bool ShowCKey { get; set; }
			public bool ShowAntag { get; set; }
			public bool ShowFriend { get; set; }
			public bool ShowPriority { get; set; }
			public bool ShowCombatMode { get; set; }
			public bool ShowImplants { get; set; }
			public bool ShowContraband { get; set; }
			public bool ShowWeapon { get; set; }
			public bool ShowNoSlip { get; set; }
			public Vector4 NameColor { get; set; }
			public Vector4 CKeyColor { get; set; }
			public Vector4 AntagColor { get; set; }
			public Vector4 FriendColor { get; set; }
			public Vector4 PriorityColor { get; set; }
			public Vector4 CombatModeColor { get; set; }
			public Vector4 ImplantsColor { get; set; }
			public Vector4 ContrabandColor { get; set; }
			public Vector4 WeaponColor { get; set; }
			public Vector4 NoSlipColor { get; set; }
			public string MainFontPath { get; set; }
			public int MainFontIndex { get; set; }
			public int MainFontSize { get; set; }
			public string OtherFontPath { get; set; }
			public int OtherFontIndex { get; set; }
			public int OtherFontSize { get; set; }
			public int FontInterval { get; set; }
		}
		public class EyeDataClass
		{
			public bool FovEnabled { get; set; }
			public bool FullBrightEnabled { get; set; }
			public float Zoom { get; set; }
			public bool SuperFastZoom { get; set; }
			public ImGuiKey FovHotKey { get; set; }
			public ImGuiKey FullBrightHotKey { get; set; }
			public ImGuiKey ZoomUpHotKey { get; set; }
			public ImGuiKey ZoomDownHotKey { get; set; }
		}
		public class HudDataClass
		{
			public bool ShowHealth { get; set; }
			public bool ShowAntag { get; set; }
			public bool ShowJobIcons { get; set; }
			public bool ShowMindShieldIcons { get; set; }
			public bool ShowCriminalRecordIcons { get; set; }
			public bool ShowSyndicateIcons { get; set; }
			public bool ChemicalAnalysis { get; set; }
			public bool ShowElectrocution { get; set; }
			public bool ShowStamina { get; set; }
			public Vector4 StaminaColor { get; set; }
		}
		public class StorageViewerDataClass
		{
			public bool Enabled { get; set; }
			public Vector4 Color { get; set; }
			public ImGuiKey HotKey { get; set; }
		}
		public class SpammerDataClass
		{
			public bool ChatEnabled { get; set; }
			public string ChatText { get; set; }
			public int ChatDelay { get; set; }
			public bool ProtectTextEnabled { get; set; }
			public bool ProtectRandomLength { get; set; }
			public int ProtectLength { get; set; }
			public bool AHelpEnabled { get; set; }
			public string AHelpText { get; set; }
			public int AHelpDelay { get; set; }
			public List<int> Channels { get; set; }
		}
		public class MiscDataClass
		{
			public bool TrashTalkEnabled { get; set; }
			public bool DamageOverlayEnabled { get; set; }
			public bool AntiSoapEnabled { get; set; }
			public bool AntiAfkEnabled { get; set; }
			public bool AntiAimEnabled { get; set; }
			public float AutoRotateSpeed { get; set; }
			public bool ItemSearcherEnabled { get; set; }

			public List<ItemSearchEntry> ItemSearchEntries { get; set; }
			public bool ItemSearcherShowName { get; set; }
			public bool ShowExplosive { get; set; }
			public bool ShowTrajectory { get; set; }
		}
		public class FunDataClass
		{
			public bool Enabled { get; set; }
			public bool RainbowEnabled { get; set; }
			public bool RotationEnabled { get; set; }
			public bool TrailsEnabled { get; set; }
			public bool JumpEnabled { get; set; }
			public bool ShakeEnabled { get; set; }
			public float RotationSpeed { get; set; }
			public Vector4 Color { get; set; }
			public float ScaleX { get; set; }
			public float ScaleY { get; set; }
			public float RainbowSpeed { get; set; }
			public bool AffectPlayer { get; set; }
			public bool AffectMobs { get; set; }
			public bool AffectOthers { get; set; }
		}
		public class TextureDataClass
		{
			public bool Enabled { get; set; }
			public float Size { get; set; }
			public bool MakeEntitiesInvisible { get; set; }
		}
		public class SettingsDataClass
		{
			public bool UiCustomizable { get; set; }
			public bool ShowMenu { get; set; }
			public ImGuiKey ShowMenuHotKey { get; set; }
			public bool ShowDebugConsole { get; set; }
			public Language CurrentLanguage { get; set; }
			public bool ClydePatch { get; set; }
			public bool OverlaysPatch { get; set; }
			public bool SmokePatch { get; set; }
			public bool AdminPatch { get; set; }
			public bool DamageForcePatch { get; set; }
			public bool NoDmgFriendPatch { get; set; }
			public bool AntiScreenGrubPatch { get; set; }
			public bool TranslateChatPatch { get; set; }
			public string TranslateChatLang { get; set; }
			public bool TranslateMePatch { get; set; }
			public string TranslateMeLang { get; set; }
			public bool NoCameraKickPatch { get; set; }
		}
		public class NotificationsDataClass
		{
			public bool Enabled { get; set; }
			public int MaxNotifications { get; set; }
			public int FontSize { get; set; }
			public Vector2 AnchorPosition { get; set; }
			public bool IgnoreSizeCheck { get; set; }
		}
		public class NoSavedConfigDataClass
		{
			public bool HasTarget { get; set; }
			public bool HasAntiCheat { get; set; }
			public string Version { get; set; }
		}
	}
}
