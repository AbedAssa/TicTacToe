using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Controllers;
using Infra;

namespace Views
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class SymbolView : MonoBehaviour,ISymbolView
    {
        public static event Action<Position> OnSymbolButtonClicked;
        public Position Position { get; private set; }
        [SerializeField] private Button button;
        [SerializeField] private Image image;
        [SerializeField] private Sprite xMarkSprite;
        [SerializeField] private Sprite oMarkSprite;
        [SerializeField] private Animator animator;
        [SerializeField] private string symbolAppearAnimationName = "ScaleUp";
        private const int FlashAnimationCount = 4; 
        private const float FlashAnimationSpeedSeconds = 0.15f; 
        private void Start()
        {
            AddListeners();
            Reset();
        }

        /// <summary>
        /// Set symbol to the view
        /// </summary>
        /// <param name="symbol">Symbol to be displayed</param>
        public void DrawSymbol(Symbol? symbol)
        {
            SetSpriteBySymbol(symbol);
        }

        /// <summary>
        /// Set image sprite according to the given symbol, if symbol is null
        /// then turn image alpha to 0
        /// </summary>
        /// <param name="symbol">Symbol to be displayed</param>
        private void SetSpriteBySymbol(Symbol? symbol)
        {
            if (symbol == null)
            {
                image.sprite = null;
                SetImageAlpha(0f);
            }
            else
            {
                if (symbol.Value == Symbol.O)
                {
                    image.sprite = oMarkSprite;
                } 
                else if (symbol.Value == Symbol.X)
                {
                    image.sprite = xMarkSprite;
                }
                animator.SetTrigger(symbolAppearAnimationName);
                SetImageAlpha(1f);
                AudioController.Instance.PlaySound(AudioTypes.Symbol);
            }
        }

        /// <summary>
        /// The animation to be played when there is a winning combination.
        /// </summary>
        public void PlayWinningAnimation()
        {
            StartCoroutine(FlashAnimation());
        }
        
        /// <summary>
        /// Flash animation (turn sprite alpha on/of)
        /// </summary>
        private IEnumerator FlashAnimation()
        {
            int count = 0;
            while (count < FlashAnimationCount)
            {
                SetImageAlpha(0f);
                yield return new WaitForSeconds(FlashAnimationSpeedSeconds);
                SetImageAlpha(1f);
                yield return new WaitForSeconds(FlashAnimationSpeedSeconds);
                count++;
            }
        }

        public void HighlightView()
        {
            StartCoroutine(HighlightAnimation());
        }
        
        private IEnumerator HighlightAnimation()
        {
            image.color = Color.yellow;
            SetImageAlpha(0.5f);
            yield return new WaitForSeconds(0.3f);
            image.color = Color.white;
            SetImageAlpha(0f);
        }

        /// <summary>
        /// Reset the symbol view.
        /// </summary>
        public void Reset()
        {
            SetSpriteBySymbol(null);
        }

        /// <summary>
        /// Change the image alpha.
        /// </summary>
        /// <param name="alphaValue">alpha value</param>
        private void SetImageAlpha(float alphaValue)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.g, alphaValue);
        }

        /// <summary>
        /// Set position to the symbol view.
        /// </summary>
        /// <param name="symbolPosition">the position to be set</param>
        public void SetPosition(Position symbolPosition)
        {
            Position = symbolPosition;
        }

        /// <summary>
        /// Prevent or allow user perform a click on the symbol view.
        /// </summary>
        /// <param name="isInteractable">if ture the symbol is clickable</param>
        public void SetInteractable(bool isInteractable)
        {
            if (button == null)
            {
                return;
            }

            button.interactable = isInteractable;
        }
        
        /// <summary>
        /// The function get called when the button clicked and fires the symbol clicked event.
        /// </summary>
        private void SquareButtonClicked()
        {
            OnSymbolButtonClicked?.Invoke(Position);
        }
        
        private void AddListeners()
        {
            if (button != null)
            {
                button.onClick.AddListener(SquareButtonClicked);
            }
        }

        private void RemoveListeners()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(SquareButtonClicked);
            }
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }
    }
}