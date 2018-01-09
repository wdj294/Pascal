// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ez.DataManager
{
    public partial class EzDataManager : MonoBehaviour
    {
        #region Singleton
        protected EzDataManager() { }

        private static EzDataManager _instance;
        public static EzDataManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    if(applicationIsQuitting) { return null; }
                    GameObject singleton = new GameObject(("(singleton) " + typeof(EzDataManager).ToString()));
                    _instance = singleton.AddComponent<EzDataManager>();
                    DontDestroyOnLoad(singleton);
                }
                return _instance;
            }
        }

        private static bool applicationIsQuitting = false;
        private void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }
        #endregion

        /// <summary>
        /// Do not use this variable. It is used by the custom editor to store the last setting you set for the variable name width.
        /// </summary>
        public float editorVariableNameWidth = 120f;

        private void Awake()
        {
            if(_instance != null)
            {
                Debug.Log("[EzDataManager] There cannot be two Data Managers active at the same time. Destryoing this one!");
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
