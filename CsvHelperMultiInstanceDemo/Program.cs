using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CsvHelperMultiInstanceDemo
{
    public class Address
    {
        public string Street { get; set; }
        public string Town { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public Address HomeAddress { get; set; }
        public Address OfficeAddress { get; set; }
    }


    /// <summary>
    /// Changes the column names of Address properties by including a 
    /// custom prefix
    /// </summary>
    public class AddressMap : CsvHelper.Configuration.ClassMap<Address>
    {
        public AddressMap(string columnPrefix)
        {
            Map(m => m.Street).Name($"{columnPrefix}.Street");
            Map(m => m.Town).Name($"{columnPrefix}.Town");
        }
    }


    /// <summary>
    /// Provides a column mapping; by default the auto-generated mappings 
    /// are used. The references mappings, which apply to the Address properties,
    /// are removed and replaced with the custom AddressMap references, so that
    /// each address property has a custom prefix in the column header.
    /// </summary>
    public class PersonDataMap : CsvHelper.Configuration.ClassMap<Person>
    {
        public PersonDataMap()
        {
            AutoMap(CultureInfo.InvariantCulture);

            ReferenceMaps.Clear();
            References<AddressMap>(m => m.HomeAddress, "Home");
            References<AddressMap>(m => m.OfficeAddress, "Office");
        }
    }


    static class Program
    {
        [STAThread]
        static void Main()
        {
            var records = new List<Person>
            {
                new Person
                {
                    Name = "Jon",
                    HomeAddress = new Address() { Street = "Downing St", Town = "London" },
                    OfficeAddress = new Address() { Street = "Farm track", Town = "Worcester" }
                },

                new Person
                {
                    Name = "Paul",
                    HomeAddress = new Address() { Street = "Pleasant Drive", Town = "York" },
                    OfficeAddress = new Address() { Street = "Village Way", Town = "New York" }
                },
            };


            var fileName = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                "people.csv");

            using (var writer = new StreamWriter(fileName))
            using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<PersonDataMap>();
                csv.WriteRecords(records);
            }
        }
    }
}
