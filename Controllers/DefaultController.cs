using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UMS_API_INTERFACE.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet]
        [Route("GetAllUserDetails")]
        public JArray getUserData()
        {
            JObject returnObject = new JObject();

            UserDataContext db = new UserDataContext();


            List<tbl_User> data = db.tbl_Users.ToList();
            string JsonResp = JsonConvert.SerializeObject(data);

            JArray array = new JArray();
            array = JArray.Parse(JsonResp);

            return array;

            //returnObject.Add("Data", array);

            //return returnObject;
        }

        [HttpGet]
        [Route("GetAllActiveUsers")]
        public JArray GetAllActiveUsers()
        {
            JArray returnObject = new JArray();


            UserDataContext db = new UserDataContext();
            List<tbl_User> data = db.tbl_Users.Where(x => x.isActive == 1).ToList();
            string JsonResp = JsonConvert.SerializeObject(data);
            returnObject = JArray.Parse(JsonResp);
            return returnObject;
        }

        [HttpPost]
        [Route("RegisterNewCustomer")]
        public JObject AddUser(JObject inputJson)
        {
            JObject returnObject = new JObject();
            try
            {             

                UserDataContext d = new UserDataContext();

                tbl_User user = new tbl_User();

                user.Name = inputJson["name"].ToString();
                user.Email = inputJson["email"].ToString();
                user.Address = inputJson["address"].ToString();
                user.MobileNo = inputJson["mobile"].ToString();
                user.city = inputJson["city"].ToString();
                user.isActive = 1;

                d.tbl_Users.InsertOnSubmit(user);
                d.SubmitChanges();

                tbl_login login = new tbl_login();

                login.id = user.id;
                login.username= inputJson["email"].ToString();
                login.password= inputJson["password"].ToString();

                d.tbl_logins.InsertOnSubmit(login);
                d.SubmitChanges();

                returnObject.Add("Status", "Successful");

            }
            catch(Exception ex)
            {
                returnObject.Add("Status", "un Successful");
                returnObject.Add("Error", ex.Message);

            }
            return returnObject;

        }

        [HttpPost]
        [Route("UserLogin")]
        public JObject UserLogin(JObject inputJson)
        {
            JObject returnJson = new JObject();

            UserDataContext d = new UserDataContext();

            var data = d.tbl_logins.Where(a => a.username == inputJson["username"].ToString() && a.password == inputJson["password"].ToString()).FirstOrDefault();



            

            if (data != null)
            {
                returnJson.Add("status","Successful");
                var userData = d.tbl_Users.Where(x => x.Email == inputJson["username"].ToString()).FirstOrDefault();
                returnJson.Add("Name", userData.Name);
                returnJson.Add("MobileNo", userData.MobileNo);
                returnJson.Add("City", userData.city);
            }
            else
            {
                returnJson.Add("Message", "UnSuccessful");
            }

            return returnJson;

        }

    }
}
