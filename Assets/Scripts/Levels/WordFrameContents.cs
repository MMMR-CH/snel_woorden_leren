using System;
using System.Collections.Generic;
using SWL;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SnelWoordenLeren.Levels
{
    public class WordFrameContents : MonoBehaviour
    {
        public struct InitData
        {
            public WoordData[] WoordData;
            public RectTransform WordDragArea;
            public Canvas Canvas;
            public HorizontalScrollSnap HorizontalScrollSnap;
            public Action<DraggableUIObject.DragContext> OnDraggedDirection;
            public Action<DraggableUIObject.DragContext> OnDraggedListener;
            public Action<DraggableUIObject.DragContext> OnDragStartedListener;
            public Action<DraggableUIObject.DragContext> OnDragEndedListener;
            public IBeginDragHandler[] ParentBeginDragHandler;
            public IDragHandler[] ParentDragHandler;
            public IEndDragHandler[] ParentEndDragHandler;
        }
        [SerializeField] private WordUIObject _wordObjectPrefab;
        private HorizontalScrollSnap _horizontalScrollSnap;
        float _spaceBetweenWords;
        RectTransform _wordDragArea;
        Canvas _canvas;
        List<WordUIObject> _wordObjects;

        [InspectorButtonAttribute("Test Init")]
        private  void TestInit()
        {
            WoordData[] testData = new WoordData[5];
            for (int i = 0; i < testData.Length; i++)
            {
                testData[i] = new WoordData { WOORD = "Test" + (i + 1) };
            }

            _wordDragArea = GetComponentInParent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            _horizontalScrollSnap = GetComponentInParent<HorizontalScrollSnap>();
            Init(new InitData
            {
                WoordData = testData,
                WordDragArea = _wordDragArea,
                Canvas = _canvas,
                HorizontalScrollSnap = _horizontalScrollSnap
            });
        }   

        public void Init(InitData initData)
        {
            // calculate space between words based on content width
            Vector2 wordSize = _wordObjectPrefab.GetComponent<RectTransform>().rect.size;
            float wordPivotPointX = _wordObjectPrefab.GetComponent<RectTransform>().pivot.x * wordSize.x;
            _spaceBetweenWords = (Screen.width - wordSize.x) * 0.35f;

            // Store references
            _wordDragArea = initData.WordDragArea;
            _canvas = initData.Canvas;
            _horizontalScrollSnap = initData.HorizontalScrollSnap;

            // Resize content rect transform based on the number of word objects, put some space between them
            RectTransform contentRect = GetComponent<RectTransform>();
            float sizeX = initData.WoordData.Length * wordSize.x + (initData.WoordData.Length - 1) * _spaceBetweenWords;
            sizeX += Screen.width; // Padding on both sides
            contentRect.sizeDelta = new Vector2(sizeX, contentRect.sizeDelta.y);
            contentRect.anchoredPosition = Vector2.zero;

            // Instantiate word objects based on the provided data
            _wordObjects = new List<WordUIObject>();
            for (int i = 0; i < initData.WoordData.Length; i++)
            {
                WordUIObject wordObject = Instantiate(_wordObjectPrefab, transform);
                RectTransform wordRect = wordObject.GetComponent<RectTransform>();
                wordObject.gameObject.name = initData.WoordData[i].WOORD;
                _wordObjects.Add(wordObject);
                wordObject.Init(new WordUIObject.InitData
                {
                    WoordData = initData.WoordData[i],
                    Canvas = _canvas,
                    DragAreaTransform = _wordDragArea,
                    OnDragDirection = initData.OnDraggedDirection,
                    OnDraggedListener = initData.OnDraggedListener,
                    OnDragStartedListener = initData.OnDragStartedListener,
                    OnDragEndedListener = initData.OnDragEndedListener,
                    ParentBeginDragHandler = initData.ParentBeginDragHandler,
                    ParentDragHandler = initData.ParentDragHandler,
                    ParentEndDragHandler = initData.ParentEndDragHandler       
                });
                _horizontalScrollSnap.AddChildItem(wordRect);

                // Position word object                
                wordRect.anchorMin = new Vector2(0, 0);
                wordRect.anchorMax = new Vector2(0, 0);
                wordRect.anchoredPosition = new Vector2(Screen.width * 0.5f + wordPivotPointX + (i * (wordSize.x + _spaceBetweenWords)), 0);
                //Debug.Log($"Positioned word object x: {_spaceBetweenWords * 0.5f + wordPivotPointX + (i * (wordSize.x + _spaceBetweenWords))}");
                //Debug.Log($"_spaceBetweenWords: {_spaceBetweenWords}, wordPivotPointX: {wordPivotPointX}, i: {i}, wordSize.x: {wordSize.x}");
            }


            _horizontalScrollSnap.SetSnappable(true);
        }
    }
}