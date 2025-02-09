using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 画廊按钮
/// </summary>
public class GalleryMenu : MonoBehaviour
{
    public static GalleryMenu I;

    private const int PAGE_BUTTON_LIMIT = 2;
    private int maxPages = 0;
    private int selectedPage = 0;

    [SerializeField] private CanvasGroup root;
    private CanvasGroupController rootCG;

    [SerializeField] private Texture[] galleryImages;

    [SerializeField] private List<Button> galleryPreviewButtons;
    [SerializeField] private Button panelSelectionButtonPrefab;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    //预览
    [SerializeField] private CanvasGroup previewPanel;
    [SerializeField] Button previewButton;
    private CanvasGroupController previewPanelCG;

    private bool initialized = false;
    private int previewsPerPage => galleryPreviewButtons.Count;

    private void Awake()
    {
        I = this;
        root = transform.FindComponent<CanvasGroup>();
        transform.FindComponent("Gallery Images").FindChildList<Button>(ref galleryPreviewButtons);
        panelSelectionButtonPrefab = transform.FindComponentByName<Button>("Page");
        prevButton = transform.FindComponentByName<Button>("Back");
        nextButton = transform.FindComponentByName<Button>("Forward");

        previewPanel = transform.FindComponentByName<CanvasGroup>("Preview Panel");
        previewButton = transform.FindComponentByName<Button>("Preview Button");


        GetAllGalleryImages(); //加载全部的画廊图片
    }

    void Start()
    {
        rootCG = new CanvasGroupController(this, root);
        previewPanelCG = new CanvasGroupController(this, previewPanel);

        GalleryConfig.Load();

        var exitButton = transform.FindComponentByName<Button>("Exit Button");
        
        previewButton.onClick.AddListener(HidePreviewImage);
        exitButton.onClick.AddListener(Close);
        prevButton.onClick.AddListener(ToPreviousPage);
        nextButton.onClick.AddListener(ToNextPage);
    }

    public void Open()
    {
        if (!initialized)
            Initialize();

        rootCG.Show();
        rootCG.SetInteractableState(true);
    }

    public void Close()
    {
        rootCG?.Hide();
        rootCG.SetInteractableState(false);
    }

    private void GetAllGalleryImages()
    {
        galleryImages = R.LoadAll<Texture>(FilePaths.resources_gallery);
    }

    private void Initialize()
    {
        initialized = true;

        ConstructNavBar();

        LoadPage(1);
    }

    private void ConstructNavBar()
    {
        int totalImages = galleryImages.Length;

        maxPages = ((int)Mathf.Ceil((float)totalImages / previewsPerPage));
        int pagelimit = PAGE_BUTTON_LIMIT < maxPages ? PAGE_BUTTON_LIMIT : maxPages;

        for (int i = 1; i <= pagelimit; i++)
        {
            GameObject buttonOB = Instantiate(panelSelectionButtonPrefab.gameObject, panelSelectionButtonPrefab.transform.parent);
            buttonOB.SetActive(true);

            Button button = buttonOB.GetComponent<Button>();
            TextMeshProUGUI txt = button.GetComponentInChildren<TextMeshProUGUI>();

            buttonOB.name = i.ToString();
            int page = i;
            button.onClick.AddListener(() => LoadPage(page));
            txt.text = i.ToString();
        }

        prevButton.gameObject.SetActive(pagelimit < maxPages);
        nextButton.gameObject.SetActive(pagelimit < maxPages);

        nextButton.transform.SetAsLastSibling();
    }

    private void LoadPage(int pageNumber)
    {
        int startingIndex = (pageNumber - 1) * previewsPerPage;

        for (int i = 0; i < previewsPerPage; i++)
        {
            int index = i + startingIndex;
            Button button = galleryPreviewButtons[i];

            button.onClick.RemoveAllListeners();

            if (index >= galleryImages.Length)
            {
                button.transform.parent.gameObject.SetActive(false);
                continue;
            }
            else
            {
                button.transform.parent.gameObject.SetActive(true);
                RawImage renderer = button.targetGraphic as RawImage;
                Texture previewImage = galleryImages[index];

                if (GalleryConfig.ImageIsUnlocked(previewImage.name))
                {
                    renderer.color = Color.white;
                    renderer.texture = previewImage;
                    button.onClick.AddListener(() => ShowPreviewImage(previewImage));
                }
                else
                {
                    renderer.color = Color.black;
                    renderer.texture = null;
                }
            }
        }

        selectedPage = pageNumber;
    }

    private void ShowPreviewImage(Texture image)
    {
        RawImage renderer = previewButton.targetGraphic as RawImage;
        renderer.texture = image;
        previewPanelCG.Show();
        previewPanelCG.SetInteractableState(true);
    }

    public void HidePreviewImage()
    {
        previewPanelCG.Hide();
        previewPanelCG.SetInteractableState(false);
    }

    public void ToNextPage()
    {
        if (selectedPage < maxPages)
            LoadPage(selectedPage + 1);
    }

    public void ToPreviousPage()
    {
        if (selectedPage > 1)
            LoadPage(selectedPage - 1);
    }
}