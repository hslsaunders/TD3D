﻿using System;
using UnityEngine;

namespace TD3D.Core.Runtime {
    [Serializable]
    public struct AnchorOptions {
        public Vector3 dimensionLockValues;
        public bool lockX;
        public bool lockY;
        public bool lockZ;
    }
}