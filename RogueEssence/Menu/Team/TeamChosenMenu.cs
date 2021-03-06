﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RogueElements;
using RogueEssence.Dungeon;
using RogueEssence.Data;
using RogueEssence.Ground;

namespace RogueEssence.Menu
{
    public class TeamChosenMenu : SingleStripMenu
    {

        private int teamSlot;

        public TeamChosenMenu(int teamSlot)
        {
            this.teamSlot = teamSlot;


            List<MenuTextChoice> choices = new List<MenuTextChoice>();

            choices.Add(new MenuTextChoice(Text.FormatKey("MENU_TEAM_SUMMARY"), SummaryAction));

            bool hasStatus = false;
            if (GameManager.Instance.CurrentScene == DungeonScene.Instance)
            {
                foreach (int status in ZoneManager.Instance.CurrentMap.Status.Keys)
                {
                    if (!ZoneManager.Instance.CurrentMap.Status[status].Hidden)
                    {
                        hasStatus = true;
                        break;
                    }
                }
                foreach (int status in DungeonScene.Instance.ActiveTeam.Players[teamSlot].StatusEffects.Keys)
                {
                    if (Data.DataManager.Instance.GetStatus(status).MenuName)
                    {
                        hasStatus = true;
                        break;
                    }
                }
            }

            choices.Add(new MenuTextChoice(Text.FormatKey("MENU_TEAM_STATUS_TITLE"), StatusAction, hasStatus, hasStatus ? Color.White : Color.Red));

            bool canAct = (GameManager.Instance.CurrentScene != DungeonScene.Instance) || (Data.DataManager.Instance.CurrentReplay == null) && (DungeonScene.Instance.FocusedCharacter == DungeonScene.Instance.ActiveTeam.Leader);

            choices.Add(new MenuTextChoice(Text.FormatKey("MENU_SHIFT_UP"), ShiftUpAction, canAct && (teamSlot > 0), canAct && (teamSlot > 0) ? Color.White : Color.Red));
            choices.Add(new MenuTextChoice(Text.FormatKey("MENU_SHIFT_DOWN"), ShiftDownAction, canAct && (teamSlot < DataManager.Instance.Save.ActiveTeam.Players.Count - 1), canAct && (teamSlot < DataManager.Instance.Save.ActiveTeam.Players.Count - 1) ? Color.White : Color.Red));

            if (teamSlot == DataManager.Instance.Save.ActiveTeam.LeaderIndex)
                choices.Add(new MenuTextChoice(Text.FormatKey("MENU_TEAM_MODE"), TeamModeAction, canAct, canAct ? Color.White : Color.Red));
            else
                choices.Add(new MenuTextChoice(Text.FormatKey("MENU_MAKE_LEADER"), MakeLeaderAction, canAct, canAct ? Color.White : Color.Red));

            bool canSendHome = canAct;
            if (DataManager.Instance.Save is RogueProgress && DataManager.Instance.GetSkin(DataManager.Instance.Save.ActiveTeam.Players[teamSlot].BaseForm.Skin).Challenge && !DataManager.Instance.Save.ActiveTeam.Players[teamSlot].Dead)
                canSendHome = false;
            if (GameManager.Instance.CurrentScene == DungeonScene.Instance && teamSlot != DataManager.Instance.Save.ActiveTeam.LeaderIndex)
                choices.Add(new MenuTextChoice(Text.FormatKey("MENU_TEAM_SEND_HOME"), SendHomeAction, canSendHome, canSendHome ? Color.White : Color.Red));


            choices.Add(new MenuTextChoice(Text.FormatKey("MENU_EXIT"), ExitAction));

            Initialize(new Loc(176, 16), CalculateChoiceLength(choices, 72), choices.ToArray(), 0);
        }

        private void SummaryAction()
        {
            MenuManager.Instance.AddMenu(new MemberFeaturesMenu(teamSlot, false, false), false);
        }

        private void StatusAction()
        {
            MenuManager.Instance.AddMenu(new StatusMenu(teamSlot), false);
        }

        private void ShiftUpAction()
        {
            MenuManager.Instance.RemoveMenu();

            MenuManager.Instance.NextAction = MoveCommand(new GameAction(GameAction.ActionType.ShiftTeam, Dir8.None, teamSlot - 1), teamSlot - 1);
        }

        private void ShiftDownAction()
        {
            MenuManager.Instance.RemoveMenu();

            MenuManager.Instance.NextAction = MoveCommand(new GameAction(GameAction.ActionType.ShiftTeam, Dir8.None, teamSlot), teamSlot + 1);
        }

        private void TeamModeAction()
        {
            MenuManager.Instance.ClearMenus();

            MenuManager.Instance.EndAction = DungeonScene.Instance.ProcessPlayerInput(new GameAction(GameAction.ActionType.TeamMode, Dir8.None));
        }

        private void MakeLeaderAction()
        {
            MenuManager.Instance.ClearMenus();

            MenuManager.Instance.EndAction = (GameManager.Instance.CurrentScene == DungeonScene.Instance) ? DungeonScene.Instance.ProcessPlayerInput(new GameAction(GameAction.ActionType.SetLeader, Dir8.None, teamSlot)) : GroundScene.Instance.ProcessInput(new GameAction(GameAction.ActionType.SetLeader, Dir8.None, teamSlot, 0));
        }

        private void SendHomeAction()
        {
            Character player = DataManager.Instance.Save.ActiveTeam.Players[teamSlot];
            MenuManager.Instance.AddMenu(MenuManager.Instance.CreateQuestion(Text.FormatKey("DLG_SEND_HOME_ASK", player.Name), () =>
            {
                MenuManager.Instance.RemoveMenu();
                List<IInteractable> save = MenuManager.Instance.SaveMenuState();

                MenuManager.Instance.ClearMenus();
                //send home
                MenuManager.Instance.EndAction = SendHomeEndAction(teamSlot, save);

            }, () => { }), false);
        }


        public IEnumerator<YieldInstruction> SendHomeEndAction(int teamSlot, List<IInteractable> save)
        {
            yield return CoroutineManager.Instance.StartCoroutine((GameManager.Instance.CurrentScene == DungeonScene.Instance) ? DungeonScene.Instance.ProcessPlayerInput(new GameAction(GameAction.ActionType.SendHome, Dir8.None, teamSlot)) : GroundScene.Instance.ProcessInput(new GameAction(GameAction.ActionType.SendHome, Dir8.None, teamSlot)));

            save[save.Count - 1] = new TeamMenu(false);
            save[save.Count - 1].BlockPrevious = true;

            if (GameManager.Instance.CurrentScene == DungeonScene.Instance)
                DungeonScene.Instance.PendingLeaderAction = MenuManager.Instance.LoadMenuState(save);
            else
                GroundScene.Instance.PendingLeaderAction = MenuManager.Instance.LoadMenuState(save);
        }

        private void ExitAction()
        {
            MenuManager.Instance.RemoveMenu();
        }



        public IEnumerator<YieldInstruction> MoveCommand(GameAction action, int switchSlot)
        {
            yield return CoroutineManager.Instance.StartCoroutine((GameManager.Instance.CurrentScene == DungeonScene.Instance) ? DungeonScene.Instance.ProcessPlayerInput(action) : GroundScene.Instance.ProcessInput(action));
            MenuManager.Instance.ReplaceMenu(new TeamMenu(false, switchSlot));
        }
    }
}
