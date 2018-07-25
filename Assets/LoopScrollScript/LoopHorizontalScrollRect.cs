using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Loop Horizontal Scroll Rect", 50)]
    [DisallowMultipleComponent]
    public class LoopHorizontalScrollRect : LoopScrollRect
    {
        protected override float GetSize(RectTransform item)
        {
            float size = contentSpacing;
            if (m_GridLayout != null)
            {
                size += m_GridLayout.cellSize.x;
            }
            else
            {
                size += LayoutUtility.GetPreferredWidth(item);
            }
            return size;
        }

        protected override float GetDimension(Vector2 vector)
        {
            return vector.x;
        }

        protected override Vector2 GetVector(float value)
        {
            return new Vector2(-value, 0);
        }

        protected override void Awake()
        {
            base.Awake();
            directionSign = 1;

            GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
            if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedRowCount)
            {
                Debug.LogError("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
            }
        }
        public override bool IsInViewport(RectTransform trans)
        {
            Bounds bounds = GetBounds(trans);
            if (m_ViewBounds.min.x < bounds.min.x && bounds.max.x < m_ViewBounds.max.x
                || m_ViewBounds.min.x < bounds.max.x && bounds.max.x < m_ViewBounds.max.x)
            {
                return true;
            }
            return false;
        }

        protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
        {
            bool changed = false;
            if (viewBounds.max.x > contentBounds.max.x)
            {
                float size = NewItemAtEnd();
                if (size > 0)
                {
                    if (threshold < size)
                    {
                        // Preventing new and delete repeatly...
                        threshold = size * 1.1f;
                    }
                    changed = true;
                }
            }
            else if (viewBounds.max.x < contentBounds.max.x - threshold)
            {
                float size = DeleteItemAtEnd();
                if (size > 0)
                {
                    changed = true;
                }
            }
            float contentMinX = contentBounds.min.x;
            if ((contentMinX > -0.1 && contentMinX < 0) || (contentMinX > 0 && contentMinX < 0.1))
            {
                contentMinX = 0;
            }
            if (viewBounds.min.x <= contentMinX)
            {
                if (movementType == MovementType.Clamped && viewBounds.min.x == contentMinX && m_Dragging)
                {
                    return changed;
                }
                float size = NewItemAtStart();
                if (size > 0)
                {
                    if (threshold < size)
                    {
                        threshold = size * 1.1f;
                    }
                    changed = true;
                }
            }
            else if (viewBounds.min.x > contentBounds.min.x + threshold)
            {
                float size = DeleteItemAtStart();
                if (size > 0)
                {
                    changed = true;
                }
            }
            return changed;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            horizontal = true;
            vertical = false;
            SetDirtyCaching();
        }
#endif
    }
}