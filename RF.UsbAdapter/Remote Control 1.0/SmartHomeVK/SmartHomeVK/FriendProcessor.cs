using System;
using Newtonsoft.Json;
using System.Threading;

namespace SmartHomeVK
{
    class FriendProcessor
    {
        private String token;

        public FriendProcessor(String token)
        {
            this.token = token;
            new Thread(ScanRequests).Start();
        }

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
            public int user_id;
        }

        private void getRequests()
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create("https://api.vk.com/method/friends.getRequests?out=0&offset=0&extended=1&need_mutual=0&out=0&access_token=" +token + "&v=5.53");
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.Stream stream = resp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            Data data = JsonConvert.DeserializeObject<Data>(Out);
            foreach (Item item in data.Response.Items)
            {
                addFriend(item.user_id);
            }
        }

        private void addFriend(int uid)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create("https://api.vk.com/method/friends.add?user_id="+uid+"&access_token=" + token + "&v=5.53");
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.Stream stream = resp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
        }

        private void setOnline()
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create("https://api.vk.com/method/account.setOnline?access_token=" + token + "&v=5.53");
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.Stream stream = resp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
        }

        public void ScanRequests()
        {
            while (true)
            {
                getRequests();
                setOnline();
                Thread.Sleep(30000);
            }
        }
    }
}
