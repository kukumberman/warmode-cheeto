using UnityEngine;
using BepInEx.Logging;

namespace Cheeto
{
    class Cheeto : MonoBehaviour
    {
        private static string ApplicationInfo = "";
        private static ManualLogSource Logger = null;

        private bool m_IsWindowOpen = true;
        private Rect m_WindowRect = new Rect(0, 0, 500, 400);

        private bool m_DrawEnemies = true;
        private bool m_DrawTeam = true;

        private Color m_EnemyColor = Color.red;
        private Color m_TeamColor = Color.green;

        private int m_LineThickness = 2;

        private GUIStyle m_TextStyle = null;
        private int m_FontSize = 20;

        private float m_HeadHeight = 0.2f;

        private bool m_TeleportHack = false;
        private int m_CircleFov = 100;
        private vp_FPCamera vp_FPCamera = null;
        private Camera m_Camera = null;
        private Vector3 m_TargetAimPosition = Vector3.zero;
        private bool m_HasTarget = false;

        private Player3rd m_TargetPlayer = null;

        int temp = 0;

        public static void Run(Plugin loader)
        {
            Logger = loader.Log;

            ApplicationInfo = string.Format("{0} {1} {2} {3}", Application.productName, Application.version, Application.unityVersion, Application.buildGUID);
            Logger.LogInfo(ApplicationInfo);

            Cheeto instance = loader.AddComponent<Cheeto>();
            DontDestroyOnLoad(instance.gameObject);
            instance.hideFlags |= HideFlags.HideAndDontSave;

            CodeStage.AntiCheat.Detectors.ObscuredCheatingDetector.StopDetection();
            CodeStage.AntiCheat.Detectors.ObscuredCheatingDetector.Dispose();
            CodeStage.AntiCheat.Detectors.SpeedHackDetector.StopDetection();
            CodeStage.AntiCheat.Detectors.SpeedHackDetector.Dispose();
            CodeStage.AntiCheat.Detectors.InjectionDetector.StopDetection();
            CodeStage.AntiCheat.Detectors.InjectionDetector.Dispose();
        }

        private void Update()
        {
            vp_FPCamera = FindObjectOfType<vp_FPCamera>();

            if (vp_FPCamera)
            {
                m_Camera = vp_FPCamera.GetComponent<Camera>();
            }

            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                m_IsWindowOpen = !m_IsWindowOpen;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                m_TargetAimPosition = vp_FPCamera.transform.position;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                TeleportBehindEnemy();
            }
        }

        private void LateUpdate()
        {
            if (m_TeleportHack)
            {
                DoTeleportHack();
            }

            if (Input.GetKey(KeyCode.Q))
            {
                DoAim(m_TargetAimPosition);
            }

            m_HasTarget = FindTarget();

            if (Input.GetKey(KeyCode.E))
            {
                if (m_HasTarget)
                {
                    DoAim(m_TargetAimPosition);
                }
            }
        }

        private void TeleportBehindEnemy()
        {
            var player = vp_FPCamera.transform.parent;
            Vector3 pos = m_TargetPlayer.transform.position - m_TargetPlayer.transform.forward * 2;

            player.transform.position = pos;

            DoAim(m_TargetAimPosition);
        }

        private bool FindTarget()
        {
            if (PlayerControll.Player == null)
            {
                return false;
            }

            if (m_Camera == null)
            {
                return false;
            }

            var center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            bool exists = false;
            Vector3 target = Vector3.zero;
            float targetDistance = float.MaxValue;

            foreach (CPlayerData player in PlayerControll.Player)
            {
                if (player == null) continue;

                if (player.Team == FirstPersonPlayer.Team) continue;

                if (player.go == null) continue;

                var thirdPerson = player.PlayertScript;

                if (thirdPerson == null) continue;

                Vector3 head = thirdPerson._head.transform.position;
                Vector3 w2s = m_Camera.WorldToScreenPoint(head);
                if (w2s.z < 0)
                {
                    continue;
                }
                w2s.y = Screen.height - w2s.y;

                float distanceFromCenter = Vector2.Distance(center, w2s);
                if (distanceFromCenter > m_CircleFov)
                {
                    continue;
                }

                if (distanceFromCenter < targetDistance)
                {
                    targetDistance = distanceFromCenter;
                    target = head;
                    exists = true;
                    m_TargetPlayer = thirdPerson;
                }
            }

            m_TargetAimPosition = target;

            return exists;
        }

        private void DoTeleportHack()
        {
            temp = 0;

            if (PlayerControll.Player == null)
            {
                return;
            }

            if (FirstPersonPlayer.go == null)
            {
                return;
            }

            if (vp_FPCamera == null)
            {
                return;
            }

            Vector3 self = vp_FPCamera.transform.position;
            Vector3 forward = vp_FPCamera.transform.forward * 3;
            forward.y = self.y;
            Vector3 pos = self + forward;

            foreach (CPlayerData player in PlayerControll.Player)
            {
                if (player == null) continue;

                if (player.Team == FirstPersonPlayer.Team) continue;

                if (player.go == null) continue;

                player.go.transform.position = pos;

                temp += 1;
            }
        }

        private void DoAim(Vector3 target)
        {
            Vector2 angles = Vector2.zero;

            Vector3 dir = target - vp_FPCamera.transform.position;

            angles.x = Mathf.Asin(dir.y / dir.magnitude) * Mathf.Rad2Deg;
            angles.y = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            vp_FPCamera.Pitch = -angles.x;
            vp_FPCamera.Yaw = angles.y;
        }

        private void OnGUI()
        {
            if (m_IsWindowOpen)
            {
                m_WindowRect = GUI.Window(146, m_WindowRect, (UnityEngine.GUI.WindowFunction)DrawWindow, "Main Window");
            }

            if (Event.current.type == EventType.Repaint)
            {
                DrawESP();
            }
        }

        private void DrawWindow(int id)
        {
            DrawContent();

            GUI.DragWindow();
        }

        private void DrawContent()
        {
            GUILayout.Label(ApplicationInfo);

            m_DrawEnemies = GUILayout.Toggle(m_DrawEnemies, nameof(m_DrawEnemies));
            m_DrawTeam = GUILayout.Toggle(m_DrawTeam, nameof(m_DrawTeam));
            m_TeleportHack = GUILayout.Toggle(m_TeleportHack, nameof(m_TeleportHack));

            GUILayout.Label($"temp: {temp}");

            DrawIncrementalLabel($"{nameof(m_CircleFov)}: {m_CircleFov}", ref m_CircleFov, 25);
            DrawIncrementalLabel($"{nameof(m_FontSize)}: {m_FontSize}", ref m_FontSize, 2);

            GUILayout.BeginHorizontal();

            if (vp_FPCamera != null)
            {
                GUILayout.Label($"yaw: {vp_FPCamera.Yaw}");
                GUILayout.Label($"pitch: {vp_FPCamera.Pitch}");
            }

            GUILayout.EndHorizontal();

            DrawDumper();
        }

        private static void DrawIncrementalLabel(string label, ref int value, int incementalValue)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(label);

            if (GUILayout.Button("-"))
            {
                value -= incementalValue;
            }
            if (GUILayout.Button("+"))
            {
                value += incementalValue;
            }

            GUILayout.EndHorizontal();
        }

        private void DrawDumper()
        {
            if (GUILayout.Button("dump scene"))
            {
                Dumper.DumpToFile(Dumper.SerializeScene(), "scene");
            }

            if (GUILayout.Button("dump players"))
            {
                string targetName = "Bip001 L Finger1";
                var go = GameObject.Find(targetName);
                if (go)
                {
                    var root = go.transform.root;
                    Dumper.DumpToFile(Dumper.Serialize(root.gameObject), "players");
                }
                else
                {
                }
            }

            if (GUILayout.Button("dump fpcamera"))
            {
                var fpcamera = FindObjectOfType<vp_FPCamera>();
                if (fpcamera)
                {
                    var root = fpcamera.transform.root;
                    Dumper.DumpToFile(Dumper.Serialize(root.gameObject), "fpcamera");
                }
                else
                {
                }
            }
        }

        private void DrawESP()
        {
            if (PlayerControll.Player == null)
            {
                return;
            }

            m_TextStyle = new GUIStyle(GUI.skin.label);
            m_TextStyle.fontSize = m_FontSize;
            m_TextStyle.alignment = TextAnchor.MiddleCenter;

            if (m_Camera == null)
            {
                return;
            }

            DrawAimbot();

            foreach (CPlayerData player in PlayerControll.Player)
            {
                if (player == null) continue;

                DrawPlayer(player);
            }
        }

        private void DrawAimbot()
        {
            var center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            GUI.color = Color.black;
            Drawing.Circle(center, m_CircleFov, m_LineThickness);

            if (!m_HasTarget)
            {
                return;
            }

            Vector3 w2s = m_Camera.WorldToScreenPoint(m_TargetAimPosition);
            if (w2s.z > 0)
            {
                w2s.y = Screen.height - w2s.y;
                GUI.color = Color.white;
                Drawing.Circle(w2s, 5, m_LineThickness);

                GUI.color = Color.blue;
                Drawing.Line(center, w2s, m_LineThickness);
            }
        }

        private void DrawPlayer(CPlayerData player)
        {
            if (m_Camera == null)
            {
                return;
            }

            bool sameTeam = FirstPersonPlayer.Team == player.Team;

            if (sameTeam && !m_DrawTeam) return;
            if (!sameTeam && !m_DrawEnemies) return;

            Vector3 position = player.position;

            if (player.go == null) return;

            position = player.go.transform.position;

            Vector3 w2s = m_Camera.WorldToScreenPoint(position);
            w2s.y = Screen.height - w2s.y;

            if (w2s.z < 0)
            {
                return;
            }

            GUI.color = sameTeam ? m_TeamColor : m_EnemyColor;
            //Drawing.Box(w2s, Vector2.one * 10, m_LineThickness);

            var thirdPerson = player.PlayertScript;

            if (thirdPerson == null)
            {
                return;
            }

            //Drawing.Circle(w2s, 20, m_LineThickness);

            var bones = thirdPerson.GetComponentsInChildren<vp_DamageHandler>();

            Vector3[] bonePositions = new Vector3[bones.Length + 1];

            for (int i = 0; i < bones.Length; i++)
            {
                bonePositions[i] = bones[i].transform.position;
            }

            // head top bone
            var headTop = bonePositions[9] + Vector3.up * m_HeadHeight;
            bonePositions[bonePositions.Length - 1] = headTop;

            for (int i = 0; i < bonePositions.Length; i++)
            {
                bonePositions[i] = m_Camera.WorldToScreenPoint(bonePositions[i]);
                bonePositions[i].y = Screen.height - bonePositions[i].y;
            }

            DrawBones(bonePositions);
        }

        private void DrawBones(Vector3[] bones)
        {
            Drawing.Line(bones[6], bones[0], m_LineThickness);
            Drawing.Line(bones[0], bones[1], m_LineThickness);
            Drawing.Line(bones[1], bones[2], m_LineThickness);

            Drawing.Line(bones[6], bones[3], m_LineThickness);
            Drawing.Line(bones[3], bones[4], m_LineThickness);
            Drawing.Line(bones[4], bones[5], m_LineThickness);

            Drawing.Line(bones[6], bones[7], m_LineThickness);
            Drawing.Line(bones[7], bones[9], m_LineThickness);

            Drawing.Line(bones[9], bones[8], m_LineThickness);
            Drawing.Line(bones[9], bones[10], m_LineThickness);

            Vector3 headCenter = Vector3.Lerp(bones[9], bones[11], 0.5f);
            float height = bones[9].y - bones[11].y;
            float radius = height * 0.5f;
            Drawing.Circle(headCenter, radius, m_LineThickness);
        }
    }
}
