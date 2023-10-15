using System.Collections.Generic;
using TD3D.Core.Runtime;
using UnityEditor;
using UnityEngine;

namespace TD3D.Core.Editor {
    [CustomEditor(typeof(CurveCreator))]
    public class CurveEditor : UnityEditor.Editor {
        private CurveCreator m_curveCreator;
        private BezierCurve m_curve;
        
        private bool m_selectionFoldoutGroup;
        private AnchorOptions m_currAnchorOptions;

        private void ResetCurve() {
            m_curveCreator.curve = new BezierCurve(new List<Vector3> {
                new(-1f, 1f, 0f), new(0f, 1f, 0f),
                new(0f, -1f, 0f), new(1f, -1, 0f)
            });
            
            m_curve = m_curveCreator.curve;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            m_selectionFoldoutGroup = EditorGUILayout.BeginFoldoutHeaderGroup(m_selectionFoldoutGroup, "Anchor Selection Options");
            if (m_selectionFoldoutGroup) {
                m_currAnchorOptions.dimensionLockValues = EditorGUILayout.Vector3Field("Dimension Locks", 
                                                                                       m_currAnchorOptions.dimensionLockValues);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Lock:");
                var newLockX = EditorGUILayout.Toggle(m_currAnchorOptions.lockX);
                var newLockY = EditorGUILayout.Toggle(m_currAnchorOptions.lockY);
                var newLockZ = EditorGUILayout.Toggle(m_currAnchorOptions.lockZ);
                if (newLockX != m_currAnchorOptions.lockX || newLockY != m_currAnchorOptions.lockX ||
                    newLockZ != m_currAnchorOptions.lockZ) {
                    
                }
                m_currAnchorOptions.lockX = newLockX;
                m_currAnchorOptions.lockY = newLockY;
                m_currAnchorOptions.lockZ = newLockZ;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                /*
                EditorGUILayout.BeginHorizontal();
                DrawAnchorLockOption(ref m_currAnchorOptions.dimensionLockValues.x, ref m_currAnchorOptions.lockX, "x");
                DrawAnchorLockOption(ref m_currAnchorOptions.dimensionLockValues.y, ref m_currAnchorOptions.lockY, "y");
                DrawAnchorLockOption(ref m_currAnchorOptions.dimensionLockValues.z, ref m_currAnchorOptions.lockZ, "z");
                EditorGUILayout.EndHorizontal();
                */
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
            //if (GUILayout.Button("Reset Curve"))
            //    ResetCurve();
        }

        private void DrawAnchorLockOption(ref float dimensionValue, ref bool lockState, string label) {
            EditorGUILayout.BeginVertical();
            dimensionValue = EditorGUILayout.FloatField(label, dimensionValue);
            lockState = EditorGUILayout.Toggle(lockState);
            EditorGUILayout.EndVertical();
        }


        private void DrawAnchor(int anchorIndex) {
            Handles.color = Color.white;
            ref Vector3 anchorPos = ref m_curve[anchorIndex].point;
            if (Handles.Button(anchorPos, Quaternion.identity, .075f, .075f, Handles.SphereHandleCap)) {
                Debug.Log("hit");
            }
            
            Handles.color = Color.red;
            Vector3 newAnchorPos = Handles.FreeMoveHandle(anchorPos,
                                                          .1f, Vector3.zero, Handles.SphereHandleCap);

            if (newAnchorPos != anchorPos) {
                Undo.RecordObject(m_curveCreator, "Move Anchor Point");
                m_curve.MoveControlPoint(anchorIndex, newAnchorPos);
            }
        }

        private void DrawControlPoint(int controlPointIndex) {
            Handles.color = Color.white;
            ref Vector3 controlPointPos = ref m_curve[controlPointIndex].point;
            Vector3 newControlPos = Handles.FreeMoveHandle(controlPointPos,
                                                           .1f, Vector3.zero, Handles.SphereHandleCap);
            if (controlPointPos != newControlPos) {
                Undo.RecordObject(m_curveCreator, "Move Control Point");
                m_curve.MoveControlPoint(controlPointIndex, newControlPos);
            }

            int anchorIndex = (controlPointIndex - 1) % 3 == 0 ? controlPointIndex - 1 : controlPointIndex + 1;
            Vector3 anchorPos = m_curve[anchorIndex].point;
            Handles.DrawLine(controlPointPos, anchorPos);
        }

        private void OnSceneGUI() {
            if (m_curveCreator == null || m_curveCreator.curve == null) {
                m_curveCreator = (CurveCreator)target;
                if (m_curveCreator.curve == null || m_curveCreator.curve.controlPoints == null || 
                    m_curveCreator.curve.controlPoints.Count < 4)
                    ResetCurve();
                m_curve = m_curveCreator.curve;
                System.Diagnostics.Debug.Assert(m_curve != null, nameof(m_curve) + " != null");
            }

            Event guiEvent = Event.current;
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift) {
                Vector3 raycastPoint = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
                Undo.RecordObject(m_curveCreator, "Add New Anchor");
                
                m_curve.AppendNewAnchorWithControlPoint(raycastPoint);
            }
            
            for (var segmentIndex = 0; segmentIndex < m_curve.NumSegments(); segmentIndex++) {
                int i = m_curve.GetSegmentStart(segmentIndex);
                if (segmentIndex == 0) 
                    DrawAnchor(i);
                DrawControlPoint(i + 1);
                DrawControlPoint(i + 2);
                DrawAnchor(i + 3);
                Handles.color = Color.green;

                Handles.DrawBezier(m_curve.controlPoints[i].point,
                                   m_curve.controlPoints[i + 3].point,
                                   m_curve.controlPoints[i + 1].point,
                                   m_curve.controlPoints[i + 2].point,
                                   Color.green, null, 3f);
            }
        }
    }
}