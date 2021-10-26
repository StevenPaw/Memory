using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Memory
{
    public class MemoryCard : MonoBehaviour
    {
        [SerializeField] private Image cardImage;
        [SerializeField] private CardStates currentState = CardStates.NONE;
        [SerializeField] private int id = 0;
        [SerializeField] private Button buttonElement;

        [Header("Sprites")] 
        [SerializeField] private Sprite frontsideSprite;
        [SerializeField] private Sprite backsideSprite;

        [Header("Colors")] 
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color collectedColor;
        [SerializeField] private Color wrongColor;
        [SerializeField] private Image contentImage;
        [SerializeField] private Image foregroundImage;
        
        private CardManager cardManager;
        private Animator animator;

        public CardStates CurrentState
        {
            get => currentState;
            set => currentState = value;
        }
        
        private void Start()
        {
            cardManager = GameObject.FindWithTag(GameTags.CARDMANAGER).GetComponent<CardManager>();
            animator = GetComponent<Animator>();
            backsideSprite = cardImage.sprite;
            UpdateButton();
        }

        /// <summary>
        /// Sets the sprite and ID of card
        /// </summary>
        /// <param name="spriteIn">The Sprite that the card should display</param>
        /// <param name="idIn">The pair-ID for this card</param>
        public void SetImageAndID(Sprite spriteIn, int idIn)
        {
            frontsideSprite = spriteIn;
            id = idIn;
            currentState = CardStates.AVAILABLE;
            UpdateButton();
        }

        /// <summary>
        /// Gets the id of the card
        /// </summary>
        /// <returns>Id of the Card</returns>
        public int GetID()
        {
            return id;
        }

        /// <summary>
        /// Compares a given ID with the id of this card
        /// </summary>
        /// <param name="idIn"></param>
        /// <returns>if the IDs match</returns>
        public bool CompareID(int idIn)
        {
            return id == idIn;
        }

        public void UpdateButton()
        {
            switch (currentState)
            {
                default:
                    buttonElement.interactable = false;
                    foregroundImage.color = collectedColor;
                    contentImage.color = Color.black;
                    contentImage.sprite = backsideSprite;
                    break;
                case CardStates.NONE:
                    buttonElement.interactable = true;
                    foregroundImage.color = collectedColor;
                    contentImage.color = Color.white;
                    contentImage.sprite = backsideSprite;
                    break;
                case CardStates.AVAILABLE:
                    buttonElement.interactable = true;
                    foregroundImage.color = Color.white;
                    contentImage.color = Color.white;
                    contentImage.sprite = backsideSprite;
                    break;
                case CardStates.SELECTED:
                    buttonElement.Select();
                    buttonElement.interactable = false;
                    foregroundImage.color = selectedColor;
                    contentImage.color = Color.white;
                    contentImage.sprite = frontsideSprite;
                    break;
                case CardStates.COLLECTED:
                    buttonElement.interactable = false;
                    foregroundImage.color = collectedColor;
                    contentImage.color = Color.white;
                    contentImage.sprite = frontsideSprite;
                    break;
                case CardStates.DISPLAYINGWRONG:
                    buttonElement.interactable = true;
                    foregroundImage.color = wrongColor;
                    contentImage.color = wrongColor;
                    contentImage.sprite = frontsideSprite;
                    break;
            }
        }

        public void OnClick()
        {
            cardManager.SelectCard(this);
            UpdateButton();
        }
    }
}
