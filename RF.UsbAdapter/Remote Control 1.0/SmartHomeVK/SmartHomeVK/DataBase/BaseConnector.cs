using System;
using System.IO;
using System.Data.SQLite;

namespace SmartHomeVK.DataBase
{
    class BaseConnector
    {
        SQLiteConnection db = null;

        struct User
        {
            public int id;
            public string user_id;
            public string user_password;
            public string user_name;
            public string phone;
        };

        struct Books
        {
            public string title;
            public string author;
            public string subject;
            public int book_id;
        };

        struct Books
        {
            public string title;
            public string author;
            public string subject;
            public int book_id;
        };

        public BaseConnector()
        {
            bool isNew = !File.Exists("smarthome.db");
            if (isNew)
            {
                SQLiteConnection.CreateFile("smarthome.db");
            }
            db = new SQLiteConnection("smarthome.db");
            db.Open();
            if (isNew)
            {
                SQLiteCommand command = new SQLiteCommand(File.ReadAllText("database.sql"), db);
                command.ExecuteNonQuery();
            }            
        }

        public void 
    }
}
