using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS
{
    class Server
    {
        public string ipadress;
        public string port;
        public string name; //przydatne?
    }

    class ServerDataBase
    {
        List<Server> serverList;
        int wolnePorty;// = 8888;
        public ServerDataBase()
        {
            serverList = new List<Server>();
            wolnePorty = 8888;
        }
        public string WolnyPort()
        {
            int temp = wolnePorty--;
            serverList.Add(new Server() { port=temp.ToString() });
            return temp.ToString();
        }
    }

    class User
    {
        public string nick;
        public string pass; // w miare mozliwosci zrobic MD5
        public bool isLogin;
    }
    class UserDataBase
    {
        // w miare mozliwosci liste podmienic na jakiegos mysqla
        private List<User> usersList;
        public UserDataBase()
        {
            usersList = new List<User>();
        }
        public bool query(string nickname, string password)
        {
            foreach (User u in usersList)
            {
                if (u.nick==nickname)
                    if (u.pass == password)
                        return true;
                    else
                        return false;
            }
            usersList.Add(new User()
            {
                nick = nickname,
                pass = password,
                isLogin = true
            });
            return true;
        }

        
    }
}
