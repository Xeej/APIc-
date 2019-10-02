using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using static System.Random;



namespace WindowsFormsApp1_lab1_OS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //format getnamecomputer
        enum COMPUTER_NAME_FORMAT
        {
            ComputerNameNetBIOS,
            ComputerNameDnsHostname,
            ComputerNameDnsDomain,
            ComputerNameDnsFullyQualified,
            ComputerNamePhysicalNetBIOS,
            ComputerNamePhysicalDnsHostname,
            ComputerNamePhysicalDnsDomain,
            ComputerNamePhysicalDnsFullyQualified
        }
        const uint SPI_GETCURSORSHADOW = 0x101A;
        const uint SPI_SETCURSORSHADOW = 0x101B;
        const int COLOR_WINDOW = 5;

        //структура батареи
        private enum ACLineStatus : byte
        {
            Offline = 0,
            Online = 1,
            Unknown = 255
        }

        private enum BatteryFlag : byte
        {
            High = 1,
            Low = 2,
            Critical = 4,
            charging = 8,
            Charging = 9,
            NoSystemBattery = 128,
            Unknown = 255
        }

        private struct SystemPowerStatus
        {
            public ACLineStatus LineStatus;
            public BatteryFlag flgBattery;
            public Byte BatteryLifePercent;
            public Byte Reserved1;
            public Int32 BatteryLifeTime;
            public Int32 BatteryFullLifeTime;
        }



        //Стуктура получения информации операционной системы
        [StructLayout(LayoutKind.Sequential)]
        public class OSVersionInfo
        {
            public uint dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public String szCSDVersion;
        }
        //формат метриков
        public enum SystemMetric
        {
            SM_CXFULLSCREEN = 16,                  
            SM_CYFULLSCREEN = 17
        }

        //cтруктура указателя мыши лучше свой
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        //подключение библиотеки имя компьютера
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetComputerNameEx(COMPUTER_NAME_FORMAT NameType,StringBuilder lpBuffer, ref uint lpnSize);
        //подключение библиотеки имя пользователя
        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetUserName(System.Text.StringBuilder sb, ref Int32 length);
        //подключение библиотеки адрес системной директории
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetSystemDirectory([Out] StringBuilder lpBuffer, uint uSize);
        //подключение библиотеки адрес системной директории
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetWindowsDirectory(StringBuilder lpBuffer,uint uSize);
        //подключение библиотеки адрес каталога временных файлов
        [DllImport("kernel32.dll")]
        static extern uint GetTempPath(uint nBufferLength,[Out] StringBuilder lpBuffer);
        //подключение библиотеки версия винды
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern bool GetVersionEx([In, Out] OSVersionInfo info);
        //подключение библиотеки метрик
        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(SystemMetric smIndex);
        //подключение библиотеки параметров
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref uint pvParam, uint fWinIni);
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SystemParametersInfoSet(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);
        //подключение библиотеки изменения цвета
        [DllImport("user32.dll")]
        static extern uint GetSysColor(int nIndex);
        [DllImport("user32.dll")]
        static extern bool SetSysColors(int cElements, int[] lpaElements,uint[] lpaRgbValues);
        //подключение библиотеки времени
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME time);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetSystemTime(ref SYSTEMTIME time);

        [DllImport("kernel32.dll")]
        static extern bool GetSystemPowerStatus(out SystemPowerStatus sps);

        [DllImport("user32.dll")]
        static extern bool SwapMouseButton(bool fSwap);


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        bool fSwap = false;


        


        private void Form1_Load(object sender, EventArgs e)
        {
            StringBuilder name = new StringBuilder(260);
            uint size = 260;
            GetComputerNameEx(COMPUTER_NAME_FORMAT.ComputerNamePhysicalDnsHostname, name, ref size);
            textBox1.Text = name.ToString();

            int nsize = 64;
            GetUserName(name, ref nsize);
            textBox2.Text = name.ToString();

            
            StringBuilder sbSystemDir = new StringBuilder(256);
            size = GetSystemDirectory(sbSystemDir, 256);
            textBox3.Text = sbSystemDir.ToString();

            nsize= 255;
            StringBuilder sb = new StringBuilder(nsize);
            int len = (int)GetWindowsDirectory(sb, size);
            textBox4.Text = sb.ToString(0, len);

            size = 255;
            GetTempPath(size, sb);
            textBox5.Text = sb.ToString();

            OSVersionInfo version = new OSVersionInfo();
            version.dwOSVersionInfoSize = (uint)Marshal.SizeOf(version);
            GetVersionEx(version);
            textBox6.Text = version.dwMajorVersion.ToString()+'.'+version.dwMinorVersion.ToString() ;

            int x, y;
            x = GetSystemMetrics(SystemMetric.SM_CXFULLSCREEN);
            y = GetSystemMetrics(SystemMetric.SM_CYFULLSCREEN);
            textBox7.Text = x.ToString() + 'x' + y.ToString();

            uint pvParam=0;
            SystemParametersInfo(SPI_GETCURSORSHADOW, 0, ref pvParam, 0);
            if (pvParam == 1)
                button1.Text = "Отключить тень курсора";
            else
                button1.Text = "Включить тень курсора";

            button2.Text = "Цвет COLOR_WINDOW:" + GetSysColor(COLOR_WINDOW).ToString("X2");


            SYSTEMTIME time = new SYSTEMTIME();
            //получаем текущее время
            GetSystemTime(ref time);
            textBox8.Text = time.wDay.ToString() + "." + time.wMonth.ToString("x2") + "." + time.wYear.ToString()+"   "+time.wHour.ToString()+":"+time.wMinute.ToString();



            SystemPowerStatus SPS = new SystemPowerStatus(); ;
            GetSystemPowerStatus(out SPS);
            label9.Text = "Заряд батареи - "+SPS.flgBattery.ToString();


        }
        
        private void Button1_Click(object sender, EventArgs e)
        {

            uint pvParam = 0;
            SystemParametersInfo(SPI_GETCURSORSHADOW, 0, ref pvParam, 0);
            if (pvParam == 1)
                SystemParametersInfoSet(SPI_SETCURSORSHADOW, 0, 0, 0); 
            else
                SystemParametersInfoSet(SPI_SETCURSORSHADOW, 0, 1, 0); 
             pvParam = 0;
            SystemParametersInfo(SPI_GETCURSORSHADOW, 0, ref pvParam, 0);
            if (pvParam == 1)
                button1.Text = "Отключить тень курсора";
            else
                button1.Text = "Включить тень курсора";

        }

        private void Button2_Click(object sender, EventArgs e)
        {

           // uint color=GetSysColor(COLOR_WINDOW);
            uint r = 0xFFFFFF;
            int[] el = {COLOR_WINDOW};
            uint[] cl = {r};
            SetSysColors(1, el, cl);
            
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            fSwap = !fSwap;
            SwapMouseButton(fSwap);
            
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            POINT p;
            GetCursorPos(out p);

            label10.Text = "Позиция мыши Х.У:       " + p.X.ToString() + "." + p.Y.ToString();
             
        }

        private void Button4_Click(object sender, EventArgs e)
        {
                        SetCursorPos(Convert.ToInt32(textBox9.Text.ToString()), Convert.ToInt32(textBox10.Text.ToString()));
        }
    }
}