#if UNITY_EDITOR && UNITY_2017_1_OR_NEWER
#define ACTIVE_BUILD_CHANGED_CALLBACK
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
    [ExecuteInEditMode]
    public class MobileControlRig : MonoBehaviour
#if ACTIVE_BUILD_CHANGED_CALLBACK
        , UnityEditor.Build.IActiveBuildTargetChanged
#endif
    {
        // this script enables or disables the child objects of a control rig
        // depending on whether the USE_MOBILE_INPUT define is declared.

        // This define is set or unset by a menu item that is included with
        // the Cross Platform Input package.


#if !UNITY_EDITOR
	void OnEnable()
	{
		CheckEnableControlRig();
	}
#elif ACTIVE_BUILD_CHANGED_CALLBACK
        public int callbackOrder
        {
            get
            {
                return 1;
            }
        }
#endif

        private void Start()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) //if in the editor, need to check if we are playing, as start is also called just after exiting play
#endif
            {
                UnityEngine.EventSystems.EventSystem system = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();

                if (system == null)
                {//the scene have no event system, spawn one
                    GameObject o = new GameObject("EventSystem");

                    o.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    o.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                }
            }
        }

#if UNITY_EDITOR

        private void OnEnable()
        {
            EditorApplication.update += Update;
#if !ACTIVE_BUILD_CHANGED_CALLBACK
            EditorUserBuildSettings.activeBuildTargetChanged += Update;
#endif
        }


        private void OnDisable()
        {
            EditorApplication.update -= Update;
#if !ACTIVE_BUILD_CHANGED_CALLBACK
            EditorUserBuildSettings.activeBuildTargetChanged -= Update;
#endif
        }


        private void Update()
        {
            CheckEnableControlRig();
        }
#endif


        private void CheckEnableControlRig()
        {
#if MOBILE_INPUT
		    EnableControlRig(true);
#else
            EnableControlRig(false);
#endif
        }


        private void EnableControlRig(bool enabled)
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(enabled);
            }
        }

#if ACTIVE_BUILD_CHANGED_CALLBACK
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            CheckEnableControlRig();
        }
#endif
    }
}