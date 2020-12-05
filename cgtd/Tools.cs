using System;
using System.Globalization;
using System.IO;

namespace cgtd
{
    public class Tools
    {
        public static bool IsDateValid(string key)
        {
            bool r = false;
            try
            {
                Convert.ToDateTime(key).ToString("yyyy/MM/dd");
                r = true;
            }
            catch (Exception)
            {
                r = false;
            }
            return r;
        }

        public static string ACConvert(string key)
        {
            string r = "ERROR";
            if (key == null)
                return key;

            if (key.IndexOf(" | ") > 0)
                r = key.Substring(0, key.IndexOf(" | "));
            else
                r = key;

            return r.ToUpper();
        }

        public static string ReverseACConvert(string key)
        {
            string r = "ERROR";
            if (key == null)
                return key;
            if (key.IndexOf(" | ") > 0)
                r = key.Substring(key.IndexOf(" | ") + 3, key.Length - key.IndexOf(" | ") - 3);
            else
                r = key;

            return r.Trim().ToUpper();
        }


        public static string FrontACConvert(string key)
        {
            string r = "ERROR";
            if (key == null)
                return key;
            if (key.IndexOf(" | ") > 0)
                r = key.Substring(0, key.IndexOf(" | "));
            else
                r = key;
            return r.Trim().ToUpper();
        }

        public static string StringDecimalIDToUS(string str)
        {
            if (str == null || str.Trim().Length < 1)
            {
                return "0";
            }
            else
            {
                CultureInfo cultureUS = new CultureInfo("en-US");
                CultureInfo cultureID = new CultureInfo("id-ID");
                decimal d = Convert.ToDecimal(str, cultureID);
                return Convert.ToString(d, cultureUS);
            }
        }

        public static bool IsValidAttachmentFile(string file)
        {
            bool r = false;

            if (
                file.EndsWith(".zip")
                || file.EndsWith(".jpg")
                || file.EndsWith(".jpeg")
                || file.EndsWith(".png")
                || file.EndsWith(".gif")
                || file.EndsWith(".doc")
                || file.EndsWith(".docx")
                || file.EndsWith(".xls")
                || file.EndsWith(".xlsx")
                || file.EndsWith(".pdf")
                || file.EndsWith(".bmp")
               )
                r = true;

            return r;
        }

        public static bool isAttachment(string type, string path)
        {
            bool r = false;
            int count = 0;
            if (Directory.Exists(path))
            {
                try
                {
                    DirectoryInfo Dir = new DirectoryInfo(path);
                    FileInfo[] FileList = Dir.GetFiles("*.*", SearchOption.AllDirectories);
                    foreach (FileInfo FI in FileList)
                    {
                        if (FI.Name.ToLower() != "thumbs.db")
                        {
                            count += 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            if (count > 0) r = true;
            return r;
        }
    }
}