using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace アプリ
{
    class CreateReadFiles
    {
        

        //Write string to the designated file
        //ファイルにデータを出力することが成功した場合、trueを返す。
        //失敗した場合、falseを返す。
        public static bool outputStringsToFile(string outputPath, string outputContent, string errorMessage, string fileType)
        {
            //Retry  when error occurs
            for (int retryTimes = 1; retryTimes <= GlobalConstants.retryTimesLimit; retryTimes++)
            {
                try
                {

                    File.AppendAllText(outputPath, outputContent, GlobalConstants.AppEncoding);

                    //ファイルにデータを出力することが成功したため、trueを返す。
                    return true;
                }
                catch (Exception e)
                {

                    // If it's still within retry times limit
                    if (retryTimes < GlobalConstants.retryTimesLimit)
                    {
                        //wait a while before starting next retry
                        Thread.Sleep(GlobalConstants.retryTimeInterval);
                    }

                    //If it has reached the retry limit
                    else
                    {
                        errorMessage = errorMessage + "\n\nエラーメッセージ：\n" + e.Message;
                        ReportErrorMsg.showMsgBox_IfNotShown(errorMessage, "システム " +  GlobalConstants.PIInspectTarget + "カメラの" + fileType + "出力エラー");

                        //output the error message  
                        //to the アプリ_エラーメッセージ folder in NAS
                        ReportErrorMsg.outputErrorMsg(fileType + "出力", errorMessage);

                        //wait a while to let the program output the 3rd retry error txt message
                        //The 3rd retry error message can not be output without this wait 
                        Thread.Sleep(1000);

                        
                    }
                }
            }
            //End this function becasue we have reached the limit of retry.
            return false;
        }





        //Get the number of lines in a file
        //読み込み成功した場合、ファイル内の行数を返す
        //読み込み失敗した場合、-1を返す
        public static int getNumOf_NonBlank_LinesInAFile(string filePath,string errorMessage,string fileType)
        {
            //Retry  when error occurs
            for (int retryTimes = 1; retryTimes <= GlobalConstants.retryTimesLimit; retryTimes++)
            {
                try
                {
                    return File.ReadLines(filePath).Count(line => !string.IsNullOrWhiteSpace(line));
                }
                catch (Exception e)
                {

                    // If it's still within retry times limit
                    if (retryTimes < GlobalConstants.retryTimesLimit)
                    {
                        //wait a while before starting next retry
                        Thread.Sleep(GlobalConstants.retryTimeInterval);
                    }

                    //If it has reached the retry limit
                    else
                    {
                        errorMessage = errorMessage + "\n\nエラーメッセージ：\n" + e.Message;
                        ReportErrorMsg.showMsgBox_IfNotShown(errorMessage, "システム " + GlobalConstants.PIInspectTarget + "カメラの" + fileType + "読み込みエラー");

                        //output the error message  
                        //to the アプリ_エラーメッセージ folder in NAS
                        ReportErrorMsg.outputErrorMsg(fileType + "読み込み", errorMessage);

                        //wait a while to let the program output the 3rd retry error txt message
                        //The 3rd retry error message can not be output without this wait 
                        Thread.Sleep(1000);

                        
                    }
                }
            }
            //End this function becasue we have reached the limit of retry.
            return -1;

        }




        //Read in all the lines in a file with a specific encoding
        //読み込み成功した場合、読み込んだ内容を返す
        //読み込み失敗した場合、nullを返す
        public static IEnumerable<string> readAllLinesOfAFile(string filePath, string errorMessage, string fileType)
        {
            //Retry  when error occurs
            for (int retryTimes = 1; retryTimes <= GlobalConstants.retryTimesLimit; retryTimes++)
            {
                try
                {
                    return File.ReadLines(filePath, GlobalConstants.AppEncoding);
                }
                catch (Exception e)
                {

                    // If it's still within retry times limit
                    if (retryTimes < GlobalConstants.retryTimesLimit)
                    {
                        //wait a while before starting next retry
                        Thread.Sleep(GlobalConstants.retryTimeInterval);
                    }

                    //If it has reached the retry limit
                    else
                    {
                        errorMessage = errorMessage + "\n\nエラーメッセージ：\n" + e.Message;
                        ReportErrorMsg.showMsgBox_IfNotShown(errorMessage, "システム " + GlobalConstants.PIInspectTarget + "カメラの" + fileType + "読み込みエラー");

                        //output the error message  
                        //to the アプリ_エラーメッセージ folder in NAS
                        ReportErrorMsg.outputErrorMsg(fileType + "読み込み", errorMessage);

                        //wait a while to let the program output the 3rd retry error txt message
                        //The 3rd retry error message can not be output without this wait 
                        Thread.Sleep(1000);


                    }
                }
            }
            //End this function becasue we have reached the limit of retry.
            return null;
            
        }




        //Read the Last line of a file and return it.
        //成功した場合、読み込んだ内容を返す。
        //失敗した場合、"読み込み失敗"を返す。
        public static string readLastLineOfAFile(string filePath,string fileType)
        {
            //Retry  when error occurs
            for (int retryTimes = 1; retryTimes <= GlobalConstants.retryTimesLimit; retryTimes++)
            {
                try
                {
                    return File.ReadLines(filePath).Last();
                }
                catch (Exception e)
                {
                    
                    // If it's still within retry times limit
                    if (retryTimes < GlobalConstants.retryTimesLimit)
                    {
                        //wait a while before starting next retry
                        Thread.Sleep(GlobalConstants.retryTimeInterval);
                    }

                    //If it has reached the retry limit
                    else
                    {
                        string errorMessage =  GlobalConstants.PIInspectTarget + "カメラの" + fileType+"("+filePath + ")の最後のデータ行を読み込むことができなかった。" +
                            "\n\nエラーメッセージ：\n" + e.Message;
                        ReportErrorMsg.showMsgBox_IfNotShown(errorMessage, "システム " + GlobalConstants.PIInspectTarget + "カメラの"+ fileType + "の最後のデータ行読み込みエラー");

                        //output the error message  
                        //to the アプリ_エラーメッセージ folder in NAS
                        ReportErrorMsg.outputErrorMsg(fileType + "最後のデータ行読み込み", errorMessage);

                        //wait a while to let the program output the 3rd retry error txt message
                        //The 3rd retry error message can not be output without this wait 
                        Thread.Sleep(1000);

                       
                    }

                    
                }
            }

            //End this function becasue we have reached the limit of retry.
            return "読み込み失敗";

        }



        //最後のデータ行を現在のデータで上書きする。
        //成功した場合、出力した内容を返す。
        //失敗した場合、"上書き失敗"を返す。
        //Notice1:パラメーターのlastLineInFileは最後に"\n"が付いていることが必要である。
        //Notice2:パラメーターのnewLastLineは最後に"\n"を付けないでください。
        public static string overwriteLastLineOfAFile(string filePath, string lastLineInFile, string newLastLine, string fileType)
        {
            

            //最後のデータ行の開始位置を計算する。
            int lastLineInFileByteLen = GlobalConstants.AppEncoding.GetBytes(lastLineInFile).Length;

            //出力しようとするデータの長さを取得する。
            int newLastLineByteLen = GlobalConstants.AppEncoding.GetBytes(newLastLine).Length;

            //最後のデータ行と現在出力しようとするデータの長さと比較し、
            //最後のデータ行の方が長い場合、" "(空白)を現在のデータの後ろに追加し、
            //現在のデータで上書きできない部分を上書きする。
            int lenDiff = lastLineInFileByteLen - newLastLineByteLen;
            if (lenDiff > 0)
            {
                lastLineInFileByteLen -= 1;
                for (int i = 0; i < lenDiff; i++)
                {
                    newLastLine += " ";
                }
            }
            //改行文字を入れる
            newLastLine += "\n";



            //Retry  when error occurs
            for (int retryTimes = 1; retryTimes <= GlobalConstants.retryTimesLimit; retryTimes++)
            {

                try
                {
                    using (FileStream fsSource = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                    {

                        using (StreamWriter writer = new StreamWriter(fsSource))
                        {

                            //Move the pointer to the starting point of the last line in the file.
                            int lastLine_StartPoint = -2;
                            fsSource.Seek(lastLine_StartPoint, SeekOrigin.End);

                            int ByteReadFromFile = 1;
                            int newLineASCII = 10;
                            while (ByteReadFromFile != newLineASCII)
                            {
                                ByteReadFromFile = fsSource.ReadByte();


                                lastLine_StartPoint--;
                                fsSource.Seek(lastLine_StartPoint, SeekOrigin.End);
                            }
                            lastLine_StartPoint += 2;
                            fsSource.Seek(lastLine_StartPoint, SeekOrigin.End);


                            // Overwrite the last line with the new content.
                            writer.Write(newLastLine);
                        }
                    }

                    //上書きが成功したため、出力した内容を返す。
                    return newLastLine;


                }
                catch (Exception e)
                {

                    // If it's still within retry times limit
                    if (retryTimes < GlobalConstants.retryTimesLimit)
                    {
                        //wait a while before starting next retry
                        Thread.Sleep(GlobalConstants.retryTimeInterval);
                    }

                    //If it has reached the retry limit
                    else
                    {

                        string errorMessage = GlobalConstants.PIInspectTarget + "カメラの" + fileType + "(" + filePath + ")の最後のデータ行を上書きすることができなかった。" +
                        "\n管理者に連絡してください。\n" +
                        "管理者への解決手順：Step 1システムを停止する。\nStep 2 NAS上の" + filePath + "にアクセスできるのかを確認してください。" +
                        "\n\nエラーメッセージ：\n" + e.Message;
                        ReportErrorMsg.showMsgBox_IfNotShown(errorMessage, "システム " + GlobalConstants.PIInspectTarget + "カメラの" + fileType + "の最後のデータ行上書き失敗エラー");

                        
                        //wait a while to let the program output the 3rd retry error txt message
                        //The 3rd retry error message can not be output without this wait 
                        Thread.Sleep(1000);

                    }

                }

            }


            //End this function becasue we have reached the limit of retry.
            return "上書き失敗";


        }

    }
}
