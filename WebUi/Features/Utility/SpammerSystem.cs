using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CerberusWareV3.Configuration;
using Content.Shared.Chat;
using Robust.Client.Player;
using Robust.Shared.Asynchronous;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

public sealed class SpammerSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IConsoleHost _console = default!;
    

    private readonly Random _random = new Random();
    private static readonly char[] Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

    public void StartSpamChat()
    {
        Task.Run(SpamChat);
    }

    public void StartSpamAHelp()
    {
        Task.Run(SpamAHelp);
    }

    public void AddChannel(ChatSelectChannel channel)
    {
        if (!CerberusConfig.Spammer.Channels.Contains((int)channel))
        {
            CerberusConfig.Spammer.Channels.Add((int)channel);
        }
    }

    public void RemoveChannel(ChatSelectChannel channel)
    {
        if (CerberusConfig.Spammer.Channels.Contains((int)channel))
        {
            CerberusConfig.Spammer.Channels.Remove((int)channel);
        }
    }

    private string GenerateProtectWord(int length = 6)
    {
        int num = CerberusConfig.Spammer.ProtectRandomLength ? _random.Next(3, 9) : length;
        char[] array = new char[num];
        for (int i = 0; i < num; i++)
        {
            array[i] = Chars[_random.Next(Chars.Length)];
        }
        return new string(array);
    }

    private async Task SpamChat()
    {
        while (CerberusConfig.Spammer.ChatEnabled)
        {
            var channelsToSpam = CerberusConfig.Spammer.Channels.ToList(); 
            foreach (var channel in channelsToSpam)
            {
                string message = CerberusConfig.Spammer.ProtectTextEnabled 
                    ? $"{CerberusConfig.Spammer.ChatText} \n{GenerateProtectWord(6)}" 
                    : CerberusConfig.Spammer.ChatText;
                
                IoCManager.Resolve<ITaskManager>().RunOnMainThread(() => 
                {
                    SendMessage(message, (ChatSelectChannel)channel);
                });

                await Task.Delay(CerberusConfig.Spammer.ChatDelay);
            }
        }
    }

    private async Task SpamAHelp()
    {
        while (CerberusConfig.Spammer.AHelpEnabled)
        {
            if (_playerManager.LocalUser != null) 
            {
                string message = CerberusConfig.Spammer.AHelpText;
                
                IoCManager.Resolve<ITaskManager>().RunOnMainThread(() =>
                {
                    _console.ExecuteCommand($"ahelp \"{CommandParsing.Escape(message)}\"");
                });

                await Task.Delay(CerberusConfig.Spammer.AHelpDelay);
            }
            else 
            {
                await Task.Delay(1000);
            }
        }
    }

    private void SendMessage(string text, ChatSelectChannel channel)
    {
        string command = "";
        switch (channel)
        {
            case ChatSelectChannel.Local:
                command = $"say \"{CommandParsing.Escape(text)}\"";
                break;
            case ChatSelectChannel.Whisper:
                command = $"whisper \"{CommandParsing.Escape(text)}\"";
                break;
            case ChatSelectChannel.Radio:
                command = $"say \";{CommandParsing.Escape(text)}\"";
                break;
            case ChatSelectChannel.LOOC:
                command = $"looc \"{CommandParsing.Escape(text)}\"";
                break;
            case ChatSelectChannel.OOC:
                command = $"ooc \"{CommandParsing.Escape(text)}\"";
                break;
            case ChatSelectChannel.Emotes:
                command = $"me \"{CommandParsing.Escape(text)}\"";
                break;
            case ChatSelectChannel.Dead:
                command = $"say \"{CommandParsing.Escape(text)}\"";
                break;
            case ChatSelectChannel.Admin:
                command = $"asay \"{CommandParsing.Escape(text)}\"";
                break;
            default:
                if (channel == (ChatSelectChannel)16384) 
                {
                    _console.ExecuteCommand(text);
                    return;
                }
                break;
        }

        if (!string.IsNullOrEmpty(command))
        {
            _console.ExecuteCommand(command);
        }
    }
}