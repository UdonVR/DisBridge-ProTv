using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UdonVR.DisBridge.Plugins.ProTv.Editor
{
    [CustomEditor(typeof(DisBridge_ProTV))]
    public class DisBridge_ProTV_Editor : UnityEditor.Editor
    {
        private DisBridge_ProTV _script;
        public override void OnInspectorGUI()
        {
            _script = (DisBridge_ProTV)target;
            Undo.RecordObject(_script,$"Generic Undo event on {nameof(DisBridge_ProTV_Editor)} -- {target.name}");
            EditorFunctions.PlaymodeWarning(_script);
            DrawPluginManagerCheck();
            EditorFunctions.GuiLine();
            DrawAuthUsers();
            EditorFunctions.GuiLine();
            DrawSuperUsers();
            PrefabUtility.RecordPrefabInstancePropertyModifications(_script);
        }
        
        private void DrawPluginManagerCheck()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            { 
                GUILayout.BeginHorizontal();
                PluginManager _imageLoader = (PluginManager)EditorGUILayout.ObjectField("DisBridge Plugin Manager:", _script.manager, typeof(PluginManager), true);
                if (_script.manager != _imageLoader)
                {
                    Undo.RecordObject(_script,$"Changed manager: {_script.manager} => {_imageLoader}");
                    _script.manager = _imageLoader;
                }

                if (_script.manager != null){
                    if (GUILayout.Button("Select", GUILayout.Width(50)))
                    {
                        Selection.activeObject = _script.manager.gameObject;
                    }
                }
                GUILayout.EndHorizontal();
                
                
                if (_script.manager == null)
                {
                    PluginManager[] pluginManagers = FindObjectsOfType<PluginManager>();
                    if (pluginManagers == null || pluginManagers.Length > 1)
                    {
                        
                        EditorGUILayout.HelpBox("!!! DisBridge PluginManager is not bound !!!\n\n multiple DisBridge PluginManagers were found, please bind one manually or below.", MessageType.Error);

                        for (int i = 0; i < pluginManagers.Length; i++)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.ObjectField(pluginManagers[i], typeof(PluginManager), true);
                            if(GUILayout.Button($"Select",GUILayout.Width(100)))
                            {
                                Undo.RecordObject(_script,$"Changed manager: {_script.manager} => {pluginManagers[i]}");
                                _script.manager = pluginManagers[i];
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    else if (pluginManagers.Length == 0)
                    {
                        EditorGUILayout.HelpBox("!!! DisBridge PluginManager is not bound !!!\n\n No DisBridge PluginManagers were found, please create one.", MessageType.Error);
                    }
                    else
                    {
                        Undo.RecordObject(_script,$"Changed manager: {_script.manager} => {pluginManagers[0]}");
                        _script.manager = pluginManagers[0];
                    }
                }
            }
        }

        private void DrawAuthUsers()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Authorized Users");
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField("Generic Staff",GUILayout.MinWidth(0f));
                        _script.checkStaff_Authorized = EditorGUILayout.Toggle(_script.checkStaff_Authorized,GUILayout.Width(15f));
                    }
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField("Generic Supporters",GUILayout.MinWidth(0f));
                        _script.checkSupporters_Authorized = EditorGUILayout.Toggle(_script.checkSupporters_Authorized,GUILayout.Width(15f));
                    }
                }
                DrawAuthSelector();
            }
        }

        private void DrawSuperUsers()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Super Users");
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField("Generic Staff",GUILayout.MinWidth(0f));
                        _script.checkStaff_Super = EditorGUILayout.Toggle(_script.checkStaff_Super,GUILayout.Width(15f));
                    }
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField("Generic Supporters",GUILayout.MinWidth(0f));
                        _script.checkSupporters_Super = EditorGUILayout.Toggle(_script.checkSupporters_Super,GUILayout.Width(15f));
                    }
                }

                DrawSuperSelector();
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public void DrawAuthSelector()
        {
            if (_script.manager == null) return;
            
            Undo.RecordObject(_script,$"RoleContainer Mask Change");
            int _flags = 0;
            String[] _RoleContainerLabels = new string[_script.manager.roles.childCount];
            RoleContainer[] _allRoleContainers = new RoleContainer[_RoleContainerLabels.Length];
            String[] _RoleContainerIds = new String[_RoleContainerLabels.Length];

            //Build current RoleContainer arrays
            for (int i = 0; i < _script.manager.roles.childCount; i++)
            {
                _allRoleContainers[i] = _script.manager.roles.GetChild(i).GetComponent<RoleContainer>();
                _RoleContainerLabels[i] = _allRoleContainers[i].roleName;
                _RoleContainerIds[i] = _allRoleContainers[i].roleId;
            }

            //calculate flags based on RoleID
            //Debug.Log("_script.roleContainerIDs.Length: " + _script.roleContainerIDs.Length);
            
            if (_script.checkRolesID_Authorized != null)
            {
                for (int i = 0; i < _script.checkRolesID_Authorized.Length; i++)
                {
                    //Debug.Log("Array.IndexOf(_RoleContainerLabels,_script.roleContainerIDs[i])): " + Array.IndexOf(_RoleContainerLabels,_script.roleContainerIDs[i]));
                    //Debug.Log(_script.roleContainerIDs[i]);
                    int j = Array.IndexOf(_RoleContainerIds, _script.checkRolesID_Authorized[i]);
                    if (j > -1)
                    {
                        //Debug.LogWarning("Found Match: " + _RoleCOntainerIds[j]);
                        _flags ^= (1 << j);
                    }
                }
            }
            

            //create mask
            _flags = EditorGUILayout.MaskField ("Role Containers", _flags, _RoleContainerLabels);
            List<RoleContainer> selectedOptions = new List<RoleContainer>();
            for (int i = 0; i < _RoleContainerLabels.Length; i++)
            {
                if ((_flags & (1 << i )) == (1 << i) ) selectedOptions.Add(_allRoleContainers[i]);
            }

            //Save data to _Script
            _script.AuthorizedUsers = new RoleContainer[selectedOptions.Count];
            //Debug.Log("_roleContainers: " + _script._roleContainers.Length);
            for (int i = 0; i < selectedOptions.Count; i++)
            {
                //Debug.Log("Adding " + selectedOptions[i].roleName);
                _script.AuthorizedUsers[i] = selectedOptions[i];
            }
            //_script._roleContainers = selectedOptions.ToArray();
            
            _script.checkRolesID_Authorized = new string[_script.AuthorizedUsers.Length];
            for (int i = 0; i < _script.AuthorizedUsers.Length; i++)
            {
                //Debug.Log("Adding " + _script._roleContainers[i].roleId);
                _script.checkRolesID_Authorized[i] = _script.AuthorizedUsers[i].roleId;
            }
            
        }
        
        public void DrawSuperSelector()
        {
            if (_script.manager == null) return;
            
            Undo.RecordObject(_script,$"RoleContainer Mask Change");
            int _flags = 0;
            String[] _RoleContainerLabels = new string[_script.manager.roles.childCount];
            RoleContainer[] _allRoleContainers = new RoleContainer[_RoleContainerLabels.Length];
            String[] _RoleContainerIds = new String[_RoleContainerLabels.Length];

            //Build current RoleContainer arrays
            for (int i = 0; i < _script.manager.roles.childCount; i++)
            {
                _allRoleContainers[i] = _script.manager.roles.GetChild(i).GetComponent<RoleContainer>();
                _RoleContainerLabels[i] = _allRoleContainers[i].roleName;
                _RoleContainerIds[i] = _allRoleContainers[i].roleId;
            }

            //calculate flags based on RoleID
            //Debug.Log("_script.roleContainerIDs.Length: " + _script.roleContainerIDs.Length);
            
            if (_script.checkRolesID_Super != null)
            {
                for (int i = 0; i < _script.checkRolesID_Super.Length; i++)
                {
                    //Debug.Log("Array.IndexOf(_RoleContainerLabels,_script.roleContainerIDs[i])): " + Array.IndexOf(_RoleContainerLabels,_script.roleContainerIDs[i]));
                    //Debug.Log(_script.roleContainerIDs[i]);
                    int j = Array.IndexOf(_RoleContainerIds, _script.checkRolesID_Super[i]);
                    if (j > -1)
                    {
                        //Debug.LogWarning("Found Match: " + _RoleCOntainerIds[j]);
                        _flags ^= (1 << j);
                    }
                }
            }
            

            //create mask
            _flags = EditorGUILayout.MaskField ("Role Containers", _flags, _RoleContainerLabels);
            List<RoleContainer> selectedOptions = new List<RoleContainer>();
            for (int i = 0; i < _RoleContainerLabels.Length; i++)
            {
                if ((_flags & (1 << i )) == (1 << i) ) selectedOptions.Add(_allRoleContainers[i]);
            }

            //Save data to _Script
            _script.SuperUsers = new RoleContainer[selectedOptions.Count];
            //Debug.Log("_roleContainers: " + _script._roleContainers.Length);
            for (int i = 0; i < selectedOptions.Count; i++)
            {
                //Debug.Log("Adding " + selectedOptions[i].roleName);
                _script.SuperUsers[i] = selectedOptions[i];
            }
            //_script._roleContainers = selectedOptions.ToArray();
            
            _script.checkRolesID_Super = new string[_script.SuperUsers.Length];
            for (int i = 0; i < _script.SuperUsers.Length; i++)
            {
                //Debug.Log("Adding " + _script._roleContainers[i].roleId);
                _script.checkRolesID_Super[i] = _script.SuperUsers[i].roleId;
            }
            
        }
    }
}