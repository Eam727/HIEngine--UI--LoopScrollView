using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Loop Vertical Scroll Rect", 55)]
    [DisallowMultipleComponent]
    public class LoopVerticalScrollRect : LoopScrollRect
    {
        protected override float GetSize(RectTransform item)
        {
            float size = contentSpacing;
            if (m_GridLayout != null)
            {
                size += m_GridLayout.cellSize.y;
            }
            else
            {
                size += LayoutUtility.GetPreferredHeight(item);
            }
            return size;
        }

        protected override float GetDimension(Vector2 vector)
        {
            return vector.y;
        }

        protected override Vector2 GetVector(float value)
        {
            return new Vector2(0, value);
        }

        protected override void Awake()
        {
            base.Awake();
            directionSign = -1;

            GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
            if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
            {
                Debug.LogError("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
            }
        }

        public override bool IsInViewport(RectTransform trans)
        {
            Bounds bounds = GetBounds(trans);
            if (m_ViewBounds.min.y < bounds.min.y && bounds.min.y < m_ViewBounds.max.y
                || m_ViewBounds.min.y < bounds.max.y && bounds.max.y < m_ViewBounds.max.y)
            {
                return true;
            }
            return false;
        }
        protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
        {
            bool changed = false;
            if (viewBounds.min.y < contentBounds.min.y + 1)
            {
                float size = NewItemAtEnd();
                if (size > 0)
                {
                    if (threshold < size)
                    {
                        threshold = size * 1.1f;
                    }
                    changed = true;
                }
            }
            else if (viewBounds.min.y > contentBounds.min.y + threshold)
            {
                float size = DeleteItemAtEnd();
                if (size > 0)
                {
                    changed = true;
                }
            }
            if (viewBounds.max.y > contentBounds.max.y - 1)
            {
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
            else if (viewBounds.max.y < contentBounds.max.y - threshold)
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
            horizontal = false;
            vertical = true;
            SetDirtyCaching();
        }
#endif
    }
}