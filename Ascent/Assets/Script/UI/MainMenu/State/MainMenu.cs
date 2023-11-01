using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State.Menu
{
    public class MainMenu : _MenuState
    {
        public override void InitState(MenuController menuController)
        {
            base.InitState(menuController);

            state = MenuController.MenuState.Main;
        }

        public void PlayGame()
        {
            menuController.PlayGame();
        }

        public void JumpToSettings()
        {
            menuController.SetActiveState(MenuController.MenuState.Settings);
        }

        public void JumpToControls()
        {
            menuController.SetActiveState(MenuController.MenuState.Controls);
        }

        public void JumpToCredits()
        {
            menuController.SetActiveState(MenuController.MenuState.Credits);
        }

        public void QuitGame()
        {
            menuController.QuitGame();
        }
    }
}
