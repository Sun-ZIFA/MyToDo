﻿namespace MyToDo.Api.Context
{
    public class User : BaseEntity
    {
        public string  Account { get; set; }
        public string  UserName { get; set; }
        public string  PassWord { get; set; }
    }
}
