using Packem.Mobile.Common.Interfaces.General;
using System.IO;
using System.Reflection;

namespace Packem.Mobile.Common.Services.General
{
    public class BarcodeService : IBarcodeService
    {
        public void PlaySound()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            Stream audioStream = assembly.GetManifestResourceStream("Packem.Mobile.Resources.Sounds.barcode-scanner-beep-sound.mp3");

            var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Load(audioStream);
            player.Play();
        }
    }
}