using UnityEngine;
using UnityEngine.Assertions;
using Febucci.UI.Core;

namespace Febucci.UI.Examples
{
	[AddComponentMenu("Febucci/TextAnimator/CustomSoundWriter")]
	[RequireComponent(typeof(TypewriterCore))]
	public class CustomSoundWriter : MonoBehaviour
	{
		[SerializeField] private AudioSource source;

		[Header("General Settings")]
		[SerializeField, Min(0f)] private float minSoundDelay = 0.07f;
		[SerializeField] private bool interruptPreviousSound = true;
		[SerializeField] private bool randomSequence = false;
		[SerializeField] private AudioClip[] sounds = new AudioClip[0];

		[Header("Special Sounds")]
		[SerializeField] private AudioClip specialDotClip;  // 마침표 전용 사운드

		private float latestTimePlayed = -1f;
		private int clipIndex = 0;

		private void Awake()
		{
			Assert.IsNotNull(source, "AudioSource가 설정되지 않았습니다.");
			Assert.IsTrue(sounds != null && sounds.Length > 0, "기본 사운드 배열이 비어 있습니다.");

			var typewriter = GetComponent<TypewriterCore>();
			Assert.IsNotNull(typewriter, "TypewriterCore를 찾을 수 없습니다.");

			if (source == null || sounds.Length <= 0)
				return;

			source.playOnAwake = false;
			source.loop = false;

			typewriter.onCharacterVisible.AddListener(OnCharacter);

			clipIndex = randomSequence ? Random.Range(0, sounds.Length) : 0;
		}

		private void OnCharacter(char character)
		{
			// 최소 딜레이 확인
			if (Time.time - latestTimePlayed <= minSoundDelay)
				return;

			AudioClip clipToPlay = null;

			// 문자 종류에 따라 다른 사운드 선택
			if (character == '.')
				clipToPlay = specialDotClip;        // 마침표 전용
			else
				clipToPlay = GetNextDefaultClip();  // 일반 타이핑 사운드

			PlayClip(clipToPlay);
			latestTimePlayed = Time.time;
		}

		private AudioClip GetNextDefaultClip()
		{
			if (sounds.Length == 0)
				return null;

			var clip = sounds[clipIndex];
			if (randomSequence)
				clipIndex = Random.Range(0, sounds.Length);
			else
				clipIndex = (clipIndex + 1) % sounds.Length;

			return clip;
		}

		private void PlayClip(AudioClip clip)
		{
			if (clip == null)
				return;

			if (interruptPreviousSound)
			{
				source.clip = clip;
				source.Play();
			}
			else
			{
				source.PlayOneShot(clip);
			}
		}
	}
}
