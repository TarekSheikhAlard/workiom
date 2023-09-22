using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workiom_app.Models;

namespace workiom_app.Services
{
    public class ContactService
    {
        private readonly IMongoCollection<Contact> _myCollection;

        public ContactService(IMongoClient client, IConfiguration config)
        {
            var database = client.GetDatabase(config.GetSection("MongoDB:Database").Value);
            _myCollection = database.GetCollection<Contact>("contact");
        }

        public void InsertOne(Contact contact)
        {
            _myCollection.InsertOne(contact);
        }
        public List<Contact> Get() => _myCollection.Find(model => true).ToList();

        public Contact Get(string id)=> _myCollection.Find(contact => contact.id.Equals(id)).First();
        

        public void Delete(string id)
        {
            _myCollection.DeleteOne(contact => contact.id.Equals(id));
        }

        public void Update(string id, Contact contactIn)
        {
            _myCollection.ReplaceOne(contact => contact.id.Equals(id), contactIn);
        }


        public List<Contact> Search(string searchTerm)
        {
            var nameFilter = Builders<Contact>.Filter.Regex("name", new BsonRegularExpression(searchTerm, "i"));

            var relatedCompany = Builders<Contact>.Filter.Regex("relatedCompany", new BsonRegularExpression(searchTerm, "i"));


            var dynamicColumnsFilter = Builders<Contact>.Filter.ElemMatch("dynamicColumns",
                Builders<Column>.Filter.Or(
                    Builders<Column>.Filter.Regex("name", new BsonRegularExpression(searchTerm, "i")),
                    Builders<Column>.Filter.Regex("type", new BsonRegularExpression(searchTerm, "i")),
                    Builders<Column>.Filter.Regex("value", new BsonRegularExpression(searchTerm, "i"))
                )
            );

            var combinedFilter = Builders<Contact>.Filter.Or(nameFilter, relatedCompany, dynamicColumnsFilter); // Combine filters

            return _myCollection.Find(relatedCompany).ToList();
        }


        public List<Contact> GetContactsForCompany(string companyId)
        {
            var filter = Builders<Contact>.Filter.Eq("relatedCompany", companyId);
            return _myCollection.Find(filter).ToList();
        }



    }
}
