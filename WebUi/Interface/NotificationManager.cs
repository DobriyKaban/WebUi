using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using CerberusWareV3.MyImGui;
using Hexa.NET.ImGui;


[CompilerGenerated]
public class NotificationManager : IOverlay
{
	public static void ShowNotification(string message, float duration = 5f, float fadeInTime = 0.3f, float fadeOutTime = 0.5f, Vector4? lineColor = null, bool useProgressBar = false)
	{
		bool uiCustomizable = CerberusConfig.Settings.UiCustomizable;
		if (!uiCustomizable)
		{
			bool flag = CerberusConfig.Notifications.MaxNotifications > 0 && NotificationManager._notifications.Count >= CerberusConfig.Notifications.MaxNotifications;
			if (flag)
			{
				NotificationManager._notifications.RemoveAt(0);
			}
			NotificationManager._notifications.Add(new NotificationManager.Notification(message, duration, fadeInTime, fadeOutTime, lineColor, useProgressBar));
		}
	}
	public void Render()
	{
		bool flag = !NotificationManager._initialized;
		if (flag)
		{
			NotificationManager.InitializeDefaultPosition();
			NotificationManager._initialized = true;
		}
		NotificationManager.ValidateAndCorrectAnchorPosition();
		ImFontPtr imFontPtr = FontManager.GetFont("global");
		switch (CerberusConfig.Notifications.FontSize)
		{
		case 0:
			imFontPtr = FontManager.GetFont("global-small");
			break;
		case 1:
			imFontPtr = FontManager.GetFont("global");
			break;
		case 2:
			imFontPtr = FontManager.GetFont("global-large");
			break;
		}
		ImGui.PushFont(imFontPtr);
		bool uiCustomizable = CerberusConfig.Settings.UiCustomizable;
		if (uiCustomizable)
		{
			NotificationManager.RenderSetupModeNotification();
			NotificationManager._notifications.Clear();
		}
		else
		{
			bool enabled = CerberusConfig.Notifications.Enabled;
			if (enabled)
			{
				NotificationManager.RenderNormalModeNotifications();
			}
		}
		ImGui.PopFont();
	}
	private unsafe static void InitializeDefaultPosition()
	{
		ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
		Vector2 vector = mainViewport.WorkPos;
		Vector2 vector2 = mainViewport.WorkSize;
		float num = 15f;
		float num2 = 15f;
		NotificationManager._defaultAnchor = vector + vector2 - new Vector2(num, num2);
		bool flag = CerberusConfig.Notifications.AnchorPosition == Vector2.Zero;
		if (flag)
		{
			CerberusConfig.Notifications.AnchorPosition = NotificationManager._defaultAnchor;
		}
	}
	private unsafe static void ValidateAndCorrectAnchorPosition()
	{
		bool ignoreSizeCheck = CerberusConfig.Notifications.IgnoreSizeCheck;
		if (!ignoreSizeCheck)
		{
			ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
			Vector2 vector = mainViewport.WorkPos;
			Vector2 vector2 = mainViewport.WorkSize;
			bool flag = CerberusConfig.Notifications.AnchorPosition.X < vector.X || CerberusConfig.Notifications.AnchorPosition.X > vector.X + vector2.X || CerberusConfig.Notifications.AnchorPosition.Y < vector.Y || CerberusConfig.Notifications.AnchorPosition.Y > vector.Y + vector2.Y;
			bool flag2 = flag;
			if (flag2)
			{
				CerberusConfig.Notifications.AnchorPosition = NotificationManager._defaultAnchor;
			}
		}
	}
	private unsafe static void RenderSetupModeNotification()
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
		defaultInterpolatedStringHandler.AppendLiteral("Notification##");
		defaultInterpolatedStringHandler.AppendFormatted<Guid>(NotificationManager._testId);
		string text = defaultInterpolatedStringHandler.ToStringAndClear();
		float num = 1f;
		Vector4 vector = new Vector4(0.7f, 0.7f, 0.7f, 1f);
		ImGui.SetNextWindowPos(CerberusConfig.Notifications.AnchorPosition, ImGuiCond.Appearing, NotificationManager._pivot);
		ImGui.SetNextWindowBgAlpha(num);
		ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings;
		ImGuiStylePtr style = ImGui.GetStyle();
		Vector2 vector2 = style.WindowPadding;
		ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
		ImGuiStyleVar imGuiStyleVar = ImGuiStyleVar.WindowPadding;
		Vector2 vector3 = vector2;
		vector3.Y = vector2.Y + NotificationManager.BorderThickness;
		ImGui.PushStyleVar(imGuiStyleVar, vector3);
		Vector4 vector4 = style.Colors[0];
		vector4.W = num;
		Vector4 vector5 = style.Colors[2];
		ImGui.PushStyleColor(ImGuiCol.Text, vector4);
		ImGui.PushStyleColor(ImGuiCol.WindowBg, vector5);
		bool flag = ImGui.Begin(text, ref CerberusConfig.Settings.UiCustomizable, imGuiWindowFlags);
		bool flag2 = flag;
		if (flag2)
		{
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			Vector2 windowPos = ImGui.GetWindowPos();
			float windowWidth = ImGui.GetWindowWidth();
			float num2 = windowPos.Y + NotificationManager.BorderThickness * 0.5f;
			float x = windowPos.X;
			float num3 = x + windowWidth;
			uint num4 = ImGui.ColorConvertFloat4ToU32(vector);
			windowDrawList.AddLine(new Vector2(x, num2), new Vector2(num3, num2), num4, NotificationManager.BorderThickness);
			ImGui.TextUnformatted(LocalizationManager.GetString("Notification_TestMessage"));
			Vector2 windowPos2 = ImGui.GetWindowPos();
			Vector2 windowSize = ImGui.GetWindowSize();
			CerberusConfig.Notifications.AnchorPosition = windowPos2 + windowSize * NotificationManager._pivot;
			ImGui.End();
		}
		ImGui.PopStyleColor(2);
		ImGui.PopStyleVar(2);
	}
	private unsafe static void RenderNormalModeNotifications()
	{
		float currentTime = (float)ImGui.GetTime();
		float num = ImGui.GetIO().DeltaTime;
		List<NotificationManager.Notification> activeNotificationsThisFrame = new List<NotificationManager.Notification>();
		NotificationManager._notifications.RemoveAll(delegate(NotificationManager.Notification n)
		{
			float num6;
			float num7;
			float num8;
			bool animationState = n.GetAnimationState(currentTime, out num6, out num7, out num8);
			bool flag3 = animationState;
			if (flag3)
			{
				activeNotificationsThisFrame.Add(n);
			}
			return !animationState;
		});
		float num2 = CerberusConfig.Notifications.AnchorPosition.Y;
		for (int i = activeNotificationsThisFrame.Count - 1; i >= 0; i--)
		{
			NotificationManager.Notification notification = activeNotificationsThisFrame[i];
			notification.TargetY = num2;
			bool flag = !notification.IsInitialized;
			if (flag)
			{
				notification.CurrentY = notification.TargetY + notification.Height;
				notification.IsInitialized = true;
			}
			notification.CurrentY = NotificationManager.MathHelper.LerpSmooth(notification.CurrentY, notification.TargetY, NotificationManager.LerpFactor, num);
			float num3;
			float num4;
			float num5;
			notification.GetAnimationState(currentTime, out num3, out num4, out num5);
			NotificationManager.RenderSingleNotification(notification, num3, num4, num5, currentTime);
			bool flag2 = num3 > 0.01f;
			if (flag2)
			{
				num2 -= notification.Height + NotificationManager.NotificationSpacing;
			}
		}
	}
	private unsafe static void RenderSingleNotification(NotificationManager.Notification notification, float alpha, float fadeInOffsetY, float lifeProgress, float currentTime)
	{
		float x = CerberusConfig.Notifications.AnchorPosition.X;
		float num = notification.CurrentY + fadeInOffsetY;
		ImGui.SetNextWindowPos(new Vector2(x, num), ImGuiCond.Always, NotificationManager._pivot);
		ImGui.SetNextWindowBgAlpha(alpha);
		ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNavInputs | ImGuiWindowFlags.NoNavFocus;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
		defaultInterpolatedStringHandler.AppendLiteral("Notification##");
		defaultInterpolatedStringHandler.AppendFormatted<Guid>(notification.Id);
		string text = defaultInterpolatedStringHandler.ToStringAndClear();
		ImGuiStylePtr style = ImGui.GetStyle();
		Vector2 vector = style.WindowPadding;
		ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
		ImGuiStyleVar imGuiStyleVar = ImGuiStyleVar.WindowPadding;
		Vector2 vector2 = vector;
		vector2.Y = vector.Y + NotificationManager.BorderThickness;
		ImGui.PushStyleVar(imGuiStyleVar, vector2);
		Vector4 vector3 = style.Colors[0];
		vector3.W = alpha;
		Vector4 vector4 = style.Colors[2];
		vector4.W *= alpha;
		ImGui.PushStyleColor(ImGuiCol.Text, vector3);
		ImGui.PushStyleColor(ImGuiCol.WindowBg, vector4);
		bool flag = ImGui.Begin(text, imGuiWindowFlags);
		bool flag2 = flag;
		if (flag2)
		{
			bool flag3 = ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left);
			if (flag3)
			{
				bool flag4 = float.IsPositiveInfinity(notification.ForceCloseStartTime);
				if (flag4)
				{
					notification.ForceCloseStartTime = currentTime;
					float num2;
					notification.GetAnimationState(currentTime, out alpha, out num2, out lifeProgress);
					vector3.W = alpha;
					vector4.W = style.Colors[2].W * alpha;
					ImGui.GetStyle().Colors[0] = vector3;
					ImGui.GetStyle().Colors[2] = vector4;
				}
			}
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			Vector2 windowPos = ImGui.GetWindowPos();
			float windowWidth = ImGui.GetWindowWidth();
			float num3 = windowPos.Y + NotificationManager.BorderThickness * 0.5f;
			float x2 = windowPos.X;
			float num4 = (notification.UseProgressBar ? (x2 + windowWidth * (1f - lifeProgress)) : (x2 + windowWidth));
			Vector4 lineColor = notification.LineColor;
			lineColor.W *= alpha;
			uint num5 = ImGui.ColorConvertFloat4ToU32(lineColor);
			bool flag5 = alpha > 0.001f;
			if (flag5)
			{
				windowDrawList.AddLine(new Vector2(x2, num3), new Vector2(num4, num3), num5, NotificationManager.BorderThickness);
			}
			ImGui.TextUnformatted(notification.Message);
			notification.Height = ImGui.GetWindowHeight();
			ImGui.End();
		}
		ImGui.PopStyleColor(2);
		ImGui.PopStyleVar(2);
	}
	private static Vector2 _defaultAnchor = Vector2.Zero;
	private static readonly Vector2 _pivot = new Vector2(1f, 1f);
	private static bool _initialized;
	private static readonly Guid _testId = Guid.Empty;
	private static readonly List<NotificationManager.Notification> _notifications = new List<NotificationManager.Notification>();
	private static readonly float NotificationSpacing = 10f;
	private static readonly float LerpFactor = 0.15f;
	private static readonly float BorderThickness = 2f;
	
	private class Notification
	{
		public Guid Id { get; }
		public string Message { get; }
		public float Duration { get; }
		public float FadeInTime { get; }
		public float FadeOutTime { get; }
		public float StartTime { get; }
		public Vector4 LineColor { get; }
		public bool UseProgressBar { get; }
		public float Height { get; set; }
		public float CurrentY { get; set; }
		public float TargetY { get; set; }
		public bool IsInitialized { get; set; }
		public float ForceCloseStartTime { get; set; }
		public Notification(string message, float duration, float fadeInTime, float fadeOutTime, Vector4? lineColor, bool useProgressBar)
		{
			this.ForceCloseStartTime = float.PositiveInfinity;
			this.Id = Guid.NewGuid();
			this.Message = message;
			this.Duration = Math.Max(duration, fadeInTime + fadeOutTime);
			this.FadeInTime = Math.Min(fadeInTime, this.Duration / 2f);
			this.FadeOutTime = Math.Min(fadeOutTime, this.Duration / 2f);
			this.StartTime = (float)ImGui.GetTime();
			this.LineColor = lineColor ?? new Vector4(1f, 1f, 1f, 1f);
			this.UseProgressBar = useProgressBar;
			this.Height = 30f;
			this.IsInitialized = false;
		}
		public bool GetAnimationState(float currentTime, out float alpha, out float fadeInOffsetY, out float lifeProgress)
		{
			float num = currentTime - this.StartTime;
			float num2 = currentTime - this.ForceCloseStartTime;
			alpha = 0f;
			fadeInOffsetY = 0f;
			lifeProgress = Math.Clamp(num / this.Duration, 0f, 1f);
			bool flag = num2 >= 0f;
			bool flag3;
			if (flag)
			{
				bool flag2 = this.FadeOutTime > 0f;
				if (flag2)
				{
					float num3 = Math.Clamp(num2 / this.FadeOutTime, 0f, 1f);
					alpha = NotificationManager.MathHelper.Lerp(1f, 0f, num3);
					flag3 = num3 < 1f;
				}
				else
				{
					alpha = 0f;
					flag3 = false;
				}
			}
			else
			{
				bool flag4 = num < 0f;
				if (flag4)
				{
					num = 0f;
				}
				bool flag5 = num < this.FadeInTime;
				if (flag5)
				{
					float num4 = ((this.FadeInTime > 0f) ? (num / this.FadeInTime) : 1f);
					alpha = NotificationManager.MathHelper.Lerp(0f, 1f, num4);
					fadeInOffsetY = NotificationManager.MathHelper.Lerp(20f, 0f, num4);
					flag3 = true;
				}
				else
				{
					bool flag6 = num < this.Duration - this.FadeOutTime;
					if (flag6)
					{
						alpha = 1f;
						flag3 = true;
					}
					else
					{
						bool flag7 = num < this.Duration;
						if (flag7)
						{
							bool flag8 = this.FadeOutTime > 0f;
							if (flag8)
							{
								float num5 = num - (this.Duration - this.FadeOutTime);
								float num6 = num5 / this.FadeOutTime;
								alpha = NotificationManager.MathHelper.Lerp(1f, 0f, num6);
							}
							else
							{
								alpha = 0f;
							}
							flag3 = alpha > 0.001f;
						}
						else
						{
							alpha = 0f;
							flag3 = false;
						}
					}
				}
			}
			return flag3;
		}
	}
	
	private static class MathHelper
	{
		public static float Lerp(float a, float b, float t)
		{
			t = Math.Max(0f, Math.Min(1f, t));
			return a + (b - a) * t;
		}
		public static float LerpSmooth(float a, float b, float factor, float deltaTime)
		{
			float num = 1f - (float)Math.Pow((double)(1f - factor), (double)(deltaTime * 60f));
			num = Math.Clamp(num, 0f, 1f);
			bool flag = Math.Abs(a - b) < 0.1f;
			float num2;
			if (flag)
			{
				num2 = b;
			}
			else
			{
				num2 = a + (b - a) * num;
			}
			return num2;
		}
	}
}
