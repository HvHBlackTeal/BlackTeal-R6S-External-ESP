using BlackTeal_R6S_External_ESP.Data;
using BlackTeal_R6S_External_ESP.Imports;
using BlackTeal_R6S_External_ESP.Memory;
using System;
using System.Diagnostics;

namespace BlackTeal_R6S_External_ESP
{
    public struct PlayerInfo
    {
        public float Health;
        public Vector3 Position;
        public Vector3 ScrenTop;
        public Vector3 w2sPos;
        public Vector3 HeadPos;
        public Vector3 w2sHead;
        public Vector3 w2sNeck;
        public Vector3 w2sRHand;
        public Vector3 w2sChest;
        public Vector3 w2sStomach;
        public Vector3 w2sPelvis;
    };

    public class MemoryHelper
    {
        private readonly Process _gameProcess;
        private readonly IntPtr GameHandle;

        public Process R6SProcess { get; private set; }
        public IntPtr R6SHandle { get; private set; }
        public int ClientModule { get; private set; }
        public int EngineModule { get; private set; }

        public MemoryHelper()
        {
            _gameProcess = GetGameProcess();
            R6SProcess = GetGameProcess();
            R6SHandle = GetGameHandle(_gameProcess);
            GameHandle = GetGameHandle(_gameProcess);
        }

        private Process GetGameProcess()
        {
            Process[] processList = Process.GetProcessesByName("RainbowSix");

            if (processList.Length < 0)
            {
                Console.WriteLine("Unable to find the Rainbow Six: Siege process. Returning an IntPtr.Zero!");
                return null;
            }

            return processList[0];
        }

        private IntPtr GetGameHandle(Process gameProcess)
        {
            return Kernel32.OpenProcess(Kernel32.ProcessAccessFlags.All, false, gameProcess.Id);
        }

        public long GetBaseAddress()
        {
            return _gameProcess.MainModule.BaseAddress.ToInt64();
        }

        public int displayWidth = 1920;
        public int displayHeight = 1080;

        public struct ViewMatrix_t
        {
            public Vector3 ViewRight;
            public uint filler0;
            public Vector3 ViewUp;
            public uint filler1;
            public Vector3 ViewForward;
            public uint filler2;
            public Vector3 ViewTranslation;
        };

        ~MemoryHelper()
        {
            Kernel32.CloseHandle(GameHandle);
        }

        public float rpmf(long address)
        {
            //The buffer for data that is going to be read from memory
            uint s = sizeof(float);
            byte[] buffer = new byte[s];

            //The actual RPM
            Kernel32.ReadProcessMemory(GameHandle, (IntPtr)address, buffer, s, out int br);

            //Return our buffer
            return BitConverter.ToSingle(buffer, 0);
        }

        public long rpml(long address)
        {
            //The buffer for data that is going to be read from memory
            uint s = sizeof(long);
            byte[] buffer = new byte[s];

            //The actual RPM
            Kernel32.ReadProcessMemory(GameHandle, (IntPtr)address, buffer, s, out int br);

            //Return our buffer
            return BitConverter.ToInt64(buffer, 0);
        }

        public Vector2 rpmv2(long address)
        {
            //The buffer for data that is going to be read from memory
            uint s = sizeof(float) * 2;
            byte[] buffer = new byte[s];

            //The actual RPM
            Kernel32.ReadProcessMemory(GameHandle, (IntPtr)address, buffer, s, out int br);

            //Return our buffer
            return new Vector2
            {
                x = BitConverter.ToSingle(buffer, 0),
                y = BitConverter.ToSingle(buffer, 4)
            };
        }

        public Vector3 rpmv3(long address)
        {
            //The buffer for data that is going to be read from memory
            uint s = sizeof(float) * 3;
            byte[] buffer = new byte[s];

            //The actual RPM
            Kernel32.ReadProcessMemory(GameHandle, (IntPtr)address, buffer, s, out int br);

            //Return our buffer
            return new Vector3
            {
                x = BitConverter.ToSingle(buffer, 0),
                y = BitConverter.ToSingle(buffer, 4),
                z = BitConverter.ToSingle(buffer, 8)
            };
        }

        public void UpdateAddresses()
        {
            Globals.pGameManager = rpml(GetBaseAddress() + Offsets.ADDRESS_GAMEMANAGER);
            Globals.pEntityList = rpml(Globals.pGameManager + Offsets.OFFSET_GAMEMANAGER_ENTITYLIST);
            Globals.pCamera = rpml(GetBaseAddress() + Offsets.ADDRESS_GAMEPROFILE);

            Globals.pCamera = rpml(Globals.pCamera + Offsets.GAMEPROFILE_CHAIN1);
            Globals.pCamera = rpml(Globals.pCamera + Offsets.GAMEPROFILE_CHAIN2);
            Globals.pCamera = rpml(Globals.pCamera + Offsets.GAMEPROFILE_CHAIN3);
        }

        public long GetEntity(int i)
        {
            long entityBase = rpml(Globals.pEntityList + (i * Offsets.OFFSET_GAMEMANAGER_ENTITY));
            return rpml(entityBase + 0x20);
        }

        public long GetEntity2(int i)
        {
            long entityBase = rpml(Globals.pEntityList + (i * Offsets.OFFSET_GAMEMANAGER_ENTITY));
            return entityBase;
        }

        public float GetEntityHealth(long entity)
        {
            //Entity info pointer from the Entity
            long EntityInfo = rpml(entity + Offsets.OFFSET_ENTITY_ENTITYINFO);
            //Main component pointer from the entity info
            long MainComponent = rpml(EntityInfo + Offsets.OFFSET_ENTITYINFO_MAINCOMPONENT);
            //Child component pointer form the main component
            long ChildComponent = rpml(MainComponent + Offsets.OFFSET_MAINCOMPONENT_CHILDCOMPONENT);

            //Finally health from the child component
            float kek = rpmf(ChildComponent + Offsets.OFFSET_CHILDCOMPONENT_HEALTH_FLOAT);
            kek *= 100;
            kek = (float)Math.Round(kek);
            return kek;
        }

        public Vector3 GetEntityFeetPosition(long entity)
        {
            //We get the feet position straight from the entity
            return rpmv3(entity + Offsets.OFFSET_ENTITY_FEET);
        }

        public Vector3 GetEntityHeadPosition(long entity)
        {
            //We get the head position straight from the entity
            return rpmv3(entity + Offsets.OFFSET_ENTITY_HEAD);
        }

        public Vector3 GetEntityNeckPosition(long entity)
        {
            //We get the neck position straight from the entity
            return rpmv3(entity + Offsets.OFFSET_ENTITY_NECK);
        }

        public Vector3 GetEntityHandPosition(long entity)
        {
            //We get the hand position straight from the entity
            return rpmv3(entity + Offsets.OFFSET_ENTITY_RIGHT_HAND);
        }

        public Vector3 GetEntityChestPosition(long entity)
        {
            //We get the chest position straight from the entity
            return rpmv3(entity + Offsets.OFFSET_ENTITY_CHEST);
        }

        public Vector3 GetEntityStomachPosition(long entity)
        {
            //We get the stomach position straight from the entity
            return rpmv3(entity + Offsets.OFFSET_ENTITY_STOMACH);
        }

        public Vector3 GetEntityPelvisPosition(long entity)
        {
            //We get the pelvis position straight from the entity
            return rpmv3(entity + Offsets.OFFSET_ENTITY_PELVIS);
        }

        public PlayerInfo GetAllEntityInfo(long entity)
        {
            PlayerInfo p;

            p.Health = GetEntityHealth(entity);
            p.Position = GetEntityFeetPosition(entity);
            p.w2sPos = WorldToScreen(p.Position);
            p.HeadPos = GetEntityHeadPosition(entity);
            p.w2sHead = WorldToScreen(p.HeadPos);
            p.ScrenTop = WorldToScreen(new Vector3
            {
                x = p.HeadPos.x,
                y = p.HeadPos.y,
                z = p.HeadPos.z + 0.2f
            });
            p.w2sNeck = WorldToScreen(GetEntityNeckPosition(entity));
            p.w2sRHand = WorldToScreen(GetEntityHandPosition(entity));
            p.w2sChest = WorldToScreen(GetEntityChestPosition(entity));
            p.w2sStomach = WorldToScreen(GetEntityStomachPosition(entity));
            p.w2sPelvis = WorldToScreen(GetEntityPelvisPosition(entity));

            return p;
        }

        public ViewMatrix_t GetViewMatrix()
        {
            uint s = sizeof(float) * 3 + sizeof(uint) + sizeof(float) * 3 + sizeof(uint) + sizeof(float) * 3 + sizeof(uint) + sizeof(float) * 3;
            byte[] buffer = new byte[s];
            Kernel32.ReadProcessMemory(GameHandle, (IntPtr)(Globals.pCamera + Offsets.OFFSET_CAMERA_VIEWRIGHT), buffer, s, out int r);
            //View translation comes straight from the camera
            return new ViewMatrix_t()
            {
                ViewRight = new Vector3
                {
                    x = BitConverter.ToSingle(buffer, 0),
                    y = BitConverter.ToSingle(buffer, 4),
                    z = BitConverter.ToSingle(buffer, 8)
                },
                filler0 = BitConverter.ToUInt32(buffer, 12),
                ViewUp = new Vector3
                {
                    x = BitConverter.ToSingle(buffer, 16),
                    y = BitConverter.ToSingle(buffer, 20),
                    z = BitConverter.ToSingle(buffer, 24)
                },
                filler1 = BitConverter.ToUInt32(buffer, 28),
                ViewForward = new Vector3
                {
                    x = BitConverter.ToSingle(buffer, 32),
                    y = BitConverter.ToSingle(buffer, 36),
                    z = BitConverter.ToSingle(buffer, 40)
                },
                filler2 = BitConverter.ToUInt32(buffer, 44),
                ViewTranslation = new Vector3
                {
                    x = BitConverter.ToSingle(buffer, 48),
                    y = BitConverter.ToSingle(buffer, 52),
                    z = BitConverter.ToSingle(buffer, 56)
                }
            };
        }

        public Vector2 GetFOV()
        {
            //FOV comes directly from the camera
            Vector2 fov = rpmv2(Globals.pCamera + Offsets.OFFSET_CAMERA_VIEWFOVX);
            fov.x = Math.Abs(fov.x);
            fov.y = Math.Abs(fov.y);
            return fov;
        }

        public Vector3 WorldToScreen(Vector3 position)
        {
            ViewMatrix_t VM = GetViewMatrix();
            Vector3 temp = new Vector3
            {
                x = position.x - VM.ViewTranslation.x,
                y = position.y - VM.ViewTranslation.y,
                z = position.z - VM.ViewTranslation.z
            };

            float x = temp.Dot(VM.ViewRight);
            float y = temp.Dot(VM.ViewUp);
            float z = temp.Dot(new Vector3
            {
                x = VM.ViewForward.x * -1,
                y = VM.ViewForward.y * -1,
                z = VM.ViewForward.z * -1
            });

            return new Vector3
            {
                x = displayWidth / 2 * (1 + x / GetFOV().x / z),
                y = displayHeight / 2 * (1 - y / GetFOV().y / z),
                z = z
            };
        }
    }
}
