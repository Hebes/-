using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 保存和加载菜单
/// </summary>
[NoDontDestroyOnLoad]
public class SaveAndLoadMenu : MenuPage
{
    public const int MAX_FILES = 99;
    private string savePath = FilePaths.gameSaves;
    private int currentPage = 1;
    private bool loadedFilesForFirstTime = false;
    public MenuFunction menuFunction = MenuFunction.Save;
    public SaveLoadSlot[] saveSlots;
    public int slotsPerPage => saveSlots.Length;
    public Texture emptyFileImage;

    private void Awake()
    {
        anim = transform.Find("Content").GetComponent<UnityEngine.Animator>();

        List<SaveLoadSlot> saveSlots = new List<SaveLoadSlot>();
        transform.FindChildList<SaveLoadSlot>(ref saveSlots);
        this.saveSlots = saveSlots.ToArray();

        pageType = PageType.SaveAndLoad;

        emptyFileImage = R.Load<Texture>("Graphics/UI/NewGame");
    }

    public override void Open()
    {
        base.Open();

        if (!loadedFilesForFirstTime)
            PopulateSaveSlotsForPage(currentPage);
    }

    public void PopulateSaveSlotsForPage(int pageNumber)
    {
        currentPage = pageNumber;
        int startingFile = ((currentPage - 1) * slotsPerPage) + 1;
        int endingFile = startingFile + slotsPerPage - 1;

        for (int i = 0; i < slotsPerPage; i++)
        {
            int fileNum = startingFile + i;
            SaveLoadSlot slot = saveSlots[i];

            if (fileNum <= MAX_FILES)
            {
                slot.root.SetActive(true);
                string filePath = $"{FilePaths.gameSaves}{fileNum}{VNGameSave.FILE_TYPE}";
                slot.fileNumber = fileNum;
                slot.filePath = filePath;
                slot.PopulateDetails(menuFunction);
            }
            else
            {
                slot.root.SetActive(false);
            }
        }
    }

    public enum MenuFunction
    {
        Save,
        Load
    }
}