using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Windows;

using static System.Array;

namespace DroneQualIT.Affichage
{
    public sealed partial class App : Application, IDisposable
    {
        private Process Process { get; set; }

        public void Dispose()
        {
            if (!Process.HasExited)
                Process?.Kill();

            Process?.Dispose();
        }

        private void OnMessageQueueNotRunning(string message)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            MessageBox.Show(message);
            Shutdown();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!(Find(ServiceController.GetServices(), service => service.ServiceName == "MSMQ") is ServiceController controller))
            {
                OnMessageQueueNotRunning("MessageQueue n'est pas installé sur cet ordinateur.");
                return;
            }
            else if (controller.Status != ServiceControllerStatus.Running)
            {
                OnMessageQueueNotRunning("MessageQueue n'est pas activé en ce moment.");
                return;
            }

            Process = Process.Start("DroneQualITLecture.exe");
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Dispose();
            base.OnExit(e);
        }
    }
}
