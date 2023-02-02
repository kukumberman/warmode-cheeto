using System;
using HarmonyLib;
using UnityEngine;

namespace Cheeto.Patches
{
    [HarmonyPatch(typeof(NetClient))]
    class NetClient_Patch
    {
        [HarmonyPatch("send_takedamage")]
        [HarmonyPatch(new Type[] { typeof(byte), typeof(byte), typeof(byte), typeof(Vector3), typeof(Vector3), typeof(float) })]
        [HarmonyPrefix]
        private static void SendTakeDamage(byte vid, byte hitzone, byte clip, Vector3 from, Vector3 to, float surfaceError)
        {
            // vid (view id - player who took damage)
            var str = $"{vid} {hitzone} {clip} ({from}) ({to}) {surfaceError}";
            Debug.Log(str);
        }
    }
}
