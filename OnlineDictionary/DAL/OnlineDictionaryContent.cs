using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OnlineDictionary.DAL {
    public class OnlineDictionaryContent : DbContext{
        public OnlineDictionaryContent() : base() { }
    }
}