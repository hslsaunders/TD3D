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
        private bool m_queueUpdateAnchorOptions;

        private void EnsureTargetSet() {
            if (m_curveCreator == null || m_curveCreator.curve == null) {
                m_curveCreator = (CurveCreator)target;
                if (m_curveCreator.curve == null || m_curveCreator.curve.controlPoints == null ||
                    m_curveCreator.curve.controlPoints.Count < 4)
                    ResetCurve();
                m_curve = m_curveCreator.curve;
                System.Diagnostics.Debug.Assert(m_curve != null, nameof(m_curve) + " != null");
            }
        }
        
        private void ResetCurve() {
            m_curveCreator.curve = new BezierCurve(new List<Vector3> {
                new(-1f, 1f, 0f), new(0f, 1f, 0f),
                new(0f, -1f, 0f), new(1f, -1, 0f)
            });
            
            m_curve = m_curveCreator.curve;
        }

        public override void OnInspectorGUI() {
            EnsureTargetSet();
            
            base.OnInspectorGUI();
            m_curveCreator.snapping = EditorGUILayout.Toggle("Snapping: ", m_curveCreator.snapping);
            if (m_curveCreator.snapping) {
                m_curveCreator.snappingSize = EditorGUILayout.FloatField("Snapping Size:", m_curveCreator.snappingSize);
                m_curveCreator.snappingSize = Mathf.Max(m_curveCreator.snappingSize, 0f);
            }
            
            m_selectionFoldoutGroup = EditorGUILayout.BeginFoldoutHeaderGroup(m_selectionFoldoutGroup, "Anchor Selection Options");
            if (m_selectionFoldoutGroup) {
                m_currAnchorOptions.dimensionLockValues = EditorGUILayout.Vector3Field("Dimension Locks", 
                                                                                       m_currAnchorOptions.dimensionLockValues);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Lock:");
                ref var lockX = ref m_currAnchorOptions.lockX;
                ref var lockY = ref m_currAnchorOptions.lockY;
                ref var lockZ = ref m_currAnchorOptions.lockZ;
                
                var newLockX = EditorGUILayout.Toggle(lockX);
                var newLockY = EditorGUILayout.Toggle(lockY);
                var newLockZ = EditorGUILayout.Toggle(lockZ);
                bool optionsChanged = newLockX != lockX || newLockY != lockY ||
                                      newLockZ != lockZ;
                lockX = newLockX;
                lockY = newLockY;
                lockZ = newLockZ;
                if (optionsChanged) {
                    ApplyAnchorOptions();
                }
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

        private void ApplyAnchorOptions() {
            foreach (ControlPoint controlPoint in m_curve.controlPoints) {
                controlPoint.lockX = m_currAnchorOptions.lockX;
                controlPoint.lockY = m_currAnchorOptions.lockY;
                controlPoint.lockZ = m_currAnchorOptions.lockZ;
                controlPoint.lockValues.x = controlPoint.lockX ? m_currAnchorOptions.dimensionLockValues.x : 0f;
                controlPoint.lockValues.y = controlPoint.lockY ? m_currAnchorOptions.dimensionLockValues.y : 0f;
                controlPoint.lockValues.z = controlPoint.lockZ ? m_currAnchorOptions.dimensionLockValues.z : 0f;
            }
        }

        private void DrawAnchorLockOption(ref float dimensionValue, ref bool lockState, string label) {
            EditorGUILayout.BeginVertical();
            dimensionValue = EditorGUILayout.FloatField(label, dimensionValue);
            lockState = EditorGUILayout.Toggle(lockState);
            EditorGUILayout.EndVertical();
        }


        private void DrawAnchor(int anchorIndex) {
            Handles.color = Color.white;
            ControlPoint anchor = m_curve[anchorIndex];
            ref Vector3 anchorPos = ref m_curve[anchorIndex].point;
            /*
            if (Handles.Button(anchorPos, Quaternion.identity, .075f, .075f, Handles.SphereHandleCap)) {
                Debug.Log("hit");
            }
            */
            
            Handles.color = Color.red;
            Vector3 newAnchorPos = Handles.FreeMoveHandle(ConstrainPosWithLocks(anchorPos, anchor.lockX, anchor.lockY, anchor.lockZ,
                                                                                anchor.lockValues),
                                                          m_curveCreator.anchorSize, 
                                                          Vector3.zero, 
                                                          Handles.SphereHandleCap);
            

            if (newAnchorPos != anchorPos) {
                Undo.RecordObject(m_curveCreator, "Move Anchor Point");
                if (m_curveCreator.snapping && m_curveCreator.snappingSize > 0f) 
                    newAnchorPos = SnapToGrid(newAnchorPos, m_curveCreator.snappingSize);
                m_curve.MoveControlPoint(anchorIndex, newAnchorPos);
            }
        }

        private void DrawControlPoint(int controlPointIndex) {
            Handles.color = Color.white;

            var controlPoint = m_curve[controlPointIndex];
            ref var controlPointPos = ref controlPoint.point;
            Vector3 newControlPos = Handles.FreeMoveHandle(ConstrainPosWithLocks(controlPointPos, controlPoint.lockX, controlPoint.lockY, controlPoint.lockZ,
                                                                                 controlPoint.lockValues),
                                                           m_curveCreator.controlPointSize, 
                                                           Vector3.zero, 
                                                           Handles.SphereHandleCap);

            if (controlPointPos != newControlPos) {
                Undo.RecordObject(m_curveCreator, "Move Control Point");
                if (m_curveCreator.snapping && m_curveCreator.snappingSize > 0f) 
                    newControlPos = SnapToGrid(newControlPos, m_curveCreator.snappingSize);
                m_curve.MoveControlPoint(controlPointIndex, newControlPos);
            }

            int anchorIndex = (controlPointIndex - 1) % 3 == 0 ? controlPointIndex - 1 : controlPointIndex + 1;
            Vector3 anchorPos = m_curve[anchorIndex].point;
            Handles.DrawLine(controlPointPos, anchorPos);
        }

        private Vector3 ConstrainPosWithLocks(Vector3 pos, bool lockX, bool lockY, bool lockZ, Vector3 lockValues) {
            if (lockX)
                pos.x = lockValues.x;
            if (lockY)
                pos.y = lockValues.y;
            if (lockZ)
                pos.z = lockValues.z;
            return pos;
        }

        private Vector3 SnapToGrid(Vector3 pos, float snappingSize) {
            return new Vector3(Mathf.RoundToInt(pos.x / snappingSize), 
                               Mathf.RoundToInt(pos.y / snappingSize), 
                               Mathf.RoundToInt(pos.z / snappingSize))
                   * snappingSize;
        }

        private void OnSceneGUI() {
            EnsureTargetSet();

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