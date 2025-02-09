/// <summary>
/// 帮助页面
/// </summary>
[NoDontDestroyOnLoad]
public class HelpPage : MenuPage
{
    private void Awake()
    {
        anim = transform.Find("Content").GetComponent<UnityEngine.Animator>();
        pageType = PageType.Help;
    }
}