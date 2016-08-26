using System;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using SmartHomeVK.DeviceWork;

namespace SmartHomeVK
{
    class MessageProcessor
    {
        Dictionary<string, string> equal = new Dictionary<string, string>();

        String client_id = "3697615";
        String client_secret = "AlVXZFMUqyrnABp8ncuU";
        static String access_token = "";
        private String login;
        private String password;
        public static String current_user = "";
        public MessageProcessor(String login, String pass)
        {
            this.login = login;
            this.password = pass;
        }
        //структура для работы с сообщениями
       
        class Data
        {
            public Response Response;
        }

        class Response
        {
            public int count;
            public Item[] Items;
        }

        class Item
        {
            public int id;
            public int date;
            public int user_id;
            public int read_state;
            public String title;
            public string body;

            public override string ToString()
            {
                return string.Format("{0} ({1})", body, user_id);
            }
        }

        public void Auth()
        {
            String par = "grant_type=password&client_id="+client_id+"&client_secret="+client_secret+"&username="+login+"&password="+password+"&v=5.53&2fa_supported=0";
            HttpReq("https://oauth.vk.com/token",par);
            VkMsgReq();
            new Thread(MessageListener).Start();
            new FriendProcessor(access_token);
        }

        public static void SendMsg(String uid, String msg)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create("https://api.vk.com/method/messages.send?user_id="+uid+ "&message="+msg+"&access_token=" + access_token + "&v=5.53");
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.Stream stream = resp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            Console.WriteLine(Out);
            sr.Close();
        }

        private void VkMsgReq()
        {
            String msg_ids = "";         
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create("https://api.vk.com/method/messages.get?out=0&offset=0&count=10&time_offset=10&preview_length=0&access_token=" + access_token + "&v=5.53");
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                string Out = sr.ReadToEnd();
                sr.Close();
                Data data = JsonConvert.DeserializeObject<Data>(Out);
                foreach (Item item in data.Response.Items)
                {
                    CmdExec(""+item.user_id, item.body);
                    msg_ids += item.id + ",";
                }
                //отмечаем как прочитанное
                req = System.Net.WebRequest.Create("https://api.vk.com/method/messages.markAsRead?message_ids=" + msg_ids + "&access_token=" + access_token + "&v=5.53");
                resp = req.GetResponse();
                stream = resp.GetResponseStream();
                sr = new System.IO.StreamReader(stream);
                sr.ReadToEnd();
                sr.Close();
                //удаляем прочитанное
                req = System.Net.WebRequest.Create("https://api.vk.com/method/messages.delete?message_ids="+msg_ids+"&access_token=" + access_token + "&v=5.53");
                resp = req.GetResponse();
                stream = resp.GetResponseStream();
                sr = new System.IO.StreamReader(stream);
                sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception e)
            {

            }
        }

        private void CmdExec(String user, String cmd)
        {
            current_user = user;
            Console.WriteLine(user + ": " + cmd);
            String[] args = cmd.ToLower().Split(' ');
            switch (args[0])
            {
                case "назвать":
                        equal.Add(args[2], args[1]);
                    SendMsg(user, "Устройству \"" + args[1] + "\" присвоено имя \"" + args[2] + "\"");
                    break;
                case "включить":
                    if (!equal.ContainsKey(args[1]))
                    {
                        SendMsg(user, "Устройство \"" + args[1] + "\" отсутствует");
                    }else
                    {
                        Switch sw = (Switch)RadioController.devices[equal[args[1]]];
                        sw.SwitchOn();
                    }
                    break;
                case "выключить":
                    if (!equal.ContainsKey(args[1]))
                    {
                        SendMsg(user, "Устройство \"" + args[1] + "\" отсутствует");
                    }
                    else
                    {
                        Switch sw = (Switch)RadioController.devices[equal[args[1]]];
                        sw.SwitchOff();
                    }
                    break;
            }
        }

        public String HttpReq(String url, String data)
        {
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(url + "?" + data);
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                string Out = sr.ReadToEnd();
                sr.Close();
                String token = Out.Split(',')[0].Split(':')[1].Replace("\"","");
                access_token = token;
                return token;
            }catch(Exception e)
            {
                
            }
            return null;
        }

        public void MessageListener()
        {
            while (true)
            {
                VkMsgReq();
                Thread.Sleep(2000);
            }
        }
    }
}
