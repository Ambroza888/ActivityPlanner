    using System.ComponentModel.DataAnnotations;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;



    namespace c_sharp_proj.Models
    {
        public class _Activity
        {

            [Key]
            public int _ActivityId { get; set; }

            [Required]
            public string Title {get;set;}

            [Required]
            [DataType(DataType.Date)]
            public DateTime Date {get;set;}
            
            [Required]
            [DataType(DataType.Time)]
            public DateTime Time {get;set;}

            [Required]
            [Range(0,int.MaxValue)]
            public int Duration {get;set;}
            [Required]
            public string HDW {get;set;}

            [Required]
            public string Description {get;set;}

            public User User {get;set;}
            public int UserId {get;set;}
            public List <Association> Associations {get;set;}



            // -----------------------------------------------------------------
            // date
            // -----------------------------------------------------------------
            public DateTime CreatedAt {get;set;} = DateTime.Now;
            public DateTime UpdatedAt {get;set;} = DateTime.Now;
        }
    }