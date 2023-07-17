using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace BrunoMikoski.DebugTools.GUI
{
    public class GraphGUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float maxGraphHeight = 420;
        [SerializeField]
        private float maxGraphWidth = 670;
        
        [Header("References")]
        [SerializeField]
        private UILineRenderer lineRenderer;

        [SerializeField]
        private TMP_Text xMediumText;
        [SerializeField]
        private TMP_Text xHighText;
        
        
        [SerializeField]
        private TMP_Text yMediumText;
        [SerializeField]
        private TMP_Text yHighText;

        [SerializeField]
        private string xFormat = "{0}";

        [SerializeField]
        private string yFormat = "{0} ms";


        private void Clear()
        {
            Array.Resize(ref lineRenderer.m_points, 0);
            xMediumText.text = "";
            xHighText.text = "";
            
            yMediumText.text = "";
            yHighText.text = "";
            
            lineRenderer.SetAllDirty();
        }


        public void SetPoints(List<Vector2> points)
        {
            Clear();
            float maxXValue = float.MinValue;
            float maxYValue = float.MinValue;
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 vector2 = points[i];
                if (vector2.y > maxYValue)
                    maxYValue = vector2.y;

                if (vector2.x > maxXValue)
                    maxXValue = vector2.x;
            }

            float averageX = maxXValue * 0.5f;
            float averageY = maxYValue * 0.5f;

            Array.Resize(ref lineRenderer.m_points, points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 performanceResult = points[i];

                float posX = Mathf.Lerp(0, maxGraphWidth, (float) i / points.Count);
                float posY = Mathf.Lerp(0, maxGraphHeight, performanceResult.y / maxYValue);
                
                lineRenderer.m_points[i] = new Vector2(posX, posY);
            }

            xMediumText.text = string.Format(xFormat, averageX);
            yMediumText.text = string.Format(yFormat, averageY);


            xHighText.text = string.Format(xFormat, maxXValue);
            yHighText.text = string.Format(yFormat, maxYValue);
            
            lineRenderer.SetAllDirty();
        }
        
    }
}