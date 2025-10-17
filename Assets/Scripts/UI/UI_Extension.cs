using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UI_Extension
{
    /// <summary>
    /// 기본으로 클릭 이벤트를 추가합니다.
    /// </summary>
	public static void BindEvent(this GameObject go, Action<PointerEventData> action, UIEvent type = UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }
}
