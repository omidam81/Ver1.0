using Orchard.Themes;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;


namespace Teeyoot.Module.Controllers
{
    public class MergeFields
    {
        public string FNAME;
		public string LNAME;
        public string CAMPAIGNID;
    };
    public class Member
    {
        public string email_address;
        public string email_type;
	    public string status;
	public MergeFields merge_fields;
	public bool vip;
    public string listid;
    };

    [Themed]
    public class MessageController : Controller
    {
        // GET: Message
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateMessage()
        {
            return View();
        }
        public ActionResult getList()
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Authorization =
                     new AuthenticationHeaderValue("Basic", "efaebe4525defd02eec94579dfc7f4ce-us11");
                var member = new Member()
                {
                    email_address = "test200s@mail.ru",
                    email_type = "html",
                    status = "subscribed",
                    merge_fields = new MergeFields()
                    {
                        FNAME = "Test",
                        LNAME = "Tester",
                        CAMPAIGNID = "#100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000##100000000#"
                    },
                    vip = false,
                    listid = "f90603a2b0",
                };
                string json = new JavaScriptSerializer().Serialize(member);
                var stringContent = new StringContent(json);
              
                
                //StringContent cont = new StringContent(@"{
                //                ""email_address"":""me""}");
                //var request = new HttpRequestMessage(new HttpMethod("PATCH"), @"https://us11.api.mailchimp.com/3.0/lists/f90603a2b0/members/fe1f141fa32b6592b1fca4d8eca2a546")
                //{
                //    Content = stringContent,
                //};
                //System.Threading.Tasks.Task<HttpResponseMessage> response = http.SendAsync(request);
                //System.Threading.Tasks.Task<HttpResponseMessage> response = http.DeleteAsync(@"https://us11.api.mailchimp.com/3.0/lists/f90603a2b0/members/fe1f141fa32b6592b1fca4d8eca2a546");
                //System.Threading.Tasks.Task<HttpResponseMessage> response = http.PostAsync(@"https://us11.api.mailchimp.com/3.0/lists/f90603a2b0/members", stringContent);
                System.Threading.Tasks.Task<HttpResponseMessage> response = http.PostAsync(@"https://us11.api.mailchimp.com/3.0/campaigns", stringContent);
               // System.Threading.Tasks.Task<string> content = http.GetStringAsync(@"https://us11.api.mailchimp.com/3.0/lists/f90603a2b0/members");
               // ViewBag.resp = content.Result;
                //Console.WriteLine(content.Result);
               ViewBag.resp = response.Result.StatusCode;
            }
            return View();
        }
    }
}