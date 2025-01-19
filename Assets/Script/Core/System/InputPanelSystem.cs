/// <summary>
/// 玩家输入系统
/// </summary>
public class InputPanelSystem : SM<InputPanelSystem>
{

    public string LastInput => R.UISystem.UIPlayerInput.lastInput;
    public bool IsWaitingonUserInput => R.UISystem.UIPlayerInput.isWaitingOnUserInput;
    public void Show(string title)
    {
        R.UISystem.UIPlayerInput.Show(title);
    }

    public void Hide()
    {
        R.UISystem.UIPlayerInput.Hide();
    }
}