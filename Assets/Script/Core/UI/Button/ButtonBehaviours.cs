using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehaviours : BaseBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static ButtonBehaviours selectedButton = null;
    public Animator anim;

    private const string Enter = "Enter";
    private const string Exit = "Exit";
    
    private void Awake()
    {
        anim = transform.FindComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectedButton != null && selectedButton != this)
            selectedButton.OnPointerExit(null);
        anim.Play(Enter);
        selectedButton = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.Play(Exit);
    }
}