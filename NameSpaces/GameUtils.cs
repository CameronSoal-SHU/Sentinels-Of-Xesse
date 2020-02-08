using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtilities
{
    public class GameUtils
    {
        // Creating Text within the game world
        public static TextMesh CreateWorldText(string text, Transform parent = null,
            Vector3 localPosition = default, int fontSize = 40, Color color = default,
            TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 999)
        {
            if (color == null) color = Color.white;

            return CreateWorldText(parent, text, localPosition, fontSize, color, textAnchor, textAlignment, sortingOrder);
        }

        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize,
            Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;

            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return textMesh;
        }

        /// <summary>
        /// Retrieves the mouse position in world in co-ordinates
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetMouseWorldPosition()
        {
            // Store and correct the position with the camera's Z-Axis offset
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z -= Camera.main.transform.position.z;

            Vector3 vectorTemp = GetMouseWorldPositionWithZ(mousePosition, Camera.main);
            vectorTemp.z = 0;   // Ensure the z co-ord is 0 for 2D plane.

            return vectorTemp;
        }

        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPos, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPos);

            return worldPosition;
        }

        public static Vector2 GetWorldPositionCanvas(Vector3 worldPosition)
        {

            return RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
        }

        public static Vector2 GetWorldPositionCanvasWithZ(Vector3 worldPosition)
        {
            return GetWorldPositionCanvasWithZ(worldPosition, Camera.main);
        }

        public static Vector2 GetWorldPositionCanvasWithZ(Vector3 worldPosition, Camera worldCamera)
        {
            return RectTransformUtility.WorldToScreenPoint(worldCamera, worldPosition);
        }
    }
}
