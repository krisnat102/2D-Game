using UnityEngine;
using UnityEngine.EventSystems;

namespace Krisnat
{
    public class DragHandler : MonoBehaviour, IDragHandler
    {
        public RectTransform gridObject;

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerDrag == gridObject.gameObject)
                return;

            transform.position = Input.mousePosition;
        }
    }
}
