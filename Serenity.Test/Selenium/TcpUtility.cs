using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Serenity.Testing
{
    public static class TcpUtility
    {
        public static bool HasExistingTCPListener(int port)
        {
            // http://stackoverflow.com/questions/570098/in-c-how-to-check-if-a-tcp-port-is-available

            var props = IPGlobalProperties.GetIPGlobalProperties();
            var listeners = props.GetActiveTcpListeners();

            foreach (var endpoint in listeners)
            {
                if (endpoint.Port == port)
                    return true;
            }

            return false;
        }

        static public int GetAvailableTCPPort(int minValue, int maxValue)
        {
            var port = minValue;

            var props = IPGlobalProperties.GetIPGlobalProperties();
            var listeners = props.GetActiveTcpListeners();
            var ports = new HashSet<Int32>();

            foreach (var endpoint in listeners)
                ports.Add(endpoint.Port);

            port = minValue;

            while (ports.Contains(port))
            {
                port++;

                if (port > maxValue)
                    throw new Exception(String.Format("Couldn't find available port... (checked {0} - {1})!", minValue, maxValue));
            }

            return port;
        }
    }
}