namespace Krisnat
{
    [System.Serializable]
    public class SettingsData
    {
        // Audio
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
        public bool mute = false;

        // Video
        public int qualityIndex = 2;
        public int resolutionIndex = 0;
        public int fpsIndex = 1;
        public bool fullscreen = true;

        // Gameplay
        public bool damagePopUps = true;
        public bool dashAimingMouse = true;
    }
}
