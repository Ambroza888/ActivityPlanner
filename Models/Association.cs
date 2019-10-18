    using System.ComponentModel.DataAnnotations;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;



    namespace c_sharp_proj.Models
    {
        public class Association
        {


            [Key]
            public int AssociationId { get; set; }
            public int UserId {get;set;}
            public User User {get;set;}
            public _Activity _Activity {get;set;}
            public int _ActivityId {get;set;}


            // -----------------------------------------------------------------
            // date
            // -----------------------------------------------------------------
            public DateTime CreatedAt {get;set;} = DateTime.Now;
            public DateTime UpdatedAt {get;set;} = DateTime.Now;
        }
    }