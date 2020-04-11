using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Utils.Async;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Scripts.Services
{
    [Serializable]
    public class SceneManagementService : IServices
    {
        private enum State
        {
            Inactive,
            FadeIn,
            WaitingSceneForLoad,
            FadingOut
        }

        private const float EPSILON = 0.001f;
        private const float ONE_EPSILON = 1.0f - EPSILON;

        private IReadOnlyDictionary<State, Action> AsSwitch
        {
            get
            {
                if (asSwitch == null)
                {
                    asSwitch = new Dictionary<State, Action>()
                    {
                        {State.Inactive, ()=>{}},
                        {State.FadeIn, OnFadeIn},
                        {State.WaitingSceneForLoad, OnWaitingSceneForLoad},
                        {State.FadingOut, OnFadeOut},
                    };
                }
                return asSwitch;
            }
        }

        public IReadOnlyList<string> Scenes
        {
            get
            {
                return scenes;
            }
        }

        [SerializeField] private VideoPlayer m_video = null;
        [SerializeField] private Camera m_camera = null;
        [SerializeField] private List<string> scenes = null;

        private State m_state;
        private IReadOnlyDictionary<State, Action> asSwitch = null;
        private IReadOnlyList<GameObject> m_hideTheseWhenActivated;
        private AsyncOperation m_asyncOperation;
        private string m_nextScene;
        private float m_fade;
        private float currentTime = 0;

        private SceneManagementService() { }

        private SceneManagementService(VideoPlayer m_video, List<string> scenes)
        {
            this.m_video = m_video;
            this.scenes = scenes;
            m_state = State.Inactive;
        }
        void IServices.Init()
        {

        }

        public bool LoadSceneWithVideo(int nextScene,
                                       IReadOnlyList<GameObject> hideInTransition = null,
                                       float fadeTime = 1.0f)
        {
            if (nextScene >= 0 && nextScene < scenes.Count)
            {
                return LoadSceneWithVideo(scenes[nextScene], hideInTransition, fadeTime);
            }
            return false;
        }

        public bool LoadSceneWithVideo(string nextScene,
                                       IReadOnlyList<GameObject> hideInTransition = null,
                                       float fadeTime = 1.0f)
        {
            if (m_state != State.Inactive)
            {
                return false;
            }
            currentTime = 0;
            m_nextScene = nextScene;
            m_video.enabled = true;
            m_fade = 1 / (fadeTime * 0.5f);
            m_video.isLooping = true;
            m_camera.enabled = true;
            m_video.targetCamera = m_camera;

            m_video.targetCameraAlpha = 0.0f;
            m_video.isLooping = true;
            m_video.Play();
            m_state = State.FadeIn;
            m_hideTheseWhenActivated = hideInTransition;

            ThreadTools.StartCoroutine(UpdateCoroutine());
            return true;
        }

        private IEnumerator UpdateCoroutine()
        {
            while (m_state != State.Inactive)
            {
                yield return null;
                AsSwitch[m_state]();
            }
        }

        private void OnFadeIn()
        {
            currentTime += Time.deltaTime;
            m_video.targetCameraAlpha = Mathf.Lerp(0, 1.0f, currentTime * m_fade);
            if (m_video.targetCameraAlpha >= ONE_EPSILON)
            {
                if (m_hideTheseWhenActivated != null)
                {
                    for (int i = 0; i < m_hideTheseWhenActivated.Count; i++)
                    {
                        m_hideTheseWhenActivated[i].SetActive(false);
                    }
                }
                m_hideTheseWhenActivated = null;
                m_state = State.WaitingSceneForLoad;
                m_asyncOperation = SceneManager.LoadSceneAsync(m_nextScene);
                m_asyncOperation.allowSceneActivation = false;
            }
        }

        private void OnWaitingSceneForLoad()
        {
            if (m_asyncOperation.isDone || m_asyncOperation.progress >= 0.9f)
            {
                m_asyncOperation.allowSceneActivation = true;
                m_state = State.FadingOut;
                currentTime = 0;
            }
        }

        private void OnFadeOut()
        {
            currentTime += Time.deltaTime;
            m_video.targetCameraAlpha = Mathf.Lerp(1.0f, 0.0f, currentTime * m_fade);
            if (m_video.targetCameraAlpha <= EPSILON)
            {
                m_video.targetCameraAlpha = 0;
                m_video.Stop();
                m_video.enabled = false;
                m_camera.enabled = false;
                m_state = State.Inactive;
            }
        }
    }
}