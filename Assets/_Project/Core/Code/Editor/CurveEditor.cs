using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using TD3D.Core.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace TD3D.Core.Editor {
    [CustomEditor(typeof(CurveCreator))]
    public class CurveEditor : OdinEditor {
        private CurveCreator m_curveCreator;
        private BezierCurve m_curve;
        
        [SerializeField, HideInInspector] private bool m_bakedWarningState;
        private bool m_queueUpdateAnchorOptions;
        private bool m_queueUpdateBakedWarningState;
        private Camera camera;

        protected override void OnEnable() {
            RenderPipelineManager.beginCameraRendering += UpdateCameraReference;
        }

        protected override void OnDisable() {
            RenderPipelineManager.beginCameraRendering += UpdateCameraReference;
        }

        private void UpdateCameraReference(ScriptableRenderContext ctx, Camera cam) {
            camera = cam;
        }
        
        private void EnsureTargetSet() {
            if (m_curveCreator == null || m_curveCreator.curve == null) {
                m_curveCreator = (CurveCreator)target;
                if (m_curveCreator.curve == null || m_curveCreator.curve.controlPoints == null ||
                    m_curveCreator.curve.controlPoints.Count < 4)
                    ResetCurve();
                m_curve = m_curveCreator.curve;
                m_curveCreator.curve!.OnCurveEdit = delegate { m_queueUpdateBakedWarningState = true; };
                m_curveCreator.curve!.OnBake = delegate { m_queueUpdateBakedWarningState = true; };
                System.Diagnostics.Debug.Assert(m_curve != null, nameof(m_curve) + " != null");
            }
        }
        
        private void ResetCurve() {
            m_curveCreator.curve = new BezierCurve(new List<Vector3> {
                new(-1f, 1f, 0f), new(0f, 1f, 0f),
                new(0f, -1f, 0f), new(1f, -1, 0f)
            });
            
            m_curveCreator.curve.OnCurveEdit = delegate 
            { 
                m_queueUpdateBakedWarningState = true;
                m_curveCreator.queueRebake = true;
            };
            m_curveCreator.curve!.OnBake = delegate { m_queueUpdateBakedWarningState = true; };
            m_curve = m_curveCreator.curve;
        }

        private void BakeCurve() {
            m_curve.BakeCurve(m_curveCreator.bakedCurveVertexSpacing, 
                              m_curveCreator.currAnchorOptions.lockX,
                              m_curveCreator.currAnchorOptions.lockY,
                              m_curveCreator.currAnchorOptions.lockZ);
        }

        public override void OnInspectorGUI() {
            EnsureTargetSet();

            if (m_curveCreator.queueRebake) {
                m_curveCreator.queueRebake = false;
                BakeCurve();
                SceneView.RepaintAll();
            }
            
            if (m_queueUpdateBakedWarningState) {
                m_queueUpdateBakedWarningState = false;
                m_bakedWarningState = m_curve.IsCurrentCurveBaked();
            }

            if (!m_bakedWarningState)
                EditorGUILayout.HelpBox("Warning: Current curve is not baked.", MessageType.Warning);
            
            base.OnInspectorGUI();

            if (GUILayout.Button("Bake Curve")) {
                try {
                    BakeCurve();
                }
                catch {
                    Debug.Log("Failed to bake");
                }

                SceneView.RepaintAll();
            }

            m_curveCreator.snapping = EditorGUILayout.Toggle("Snapping: ", m_curveCreator.snapping);
            if (m_curveCreator.snapping) {
                m_curveCreator.snappingSize = EditorGUILayout.FloatField("Snapping Size:", m_curveCreator.snappingSize);
                m_curveCreator.snappingSize = Mathf.Max(m_curveCreator.snappingSize, 0f);
            }

            if (m_curve.HasBakedCurve) {
                m_curveCreator.bakedCurveInfoFoldoutOpen =
                    EditorGUILayout.BeginFoldoutHeaderGroup(m_curveCreator.bakedCurveInfoFoldoutOpen, "Baked Curve Info");
                if (m_curveCreator.bakedCurveInfoFoldoutOpen) {
                    EditorGUILayout.LabelField("Length:", m_curve.bakedCurve.totalLength.ToString());
                    EditorGUILayout.LabelField("Num Points:", m_curve.bakedCurve.numPoints.ToString());
                }
                
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            m_curveCreator.anchorOptionsFoldoutOpen = 
                EditorGUILayout.BeginFoldoutHeaderGroup(m_curveCreator.anchorOptionsFoldoutOpen, "Anchor Selection Options");
            if (m_curveCreator.anchorOptionsFoldoutOpen) {
                m_curveCreator.currAnchorOptions.dimensionLockValues = EditorGUILayout.Vector3Field("Dimension Locks", 
                    m_curveCreator.currAnchorOptions.dimensionLockValues);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Lock:");
                ref var lockX = ref m_curveCreator.currAnchorOptions.lockX;
                ref var lockY = ref m_curveCreator.currAnchorOptions.lockY;
                ref var lockZ = ref m_curveCreator.currAnchorOptions.lockZ;
                
                var newLockX = EditorGUILayout.Toggle(lockX);
                var newLockY = EditorGUILayout.Toggle(lockY);
                var newLockZ = EditorGUILayout.Toggle(lockZ);
                bool optionsChanged = newLockX != lockX || newLockY != lockY ||
                                      newLockZ != lockZ;
                if (optionsChanged) {
                    Undo.RecordObject(m_curveCreator, "Lock Change");
                    lockX = newLockX;
                    lockY = newLockY;
                    lockZ = newLockZ;
                    ApplyAnchorOptions();
                    SceneView.RepaintAll();
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
                controlPoint.lockX = m_curveCreator.currAnchorOptions.lockX;
                controlPoint.lockY = m_curveCreator.currAnchorOptions.lockY;
                controlPoint.lockZ = m_curveCreator.currAnchorOptions.lockZ;
                controlPoint.lockValues.x = controlPoint.lockX ? m_curveCreator.currAnchorOptions.dimensionLockValues.x : 0f;
                controlPoint.lockValues.y = controlPoint.lockY ? m_curveCreator.currAnchorOptions.dimensionLockValues.y : 0f;
                controlPoint.lockValues.z = controlPoint.lockZ ? m_curveCreator.currAnchorOptions.dimensionLockValues.z : 0f;
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
                m_curveCreator.QueueBakeCurve();
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
                m_curveCreator.QueueBakeCurve();
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
            Vector3 mouseWorldPos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift) {
                
                Undo.RecordObject(m_curveCreator, "Add New Anchor");
                m_curve.AppendNewAnchorWithControlPoint(mouseWorldPos);
            }

            int numSegments = m_curve.NumSegments();
            int selectedSegmentIndex = -1;

            /*
            if (guiEvent.type == EventType.MouseMove && camera != null) {
                float minDstToSegment = 10f;

                for (int segmentIndex = 0; segmentIndex < numSegments; segmentIndex++) {
                    int i = m_curve.GetSegmentStart(segmentIndex);
                    Vector2 point1 = camera.WorldToScreenPoint(m_curve.controlPoints[i].point);
                    Vector2 point2 = camera.WorldToScreenPoint(m_curve.controlPoints[i + 3].point);
                    Vector2 point3 = camera.WorldToScreenPoint(m_curve.controlPoints[i + 1].point);
                    Vector2 point4 =  camera.WorldToScreenPoint(m_curve.controlPoints[i + 2].point);

                    float dst = HandleUtility.DistancePointBezier(guiEvent.mousePosition,
                                                                  point1,
                                                                  point2,
                                                                  point3,
                                                                  point4);
                    //Debug.Log($"{segmentIndex}: {dst}");
                    if (dst < minDstToSegment) {
                        minDstToSegment = dst;
                        selectedSegmentIndex = segmentIndex;
                    }
                }
            }
            */
            
            for (var segmentIndex = 0; segmentIndex < numSegments; segmentIndex++) {
                int i = m_curve.GetSegmentStart(segmentIndex);
                if (segmentIndex == 0) 
                    DrawAnchor(i);
                DrawControlPoint(i + 1);
                DrawControlPoint(i + 2);
                DrawAnchor(i + 3);
                
                Color segmentColor = segmentIndex == selectedSegmentIndex ? Color.red : Color.green;

                Handles.color = segmentColor;

                Handles.DrawBezier(m_curve.controlPoints[i].point,
                                   m_curve.controlPoints[i + 3].point,
                                   m_curve.controlPoints[i + 1].point,
                                   m_curve.controlPoints[i + 2].point,
                                   segmentColor, null, 3f);
            }

            if (m_curve.HasBakedCurve) {
                Handles.color = Color.red; 
                int n = 300;
                Vector3 last = m_curve.bakedCurve.EvaluatePointAtTime(0f / n);
                for (int i = 1; i <= n; i++) {
                    Vector3 curr = m_curve.bakedCurve.EvaluatePointAtTime(i / (float)n);
                    Handles.DrawLine(last, curr);
                    last = curr;
                }
                
            }
        }
    }
}