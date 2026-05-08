using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace HwigiTower.UI
{
    public sealed class PrototypeCutscenePlayer : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image scrim;
        [SerializeField] private Image frame;
        [SerializeField] private Image image;
        [SerializeField] private Text text;

        private Coroutine _playRoutine;

        public bool IsPlaying => _playRoutine != null;
        public event Action Finished;

        public void Play(CutsceneData cutscene)
        {
            gameObject.SetActive(true);
            EnsureView();
            if (cutscene == null || !cutscene.HasPlayableContent)
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
            var wasPlaying = _playRoutine != null;
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

            gameObject.SetActive(false);
            if (wasPlaying)
            {
                Finished?.Invoke();
            }
        }

        private IEnumerator PlayRoutine(CutsceneData cutscene)
        {
            var steps = cutscene.Steps;
            if (steps.Length == 0 && cutscene.FallbackSprite != null)
            {
                image.sprite = cutscene.FallbackSprite;
                image.preserveAspect = true;
                image.gameObject.SetActive(true);
                frame.gameObject.SetActive(true);
                text.text = string.Empty;
                yield return FadeTo(1f, 0.15f);
                yield return new WaitForSeconds(0.75f);
                yield return FadeTo(0f, 0.15f);
                Hide();
                yield break;
            }

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
                frame.gameObject.SetActive(step.Image != null);
                text.text = ShouldShowTextKey(step.TextKey) ? step.TextKey : string.Empty;
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

            canvasGroup.blocksRaycasts = targetAlpha > 0f || canvasGroup.alpha > 0.01f;
            canvasGroup.interactable = targetAlpha > 0f || canvasGroup.alpha > 0.01f;
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

            if (scrim == null)
            {
                var scrimObject = new GameObject("Cutscene Scrim");
                scrimObject.transform.SetParent(transform, false);
                var rect = scrimObject.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                scrim = scrimObject.AddComponent<Image>();
                scrim.color = new Color(0.01f, 0.015f, 0.02f, 0.72f);
                scrim.raycastTarget = true;
            }

            if (frame == null)
            {
                var frameObject = new GameObject("Cutscene Focus Frame");
                frameObject.transform.SetParent(transform, false);
                var rect = frameObject.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.12f, 0.58f);
                rect.anchorMax = new Vector2(0.88f, 0.88f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                frame = frameObject.AddComponent<Image>();
                frame.color = new Color(0.05f, 0.06f, 0.07f, 0.88f);
                frame.raycastTarget = false;
            }

            if (image == null)
            {
                var imageObject = new GameObject("Cutscene Image");
                imageObject.transform.SetParent(frame.transform, false);
                var rect = imageObject.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.04f, 0.06f);
                rect.anchorMax = new Vector2(0.96f, 0.94f);
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
                rect.anchorMin = new Vector2(0.14f, 0.47f);
                rect.anchorMax = new Vector2(0.86f, 0.56f);
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

        private static bool ShouldShowTextKey(string textKey)
        {
            return !string.IsNullOrEmpty(textKey) && !textKey.StartsWith("PLACEHOLDER_", System.StringComparison.Ordinal);
        }
    }
}
