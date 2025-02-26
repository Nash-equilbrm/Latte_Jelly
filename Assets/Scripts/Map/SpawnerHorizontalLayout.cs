using UnityEngine;


namespace Game.Map
{
    public class SpawnerHorizontalLayout : MonoBehaviour
    {
        public float spacing = 1.0f; // Khoảng cách giữa các GameObject con

        [ContextMenu("Arrange Active Children")]
        internal void Arrange()
        {
            Transform[] activeChildren = GetActiveChildren();
            int activeChildCount = activeChildren.Length;

            if (activeChildCount == 0) return;

            float totalWidth = (activeChildCount - 1) * spacing;
            Vector3 startPos = transform.position - new Vector3(totalWidth / 2, 0, 0);

            for (int i = 0; i < activeChildCount; i++)
            {
                activeChildren[i].position = startPos + new Vector3(i * spacing, 0, 0);
            }
        }

        private Transform[] GetActiveChildren()
        {
            int childCount = transform.childCount;
            Transform[] activeChildren = new Transform[childCount];
            int activeIndex = 0;

            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    activeChildren[activeIndex] = child;
                    activeIndex++;
                }
            }

            Transform[] result = new Transform[activeIndex];
            System.Array.Copy(activeChildren, result, activeIndex);
            return result;
        }
    }
}

