using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CerberusWareV3.Localization;
using Hexa.NET.ImGui;

namespace CerberusWareV3.Configuration
{
	[CompilerGenerated]
	public static class CerberusConfig
	{
		// Добавляем отсутствующее поле!
		public static string ConfigName = "default"; 

		public static class GunAimBot
		{
			public static bool Enabled;
			public static ImGuiKey HotKey = ImGuiKey.MouseRight;
			public static bool TargetCritical;
			public static bool MinSpread;
			public static bool HitScan;
			public static bool AutoPredict = true;
			public static bool PredictEnabled = true;
			public static float PredictCorrection;
			public static bool ShowCircle = true;
			public static bool ShowLine = true;
			public static float CircleRadius = 2.5f;
			public static Vector4 Color = new Vector4(1f, 0f, 0f, 1f);
			public static int TargetPriority;
			public static bool OnlyPriority;
		}
		public static class MeleeAimBot
		{
			public static bool Enabled;
			public static ImGuiKey LightHotKey = ImGuiKey.MouseX2;
			public static ImGuiKey HeavyHotKey = ImGuiKey.MouseX1;
			public static bool TargetCritical;
			public static bool ShowCircle = true;
			public static bool ShowLine = true;
			public static float CircleRadius = 2.5f;
			public static Vector4 Color = new Vector4(1f, 0f, 0f, 1f);
			public static int TargetPriority;
			public static bool OnlyPriority;
			public static bool FixNetworkDelay;
			public static float FixDelay = 0.6f;
		}
		public static class GunHelper
		{
			public static bool Enabled;
			public static bool ShowAmmo;
			public static bool AutoBolt;
			public static bool AutoReload;
			public static float AutoReloadDelay = 0.1f;
		}
		public static class MeleeHelper
		{
			public static bool Enabled;
			public static bool Attack360;
			public static bool AutoAttack;
			public static bool RotateToTarget = true;
		}
		
		
		public static class Esp
		{
			public static bool Enabled;
			public static bool ShowName = true;
			public static bool ShowCKey = true;
			public static bool ShowAntag = true;
			public static bool ShowFriend = true;
			public static bool ShowPriority = true;
			public static bool ShowCombatMode = true;
			public static bool ShowImplants = true;
			public static bool ShowContraband = true;
			public static bool ShowWeapon = true;
			public static bool ShowNoSlip = true;
			public static Vector4 NameColor = new Vector4(0.49803922f, 1f, 0.83137256f, 1f);
			public static Vector4 CKeyColor = new Vector4(1f, 1f, 0f, 1f);
			public static Vector4 AntagColor = new Vector4(0.54509807f, 0f, 0f, 1f);
			public static Vector4 FriendColor = new Vector4(0f, 0.5019608f, 0f, 1f);
			public static Vector4 PriorityColor = new Vector4(0f, 0.5019608f, 0f, 1f);
			public static Vector4 CombatModeColor = new Vector4(1f, 0f, 0f, 1f);
			public static Vector4 ImplantsColor = new Vector4(1f, 0.27058825f, 0f, 1f);
			public static Vector4 ContrabandColor = new Vector4(1f, 0.27058825f, 0f, 1f);
			public static Vector4 WeaponColor = new Vector4(1f, 0.27058825f, 0f, 1f);
			public static Vector4 NoSlipColor = new Vector4(0.6784314f, 0.84705883f, 0.9019608f, 1f);
			public static string MainFontPath = "/Fonts/Boxfont-round/Boxfont Round.ttf";
			public static int MainFontIndex;
			public static int MainFontSize = 10;
			public static string OtherFontPath = "/Fonts/Boxfont-round/Boxfont Round.ttf";
			public static int OtherFontIndex;
			public static int OtherFontSize = 8;
			public static int FontInterval = 15;
		}
		public static class Eye
		{
			public static bool FovEnabled;
			public static bool FullBrightEnabled;
			public static float Zoom = 1f;
			public static bool SuperFastZoom;
			public static ImGuiKey FovHotKey = ImGuiKey.None;
			public static ImGuiKey FullBrightHotKey = ImGuiKey.None;
			public static ImGuiKey ZoomUpHotKey = ImGuiKey.UpArrow;
			public static ImGuiKey ZoomDownHotKey = ImGuiKey.DownArrow;
		}
		public static class Hud
		{
			public static bool ShowHealth;
			public static bool ShowAntag;
			public static bool ShowJobIcons;
			public static bool ShowMindShieldIcons;
			public static bool ShowCriminalRecordIcons;
			public static bool ShowSyndicateIcons;
			public static bool ChemicalAnalysis;
			public static bool ShowElectrocution;
			public static bool ShowStamina;
			public static Vector4 StaminaColor = new Vector4(1f, 0.60784316f, 0.6666667f, 1f);
		}
		public static class StorageViewer
		{
			public static bool Enabled;
			public static Vector4 Color = new Vector4(1f, 0f, 0f, 1f);
			public static ImGuiKey HotKey = ImGuiKey.MouseMiddle;
		}
		
		
		public static class Spammer
		{
			unsafe static Spammer()
			{
				int num = 1;
				List<int> list = new List<int>(num);
				CollectionsMarshal.SetCount<int>(list, num);
				Span<int> span = CollectionsMarshal.AsSpan<int>(list);
				int num2 = 0;
				span[num2] = 16;
				CerberusConfig.Spammer.Channels = list;
			}
			public static bool ChatEnabled;
			public static string ChatText = "https://t.me/RobusterHome";
			public static int ChatDelay = 200;
			public static bool ProtectTextEnabled;
			public static bool ProtectRandomLength = true;
			public static int ProtectLength = 6;
			public static bool AHelpEnabled;
			public static string AHelpText = "https://t.me/RobusterHome";
			public static int AHelpDelay = 200;
			public static List<int> Channels;
		}
		public static class Misc
		{
			public static bool TrashTalkEnabled;
			public static bool DamageOverlayEnabled;
			public static bool AntiSoapEnabled;
			public static bool AntiAfkEnabled;
			public static bool AntiAimEnabled;
			public static float AutoRotateSpeed = 2700f;
			public static bool ItemSearcherEnabled;
			
			public static List<ItemSearchEntry> ItemSearchEntries = new List<ItemSearchEntry>();
			public static bool ItemSearcherShowName = true;
			public static bool ShowExplosive;
			public static bool ShowTrajectory;
		}
		public static class Fun
		{
			public static bool Enabled;
			public static bool RainbowEnabled;
			public static bool RotationEnabled;
			public static bool TrailsEnabled;
			public static bool JumpEnabled;
			public static bool ShakeEnabled;
			public static float RotationSpeed = 180f;
			public static Vector4 Color = new Vector4(1f, 1f, 1f, 1f);
			public static float ScaleX = 1f;
			public static float ScaleY = 1f;
			public static float RainbowSpeed = 1f;
			public static bool AffectPlayer;
			public static bool AffectMobs = true;
			public static bool AffectOthers = true;
		}
		public static class Texture
		{
			public static bool Enabled;
			public static float Size = 1.5f;
			public static bool MakeEntitiesInvisible;
		}
		
		
		public static class Settings
		{
			public static bool UiCustomizable;
			public static bool ShowMenu = true;
			public static ImGuiKey ShowMenuHotKey = ImGuiKey.Delete;
			public static bool ShowDebugConsole;
			public static Language CurrentLanguage = Language.En;
			public static bool ClydePatch = true;
			public static bool OverlaysPatch = true;
			public static bool SmokePatch = true;
			public static bool AdminPatch = true;
			public static bool DamageForcePatch = true;
			public static bool NoDmgFriendPatch = true;
			public static bool AntiScreenGrubPatch = true;
			public static bool TranslateChatPatch;
			public static string TranslateChatLang = "ru";
			public static bool TranslateMePatch;
			public static string TranslateMeLang = "en";
			public static bool NoCameraKickPatch = true;
		}
		public static class Notifications
		{
			public static bool Enabled = true;
			public static int MaxNotifications = 5;
			public static int FontSize = 1;
			public static Vector2 AnchorPosition = Vector2.Zero;
			public static bool IgnoreSizeCheck;
		}
		public static class NoSavedConfig
		{
			public static bool HasTarget;
			public static bool HasAntiCheat;
			
			public static string Version = "V3.1.2";
		}
	}
}
