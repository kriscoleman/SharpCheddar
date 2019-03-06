using System;
using System.ComponentModel.DataAnnotations;
using SharpCheddar.Core;

namespace Tests
{
    public class MyModel : IDataModel<int>
    {
        public const int SeedEntityId = 1;
        public DateTime CreatedOn { get; set; }

        [Key] public int Id { get; set; }
    }
}