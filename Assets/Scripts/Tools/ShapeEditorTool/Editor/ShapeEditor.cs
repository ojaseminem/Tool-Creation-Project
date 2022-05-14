using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

namespace Tools.ShapeEditorTool.Editor
{
    [CustomEditor(typeof(ShapeCreator))]
    public class ShapeEditor : UnityEditor.Editor
    {
        private ShapeCreator m_ShapeCreator;
        private SelectionInfo m_SelectionInfo;
        
        private bool m_NeedRepaint;
        
        private void OnSceneGUI()
        {
            Event guiEvent = Event.current;

            if (guiEvent.type == EventType.Repaint)
            {
                Draw();
            }
            else if (guiEvent.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
            else
            {
                HandleInput(guiEvent);
                
                if (m_NeedRepaint)
                {
                    HandleUtility.Repaint();
                }            
            }
        }
        
        void HandleInput(Event guiEvent )
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            float drawPlaneHeight = 0;
            float dstToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
            Vector3 mousePosition = mouseRay.GetPoint(dstToDrawPlane);

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
            {
                HandleLeftMouseDown(mousePosition);
            }

            if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
            {
                HandleLeftMouseUp(mousePosition);
            }

            if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 &&
                guiEvent.modifiers == EventModifiers.None)
            {
                HandleLeftMouseDrag(mousePosition);
            }
            
            UpdateMouseOverSelection(mousePosition);
        }

        void UpdateMouseOverSelection(Vector3 mousePos)
        {
            int mouseOverPointIndex = -1;
            for (int i = 0; i < m_ShapeCreator.points.Count; i++)
            {
                if (Vector3.Distance(mousePos, m_ShapeCreator.points[i]) < m_ShapeCreator.handleRadius)
                {
                    mouseOverPointIndex = 1;
                    break;
                }

                if (mouseOverPointIndex != m_SelectionInfo.pointIndex)
                {
                    m_SelectionInfo.pointIndex = mouseOverPointIndex;
                    m_SelectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;

                    m_NeedRepaint = true;
                }
            }
        }

        void Draw()
        {
            for (int i = 0; i < m_ShapeCreator.points.Count; i++)
            {
                Vector3 nextPoint = m_ShapeCreator.points[(i + 1) % m_ShapeCreator.points.Count];
                Handles.color = Color.black;
                Handles.DrawDottedLine(m_ShapeCreator.points[i], nextPoint, 4);

                Handles.color = i == m_SelectionInfo.pointIndex ? m_SelectionInfo.pointIsSelected?Color.black : Color.red : Color.white;
                Handles.DrawSolidDisc(m_ShapeCreator.points[i], Vector3.up, m_ShapeCreator.handleRadius);
            }

            m_NeedRepaint = false;
        }

        void HandleLeftMouseDown(Vector3 mousePos)
        {
            if (!m_SelectionInfo.mouseIsOverPoint)
            {
                Undo.RecordObject(m_ShapeCreator, "Add point");
                m_ShapeCreator.points.Add(mousePos);
                m_SelectionInfo.pointIndex = m_ShapeCreator.points.Count - 1;
            }

            m_SelectionInfo.pointIsSelected = true;
            m_NeedRepaint = true;
        }
        void HandleLeftMouseUp(Vector3 mousePos)
        {
            if (m_SelectionInfo.pointIsSelected)
            {
                m_SelectionInfo.pointIsSelected = false;
                m_SelectionInfo.pointIndex = -1;
                m_NeedRepaint = true;
            }
        }
        void HandleLeftMouseDrag(Vector3 mousePos)
        {
            if (m_SelectionInfo.pointIsSelected)
            {
                m_ShapeCreator.points[m_SelectionInfo.pointIndex] = mousePos;
                m_NeedRepaint = true;
            }
        }
        
        private void OnEnable()
        {
            m_ShapeCreator = target as ShapeCreator;
            m_SelectionInfo = new SelectionInfo();
        }

        public class SelectionInfo
        {
            public int pointIndex = -1;
            public bool mouseIsOverPoint;
            public bool pointIsSelected;
        }
    }
}
