using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HwigiTower.UI
{
    public sealed class PrototypeCutscenePlayer : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image image;
        [SerializeField] private Text text;

        private Coroutine _playRoutine;

        public bool IsPlaying => _playRoutine != null;

        public void Play(CutsceneData cutscene)
        {
            EnsureView();
            if (cutscene == null || !cutscene.HasSteps)
            {
                Hide();
                return;
            }

            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
            }

            _playRoutine = StartCoroutine(PlayRoutine(cutscene));
        }

        public void Hide()
        {
            EnsureView();
            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
                _playRoutine = null;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }
        }

        private IEnumerator PlayRoutine(CutsceneData cutscene)
        {
            var steps = cutscene.Steps;
            for (var i = 0; i < steps.Length; i++)
            {
                var step = steps[i];
                if (step == null)
                {
                    continue;
                }

                image.sprite = step.Image;
                image.preserveAspect = true;
                image.gameObject.SetActive(step.Image != null);
                text.text = step.TextKey;
                yield return FadeTo(1f, step.FadeInSeconds);
                yield return new WaitForSeconds(step.DurationSeconds);
                yield return FadeTo(0f, step.FadeOutSeconds);
            }

            Hide();
        }

        private IEnumerator FadeTo(float targetAlpha, float duration)
        {
            if (canvasGroup == null)
            {
                yield break;
            }

            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            if (duration <= 0f)
            {
                canvasGroup.alpha = targetAlpha;
                yield break;
            }

            var startAlpha = canvasGroup.alpha;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }

        private void EnsureView()
        {
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            }

            if (image == null)
            {
                var imageObject = new GameObject("Cutscene Image");
                imageObject.transform.SetParent(transform, false);
                var rect = imageObject.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.08f, 0.52f);
                rect.anchorMax = new Vector2(0.92f, 0.86f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                image = imageObject.AddComponent<Image>();
                image.raycastTarget = false;
            }

            if (text == null)
            {
                var textObject = new GameObject("Cutscene Text");
                textObject.transform.SetParent(transform, false);
                var rect = textObject.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.10f, 0.42f);
                rect.anchorMax = new Vector2(0.90f, 0.52f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                text = textObject.AddComponent<Text>();
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 24;
                text.alignment = TextAnchor.MiddleCenter;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Truncate;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 13;
                text.resizeTextMaxSize = 24;
                text.color = new Color(0.86f, 0.90f, 0.92f, 1f);
            }
        }
    }
}
