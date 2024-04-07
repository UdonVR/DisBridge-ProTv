
using System;
using ArchiTech.ProTV;
using UdonSharp;
using UdonVR.DisBridge;
using UnityEngine;
using VRC.SDKBase;

namespace UdonVR.DisBridge.Plugins.ProTv
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    [DefaultExecutionOrder(-1)]
    public class DisBridge_ProTV : TVAuthPlugin
    {
        [Tooltip("Leave this blank if you don't want it to unlock automatically.")]
        [SerializeField] private PluginManager manager;
        
        [Space(5)]
        [Header("Authorized User")]
        ///////////////////
        [Tooltip("If this is True, it will allow All Staff to use the button.")]
        [SerializeField] private bool checkStaff_Authorized = false;
        [Tooltip("If this is True, it will allow All Supporters to use the button.")]
        [SerializeField] private bool checkSupporters_Authorized = false;
        [Tooltip("Adds the role specified to the unlock check./nUses Role ID")]
        [SerializeField] private string[] checkRolesID_Authorized;
        [Tooltip("Adds the role specified to the unlock check./nUses Role Index")]
        [SerializeField] private int[] checkRoles_Authorized;
        
        ///////////////////
        [Space(5)]
        [Header("Super User")]
        [Tooltip("If this is True, it will allow All Staff to use the button.")]
        [SerializeField] private bool checkStaff_Super = false;
        [Tooltip("If this is True, it will allow All Supporters to use the button.")]
        [SerializeField] private bool checkSupporters_Super = false;
        [Tooltip("Adds the role specified to the unlock check./nUses Role ID")]
        [SerializeField] private string[] checkRolesID_Super;
        [Tooltip("Adds the role specified to the unlock check./nUses Role Index")]
        [SerializeField] private int[] checkRoles_Super;
        public override void Start()
        {
            manager.AddPlugin(gameObject);
        }

        public void _UVR_Init()
        {
            
        }

        public override bool _IsAuthorizedUser(VRCPlayerApi who)
        {
            if (checkStaff_Authorized && manager.IsStaff())
            {
                return true;
            }
            if (checkSupporters_Authorized && manager.IsSupporter())
            {
                return true;
            }

            if (checkRoles_Authorized != null && checkRoles_Authorized.Length != 0)
            {
                for (int i = 0; i < checkRoles_Authorized.LongLength; i++)
                {
                    if (manager.IsMember(checkRoles_Authorized[i],Networking.LocalPlayer))
                    {
                        return true;
                    }
                }
            }
            
            if (checkRolesID_Authorized != null && checkRolesID_Authorized.Length != 0)
            {
                for (int i = 0; i < checkRolesID_Authorized.LongLength; i++)
                {
                    if (manager.IsMember(checkRolesID_Authorized[i],Networking.LocalPlayer))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool _IsSuperUser(VRCPlayerApi who)
        {
            if (checkStaff_Super && manager.IsStaff())
            {
                return true;
            }
            if (checkSupporters_Super && manager.IsSupporter())
            {
                return true;
            }

            if (checkRoles_Super != null && checkRoles_Super.Length != 0)
            {
                for (int i = 0; i < checkRoles_Super.LongLength; i++)
                {
                    if (manager.IsMember(checkRoles_Super[i],Networking.LocalPlayer))
                    {
                        return true;
                    }
                }
            }
            
            if (checkRolesID_Super != null && checkRolesID_Super.Length != 0)
            {
                for (int i = 0; i < checkRolesID_Super.LongLength; i++)
                {
                    if (manager.IsMember(checkRolesID_Super[i],Networking.LocalPlayer))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
