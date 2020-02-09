namespace BlackTeal_R6S_External_ESP.Memory
{
    public struct Offsets
    {
        //Game ->
        public const int ADDRESS_GAMEMANAGER = 0x52B2240;
        public const int ADDRESS_GAMEPROFILE = 0x52A1AE0;

        //Game -> GameProfile ->
        public const int GAMEPROFILE_CHAIN1 = 0x78;
        public const int GAMEPROFILE_CHAIN2 = 0x0;
        public const int GAMEPROFILE_CHAIN3 = 0x130;

        //Game -> GameProfile -> Camera ->
        public const int OFFSET_CAMERA_VIEWRIGHT = 0x1C0;
        public const int OFFSET_CAMERA_VIEWUP = 0x1D0;
        public const int OFFSET_CAMERA_VIEWFORWARD = 0x1E0;
        public const int OFFSET_CAMERA_VIEWTRANSLATION = 0x1F0;
        public const int OFFSET_CAMERA_VIEWFOVX = 0x380;
        public const int OFFSET_CAMERA_VIEWFOVY = 0x384;

        //Game -> GameManager ->
        public const int OFFSET_GAMEMANAGER_ENTITYLIST = 0x1C8;
        public const int OFFSET_GAMEMANAGER_ENTITY = 0x8; //Size

        //Game -> GameManager -> EntityList -> INDEX -> Entity ->
        public const int OFFSET_ENTITY_ENTITYINFO = 0x28;
        public const int OFFSET_ENTITYINFO_MAINCOMPONENT = 0xD8; // EntityInfo ->
        public const int OFFSET_MAINCOMPONENT_CHILDCOMPONENT = 0x8; // MainComponent ->
        public const int OFFSET_CHILDCOMPONENT_HEALTH_INT = 0x148; // ChildComponent ->
        public const int OFFSET_CHILDCOMPONENT_HEALTH_FLOAT = 0x238; // ChildComponent ->

        //Game -> GameManager -> EntityList -> INDEX ->
        public const int OFFSET_ENTITY_REF = 0x20;
        public const int OFFSET_ENTITY_HEAD = 0x670;  //670
        public const int OFFSET_ENTITY_NECK = 0xF70;
        public const int OFFSET_ENTITY_RIGHT_HAND = 0x6A0;
        public const int OFFSET_ENTITY_CHEST = 0xFB0;
        public const int OFFSET_ENTITY_STOMACH = 0xF90;
        public const int OFFSET_ENTITY_PELVIS = 0xFD0;
        public const int OFFSET_ENTITY_FEET = 0x6C0;
    }
}
