using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace AsteroidRebuttal.Core
{
    public static class AudioManager
    {
        public static float masterVolume = 1;
        public static float musicVolume = 1;
        public static float sfxVolume = 1;

        public static List<SoundEffectQueueItem> sfxQueue = new List<SoundEffectQueueItem>();
		private static List<SoundEffectInstanceWrapper> currentSfx = new List<SoundEffectInstanceWrapper>();


        public static void PlaySong(Song thisSong, bool looping = true, float volumeMod = 1f)
        {
            MediaPlayer.Volume = masterVolume * musicVolume * volumeMod;
            MediaPlayer.IsRepeating = looping;
            MediaPlayer.Play(thisSong);
        }

        public static SoundEffectInstance PlaySong(SoundEffect thisSong, bool looping = true, float volumeMod = 1f)
        {
            SoundEffectInstance instance = thisSong.CreateInstance();
            instance.IsLooped = looping;
            instance.Volume = masterVolume * musicVolume * volumeMod;
            instance.Play();

            return instance;
        }

		public static void PlaySoundEffect(SoundEffect sfx, float volumeMod = 1f, float pitchMod = 0f, bool cancellable = true)
        {
            List<SoundEffectQueueItem> conflictingQueueItems = new List<SoundEffectQueueItem>(sfxQueue.FindAll(x => x.soundEffect == sfx));

            if (conflictingQueueItems.Count > 0)
            {
                // This sound effect has been queued already; update the volume if the requested volume is louder.
                if (volumeMod > conflictingQueueItems[0].soundEffectVolume)
                    conflictingQueueItems[0].SetVolume(volumeMod);
            }
            else
            {
                sfxQueue.Add(new SoundEffectQueueItem(sfx, volumeMod, pitchMod, cancellable));
            }
        }

		public static void CancelEarliestSoundEffect ()
		{
			SoundEffectInstanceWrapper toCancel = currentSfx.FindAll(x => x.Cancellable)[0];
			toCancel.Instance.Stop();
			currentSfx.Remove(toCancel);
		}

        public static void PlayQueuedSoundEffects()
        {
			// Remove all sound effects that have finished.
			currentSfx.RemoveAll(x => x.Instance.State == SoundState.Stopped);

			SoundEffectInstance sound;
			SoundEffectInstanceWrapper newSound;

            foreach(SoundEffectQueueItem sfx in sfxQueue)
            {
				try
				{
					if(currentSfx.Count >= 28)
						CancelEarliestSoundEffect();

					sound = sfx.soundEffect.CreateInstance();
					sound.Volume = masterVolume * sfx.soundEffectVolume * sfxVolume;
					sound.Pitch = sfx.soundEffectPitch;

					newSound = new SoundEffectInstanceWrapper(sound, sfx.cancellable);
					currentSfx.Add(newSound);

					newSound.Instance.Play();
				}
				catch(InstancePlayLimitException e)
				{
					Console.WriteLine("InstancePlayLimitException at " + currentSfx.Count + " instances.");
				}
            }

            sfxQueue.Clear();
        }

        public struct SoundEffectQueueItem
        {
            public SoundEffect soundEffect;
            public float soundEffectVolume;
            public float soundEffectPitch;
			public bool cancellable;

            public SoundEffectQueueItem(SoundEffect sfx, float volume, float pitch = 0f, bool canCancel = true)
            {
                soundEffect = sfx;
                soundEffectVolume = volume;
                soundEffectPitch = pitch;
				cancellable = canCancel;
            }

            public void SetVolume(float volume)
            {
                soundEffectVolume = volume;
            }
        }

		public class SoundEffectInstanceWrapper
		{
			public SoundEffectInstance Instance;
			public bool Cancellable = true;

			public SoundEffectInstanceWrapper(SoundEffectInstance sfx, bool canCancel)
			{
				Instance = sfx;
				Cancellable = canCancel;
			}

			public void Play ()
			{
				Instance.Play();
			}
		}
    }
}
