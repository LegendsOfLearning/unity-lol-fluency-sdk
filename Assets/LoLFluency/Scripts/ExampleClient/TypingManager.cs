using System.Collections;
using System.Diagnostics;
using LoL.Fluency;
using UnityEngine;
using UnityEngine.UI;

namespace SomeDev.Test
{
    public class TypingManager : MonoBehaviour
    {
        [SerializeField] Text title;
        [SerializeField] Text text;
        [SerializeField] InputField numInput;

        int _correctCount;
        int _currentNum;
        Stopwatch _timer;
        WaitForSeconds _resetWait = new WaitForSeconds(0.5f);

        TypingData _typingStartData;

        void Start ()
        {
            _typingStartData = LoLFluencySDK.GetStartData<TypingData>();
            title.text = LoLFluencySDK.GetLanguageText("inTyping", "IN TYPING");
            numInput.onValueChanged.AddListener(ValueChanged);
            _timer = new Stopwatch();
            SetNumber();
        }

        void SetNumber ()
        {
            _currentNum = GetRandomNumber();
            text.text = _currentNum.ToString();
            numInput.SetTextWithoutNotify(string.Empty);
            numInput.interactable = true;
            numInput.GetComponent<Image>().color = Color.white;
            numInput.Select();
            numInput.ActivateInputField();
            _timer.Restart();
        }

        int GetRandomNumber ()
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            // Alternate between low range and high range numbers for typing test.
            // This will test single and double digit typing.
            var inLowerRange = _currentNum < _typingStartData.endRange / 2;
            // in lower, start from mid, else start from beginning.
            var start = inLowerRange
                ? _typingStartData.endRange / 2
                : 0;
            // in lower, end at end, else end at mid.
            var end = inLowerRange
                ? _typingStartData.endRange + 1
                : _typingStartData.endRange / 2;

            var num = Random.Range(start, end);
            while (_currentNum == num)
            {
                num = Random.Range(start, end);
            }
            return num;
        }

        void ValueChanged (string value)
        {
            var currentNumStr = _currentNum.ToString();
            var length = Mathf.Min(value.Length, currentNumStr.Length);
            for (int i = 0; i < length; i++)
            {
                if (currentNumStr[i] != value[i])
                {
                    WaitThenReset(false);
                    return;
                }
            }

            if (length != currentNumStr.Length)
            {
                return;
            }

            var elapsed = (int)_timer.ElapsedMilliseconds;

            // Only count entries that are under the threshold but show correct.
            if (elapsed > _typingStartData.latencyThresholdMs)
            {
                WaitThenReset(true);
                return;
            }

            LoLFluencySDK.AddResult(
                _currentNum,
                _currentNum,
                FluencyFactOperation.MATCH,
                _currentNum,
                System.DateTime.UtcNow,
                elapsed);

            _correctCount++;

            if (_correctCount >= _typingStartData.minCorrect)
            {
                text.text = "COMPLETE";
                numInput.interactable = false;
                LoLFluencySDK.SendResults();
                return;
            }

            WaitThenReset(true);
        }

        void WaitThenReset (bool correct)
        {
            StartCoroutine(_WaitThenReset(correct));
        }

        IEnumerator _WaitThenReset (bool correct)
        {
            numInput.interactable = false;
            numInput.GetComponent<Image>().color = correct ? Color.green : Color.red;
            yield return _resetWait;
            SetNumber();
        }
    }
}