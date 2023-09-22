using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using workiom_app.Models;
using MongoDB.Bson;

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
            _myCollection.ReplaceOne(company => company.id.Equals(id), companyIn);
        }

        public List<Company> Search(string searchTerm)
        {
            var nameFilter = Builders<Company>.Filter.Regex("name", new BsonRegularExpression(searchTerm, "i"));

            // Assuming searchTerm can be cast to int
            int searchInt;
            var employeesCount = Int32.TryParse(searchTerm, out searchInt) ?
                Builders<Company>.Filter.Eq("employeesCount", searchInt) : Builders<Company>.Filter.Empty;

            var dynamicColumnsFilter = Builders<Company>.Filter.ElemMatch("dynamicColumns",
                Builders<Column>.Filter.Or(
                    Builders<Column>.Filter.Regex("name", new BsonRegularExpression(searchTerm, "i")),
                    Builders<Column>.Filter.Regex("type", new BsonRegularExpression(searchTerm, "i")),
                    Builders<Column>.Filter.Regex("value", new BsonRegularExpression(searchTerm, "i"))
                )
            );

            var combinedFilter = Builders<Company>.Filter.Or(nameFilter, employeesCount, dynamicColumnsFilter); // Combine filters

            return _myCollection.Find(combinedFilter).ToList();
        }


        public List<BsonDocument> GetCompaniesWithContacts()
        {
            var pipeline = new BsonDocument[]
            {
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "contacts" },
                { "localField", "id" },
                { "foreignField", "companyId" },
                { "as", "contactDetails" }
            })
            };

            return _myCollection.Aggregate<BsonDocument>(pipeline).ToList();
        }


    }
}
