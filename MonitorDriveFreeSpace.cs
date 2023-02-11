using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataManagementApp
{
    //[「古い画像削除進捗非常に遅い」により、カメラフォルダードライブの容量不足対策]
    class MonitorDriveFreeSpace
    {
        //カメラフォルダードライブの空き容量を定期的に確認するためのTimerを宣言する
        private static Timer checkDriveFreeSpaceTimer;


        //カメラフォルダードライブの容量不足防止機能:
        //カメラフォルダードライブの空き容量のを開始させる。
        public static void startCheckDriveFreeSpaceTimer(int monitorInterval)
        {
            int checkDriveFreeSpaceTimer_Interval = monitorInterval * 60 * 1000;

            //定期的に履歴ファイルを生成するTimerを起動する
            checkDriveFreeSpaceTimer = new Timer(
                _ => checkDriveFreeSpace(),
                null,
                checkDriveFreeSpaceTimer_Interval,
                checkDriveFreeSpaceTimer_Interval);
        }



        //カメラフォルダードライブの空き容量が「ドライブの空き容量限界」閾値以下の場合、
        //「古い画像削除進捗非常に遅い」エラーメッセージを表示し、
        //管理者に上記の[カメラフォルダーから画像をDeleteListに移動する周期]、
        //[画像を削除する周期]、[一回削除する枚数] を調整して、削除進捗を加速させてもらう。
        public static void checkDriveFreeSpace()
        {
            long currentFreeSpace = GlobalConstants.cameraFolderDrive.AvailableFreeSpace;

            //空き容量の比率を計算
            double freeSpaceRate = Math.Round((100.0 * currentFreeSpace / GlobalConstants.cameraFolderDriveSpace), 3);



            if (freeSpaceRate <= GlobalConstants.diskFreeSpaceLimit)
            {
                //display this error message to inform the user
                string errorMessage = "カメラフォルダー内の古い画像の削除進捗が非常に遅いため、\nカメラフォルダードライブの空き容量が不足している。" + "\n\n停止ボタンを押して、\nを停止して管理者に連絡してください。" +
                    "\n\n\nエラーの原因：\n空き容量:"+ freeSpaceRate + "%が\n設定ファイルApp7Settings.txtに設定された「ドライブの空き容量限界」：" + GlobalConstants.diskFreeSpaceLimit + "%より以下になっているためである。" +
                    "\n\n\n管理者への解決手順：\n設定ファイルの[カメラフォルダーから画像をDeleteListに移動する周期]、\n" +
                    "[画像を削除する周期]、[一回削除する枚数] の設定を調整することで、\n削除進捗を加速させてください。"; 
                string errorMessageBoxTitle = " " + GlobalConstants.TCPSocketServerName + " カメラフォルダードライブの空き容量不足";
                string NASErrorTxtFileName = "カメラフォルダードライブの空き容量不足";
                //show the error message box (if currently no error message box is shown) to inform the user and upload the error message to the アプリ_エラーメッセージ folder on NAS
                ReportErrorMsg.showMsgBoxIfNotShown_UploadErrMsgToNAS(errorMessage, errorMessageBoxTitle, NASErrorTxtFileName);

            }
        }

    }
}
