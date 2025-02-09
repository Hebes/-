using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Serialization;

/// <summary>
/// 存档和加载
/// </summary>
public class SaveLoadSlot : MonoBehaviour
{
    public GameObject root;
    public RawImage previewImage;
    public TextMeshProUGUI titleText;
    public Button deleteButton;
    public Button saveButton;
    public Button loadButton;

    [SerializeField] private SaveAndLoadMenu saveAndLoadMenu;
    [SerializeField] private MainMenu mainMenu;

    [HideInInspector] public int fileNumber = 0;
    [HideInInspector] public string filePath = "";

    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.I;

    private void Awake()
    {
        root = this.gameObject;

        saveAndLoadMenu = transform.FindParentByName<SaveAndLoadMenu>("SaveAndLoadMenu");
        mainMenu = R.UITotalRoot.FindComponent<MainMenu>();

        previewImage = transform.Find("Preview Image").GetComponent<UnityEngine.UI.RawImage>();
        titleText = transform.Find("Description").GetComponent<TMPro.TextMeshProUGUI>();
        deleteButton = transform.Find("Options/Delete").GetComponent<UnityEngine.UI.Button>();
        loadButton = transform.Find("Options/Load").GetComponent<UnityEngine.UI.Button>();
        saveButton = transform.Find("Options/Save").GetComponent<UnityEngine.UI.Button>();
    }

    private void OnEnable()
    {
        deleteButton.onClick.AddListener(Delete);
        loadButton.onClick.AddListener(Load);
        saveButton.onClick.AddListener(Save);
    }

    private void OnDisable()
    {
        deleteButton.onClick.RemoveListener(Delete);
        loadButton.onClick.RemoveListener(Load);
        saveButton.onClick.RemoveListener(Save);
    }


    public void PopulateDetails(SaveAndLoadMenu.MenuFunction function)
    {
        if (File.Exists(filePath))
        {
            VNGameSave file = VNGameSave.Load(filePath);
            PopulateDetailsFromFile(function, file);
        }
        else
        {
            PopulateDetailsFromFile(function, null);
        }
    }

    private void PopulateDetailsFromFile(SaveAndLoadMenu.MenuFunction function, VNGameSave file)
    {
        if (file == null)
        {
            titleText.text = $"{fileNumber}. Empty File";
            deleteButton.gameObject.SetActive(false);
            loadButton.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(function == SaveAndLoadMenu.MenuFunction.Save);
            previewImage.texture = saveAndLoadMenu.emptyFileImage;
        }
        else
        {
            titleText.text = $"{fileNumber}. {file.timestamp}";
            deleteButton.gameObject.SetActive(true);
            loadButton.gameObject.SetActive(function == SaveAndLoadMenu.MenuFunction.Load);
            saveButton.gameObject.SetActive(function == SaveAndLoadMenu.MenuFunction.Save);

            byte[] data = File.ReadAllBytes(file.screenshotPath);
            Texture2D screenshotPreview = new Texture2D(1, 1);
            ImageConversion.LoadImage(screenshotPreview, data);
            previewImage.texture = screenshotPreview;
        }
    }

    private void OnConfirmDelete()
    {
        File.Delete(filePath);
        PopulateDetails(saveAndLoadMenu.menuFunction);
    }


    public void Delete()
    {
        var yes1 = new UIConfirmationMenu.ConfirmationButton("确定", OnConfirmDelete);
        var yes2 = new UIConfirmationMenu.ConfirmationButton("没关系", null);
        var no = new UIConfirmationMenu.ConfirmationButton("不", null);

        uiChoiceMenu.Show(
            //Title
            "删除此文件? (<i>这是无法挽回的!</i>)",
            //Choice 1
            new UIConfirmationMenu.ConfirmationButton("是", () => { uiChoiceMenu.Show("确定?", yes1, yes2); },
                autoCloseOnClick: false
            ),
            //Choice 2
            no
        );
    }

    public void Load()
    {
        VNGameSave file = VNGameSave.Load(filePath, false);
        saveAndLoadMenu.Close(closeAllMenus: true);

        if (R.currentScene == ConfigScene.MainMenu)
        {
            mainMenu.LoadGame(file);
        }
        else
        {
            file.Activate();
        }
    }

    public void Save()
    {
        if (R.HistorySystem.isViewingHistory)
        {
            string title = R.LanguageSystem.Get("查看历史记录时无法保存");
            string okay = R.LanguageSystem.Get("可以");
            UIConfirmationMenu.I.Show(title, new UIConfirmationMenu.ConfirmationButton(okay, null));
            return;
        }

        VNGameSave activeSave = VNGameSave.activeFile;
        activeSave.slotNumber = fileNumber;

        activeSave.Save();

        PopulateDetailsFromFile(saveAndLoadMenu.menuFunction, activeSave);
    }
}