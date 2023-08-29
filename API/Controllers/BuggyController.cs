using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        public readonly Datacontext _dbcontext;
        public BuggyController(Datacontext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret(){
            return "secret Key";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUsers> GetNotfound(){
            var things = _dbcontext.Users.Find(-1);
            if(things == null) return NotFound();
            return Ok(things);
        }

        [HttpGet("server-error")]
        public ActionResult<char?> GetServerError(){
            var things = _dbcontext.Users.Find(-1);
            var thingstoreturn = things.ToString();
            return thingstoreturn[0];
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest(){
            return BadRequest();
        }


    }
}