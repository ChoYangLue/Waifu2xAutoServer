using System;
using System.IO;
using System.Collections.Generic;
using System.Net;

namespace waifu2x_auto_server
{
class MainClass
{
        #region 変数
        static string INPUT_DIR_PATH = @"/home/ken/server/input";
        static string OUTPUT_DIR_PATH = @"/home/ken/server/output";
        static string ORDER_OPTION_PATH = @"/home/ken/server/waifu2x_option.txt";

        struct Order
        {
            public string order;
            public string option;
        }
        #endregion

        public static void Main(string[] args)
    {

            while (true)
            {
                var file_list = new List<string>();
                FileTimer(ref file_list);

                for (int i = 0; i < file_list.Count; i++)
                {
                    Console.WriteLine(file_list[i]);
                    Console.WriteLine(IsImageFile(file_list[i]));
                }
                string option_str = LoadOrderOption(ORDER_OPTION_PATH);
                Console.WriteLine(option_str);

                Order com;
                foreach (string element in file_list)
                {
                    com.order = @"waifu2x-converter-cpp";
                    com.option = @"-i " + element + " -o " + OUTPUT_DIR_PATH + @"/" + Path.GetFileName(element) + option_str;
                    ExecOrder(com);

                    File.Delete(element);
                }
            }

        }

        public static int FileTimer(ref List<string> files)
        {
            string ip_string = GetServerAddress();
            while (true)
        {
                try
                {
                    string[] names = Directory.GetFiles(INPUT_DIR_PATH, "*");
                    if (names.Length == 0)
                    {
                        Console.WriteLine("no files");
                        Console.WriteLine("server IP:"+ip_string);
                        System.Threading.Thread.Sleep(1000);
                    }
                    else
                    {
                    foreach (string name in names)
                        {
                        files.Add(name);
                        }
                        return 0;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return -1;
                }
            }
        }

        private static int ExecOrder(Order com)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = com.order;
            proc.StartInfo.Arguments = com.option;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            if (!proc.Start())
            {
                Console.WriteLine("Error starting");
                return -1;
            }
            StreamReader reader = proc.StandardError;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
            proc.Close();

            return 0;
        }

        private static string GetServerAddress()
        {
            String hostName = Dns.GetHostName();    // 自身のホスト名を取得
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            foreach (IPAddress address in addresses)
            {
                // IPv4 のみを追加する
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }
            }
            return "IP Address is not found";
        }

        private static bool IsImageFile(string fullPath)
        {
            string[] extList = { ".jpg", ".JPG", ".png", ".bmp", ".webp"};
            string ext = System.IO.Path.GetExtension(fullPath);
            foreach (string en in extList)
            {
                if (ext == en) return true;
            }
            return false;
        }

        private static string LoadOrderOption(string filePath)
        {
            string ret_val = " -m noise-scale --noise-level 1 --scale-ratio 1.5 -c 0";

            if (File.Exists(filePath) == false)
            {
                Console.WriteLine(filePath + " is not found. please check path");
                return ret_val;
            }
            var encoding = System.Text.Encoding.GetEncoding("UTF-8");

            var reader = new System.IO.StreamReader(filePath, encoding);
            ret_val = reader.ReadLine();
            reader.Close();

            return ret_val;
        }


    }
}
