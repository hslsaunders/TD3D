using Sirenix.OdinInspector;
using TD3D.Core.Runtime;
using UnityEditor;
using UnityEngine;

namespace TD3D.Core.Editor {
    [CustomEditor(typeof(CurveCreator))]
    public class CurveEditor : UnityEditor.Editor {
        private CurveCreator m_curveCreator;
        
        private void OnEnable() {
            m_curveCreator = (CurveCreator)target;
        }

        [Button(ButtonSizes.Large)]
        private void CreateNewCurve() {
            
            
            //m_curveCreator.curve = new Curve();
        }

        public override void OnInspectorGUI() {
            
        }

        private void OnSceneGUI() {
            if (m_curveCreator.curve == null) return;
            
            foreach (Anchor anchor in m_curveCreator.curve.anchors) {
                
            }
        }
    }
}