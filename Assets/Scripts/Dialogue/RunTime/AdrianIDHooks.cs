using System.Collections;
using UnityEngine;

public class AdrianIDHooks : MonoBehaviour, IDialogueHookProvider
{
	public IEnumerator OnPreEnter(string id)
	{
		switch (id)
		{
			// 영애
			case "3001002": 
				yield return PlayRepeat("ladies_jump", 3, 0.2f); 
				yield break;
			case "3001007": Managers.Sound.Play("lady1", Sound.Effect);
				yield break;
			case "3001016":
				Managers.Sound.Play("lady2", Sound.Effect);
				yield break;
			case "3001022": Managers.Sound.Play("lady3", Sound.Effect);
				yield break;
			case "3001047": Managers.Sound.Play("lady3_thump", Sound.Effect); 
				yield break;
			case "3001033": Managers.Sound.Play("lady1_choice", Sound.Effect);
				yield break;
			case "3001038": Managers.Sound.Play("lady2_choice", Sound.Effect);
				yield break;
			case "3001044": Managers.Sound.Play("lady3_choice", Sound.Effect);
				yield break;

			// 가난한 아이
			case "3001057-1": Managers.Sound.Play("accept_gift", Sound.Effect); 
				yield break;
			case "3001060": Managers.Sound.Play("lukis_poop", Sound.Effect); 
				yield break;
			case "3001066-1": Managers.Sound.Play("not_accept_gift", Sound.Effect); 
				yield break;
			case "3001069": Managers.Sound.Play("twinkle", Sound.Effect); 
				yield break;

			// 상인
			case "3001130-1": Managers.Sound.Play("thunder", Sound.Effect); 
				yield break;

			// 꽃 상인 
			case "3001074": Managers.Sound.Play("give_flower", Sound.Effect); 
				yield break;
			case "3001086":
				Managers.Sound.Play("happiness_effect", Sound.Effect);
				yield break;

			// 화가
			case "3001101-1": Managers.Sound.Play("move_impact", Sound.Effect); 
				yield break;
			case "3001111": Managers.Sound.Play("funny_impact", Sound.Effect); 
				yield break;

			// 학자
			case "3001124": Managers.Sound.Play("notice_effect", Sound.Effect); 
				yield break;

			default: yield break;
		}
	}

	private IEnumerator PlayRepeat(string key, int count, float interval)
	{
		for (int i = 0; i < count; i++)
		{
			Managers.Sound.Play(key, Sound.Effect);
			yield return new WaitForSeconds(interval);
		}
	}
}
