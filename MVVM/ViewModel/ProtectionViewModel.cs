using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ByteShield.Core;
using ByteShield.MVVM.Model;

namespace ByteShield.MVVM.ViewModel
{
    internal class ProtectionViewModel : ObservableObject
    {
        public ObservableCollection<ServerModel> Servers { get; set; }

        private string _connectionStatus;

        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                _connectionStatus = value;
                OnPropertyChanged();
            }

        }

        public RelayCommand ConnectCommand { get; set; }

        public ProtectionViewModel()
        {
            Servers = new ObservableCollection<ServerModel>();
            for (int i = 0; i < 10; i++)
            {
                Servers.Add(new ServerModel
                {
                    Country = "USA"
                });
            }

            ConnectCommand = new RelayCommand(o =>
            {
                //ConnectionStatus = "Connecting...";

                Task.Run(() =>
                {
                    ConnectionStatus = "Connecting...";
                    var process = new Process();
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                    process.StartInfo.ArgumentList.Add(@"/c rasdial MyServer vpnbook hu86c9k /phonebook:./VPN/VPN.pbk");

                    process.Start();
                    process.WaitForExit();

                    switch (process.ExitCode)
                    {
                        case 0:
                            Debug.WriteLine("Success!");
                            ConnectionStatus = "Connected!";
                            break;
                        case 691:
                            Debug.WriteLine("Wrong credentials!");
                            ConnectionStatus = "Wrong Credentials";
                            break;
                        default:
                            Debug.WriteLine($"Error: {process.ExitCode}");
                            ConnectionStatus = $"Error: {process.ExitCode}";
                            break;
                    }
                });
                //ServerBuilder();
            });

        }

        private void ServerBuilder()
        {
            var address = "US178.vpnbook.com";
            var FolderPath = $"{Directory.GetCurrentDirectory()}/VPN";
            var PbkPath = $"{FolderPath}/{address}.pbk";

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            if (File.Exists(PbkPath))
            {
                MessageBox.Show("Connection Already Exists!");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("[MyServer]");
            sb.AppendLine("MEDIA=rastapi");
            sb.AppendLine("Port=VPN2-0");
            sb.AppendLine("Device=WAN Miniport (IKEv2)");
            sb.AppendLine("DEVICE=vpn");
            sb.AppendLine($"PhoneNumber={address}");
            File.WriteAllText(PbkPath, sb.ToString());


        }
    }
}