using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreviewDemo
{
    public partial class Preview
    {

        const int CHANNEL = 1;
        const int STREAMID = 2;

        private void Camera_Login()
        {
            if (m_lUserID < 0)
            {
                string DVRIPAddress = "192.168.1.64"; //Éè±¸IPµØÖ·»òÕßÓòÃû
                Int16 DVRPortNumber = 8000;//Éè±¸·þÎñ¶Ë¿ÚºÅ
                string DVRUserName = "admin";//Éè±¸µÇÂ¼ÓÃ»§Ãû
                string DVRPassword = "Admin123";//Éè±¸µÇÂ¼ÃÜÂë

                CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

                //µÇÂ¼Éè±¸ Login the device
                m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
                if (m_lUserID < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_Login_V30 failed, error code= " + iLastErr; //µÇÂ¼Ê§°Ü£¬Êä³ö´íÎóºÅ
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //µÇÂ¼³É¹¦
                    MessageBox.Show("Login Success!");
                    btnLogin.Text = "Logout";
                }

            }
            else
            {
                //×¢ÏúµÇÂ¼ Logout the device
                if (m_lRealHandle >= 0)
                {
                    MessageBox.Show("Please stop live view firstly");
                    return;
                }

                if (!CHCNetSDK.NET_DVR_Logout(m_lUserID))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_Logout failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                m_lUserID = -1;
                btnLogin.Text = "Login";
            }
        }

        private void Camera_Record()
        {
            //Â¼Ïñ±£´æÂ·¾¶ºÍÎÄ¼þÃû the path and file name to save
            string sVideoFileName;
            sVideoFileName = "Record_test.mp4";

            if (m_bRecord == false)
            {
                CHCNetSDK.NET_DVR_MakeKeyFrame(m_lUserID, CHANNEL);

                //¿ªÊ¼Â¼Ïñ Start recording
                if (!CHCNetSDK.NET_DVR_SaveRealData(m_lRealHandle, sVideoFileName))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_SaveRealData failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    btnRecord.Text = "Stop Record";
                    m_bRecord = true;
                }
            }
            else
            {
                //Í£Ö¹Â¼Ïñ Stop recording
                if (!CHCNetSDK.NET_DVR_StopSaveRealData(m_lRealHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_StopSaveRealData failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    str = "Successful to stop recording and the saved file is " + sVideoFileName;
                    MessageBox.Show(str);
                    btnRecord.Text = "Start Record";
                    m_bRecord = false;
                }
            }

            return;
        }
        
        private void Application_Exit()
        {
            //Í£Ö¹Ô¤ÀÀ Stop live view 
            if (m_lRealHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
                m_lRealHandle = -1;
            }

            //×¢ÏúµÇÂ¼ Logout the device
            if (m_lUserID >= 0)
            {
                CHCNetSDK.NET_DVR_Logout(m_lUserID);
                m_lUserID = -1;
            }

            CHCNetSDK.NET_DVR_Cleanup();

            Application.Exit();
        }

        private void Camera_SaveBMP(string Filename)
        {
            string sBmpPicFileName = "BMP_test.bmp";

            //BMP×¥Í¼ Capture a BMP picture
            if (!CHCNetSDK.NET_DVR_CapturePicture(m_lRealHandle, Filename))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_CapturePicture failed, error code= " + iLastErr;
                MessageBox.Show(str);
                return;
            }
            else
            {
                str = "Successful to capture the BMP file and the saved file is " + Filename;
                MessageBox.Show(str);
            }
            return;
        }

        private void Camera_Save_JPG(string FileName)
        {
            CHCNetSDK.NET_DVR_JPEGPARA lpJpegPara = new CHCNetSDK.NET_DVR_JPEGPARA();
            lpJpegPara.wPicQuality = 0; //Í¼ÏñÖÊÁ¿ Image quality
            lpJpegPara.wPicSize = 0xff; //×¥Í¼·Ö±æÂÊ Picture size: 2- 4CIF£¬0xff- Auto(Ê¹ÓÃµ±Ç°ÂëÁ÷·Ö±æÂÊ)£¬×¥Í¼·Ö±æÂÊÐèÒªÉè±¸Ö§³Ö£¬¸ü¶àÈ¡ÖµÇë²Î¿¼SDKÎÄµµ

            //JPEG×¥Í¼ Capture a JPEG picture
            if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(m_lUserID, CHANNEL, ref lpJpegPara, FileName))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_CaptureJPEGPicture failed, error code= " + iLastErr;
                MessageBox.Show(str);
                return;
            }
            else
            {
                str = "Successful to capture the JPEG file and the saved file is " + FileName;
                MessageBox.Show(str);
            }
            return;
        }

        private void Camera_RecordVideo(string FileName)
        {
            if (m_bRecord == false)
            {
                CHCNetSDK.NET_DVR_MakeKeyFrame(m_lUserID, CHANNEL);

                //¿ªÊ¼Â¼Ïñ Start recording
                if (!CHCNetSDK.NET_DVR_SaveRealData(m_lRealHandle, FileName))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_SaveRealData failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    btnRecord.Text = "Stop Record";
                    m_bRecord = true;
                }
            }
            else
            {
                //Í£Ö¹Â¼Ïñ Stop recording
                if (!CHCNetSDK.NET_DVR_StopSaveRealData(m_lRealHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_StopSaveRealData failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    str = "Successful to stop recording and the saved file is " + FileName;
                    MessageBox.Show(str);
                    btnRecord.Text = "Start Record";
                    m_bRecord = false;
                }
            }

            return;
        }

        private void Camera_LiveView()
        {
            if (m_lUserID < 0)
            {
                MessageBox.Show("Please login the device firstly");
                return;
            }

            if (m_lRealHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = RealPlayWnd.Handle;//Ô¤ÀÀ´°¿Ú
                lpPreviewInfo.lChannel = CHANNEL;//Ô¤teÀÀµÄÉè±¸Í¨µÀ
                lpPreviewInfo.dwStreamType = 0;//ÂëÁ÷ÀàÐÍ£º0-Ö÷ÂëÁ÷£¬1-×ÓÂëÁ÷£¬2-ÂëÁ÷3£¬3-ÂëÁ÷4£¬ÒÔ´ËÀàÍÆ
                lpPreviewInfo.dwLinkMode = 0;//Á¬½Ó·½Ê½£º0- TCP·½Ê½£¬1- UDP·½Ê½£¬2- ¶à²¥·½Ê½£¬3- RTP·½Ê½£¬4-RTP/RTSP£¬5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- ·Ç×èÈûÈ¡Á÷£¬1- ×èÈûÈ¡Á÷
                lpPreviewInfo.dwDisplayBufNum = 1; //²¥·Å¿â²¥·Å»º³åÇø×î´ó»º³åÖ¡Êý
                lpPreviewInfo.byProtoType = 0;
                lpPreviewInfo.byPreviewMode = 0;

                //String StreamID = "";

                //if (StreamID != "")
                //{
                //    lpPreviewInfo.lChannel = -1;
                //    byte[] byStreamID = System.Text.Encoding.Default.GetBytes(StreamID);
                //    lpPreviewInfo.byStreamID = new byte[32];
                //    byStreamID.CopyTo(lpPreviewInfo.byStreamID, 0);
                //}


                if (RealData == null)
                {
                    RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//Ô¤ÀÀÊµÊ±Á÷»Øµ÷º¯Êý
                }

                IntPtr pUser = new IntPtr();//ÓÃ»§Êý¾Ý

                //´ò¿ªÔ¤ÀÀ Start live view 
                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);
                if (m_lRealHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr; //Ô¤ÀÀÊ§°Ü£¬Êä³ö´íÎóºÅ
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //Ô¤ÀÀ³É¹¦
                    btnPreview.Text = "Stop Live View";
                }
            }
            else
            {
                //Í£Ö¹Ô¤ÀÀ Stop live view 
                if (!CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                m_lRealHandle = -1;
                btnPreview.Text = "Live View";

            }
            return;
        }

        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
            if (dwBufSize > 0)
            {
                byte[] sData = new byte[dwBufSize];
                Marshal.Copy(pBuffer, sData, 0, (Int32)dwBufSize);

                string str = "ÊµÊ±Á÷Êý¾Ý.ps";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)dwBufSize;
                fs.Write(sData, 0, iLen);
                fs.Close();
            }
        }

        //private void StartTalk()
        //{
        //    if (m_bTalk == false)
        //    {
        //        //¿ªÊ¼ÓïÒô¶Ô½² Start two-way talk
        //        CHCNetSDK.VOICEDATACALLBACKV30 VoiceData = new CHCNetSDK.VOICEDATACALLBACKV30(VoiceDataCallBack);//Ô¤ÀÀÊµÊ±Á÷»Øµ÷º¯Êý

        //        lVoiceComHandle = CHCNetSDK.NET_DVR_StartVoiceCom_V30(m_lUserID, 1, true, VoiceData, IntPtr.Zero);
        //        //bNeedCBNoEncData [in]ÐèÒª»Øµ÷µÄÓïÒôÊý¾ÝÀàÐÍ£º0- ±àÂëºóµÄÓïÒôÊý¾Ý£¬1- ±àÂëÇ°µÄPCMÔ­Ê¼Êý¾Ý

        //        if (lVoiceComHandle < 0)
        //        {
        //            iLastErr = CHCNetSDK.NET_DVR_GetLastError();
        //            str = "NET_DVR_StartVoiceCom_V30 failed, error code= " + iLastErr;
        //            MessageBox.Show(str);
        //            return;
        //        }
        //        else
        //        {
        //            //btnVioceTalk.Text = "Stop Talk";
        //            m_bTalk = true;
        //        }
        //    }
        //    else
        //    {
        //        //Í£Ö¹ÓïÒô¶Ô½² Stop two-way talk
        //        if (!CHCNetSDK.NET_DVR_StopVoiceCom(lVoiceComHandle))
        //        {
        //            iLastErr = CHCNetSDK.NET_DVR_GetLastError();
        //            str = "NET_DVR_StopVoiceCom failed, error code= " + iLastErr;
        //            MessageBox.Show(str);
        //            return;
        //        }
        //        else
        //        {
        //            //btnVioceTalk.Text = "Start Talk";
        //            m_bTalk = false;
        //        }
        //    }
        //}

        //public void VoiceDataCallBack(int lVoiceComHandle, IntPtr pRecvDataBuffer, uint dwBufSize, byte byAudioFlag, System.IntPtr pUser)
        //{
        //    byte[] sString = new byte[dwBufSize];
        //    Marshal.Copy(pRecvDataBuffer, sString, 0, (Int32)dwBufSize);

        //    if (byAudioFlag == 0)
        //    {
        //        //½«»º³åÇøÀïµÄÒôÆµÊý¾ÝÐ´ÈëÎÄ¼þ save the data into a file
        //        string str = "PC²É¼¯ÒôÆµÎÄ¼þ.pcm";
        //        FileStream fs = new FileStream(str, FileMode.Create);
        //        int iLen = (int)dwBufSize;
        //        fs.Write(sString, 0, iLen);
        //        fs.Close();
        //    }
        //    if (byAudioFlag == 1)
        //    {
        //        //½«»º³åÇøÀïµÄÒôÆµÊý¾ÝÐ´ÈëÎÄ¼þ save the data into a file
        //        string str = "Éè±¸ÒôÆµÎÄ¼þ.pcm";
        //        FileStream fs = new FileStream(str, FileMode.Create);
        //        int iLen = (int)dwBufSize;
        //        fs.Write(sString, 0, iLen);
        //        fs.Close();
        //    }

        //}
    }
}
