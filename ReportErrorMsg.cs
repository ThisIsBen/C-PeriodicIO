using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DataManagementApp
{
    class ReportErrorMsg
    {
       

        //To display error message to the user
        //Async output error message to a txt file in エラーメッセージ folder in NAS 
        public static async void outputErrorMsg(string errorSource, string errorMessage)
        {
            //output an error message to event log 
            /*
            EventLogHandler.outputLog(errorMessage);
            */
            try
            {
                using (StreamWriter writer = new StreamWriter(GlobalConstants.errorMessageTxtFolderPath + "\\" + DateTime.Now.ToString("yyyy_MM_dd--HH_mm_ss") + errorSource + "エラー.txt", false))
                {
                    await writer.WriteAsync(errorMessage);
                }
            }
            catch
            {
                return;
            }
           
           
        }

        //Display message with a pop-up message box
        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        //Display message with a pop-up window
        //[Use this function if the program is going to be shut down after displaying this pop-up message]
        //For example, use a pop-up window to display a error message when the exe is going to 
        //shut down automatically due to the error.
        public static void showMsgBox_Anyway(string message, string title)
        {
            //エラーメッセージを本アプリが使っている文字コードに変換し、日本語パスの文字化けを解決する。
            message = GlobalConstants.AppEncoding.GetString(GlobalConstants.AppEncoding.GetBytes(message));
            //Plan B: message = GlobalConstants.AppEncoding.GetString(Encoding.Convert(Encoding.UTF8, GlobalConstants.AppEncoding, Encoding.UTF8.GetBytes(message)));
            int resp = 0; WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, WTSGetActiveConsoleSessionId(), title, title.Length, message, message.Length, 0, 0, out resp, false);
        }

        [DllImport("kernel32.dll", SetLastError = true)] public static extern int WTSGetActiveConsoleSessionId();[DllImport("wtsapi32.dll", SetLastError = true)] public static extern bool WTSSendMessage(IntPtr hServer, int SessionId, String pTitle, int TitleLength, String pMessage, int MessageLength, int Style, int Timeout, out int pResponse, bool bWait);





        //Only display error message with a pop-up window when no pop-up window is being shown.
        //[Use this function if the program will still running after displaying this pop-up message.]
        //For example, when displaying "画像を処理できない"、"カメラが移動された"　error message
        //with a pop-up window. 
        //Because these 画像状態エラー are likely to occur every second until the problem is fixed.

        //Record the ID of the csrss that shows message box
        private static int showMsgBox_csrssIndex = -1;
        public static void showMsgBox_IfNotShown(string message, string title)
        {

            //We show message box if it is the first time error message box is shown.
            if (showMsgBox_csrssIndex == -1)
            {


                //show the message box 
                showMsgBox_Anyway(message, title);

                //Get the Process ID of the csrss.exe that shows the error message box
                Process[] csrssProcess = Process.GetProcessesByName("csrss");


                for (byte i = 0; i < csrssProcess.Length; i++)
                {
                    if (csrssProcess[i].MainWindowTitle != "")
                    {


                        showMsgBox_csrssIndex = csrssProcess[i].Id;

                    }


                }

            }

            //If it is not the first time error message box is shown,
            //we check if the csrss.exe that shows the error message box is showing an error message box right now,
            //if so, we do not show the message box.
            //if not, we show the message box.
            else
            {
                Process showMsgBox_csrssProcess = Process.GetProcessById(showMsgBox_csrssIndex);


                if (showMsgBox_csrssProcess.MainWindowTitle == "")
                {
                    showMsgBox_Anyway(message, title);

                }




            }


        }

        //Display message with a pop-up window if currently no pop-up window is shown and upload the error message to NAS
        public static void showMsgBoxIfNotShown_UploadErrMsgToNAS(string errorMessage, string errorMessageBoxTitle, string NASErrorTxtFileName)
        {
            // MessageBoxでオペレーターに知らせ、再起動してもらう。
            //show the error message box to inform the user if the same message box is not being shown 
            ReportErrorMsg.showMsgBox_IfNotShown(errorMessage, errorMessageBoxTitle);

            //output the error message  
            //to the アプリ_エラーメッセージ folder on NAS
            ReportErrorMsg.outputErrorMsg(NASErrorTxtFileName, errorMessage);

            Console.WriteLine(errorMessage);
        }
    }
}
