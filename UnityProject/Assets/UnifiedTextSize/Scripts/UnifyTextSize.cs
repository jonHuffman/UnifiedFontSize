// MIT License

// Copyright(c) 2019 Jonathan Huffman

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnifiedTextSize.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace UnifiedTextSize
{
    public class UnifyTextSize : MonoBehaviour
    {
        [SerializeField, Tooltip("The absolute maximum font size your TextComponents can have")]
        private int maxFontSize = 250;
        [SerializeField]
        private List<Text> textComponents;

        private int currentSmallestSize;

        private Coroutine recalculateBestFitCoroutine;

        private void Awake()
        {
            currentSmallestSize = int.MaxValue;
        }

        private void Start()
        {
            RecalculateBestFitImmediately();
        }

        /// <summary>
        /// Resets the maximum possible best fit size and then on the next frame recalculates the smallest font.
        /// If you cannot wait a frame use <see cref="RecalculateBestFitImmediately"/>
        /// </summary>
        public void RecalculateBestFit()
        {
            if (recalculateBestFitCoroutine != null)
            {
                StopCoroutine(recalculateBestFitCoroutine);
            }

            recalculateBestFitCoroutine = StartCoroutine(RecalculateBestFitOverFrames());
        }

        /// <summary>
        /// Resets the maximum possible best fit size and then immediately recalculates the smallest font.
        /// Due to a Canvas rebuild requirement this is an expensive operation.
        /// </summary>
        /// <remarks>
        /// Currently <see cref="LayoutRebuilder"/> does do cause font size to be recalculated. Its possible this will change in future Unity versions.
        /// Last tested in Unity 2018.2.15
        /// </remarks>
        public void RecalculateBestFitImmediately()
        {
            ResetMaxFontSize();

            Canvas.ForceUpdateCanvases();

            UpdateFontSizes();
        }

        /// <summary>
        /// Adds a TextComponent to have its font size unified with others controlled by this component.
        /// TextComponents will have their font size lowered if need be.
        /// A larger overall font size may be supported, <see cref="RecalculateBestFit"/> if you wish to calculate this.
        /// </summary>
        public void AddText(Text newComponent)
        {
            Debug.AssertFormat(!textComponents.Contains(newComponent), "Adding duplicate entry for Text component {0}. You should avoid this.", newComponent.gameObject.name);

            textComponents.Add(newComponent);
            UpdateFontSizes();
        }

        private IEnumerator RecalculateBestFitOverFrames()
        {
            ResetMaxFontSize();

            yield return null;

            UpdateFontSizes();

            recalculateBestFitCoroutine = null;
        }

        private void ResetMaxFontSize()
        {
            foreach (Text text in textComponents)
            {
                text.resizeTextMaxSize = maxFontSize;
            }
        }

        private void UpdateFontSizes()
        {
            int smallestFontSize = GetSmallestFontSize();

            if (currentSmallestSize == smallestFontSize)
            {
                return;
            }

            foreach (Text text in textComponents)
            {
                text.resizeTextMaxSize = smallestFontSize;
            }
        }

        private int GetSmallestFontSize()
        {
            int smallestFontSize = int.MaxValue;
            
            foreach (Text text in textComponents)
            {
                int fontSize = text.GetCurrentFontSize();

                if (smallestFontSize > fontSize)
                {
                    smallestFontSize = fontSize;
                }
            }

            return smallestFontSize;
        }
    }
}
