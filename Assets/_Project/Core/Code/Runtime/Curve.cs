using System.Collections.Generic;
using UnityEngine;

namespace TD3D.Core.Runtime {
    public class Curve {
        public List<Anchor> anchors = new();

        public Curve(List<Vector3> points) {
            foreach (var point in points) {
                AddAnchor(point);
            }
        }

        public void AddAnchor(Vector3 pos, int index = -1) {
            Vector3 defaultCP1 = pos + Vector3.up * .5f;
            Vector3 defaultCP2 = pos - Vector3.up * .5f;
            
            var newAnchor = new Anchor(pos, defaultCP1, defaultCP2);
            
            if (index == -1)
                anchors.Add(newAnchor);
            else
                anchors.Insert(index, newAnchor);
        }
    }

    public class Anchor {
        public Vector3 point;
        public Vector3 cp1;
        public Vector3 cp2;
        //public bool freeCPs;

        public Anchor(Vector3 point, Vector3 cp1,Vector3 cp2) {
            this.point = point;
            this.cp1 = cp1;
            this.cp2 = cp2;
        }
    }
}