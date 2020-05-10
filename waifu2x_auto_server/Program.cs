using System;
using System.IO;
using System.Collections.Generic;

namespace waifu2x_auto_server
{
class MainClass
{
        #region 変数
        static string INPUT_DIR_PATH = @"/home/ken/server/input";
        static string OUTPUT_DIR_PATH = @"/home/ken/server/output";

        struct Order
        {
            public string order;
            public string option;
        }
        #endregion

        public static void Main(string[] args)
    {
        var file_list = new List<string>();
        FileTimer(ref file_list);
            for (int i=0;i<file_list.Count;i++)
            {
                Console.WriteLine(file_list[i]);
            }

            Order com;
            foreach (string element in file_list)
            {
                com.order = @"waifu2x-converter-cpp";
                com.option = @"-i " + element + " -o " + OUTPUT_DIR_PATH +@"/"+ Path.GetFileName(element) + " -m noise-scale --noise-level 1 --scale-ratio 1.5 -c 0";
                ExecOrder(com);

                File.Delete(element);
            }

        }

        public static int FileTimer(ref List<string> files)
        {
        while (true)
        {
                try
                {
                    string[] names = Directory.GetFiles(INPUT_DIR_PATH, "*");
                    if (names.Length == 0)
                    {
                        Console.WriteLine("no files");
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

    }
}
