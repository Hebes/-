using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 对话继续提示
/// </summary>
public class DialogueContinuePrompt : BaseBehaviour
{
    private RectTransform root;
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshProUGUI tmPro;
    public bool isShowing => anim.gameObject.activeSelf;

    private void Awake()
    {
        root = GetComponent<RectTransform>();
        anim = transform.Find("Arrow").GetComponent<UnityEngine.Animator>();
        tmPro = R.UITotalRoot.FindComponent<UIDialogue>().dialogueText;
    }

    public void Show()
    {
        if (tmPro.text == String.Empty)
        {
            if (isShowing)
                Hide();
            return;
        }

        tmPro.ForceMeshUpdate();
        anim.gameObject.SetActive(true);
        root.transform.SetParent(tmPro.transform);
        TMP_CharacterInfo finalCharacter = tmPro.textInfo.characterInfo[tmPro.textInfo.characterCount - 1];
        Vector3 targetPos = finalCharacter.bottomRight;
        float characterWidth = finalCharacter.pointSize * 0.5f;
        targetPos = new Vector3(targetPos.x + characterWidth, targetPos.y, 0);
        root.localPosition = targetPos;
    }

    public void Hide()
    {
        anim.gameObject.SetActive(false);
    }
}