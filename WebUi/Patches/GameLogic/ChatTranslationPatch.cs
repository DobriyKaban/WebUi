using System;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CerberusWareV3.Configuration;
using Content.Client.UserInterface.Systems.Chat;
using Content.Shared.Chat;
using HarmonyLib;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;


[CompilerGenerated]
public static class ChatTranslationPatch
{
	
	private static MethodInfo TargetMethod()
	{
		return AccessTools.Method(typeof(ChatUIController), "OnChatMessage", null, null);
	}
	public static void Patch()
	{
		MethodInfo methodInfo = ChatTranslationPatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(ChatTranslationPatch), "Prefix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Prefix);
	}
	public static bool Prefix(ChatUIController __instance, MsgChatMessage message)
	{
		bool flag = !CerberusConfig.Settings.TranslateChatPatch || string.IsNullOrEmpty(message.Message.Message);
		bool flag2;
		if (flag)
		{
			flag2 = true;
		}
		else
		{
			ChatTranslationPatch.Translate(message, __instance);
			flag2 = false;
		}
		return flag2;
	}
	private static async Task Translate(MsgChatMessage message, ChatUIController __instance)
	{
		ChatMessage msg = message.Message;
		try
		{
			TranslationResponse response = await TranslationService.TranslateAsync(msg.Message, CerberusConfig.Settings.TranslateChatLang, null);
			TranslationResponse translatedText = response;
			response = null;
			msg.WrappedMessage = ChatTranslationPatch.ReplaceContent(msg, translatedText.DestinationText);
			__instance.ProcessChatMessage(msg, true);
			translatedText = null;
		}
		catch (Exception)
		{
			NotificationManager.ShowNotification("Failed to translate message", 5f, 0.3f, 0.5f, new Vector4?(ChatTranslationPatch.ErrorColor), true);
		}
	}
	private static string ReplaceContent(ChatMessage msg,  string newContent)
	{
		IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
		EntityUid entity = entityManager.GetEntity(msg.SenderEntity);
		ChatChannel channel = msg.Channel;
		ChatChannel chatChannel = channel;
		string text;
		string text2;
		if (chatChannel <= (ChatChannel)16)
		{
			if (chatChannel - 1 > (ChatChannel)1)
			{
				if (chatChannel != (ChatChannel)16)
				{
					goto IL_0270;
				}
				text = "(\\[font=.*?\\])(\"|)(.*?)(\\2)(\\[/font\\])";
				text2 = "$1\"" + newContent + "\"$5";
				string text3 = Regex.Replace(msg.WrappedMessage, text, text2);
				bool flag = text3 == msg.WrappedMessage;
				if (flag)
				{
					string text4 = "\\[font size=12\\]\\s*(.*?)\\s*\\[\\/bold\\]";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 3);
					defaultInterpolatedStringHandler.AppendLiteral("[font size=12]");
					defaultInterpolatedStringHandler.AppendFormatted(Environment.NewLine);
					defaultInterpolatedStringHandler.AppendFormatted(newContent);
					defaultInterpolatedStringHandler.AppendFormatted(Environment.NewLine);
					defaultInterpolatedStringHandler.AppendLiteral("[/bold]");
					text2 = defaultInterpolatedStringHandler.ToStringAndClear();
					text3 = Regex.Replace(msg.WrappedMessage, text4, text2);
				}
				return text3;
			}
		}
		else
		{
			if (chatChannel == (ChatChannel)512)
			{
				string entityName = entityManager.GetComponent<MetaDataComponent>(entity).EntityName;
				text = "(\\[italic\\])(.*?)(\\[/italic\\])";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
				defaultInterpolatedStringHandler.AppendLiteral("$1");
				defaultInterpolatedStringHandler.AppendFormatted(entityName);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted(newContent);
				defaultInterpolatedStringHandler.AppendLiteral("$3");
				text2 = defaultInterpolatedStringHandler.ToStringAndClear();
				return Regex.Replace(msg.WrappedMessage, text, text2);
			}
			if (chatChannel != (ChatChannel)1024)
			{
				goto IL_0270;
			}
		}
		text = "(\\[BubbleContent\\])(.*?)(\\[/BubbleContent\\])";
		text2 = "[BubbleContent]" + newContent + "[/BubbleContent]";
		return Regex.Replace(msg.WrappedMessage, text, text2);
		IL_0270:
		text = "(\\[bold\\].*?\\[\\/bold\\])\\s*(.*)";
		text2 = "$1 " + newContent;
		return Regex.Replace(msg.WrappedMessage, text, text2);
	}
	private static readonly Vector4 ErrorColor = new Vector4(0.9f, 0.2f, 0.2f, 1f);
}
