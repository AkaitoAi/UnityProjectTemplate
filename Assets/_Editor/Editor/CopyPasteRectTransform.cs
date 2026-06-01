using UnityEngine;
using UnityEditor;

namespace AkaitoAi
{
    public static class CopyPasteRectTransform
    {

        private class RectTransformData
        {

            public Vector2 anchoredPosition;
            public Vector2 sizeDelta;
            public Quaternion rotation;
            public Vector2 anchorMin;
            public Vector2 anchorMax;
            public Vector2 pivot;

        }
        private static RectTransformData _data = null;

        [MenuItem("AkaitoAi/Edit/Copy RectTransform Values &c", false, -101)]
        public static void CopyTransformValues()
        {
            if (Selection.gameObjects.Length == 0) return;
            RectTransform _rectTransform = Selection.gameObjects[0].GetComponent<RectTransform>();
            if (_rectTransform)
            {

                _data = new RectTransformData();
                _data.anchoredPosition = _rectTransform.anchoredPosition;
                _data.sizeDelta = _rectTransform.sizeDelta;
                _data.rotation = _rectTransform.rotation;
                _data.anchorMin = _rectTransform.anchorMin;
                _data.anchorMax = _rectTransform.anchorMax;
                _data.pivot = _rectTransform.pivot;
            }
        }

        [MenuItem("AkaitoAi/Edit/Paste RectTransform Values &v", false, -101)]
        public static void PasteTransformValues()
        {

            if (_data != null)
                foreach (var selection in Selection.gameObjects)
                {

                    RectTransform selectionRT = selection.GetComponent<RectTransform>();

                    if (selectionRT)
                    {
                        Undo.RecordObject(selectionRT, "Paste RectTransform Values");
                        selectionRT.anchoredPosition = _data.anchoredPosition;
                        selectionRT.sizeDelta = _data.sizeDelta;
                        selectionRT.rotation = _data.rotation;
                        selectionRT.anchorMin = _data.anchorMin;
                        selectionRT.anchorMax = _data.anchorMax;
                        selectionRT.pivot = _data.pivot;
                    }
                }
        }
    }
}
