﻿namespace IdentityService.WebAPI.Controller.Login
{
    public class LoginVM
    {
        public int Code { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
        public bool Ok { get; set; }
    }
}