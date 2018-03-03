using System.Collections.Generic;
using Assets.Cinemachine.Base.Runtime.Behaviours;
using UnityEditor;

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineExternalCamera))]
    internal class CinemachineExternalCameraEditor 
        : CinemachineVirtualCameraBaseEditor<CinemachineExternalCamera>
    {
        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add("Extensions");
            return excluded;
        }
    }
}
