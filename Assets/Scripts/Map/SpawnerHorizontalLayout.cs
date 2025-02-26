using UnityEngine;


namespace Game.Map
{
    public class SpawnerHorizontalLayout : MonoBehaviour
    {
        public float spacing = 1.0f; // Khoảng cách giữa các GameObject con

        void Start()
        {
            Arrange();
        }

        [ContextMenu("Arrange Children")]
        private void Arrange()
        {
            int childCount = transform.childCount;
            if (childCount == 0) return;

            // Tính toán vị trí bắt đầu sao cho trung tâm là vị trí của cha
            float totalWidth = (childCount - 1) * spacing;
            Vector3 startPos = transform.position - new Vector3(totalWidth / 2, 0, 0);

            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.position = startPos + new Vector3(i * spacing, 0, 0);
            }
        }
    }
}

