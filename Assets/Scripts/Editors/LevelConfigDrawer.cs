//using UnityEditor;
//using UnityEngine;
//using Game.Level;

//namespace MyEditors
//{
//    [CustomPropertyDrawer(typeof(LevelConfig))]
//    public class LevelConfigDrawer : PropertyDrawer
//    {
//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            EditorGUI.BeginProperty(position, label, property);

//            SerializedProperty id = property.FindPropertyRelative("id");
//            SerializedProperty levelRequirements = property.FindPropertyRelative("levelRequirements");
//            SerializedProperty colors = property.FindPropertyRelative("colors");
//            SerializedProperty rows = property.FindPropertyRelative("rows");
//            SerializedProperty columns = property.FindPropertyRelative("columns");
//            SerializedProperty serializedGrid = property.FindPropertyRelative("serializedGrid");

//            EditorGUILayout.LabelField($"Level ID: {id.intValue}", EditorStyles.boldLabel);

//            EditorGUILayout.PropertyField(levelRequirements, true);
//            EditorGUILayout.PropertyField(colors, true);

//            // Grid Size Fields
//            rows.intValue = EditorGUILayout.IntField("Rows", rows.intValue);
//            columns.intValue = EditorGUILayout.IntField("Columns", columns.intValue);

//            // Ensure serializedGrid is correctly sized
//            int newSize = rows.intValue * columns.intValue;
//            if (serializedGrid.arraySize != newSize)
//            {
//                serializedGrid.arraySize = newSize;
//            }

//            // Draw Grid
//            EditorGUILayout.LabelField("Grid Layout:");
//            for (int r = 0; r < rows.intValue; r++)
//            {
//                EditorGUILayout.BeginHorizontal();
//                for (int c = 0; c < columns.intValue; c++)
//                {
//                    int index = r * columns.intValue + c;
//                    if (index < serializedGrid.arraySize)
//                    {
//                        SerializedProperty cell = serializedGrid.GetArrayElementAtIndex(index);
//                        cell.boolValue = EditorGUILayout.Toggle(cell.boolValue, GUILayout.Width(20));
//                    }
//                }

//                EditorGUILayout.EndHorizontal();
//            }

//            property.serializedObject.ApplyModifiedProperties();
//            EditorGUI.EndProperty();
//        }
//    }
//}
