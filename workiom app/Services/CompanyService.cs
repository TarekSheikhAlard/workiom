using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using workiom_app.Models;

namespace workiom_app.Services
{
    public class CompanyService
    {
        private readonly IMongoCollection<Company> _myCollection;

        public CompanyService(IMongoClient client, IConfiguration config)
        {
            var database = client.GetDatabase(config.GetSection("MongoDB:Database").Value);
            _myCollection = database.GetCollection<Company>("company");
        }

        public List<Company> Get() => _myCollection.Find(model => true).ToList();

        public void InsertOne(Company company)
        {


            _myCollection.InsertOne(company);
        }

        public Company Get(string id) => _myCollection.Find(company => company.id.Equals(id)).First();

        public void Delete(string id)
        {
            _myCollection.DeleteOne(company => company.id.Equals(id));
        }

        public void Update(string id, Company companyIn)
        {
            _myCollection.ReplaceOne(company => company.id == id, companyIn);
        }


    }
}
