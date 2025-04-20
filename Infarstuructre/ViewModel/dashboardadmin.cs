using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domin.Entity;

namespace Infarstuructre.ViewModel
{
    public class dashboardadmin
    {
        public List<Service> services {  get; set; }= new List<Service>();
        public List<Employees> employees {  get; set; }=new List<Employees>();
        public List<Category> categories {  get; set; }=new List<Category> (){ };   
        public List<historypaid> historypaids {  get; set; }=new List<historypaid> (){ };
        public List<Request> requests { get; set; } = new List<Request>();


        public int ActiveUsersCount { get; set; }
        public int InactiveUsersCount { get; set; }

        /// <summary>Number of users in the “BasicUser” role</summary>
        public int BasicUserCount { get; set; }
    
}
}
