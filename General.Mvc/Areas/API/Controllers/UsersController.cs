using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace General.Mvc.Areas.API.Controllers
{    //http://localhost:50491/api/users/1
    [Route("api/[controller]")]
    public class UsersController : Controller
    {   
        //// GET: api/<controller>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}


        //测试API啥的
        //
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Web API", "ASP.NET Core" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "User ID-" + id;
        }

        //// POST api/<controller>
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
