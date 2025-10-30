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
		[SerializeField] private AudioClip specialDotClip;  // ��ħǥ ���� ����

		private float latestTimePlayed = -1f;
		private int clipIndex = 0;

		private void Awake()
		{
			Assert.IsNotNull(source, "AudioSource�� �������� �ʾҽ��ϴ�.");
			Assert.IsTrue(sounds != null && sounds.Length > 0, "�⺻ ���� �迭�� ��� �ֽ��ϴ�.");

			var typewriter = GetComponent<TypewriterCore>();
			Assert.IsNotNull(typewriter, "TypewriterCore�� ã�� �� �����ϴ�.");

			if (source == null || sounds.Length <= 0)
				return;

			source.playOnAwake = false;
			source.loop = false;

			typewriter.onCharacterVisible.AddListener(OnCharacter);

			clipIndex = randomSequence ? Random.Range(0, sounds.Length) : 0;
		}

		private void OnCharacter(char character)
		{
			// �ּ� ������ Ȯ��
			if (Time.time - latestTimePlayed <= minSoundDelay)
				return;

			AudioClip clipToPlay = null;

			// ���� ������ ���� �ٸ� ���� ����
			if (character == '.')
				clipToPlay = specialDotClip;        // ��ħǥ ����
			else
				clipToPlay = GetNextDefaultClip();  // �Ϲ� Ÿ���� ����

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
