﻿using System.Collections.Generic;
using RogueElements;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueEssence.Content;

namespace RogueEssence.Menu
{
    public class RecordsMenu : SingleStripMenu
    {
        public RecordsMenu()
        {
            List<MenuTextChoice> choices = new List<MenuTextChoice>();
            if (DataManager.Instance.FoundRecords(DataManager.REPLAY_PATH))
                choices.Add(new MenuTextChoice(Text.FormatKey("MENU_REPLAYS_TITLE"), () => { MenuManager.Instance.AddMenu(new ReplaysMenu(), false); }));
            List<RecordHeaderData> scores = RecordHeaderData.LoadHighScores();
            if (scores.Count > 0)
            {
                choices.Add(new MenuTextChoice(Text.FormatKey("MENU_SCORES_TITLE"), () => { MenuManager.Instance.AddMenu(new ScoreMenu(scores, null), false); }));
            }

            if (DataManager.Instance.Save != null)
            {
                choices.Add(new MenuTextChoice(Text.FormatKey("MENU_DEX_TITLE"), () => { MenuManager.Instance.AddMenu(new DexMenu(), false); }));
                choices.Add(new MenuTextChoice(Text.FormatKey("MENU_DELETE_SAVE_TITLE"), DeleteAction));
            }

            Initialize(new Loc(16, 16), CalculateChoiceLength(choices, 72), choices.ToArray(), 0);

        }



        private void DeleteAction()
        {
            MenuManager.Instance.AddMenu(MenuManager.Instance.CreateDialogue(MonsterID.Invalid, null, new EmoteStyle(0), true, 
                () => {
                    MenuManager.Instance.AddMenu(MenuManager.Instance.CreateQuestion(MonsterID.Invalid, null, new EmoteStyle(0), Text.FormatKey("DLG_DELETE_CONFIRM"), true, () =>
                    {
                        MenuManager.Instance.ClearMenus();
                        DataManager.Instance.DeleteSaveData();
                        MenuManager.Instance.AddMenu(MenuManager.Instance.CreateDialogue(false, Text.FormatKey("DLG_DELETE_COMPLETE")), false);
                        MenuManager.Instance.EndAction = GameManager.Instance.FadeOut(false);
                        GameManager.Instance.SceneOutcome = GameManager.Instance.RestartToTitle();
                    }, () => { }, true), false);
                },
                -1, Text.FormatKey("DLG_DELETE_SAVE")), false);
        }
    }
}
