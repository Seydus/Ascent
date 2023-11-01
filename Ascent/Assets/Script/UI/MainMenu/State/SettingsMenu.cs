namespace State.Menu
{
    public class SettingsMenu : _MenuState
    {
        //Specific for this state
        public override void InitState(MenuController menuController)
        {
            base.InitState(menuController);

            state = MenuController.MenuState.Settings;
        }
    }
}