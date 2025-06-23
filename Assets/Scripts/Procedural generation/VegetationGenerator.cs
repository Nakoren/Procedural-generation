using NUnit.Framework;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class VegetationGenerator : BaseVegetationGenerator
{
    private class PointData
    {
        public Vector2 position;
        public Vector2 positionInArea;
        public float value;
        public Biom biom;

        public PointData(Vector2 position, Vector2 pointInArea, float value, Biom biom)
        {
            this.position = position;
            this.positionInArea = pointInArea;
            this.value = value;
            this.biom = biom;
        }
    }

    
    private WhiteNoise m_whiteNoiseGenerator;

    public override int Seed
    {
        get { return seed; }
        set { 
            seed = value;
            if(m_whiteNoiseGenerator != null) m_whiteNoiseGenerator.Seed = value;
        }
    }
    private void Awake()
    {
        m_whiteNoiseGenerator = new WhiteNoise();
    }

    public override void ApplyVegetation(Terrain terrain, Vector2 offset, int size, Biom[,] biomMap)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector2 areaCenter = new Vector2(size * offset.x - offset.x, size * offset.y - offset.y);
        int halfSize = size / 2;
        float[,] terrainHeights = terrainData.GetHeights(0, 0, size, size);
        List<PointData> dataList = new List<PointData>();

        for (int x=0; x<size; x++)
        {
            for(int y = 0; y < size; y++)
            {
                if (terrainHeights[x,y] <= minVegetationHeight)
                {
                    continue;
                }

                Vector2 pointPosition = new Vector2(areaCenter.x + (x - halfSize), areaCenter.y + (y - halfSize));
                Biom pointBiom = biomMap[y,x];
                int chanceAtPoint = (int)(m_whiteNoiseGenerator.GetValueAtPoint(pointPosition) * 100);
                
                if (pointBiom.CheckPlantSpawn(chanceAtPoint))
                {
                    dataList.Add(new PointData(pointPosition, new Vector2(x,y), chanceAtPoint, pointBiom));
                }
            }
        }
        for (int i = 0; i < dataList.Count; i++)
        {
            PointData pointData = dataList[i];
            Biom curBiom = pointData.biom;

            bool bestInSorroundings = true;
            for (int j=0; j < dataList.Count; j++)
            {
                if (i == j) continue;
                PointData otherPointData = dataList[j];
                if(
                    (Vector2.Distance(pointData.position, otherPointData.position) < curBiom.vegetationIsolationRange)
                    &&
                    (pointData.value < otherPointData.value))
                {
                    bestInSorroundings = false;
                }
                if (!bestInSorroundings) break;
            }

            if (bestInSorroundings)
            {
                Vector3 spawnPosition = new Vector3(
                    pointData.positionInArea.x, 
                    terrainData.GetHeight((int)pointData.positionInArea.x, (int)pointData.positionInArea.y), 
                    pointData.positionInArea.y
                    );
                GameObject spawnObject = curBiom.GetRandomPlantAtPoint(pointData.position, seed);
                if (spawnObject == null) continue;
                GameObject newObject = Instantiate(spawnObject, terrain.gameObject.transform);
                newObject.transform.SetLocalPositionAndRotation(spawnPosition, Quaternion.identity);
            }
        }
    }
}
