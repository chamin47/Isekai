using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UI_Extension
{
    /// <summary>
    /// �⺻���� Ŭ�� �̺�Ʈ�� �߰��մϴ�.
    /// </summary>
	public static void BindEvent(this GameObject go, Action<PointerEventData> action, UIEvent type = UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }
}
