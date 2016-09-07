using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NCP_CallRecording.Configuration
{
    public class Settings
    {
        public static string IPC_ADDRESS
        {
            get
            {
                if(System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(x => x =="IPC_ADDRESS").Count() == 1)
                {
                    return (string)System.Configuration.ConfigurationManager.AppSettings["IPC_ADDRESS"];
                }
                else
                {
                    return "";
                }
            }
        }

        public static string ROOT_FILE_FOLDER
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(x => x == "ROOT_FILE_FOLDER").Count() == 1)
                {
                    return (string)System.Configuration.ConfigurationManager.AppSettings["ROOT_FILE_FOLDER"];
                }
                else
                {
                    return "";
                }
            }
        }

        public static string ROOT_TEMP_FOLDER
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(x => x == "ROOT_TEMP_FOLDER").Count() == 1)
                {
                    return (string)System.Configuration.ConfigurationManager.AppSettings["ROOT_TEMP_FOLDER"];
                }
                else
                {
                    return "";
                }
            }
        }

        public static string KEY_LOC
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(x => x == "KEY_LOC").Count() == 1)
                {
                    return (string)System.Configuration.ConfigurationManager.AppSettings["KEY_LOC"];
                }
                else
                {
                    return "";
                }
            }
        }

        public static string SMTP_HOST
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(x => x == "SMTP_HOST").Count() == 1)
                {
                    return (string)System.Configuration.ConfigurationManager.AppSettings["SMTP_HOST"];
                }
                else
                {
                    return "";
                }
            }
        }

        public static Int16 SMTP_PORT
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(x => x == "SMTP_PORT").Count() == 1)
                {
                    return Int16.Parse((string)System.Configuration.ConfigurationManager.AppSettings["SMTP_PORT"]);
                }
                else
                {
                    return (Int16)25;
                }
            }
        }

        public static string FROM_ADDRESS
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(x => x == "FROM_ADDRESS").Count() == 1)
                {
                    return (string)System.Configuration.ConfigurationManager.AppSettings["FROM_ADDRESS"];
                }
                else
                {
                    return "";
                }
            }
        }

        public static string FROM_NAME
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(x => x == "FROM_NAME").Count() == 1)
                {
                    return (string)System.Configuration.ConfigurationManager.AppSettings["FROM_NAME"];
                }
                else
                {
                    return "";
                }
            }
        }

        public static string FROM_ACCOUNT
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(x => x == "FROM_ACCOUNT").Count() == 1)
                {
                    return (string)System.Configuration.ConfigurationManager.AppSettings["FROM_ACCOUNT"];
                }
                else
                {
                    return "";
                }
            }
        }

        public static string FROM_PASSWORD
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(x => x == "FROM_PASSWORD").Count() == 1)
                {
                    return (string)System.Configuration.ConfigurationManager.AppSettings["FROM_PASSWORD"];
                }
                else
                {
                    return "";
                }
            }
        }

        public static string TO_ADDRESS
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(x => x == "TO_ADDRESS").Count() == 1)
                {
                    return (string)System.Configuration.ConfigurationManager.AppSettings["TO_ADDRESS"];
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
