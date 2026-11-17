using InTheHand.Net;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace RemoteController
{
    /// <summary>
    /// Talks to a running BrewCode terminal's BluetoothControlServer
    /// (Inventory\BluetoothControlServer.cs) over a direct RFCOMM connection - the two machines must
    /// already be paired at the OS level (Windows Settings > Bluetooth) before this will connect.
    /// Same request/response framing as RemoteControlClient's TCP path
    /// (RemoteControlClient.SendOverStream), just a different transport - no TLS here, since
    /// RFCOMM's pairing step is its own trust boundary.
    /// </summary>
    internal static class BluetoothControlClient
    {
        // Must match Inventory\BluetoothControlServer.cs's ServiceGuid exactly, or this app's service
        // won't be found on the paired device.
        private static readonly Guid ServiceGuid = new Guid("6f1b2c3d-4e5f-4a6b-8c7d-9e0f1a2b3c4d");

        /// <summary>Devices already paired with this machine at the OS level - RFCOMM can only
        /// connect to a paired device, so there's nothing to "discover" here that pairing hasn't
        /// already surfaced in Windows Settings.</summary>
        public static List<(string Name, string Address)> GetPairedDevices()
        {
            using (var client = new BluetoothClient())
            {
                return client.PairedDevices
                    .Select(d => (d.DeviceName, d.DeviceAddress.ToString()))
                    .ToList();
            }
        }

        public static string SendCommand(string deviceAddress, string token, string command, string arg, int timeoutMs = 8000)
        {
            BluetoothAddress address = BluetoothAddress.Parse(deviceAddress);

            using (var client = new BluetoothClient())
            {
                System.Threading.Tasks.Task connectTask = client.ConnectAsync(address, ServiceGuid);
                if (!connectTask.Wait(timeoutMs))
                    throw new TimeoutException($"Could not connect to {deviceAddress} within {timeoutMs}ms.");

                using (NetworkStream stream = client.GetStream())
                {
                    stream.ReadTimeout = timeoutMs;
                    stream.WriteTimeout = timeoutMs;
                    return RemoteControlClient.SendOverStream(stream, token, command, arg);
                }
            }
        }
    }
}
