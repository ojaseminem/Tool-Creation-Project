using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tools.ProceduralIslandGenerationTool
{
    public class ProceduralIslandGenerator : MonoBehaviour
    {
        public GameObject[] objectPrefabs;
        public GameObject container;

        public int rows;
        public int columns;

        public int numberOfSeeds;

        private List<Vector3> m_Seeds = new List<Vector3>();
        private List<int> m_ObjectPrefabIndex = new List<int>();

        private void Start()
        {
            CreateRandomPoints();
            StartCoroutine(GenerateIslands());
        }

        private void CreateRandomPoints()
        {
            for (int i = 0; i < numberOfSeeds; i++)
            {
                Vector3 randomPosition = new Vector3(Random.Range(0, rows), 0, Random.Range(0, columns));
                m_Seeds.Add(randomPosition);

                int randomObjectNumber = Random.Range(0, objectPrefabs.Length);
                m_ObjectPrefabIndex.Add(randomObjectNumber);

                GameObject newObject =
                    Instantiate(objectPrefabs[randomObjectNumber], randomPosition, Quaternion.Euler(90,0,0));
                newObject.transform.parent = container.transform;
            }
        }

        private int FindClosestPoint(Vector3 point)
        {
            int closestPointIndex = 0;
            var distance = Vector3.Distance(point, m_Seeds[Random.Range(0, m_Seeds.Count)]);

            for (int i = 0; i < m_Seeds.Count; i++)
            {
                var tempDistance = Vector3.Distance(point, m_Seeds[Random.Range(0, m_Seeds.Count)]);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    closestPointIndex = i;
                }
            }
            return closestPointIndex;
        }

        IEnumerator GenerateIslands()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    yield return new WaitForSeconds(.1f);
                    Vector3 point = new Vector3(row, 0, column);

                    if (!m_Seeds.Contains(point))
                    {
                        int closestPointIndex = FindClosestPoint(point);

                        GameObject newObject = Instantiate(objectPrefabs[m_ObjectPrefabIndex[closestPointIndex]], point,
                            Quaternion.Euler(90, 0, 0));
                        newObject.transform.parent = container.transform;
                    }
                }
            }
        }
    }
}