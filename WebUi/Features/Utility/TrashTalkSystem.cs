using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Client.Chat.Managers;
using Content.Shared.Chat;
using Content.Shared.Ghost;
using Content.Shared.Mobs;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;


[CompilerGenerated]
public sealed class TrashTalkSystem : EntitySystem
{
	public bool Enabled { get; set; }
	public override void Initialize()
	{
		base.Initialize();
		base.SubscribeLocalEvent<MobStateChangedEvent>(new EntityEventHandler<MobStateChangedEvent>(this.TrashTalkSend), null, null);
		this.LoadPhrases();
	}
	private void LoadPhrases()
	{
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		string text = Path.Combine(folderPath, "CerberusWare");
		string text2 = Path.Combine(text, "TrashTalkPhrases.txt");
		bool flag = !Directory.Exists(text);
		if (flag)
		{
			Directory.CreateDirectory(text);
		}
		bool flag2 = !File.Exists(text2);
		if (flag2)
		{
			File.WriteAllLines(text2, new string[]
			{
				"Ниже твоего кд только твой iq", "Купи себе ПК, хватит играть на компе из школьной библиотеки", "Назвать тебя дауном это комплимент, учитывая то, насколько ты на самом деле туп.", "В твоём теле около 37 миллионов клеток, и ты, прямо сейчас, разочаровываешь их всех.", "Если бы я спрыгнул с твоего чсв на твой iq я бы умер на полпути от голода.", "Учёные придумали число 0 когда подсчитали твои шансы сделать что-нибудь полезное.", "Ты из тех людей которые занимают третье место, играя 1 v 1", "Ты причина легализации абортов.", "Кто поставил сложность ботов на мирную?", "Я бы назвал тебя раком, но рак убивает.",
				"Я бы спросил сколько тебе лет, но я знаю что ты не умеешь считать до таких больших чисел.", "Некоторым  платят за оральный ceкc, но ты делаешь это бесплатно.", "Две ошибки всегда приводят к третьей. Твои родители наглядный тому пример.", "Я бы посоветовал тебе застрелиться, но уверен что ты промажешь.", "Посоветуй сайт где можно скинуться тебе на лечение.", "Дешевле тебя был только тот рваный гандон который использовал твой отец.", "Бог юморист: не веришь — посмотри на себя в зеркало.", "Любое сходство между тобой и человеком является чисто случайным.", "Твой плейстайл доказательство того, что мастурбация вызывает слепоту", "Сразу видно: мать не хотела, отец не старался.",
				"Я уверен что твоя дакимакура гордиться тобой", "Некоторых детей роняли на голову, но тебя явно кидали об стену.", "Даже если ты выстрелишь в землю, ты промажешь.", "Ты знаешь что акулы убивают 5 человек за год? Похоже у тебя есть серьёзные конкуренты.", "Выключи сску. Просто выйди на улицу и подойди к ближайшему дереву и извинись за то, что тратишь кислород.", "Тебя MEE6 роль не дал ибо даже за человека тебя не считает", "Извините за нецензурную брань. Но ты упал башкой об пол, 1, сукин сын", "Я твою \"Инв4лидную к0ляску\" \"КеРпИчЕм\" отхуярил", "упал минус мать", "nn4ik shat on",
				"a вы (you) сэр собственно кто (who)?", "ой а кто (who) ты (you) такой вотзефак мен))))))", "плиз скажи мне свой реальный никнейм, мне для медии надо)))", "нищий улетел", "*DEAD* пофикси нищ", "але найс упал нищ ЛОООООООЛ", "ой нищий упал щас скорую вызовем", "бля че тут эта нищая собака заскулила", "на мыло и веревку то деньги есть нищ????", "жаль конечно что против нищей играть надо)))",
				"не хотелось даже руки об тебя марать нищ сука", "БЛЯ НИЩ ХУЯК ХУЯК И ТЕБЯ НЕТ КАК МОЖНО ТАКИМ БЫТЬ?????? ОБЬЯСНИСЬ БЛЯТЬ", "с тобой там все хорошо????????????? а да ты нищ нахуя я спрашиваю ПХАХАХАХАХХА", "бля я рядом только прошел а ты уже упал АУУ НИЩ С ТОБОЙ ВСЕ ХОРОШО??????????))))))", "алло это скорая? тут такая ситуация нищ упал))) ОЙ А ВЫ НИЩАМ ТО НЕ ПОМОГАЕТЕ?? ПОНЯТНО Я ПОЙДУ ТОГДА))))))))", "спи", "спать", "на завод иди", "а вы че клины???", "ебать тебя унесло",
				"набутылирован лол", "рефандни пожалуйста", "ты че там отлетел то", "обоссал дауна лол", "прости что без смазки)))", "але а противники то где???", "бля пиздос может рефнешь???", "хуя тебя опустили манька))))", "сука не позорься и ливни лол", "как там жизнь с рупастой??????",
				"даун down, на завод нахуй", "насрал тебе в ротешник проверяй", "ебать ты красиво на бутылку упал", "научи потом как так сосать на хвх", "iq больше двух будет пмнешь ок????", "ты можешь заселлить лишнюю хромосому???", "как ты на пк накопил даже не знаю )))))))))", "тебе права голоса не давали thirdworlder ебаный", "когда не накопил на гормоны и чулки)))))))))))))", "нихуя ты там как самолет отлетел ХАХАХХАХАХАХАХХХААХАА",
				"опущены стяги, легион и.. А БЛЯТЬ ТЫЖ ТУТ ОПУЩ НАХУЙ ПХГАХААХАХАХАХА)))))))", "але я бот_кик в консоль вроде прописал а вас там не кикнуло эт че баг??)))))))))", "я не понял ты такой жирный потомучто дошики каждый день жрешь???? нормальную работу найди может на еду денег хватит))))))))))))", "Устал от того, что тебя постоянно овнят? CerberusWare прикупи nn4ik ах да я забыл ты нищий тебе даже на дошик не хватает, сочувствую)))))"
			});
		}
		this._phrases = new List<string>(File.ReadAllLines(text2));
	}
	private void TrashTalkSend(MobStateChangedEvent args)
	{
		bool flag = !CerberusConfig.Misc.TrashTalkEnabled;
		if (!flag)
		{
			bool flag2 = this._playerManager.LocalEntity == args.Target || base.HasComp<GhostComponent>(this._playerManager.LocalEntity);
			if (!flag2)
			{
				bool flag3 = args.NewMobState != (MobState)2 || args.OldMobState != (MobState)1;
				if (!flag3)
				{
					TimeSpan timeSpan;
					bool flag4 = this._cooldowns.TryGetValue(args.Target, out timeSpan) && this._timing.CurTime - timeSpan < TimeSpan.FromSeconds(1L);
					if (!flag4)
					{
						Random random = new Random();
						string text = this._phrases[random.Next(this._phrases.Count)];
						this._chatManager.SendMessage(text, (ChatSelectChannel)1);
						this._cooldowns[args.Target] = this._timing.CurTime;
					}
				}
			}
		}
	}
	
	private readonly IChatManager _chatManager = null;
	
	private readonly IGameTiming _timing = null;
	private readonly Dictionary<EntityUid, TimeSpan> _cooldowns = new Dictionary<EntityUid, TimeSpan>();
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	private List<string> _phrases = new List<string>();
}
