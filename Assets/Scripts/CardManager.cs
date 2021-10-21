using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Memory
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private int numberOfCards;
        [SerializeField] private string pathToImages;
        [SerializeField] private GameObject playfield;
        [SerializeField] private GameObject memoryCardPrefab;
        
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private TMP_Text levelText;
        
        [DllImport("__Internal")]
        private static extern void SendMessageToBrowser(string str);

        private List<MemoryCard> memoryCards = new List<MemoryCard>();

        private List<Sprite> images = new List<Sprite>();
        
        private MemoryCard selectedCard1 = null;
        private MemoryCard selectedCard2 = null;
        private bool displayingWrong;
        private int collectedPairs;

        private void Start()
        {
            LoadSprites();
            LoadCards();
            UpdateCounter();
        }

        private void LoadSprites()
        {
            Object[] loadedImages = Resources.LoadAll(pathToImages, typeof(Sprite));
            Debug.Log("Found " + loadedImages.Length + " Images");

            foreach (Sprite spr in loadedImages)
            {
                Debug.Log("Sprite: " + spr);
                images.Add(spr);
            }
            Debug.Log("Loaded " + images.Count + " Images");
        }

        private void LoadCards()
        {
            //Spawn each card
            for (int i = 0; i < numberOfCards / 2; i++)
            {
                Sprite pairImage = GetRandomSprite();
                int pairID = i;
                for (int j = 0; j < 2; j++)
                {
                    GameObject spawnedPrefab = Instantiate(memoryCardPrefab);
                    spawnedPrefab.transform.SetParent(playfield.transform, false);
                    MemoryCard spawnedMemoryCard = spawnedPrefab.GetComponent<MemoryCard>();
                    spawnedMemoryCard.SetImageAndID(pairImage, pairID);
                    memoryCards.Add(spawnedMemoryCard);
                }
            }
            
            //Randomize Cards
            Transform[] cards = playfield.transform.GetComponentsInChildren<Transform>();
            foreach (Transform trans in cards)
            {
                trans.SetSiblingIndex(Random.Range(0, cards.Length));
            }
        }

        private Sprite GetRandomSprite()
        {
            bool foundImage = false;
            Sprite selectedImage = images[0];
            int searches = 0;
            while (foundImage != true && searches < 100)
            {
                int selectedIndex = Random.Range(0, images.Count);
                selectedImage = images[selectedIndex];
                if (selectedImage != null)
                {
                    images[selectedIndex] = null;
                    foundImage = true;
                }

                searches += 1;
            }
            return selectedImage;
        }

        public void SelectCard(MemoryCard cardIn)
        {
            if (displayingWrong)
            {
                //If a false pair is selected, the cards are displayed wrong
                displayingWrong = false;
                selectedCard1.CurrentState = CardStates.AVAILABLE;
                selectedCard2.CurrentState = CardStates.AVAILABLE;
                selectedCard1.UpdateButton();
                selectedCard2.UpdateButton();
                selectedCard1 = null;
                selectedCard2 = null;
            }
            else
            {
                if (selectedCard1 == null)
                {
                    //If no other card is selected, select this one
                    selectedCard1 = cardIn;
                    selectedCard1.CurrentState = CardStates.SELECTED;
                    selectedCard1.UpdateButton();
                }
                else
                {
                    //If another card is selected, check if they are the same
                    selectedCard2 = cardIn;
                    if (selectedCard1.CompareID(selectedCard2.GetID()))
                    {
                        //Give Points if pair is correct
                        selectedCard1.CurrentState = CardStates.COLLECTED;
                        selectedCard2.CurrentState = CardStates.COLLECTED;
                        selectedCard1.UpdateButton();
                        selectedCard2.UpdateButton();
                        selectedCard1 = null;
                        selectedCard2 = null;
                        collectedPairs += 1;
                    }
                    else
                    {
                        //Display wrong cards if pair is false
                        selectedCard1.CurrentState = CardStates.DISPLAYINGWRONG;
                        selectedCard2.CurrentState = CardStates.DISPLAYINGWRONG;
                        selectedCard1.UpdateButton();
                        selectedCard2.UpdateButton();
                        displayingWrong = true;
                    }
                }
            }
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            pointsText.text = "Erreichte Punkte: " + collectedPairs * 10;
            levelText.text = "Level: 1";
            if (collectedPairs >= numberOfCards / 2)
            {
                //ToDo: Display Memory Finished
                //ToDo: Add Points

                foreach (MemoryCard mc in memoryCards)
                {
                    mc.UpdateButton();
                }
                
                #if UNITY_WEBGL
                    SendMessageToBrowser("Memory finished! Here are 60 points!");
                #endif
                
                Debug.Log("Memory finished!");
            }
        }
    }
}