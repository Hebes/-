/// <summary>
/// 选择面板系统
/// </summary>
public class ChoicePanelSystem : SM<ChoicePanelSystem>
{
    public void Show(string question, string[] choices)
    {
        R.UISystem.UIChoicePanel.Show(question, choices);
    }

    public void Hide()
    {
        R.UISystem.UIChoicePanel.Hide();
    }
}