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
        [SerializeField, Tooltip("The absolute minimum font size your TextComponents can have")]
        private int minFontSize = 8;
        [SerializeField, Tooltip("The absolute maximum font size your TextComponents can have")]
        private int maxFontSize = 250;

        [SerializeField]
        private bool unifyImmediately = true;
        [SerializeField]
        private List<Text> textComponents;

        private int currentSmallestSize;

        private Coroutine recalculateBestFitCoroutine;

        private void Awake()
        {
            currentSmallestSize = maxFontSize;
        }

        private void Start()
        {
            if (unifyImmediately)
            {
                RecalculateBestFitImmediately();
            }
            else
            {
                RecalculateBestFit();
            }
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
        /// Currently <see cref="LayoutRebuilder"/> does not cause font size to be recalculated. Its possible this will change in future Unity versions.
        /// Last tested in Unity 2018.2.15
        /// </remarks>
        public void RecalculateBestFitImmediately()
        {
            ResetFontSizes();

            Canvas.ForceUpdateCanvases();

            UpdateFontSizes();
        }

        /// <summary>
        /// Adds a Text Component to have its font size unified with others controlled by this component.
        /// TextComponents will have their font size lowered if need be.
        /// A larger overall font size may be supported, <see cref="RecalculateBestFit"/> if you wish to calculate this.
        /// </summary>
        public void AddTextComponent(Text newTextComponent)
        {
            Debug.AssertFormat(!textComponents.Contains(newTextComponent), "Adding duplicate entry for Text component {0}. You should avoid this.", newTextComponent.gameObject.name);

            SetSizeConstraints(newTextComponent, currentSmallestSize);
            textComponents.Add(newTextComponent);
            UpdateFontSizes();
        }

        /// <summary>
        /// Removes a Text component from the list of components managed by UnifyTextSize
        /// </summary>
        /// <returns>True if successfull removed, false if failed to remove or item not found</returns>
        public bool RemoveTextComponent(Text componentToRemove)
        {
            return textComponents.Remove(componentToRemove);
        }

        /// <summary>
        /// Removes all Text Components that are being managed by the unifier.
        /// They will retain their modified "resizeText" size settings, so it is up to you to reset them to values you find acceptable.  
        /// </summary>
        public void ClearManagedTextComponents()
        {
            textComponents.Clear();
        }

        private IEnumerator RecalculateBestFitOverFrames()
        {
            ResetFontSizes();

            yield return null;

            UpdateFontSizes();

            recalculateBestFitCoroutine = null;
        }

        private void ResetFontSizes()
        {
            foreach (Text text in textComponents)
            {
                text.resizeTextMinSize = minFontSize;
                text.resizeTextMaxSize = maxFontSize;
            }

            currentSmallestSize = maxFontSize;
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
                SetSizeConstraints(text, smallestFontSize);
            }

            currentSmallestSize = smallestFontSize;
        }

        private int GetSmallestFontSize()
        {
            int smallestFontSize = maxFontSize;

            foreach (Text text in textComponents)
            {
                int fontSize = text.GetCurrentFontSize();

                if (smallestFontSize > fontSize)
                {
                    smallestFontSize = fontSize >= minFontSize ? fontSize : minFontSize;
                }
            }

            return smallestFontSize;
        }

        private static void SetSizeConstraints(Text text, int smallestFontSize)
        {
            text.resizeTextMaxSize = smallestFontSize;

            if (smallestFontSize < text.resizeTextMinSize)
            {
                text.resizeTextMinSize = smallestFontSize;
            }
        }
    }
}
